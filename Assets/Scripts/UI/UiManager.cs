using TMPro;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject confirmMenu;
    [SerializeField] private GameObject splashScreen;

    [SerializeField] private GameObject turnUI;
    [SerializeField] private GameObject controlsUI;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private TMP_Text _gameOverText;
    [SerializeField] private GameObject MainMenuFight;
    
    private UIController uiController;
    private TMP_Text _turnText;
    private PlayerControlsUI _controlsController;
    private int activePlayerIndex;

    void Start()
    {
        uiController = GetComponent<UIController>();
        _turnText = turnUI.GetComponent<TMP_Text>();
        _controlsController = controlsUI.GetComponent<PlayerControlsUI>();
    }

    public void StartGameButton()
    {
        splashScreen.gameObject.SetActive(false);
        _controlsController.GameStart();
        MainMenuFight.SetActive(false);
        uiController.DisableControls();
    }

    public void ExitGameButton()
    {
        confirmMenu.gameObject.SetActive(true);
        mainMenu.gameObject.SetActive(false);
        uiController.GetEnabledButtons();
    }

    public void ConfirmButton()
    {
        Application.Quit();
    }

    public void CancelButton()
    {
        confirmMenu.gameObject.SetActive(false);
        mainMenu.gameObject.SetActive(true);
        uiController.GetEnabledButtons();
    }
    
    public void NextTurn(int nextPlayerIndex)
    {
        activePlayerIndex = nextPlayerIndex;
        turnUI.GetComponent<Animator>().SetTrigger("NewTurn");
        _turnText.text = _turnText.text.Replace("%", (1 - nextPlayerIndex + 1).ToString());
        
        _controlsController.EnableControls();
        _controlsController.Next(activePlayerIndex == 1);
    }

    public void ResetTurnNumber(int currentPlayerIndex)
    {
        _turnText.text = _turnText.text.Replace((1 - currentPlayerIndex + 1).ToString(), "%");
    }

    public void DisableControls()
    {
        _controlsController.DisableControls();
    }

    public void NextControls()
    {
        _controlsController.Next(activePlayerIndex == 1);
    }

    public void GameOver(int winningPlayer)
    {
        gameOverUI.SetActive(true);
        _gameOverText.text = _gameOverText.text.Replace("%", winningPlayer.ToString());
        uiController.GetEnabledButtons();
    }

    public void RestartGameButton()
    {
        gameOverUI.gameObject.SetActive(false);
        uiController.DisableControls();
    }

    public void ReturnToMenuButton()
    {
        gameOverUI.gameObject.SetActive(false);
        splashScreen.gameObject.SetActive(true);
        mainMenu.gameObject.SetActive(true);
        MainMenuFight.SetActive(false);
        uiController.GetEnabledButtons();
    }
}