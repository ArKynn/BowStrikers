using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class GameCamera : MonoBehaviour
{
    [SerializeField] private GameManager _gm;

    [SerializeField] private CinemachineVirtualCamera _gameCamera;
    [SerializeField] private CinemachineVirtualCamera _menuCamera;

    private bool GameActive
    {
        get => gameActive;
        set
        {
            gameActive = value;
            OnGameActiveChanged();
        }
    }

    private bool gameActive;

    // Start is called before the first frame update
    void Start()
    {
        _gameCamera.enabled = false;
    }

    private void OnGameActiveChanged()
    {
        _gameCamera.enabled = gameActive;
        _menuCamera.enabled = !gameActive;
    }

    public void GameStateChange(bool value)
    {
        GameActive = value;
    }

    public void UpdateFollowTarget(GameObject target)
    {
        _gameCamera.Follow = target.transform;
    }
}
