using System;
using System.Collections;
using System.Timers;
using UnityEngine;
public class GameManager : MonoBehaviour
{

    private Archer[] _players;
    private Unity.Mathematics.Random _rnd;
    
    [SerializeField] private GameCamera _gameCamera;
    [SerializeField] private UiManager _uiManager;

    private bool _gameActive
    {
        get => gameActive;
        set
        {
            gameActive = value;
            _gameCamera.GameStateChange(_gameActive);
        }   
    }
    
    private bool gameActive;

    public GameObject _activePlayer
    {
        get => activePlayer;
        private set
        {
            activePlayer = value;
            _gameCamera.UpdateFollowTarget(_activePlayer.gameObject);
            activePlayerIndex = Array.IndexOf(_players, activePlayer.GetComponent<Archer>());
        }
    }

    private GameObject activePlayer;
    private int activePlayerIndex;
    
    private bool gameOver;

    private void Start()
    {
        _players = FindObjectsOfType<Archer>();
        _rnd = new Unity.Mathematics.Random((uint)UnityEngine.Random.Range(0, int.MaxValue));
    }

    private void Update()
    {
        if (!gameActive || gameOver) return;
        if(CheckActivePlayerShot()) _gameCamera.UpdateFollowTarget(_activePlayer.GetComponent<Archer>().activeShot.gameObject);
    }

    public void StartGame()
    {
        _gameActive = true;
        gameOver = false;
        StartCoroutine(NextTurn(true));
        CleanArrows();
    }

    public void ShotHit(Archer hitObj)
    {
        if (hitObj == _activePlayer.GetComponent<Archer>() || hitObj == null) StartCoroutine(NextTurn(false));
        else
        {
            hitObj.GetHit();
            StartGameOver();
        }
    }

    private GameObject GetStartingPlayer()
    {
        Archer startingPlayer = _players[_rnd.NextInt(0, 2)];
        return startingPlayer.gameObject;
    }

    private bool CheckActivePlayerShot()
    {
        if(activePlayer is null || activePlayer.GetComponent<Archer>() is null) return false;
        return _activePlayer.GetComponent<Archer>().activeShot is not null;
    }

    private IEnumerator NextTurn(bool firstTurn)
    {
        yield return new WaitForSeconds(0.1f);

        if (firstTurn) _activePlayer = GetStartingPlayer();
        
        _activePlayer = _players[1 - activePlayerIndex].gameObject;
        _activePlayer.gameObject.GetComponentInParent<Archer>().GetTurn();
        
        _uiManager.ResetTurnNumber(1 - activePlayerIndex);
        _uiManager.NextTurn(activePlayerIndex);
    }

    private void StartGameOver()
    {
        gameOver = true;
        StartCoroutine(NewTimer(1f, GameOver));
    }

    private IEnumerator NewTimer(float time, Action action)
    {
        yield return new WaitForSeconds(time);
        
        action?.Invoke();
    }

    private void GameOver()
    {
        _gameCamera.UpdateFollowTarget(_activePlayer.gameObject);
        _uiManager.GameOver(1 - activePlayerIndex + 1);
    }

    public void ReturntoMainMenu()
    {
        _gameActive = false;
    }

    private void CleanArrows()
    {
        var arrows = FindObjectsByType<ShotProjectile>(FindObjectsSortMode.None);
        foreach (var arrow in arrows)
        {
            Destroy(arrow.gameObject);
        }

        if(activePlayer != null) activePlayer.GetComponent<Archer>().ResetArrow();
    }
}
