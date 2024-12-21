using System;
using System.Collections;
using Cinemachine;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class GameManager : MonoBehaviour
{

    private Archer[] _players;
    private Random _rnd;
    
    [Header("Game Variables")]
    [SerializeField] private GameCamera gameCamera;
    [SerializeField] private UiManager uiManager;

    [Header("Level Generation Variables")] 
    [SerializeField] private CinemachineSmoothPath trackedDolly;
    [SerializeField] private float trackOffsetToSpawn;
    [SerializeField] private GameObject[] gameBoundaries;
    [SerializeField] private float boundaryOffsetToSpawn;
    [SerializeField] private float minSpawnDistance;
    [SerializeField] private float maxSpawnDistance;
    private float spawnDistance;
    private float direction = 1;

    private bool _gameActive
    {
        get => gameActive;
        set
        {
            gameActive = value;
            gameCamera.GameStateChange(_gameActive);
        }   
    }
    
    private bool gameActive;

    public GameObject _activePlayer
    {
        get => activePlayer;
        private set
        {
            activePlayer = value;
            gameCamera.UpdateFollowTarget(_activePlayer.gameObject);
            activePlayerIndex = Array.IndexOf(_players, activePlayer.GetComponent<Archer>());
        }
    }

    private GameObject activePlayer;
    private int activePlayerIndex;
    
    private bool gameOver;

    private void Start()
    {
        _players = FindObjectsOfType<Archer>();
        _rnd = new Random((uint)UnityEngine.Random.Range(0, int.MaxValue));
    }

    private void Update()
    {
        if (!gameActive || gameOver) return;
        if(CheckActivePlayerShot()) gameCamera.UpdateFollowTarget(_activePlayer.GetComponent<Archer>().activeShot.gameObject);
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

        if (firstTurn)
        {
            RandomizePlayerPositions();
            _activePlayer = GetStartingPlayer();
        }
        
        _activePlayer = _players[1 - activePlayerIndex].gameObject;
        _activePlayer.gameObject.GetComponentInParent<Archer>().GetTurn();
        
        uiManager.ResetTurnNumber(1 - activePlayerIndex);
        uiManager.NextTurn(activePlayerIndex);
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
        gameCamera.UpdateFollowTarget(_activePlayer.gameObject);
        uiManager.GameOver(1 - activePlayerIndex + 1);
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

    private void RandomizePlayerPositions()
    {
        for (int i = 0; i < _players.Length; i++)
        {
            spawnDistance = _rnd.NextFloat(minSpawnDistance, maxSpawnDistance) * direction;
            
            var pos = _players[i].transform.position;
            _players[i].transform.position = new Vector3(spawnDistance, pos.y, pos.z);
            
            pos = trackedDolly.m_Waypoints[i].position;
            trackedDolly.m_Waypoints[i].position = new Vector3(spawnDistance + trackOffsetToSpawn * direction, pos.y, pos.z);
            
            pos = gameBoundaries[i].transform.position;
            gameBoundaries[i].transform.position = new Vector3(spawnDistance + boundaryOffsetToSpawn * direction, pos.y, pos.z);
            
            direction = -direction;
        }
    }
}
