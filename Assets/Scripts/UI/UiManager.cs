using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject creditsMenu;
    [SerializeField] private GameObject confirmMenu;
    [SerializeField] private GameObject splashScreen;

    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private Image fullScreenIcon;
    [SerializeField] private Image showLastShotIcon;
    [SerializeField] private Sprite disabledToggleSprite;
    [SerializeField] private Sprite enabledToggleSprite;
    [SerializeField] private GameObject turnUI;
    [SerializeField] private GameObject controlsUI;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private TMP_Text gameOverText;
    [SerializeField] private GameObject mainMenuFight;
    
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
            fullScreenIcon.sprite = FullScreen == 1 ? enabledToggleSprite : disabledToggleSprite;
        }
    }

    private int[] Resolution
    {
        get => new int[] { PlayerPrefs.GetInt("ResolutionWidth"), PlayerPrefs.GetInt("ResolutionHeight") };
        set
        {
            PlayerPrefs.SetInt("ResolutionWidth", value[0]);
            PlayerPrefs.SetInt("ResolutionHeight", value[1]);
            Screen.SetResolution(value[0], value[1], FullScreen == 1);
        }
    }

    private void Awake()
    {
        Screen.fullScreen = FullScreen == 1;
        resolutions = Screen.resolutions;
        if (Resolution == new int[] { 0, 0 })
        {
            Screen.SetResolution(resolutions[0].width, resolutions[0].height, FullScreen == 1);
        }
        else
        {
            Screen.SetResolution(Resolution[0], Resolution[1], FullScreen == 1);
        }
    }

    void Start()
    {
        uiController = GetComponent<UIController>();
        _turnText = turnUI.GetComponent<TMP_Text>();
        _controlsController = controlsUI.GetComponent<PlayerControlsUI>();
        UpdateResolutionsDropdown();
        ToggleShowLastShot();
    }

    private void UpdateResolutionsDropdown()
    {
        resolutionDropdown.ClearOptions();
        foreach (Resolution resolution in resolutions)
        {
            resolutionDropdown.options.Add(new TMP_Dropdown.OptionData(resolution.ToString()));
            if(resolution.ToString() == Screen.currentResolution.ToString()) resolutionDropdown.value = Array.IndexOf(resolutions, resolution);
        }
    }

    public void StartGameButton()
    {
        splashScreen.gameObject.SetActive(false);
        _controlsController.GameStart();
        mainMenuFight.SetActive(false);
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
        gameOverText.text = gameOverText.text.Replace("%", winningPlayer.ToString());
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
        mainMenuFight.SetActive(false);
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

    public void OpenCredits()
    {
        creditsMenu.gameObject.SetActive(true);
        mainMenu.gameObject.SetActive(false);
        uiController.GetEnabledButtons();
    }

    public void CloseCredits()
    {
        creditsMenu.gameObject.SetActive(false);
        mainMenu.gameObject.SetActive(true);
        uiController.GetEnabledButtons();
    }

    public void ToggleFullScreen()
    {
        FullScreen = 1 - FullScreen;
    }

    public void SetResolution()
    {
        var selectedResolutionWidth = resolutions[resolutionDropdown.value].width;
        var selectedResolutionHeight = resolutions[resolutionDropdown.value].height;
        Resolution = new int[]{selectedResolutionWidth, selectedResolutionHeight};
    }

    public void ToggleShowLastShot()
    {
        showLastShotIcon.sprite = PlayerPrefs.GetInt("ShowLastShot", 1) == 1 ? enabledToggleSprite : disabledToggleSprite;
    }
}