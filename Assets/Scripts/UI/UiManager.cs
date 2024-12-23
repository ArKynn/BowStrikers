using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject confirmMenu;
    [SerializeField] private GameObject splashScreen;

    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private GameObject turnUI;
    [SerializeField] private GameObject controlsUI;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private TMP_Text _gameOverText;
    [SerializeField] private GameObject MainMenuFight;
    
    private Resolution[] resolutions;
    private UIController uiController;
    private TMP_Text _turnText;
    private PlayerControlsUI _controlsController;
    private int activePlayerIndex;

    private int FullScreen
    {
        get => PlayerPrefs.GetInt("FullScreen", 1);
        set
        {
            PlayerPrefs.SetInt("FullScreen", value);
            Screen.fullScreen = FullScreen == 1;
        }
    }

    void Start()
    {
        uiController = GetComponent<UIController>();
        _turnText = turnUI.GetComponent<TMP_Text>();
        _controlsController = controlsUI.GetComponent<PlayerControlsUI>();
        UpdateResolutionsDropdown();
    }

    private void UpdateResolutionsDropdown()
    {
        resolutionDropdown.ClearOptions();
        resolutions = Screen.resolutions;
        foreach (Resolution resolution in resolutions)
        {
            resolutionDropdown.options.Add(new TMP_Dropdown.OptionData(resolution.ToString()));
        }
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

    public void OpenOptionsMenu()
    {
        settingsMenu.gameObject.SetActive(true);
        mainMenu.gameObject.SetActive(false);
        uiController.GetEnabledButtons();
    }

    public void CloseOptionsMenu()
    {
        settingsMenu.gameObject.SetActive(false);
        mainMenu.gameObject.SetActive(true);
        uiController.GetEnabledButtons();
    }

    public void ToggleFullScreen()
    {
        FullScreen = 1 - FullScreen;
    }

    public void SetResolution()
    {
        print(resolutionDropdown.options[resolutionDropdown.value].text);
    }
}