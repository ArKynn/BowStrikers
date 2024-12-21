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
    
    private TMP_Text _turnText;
    private PlayerControlsUI _controlsController;   

    void Start()
    {
        _turnText = turnUI.GetComponent<TMP_Text>();
        _controlsController = controlsUI.GetComponent<PlayerControlsUI>();
        
    }

    public void StartGameButton()
    {
        splashScreen.gameObject.SetActive(false);
        _controlsController.GameStart();
        MainMenuFight.SetActive(false);
    }

    public void ExitGameButton()
    {
        confirmMenu.gameObject.SetActive(true);
        mainMenu.gameObject.SetActive(false);
    }

    public void ConfirmButton()
    {
        Application.Quit();
    }

    public void CancelButton()
    {
        confirmMenu.gameObject.SetActive(false);
        mainMenu.gameObject.SetActive(true);
    }
    
    public void NextTurn(int nextPlayerIndex)
    {
        turnUI.GetComponent<Animator>().SetTrigger("NewTurn");
        _turnText.text = _turnText.text.Replace("%", (1 - nextPlayerIndex + 1).ToString());
        
        _controlsController.EnableControls();
        _controlsController.Next();
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
        _controlsController.Next();
    }

    public void GameOver(int winningPlayer)
    {
        gameOverUI.SetActive(true);
        _gameOverText.text = _gameOverText.text.Replace("%", winningPlayer.ToString());
    }

    public void RestartGameButton()
    {
        gameOverUI.gameObject.SetActive(false);
    }

    public void ReturnToMenuButton()
    {
        gameOverUI.gameObject.SetActive(false);
        splashScreen.gameObject.SetActive(true);
        mainMenu.gameObject.SetActive(true);
        MainMenuFight.SetActive(false);
    }
}