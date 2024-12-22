using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private Button[] buttons;

    private EventSystem eventSystem;
    private List<Button> activeButtons;
    private int selectedButtonIndex;
    private Button selectedButton;
    private bool switchSelectedButton;
    private bool pressSelectedButton;

    private void Start()
    {
        eventSystem = EventSystem.current;
        activeButtons = new List<Button>();
        GetEnabledButtons();
    }
    
    private void Update()
    {
        if(activeButtons.Count <= 0) return;
        if(GetInput()) ControlUI();
    }

    public void GetEnabledButtons()
    {
        activeButtons.Clear();
        foreach (var button in buttons)
        {
            if (button.transform.parent.gameObject.activeSelf)
            {
                activeButtons.Add(button);
            }
        }
        
        ToggleSelectedButton(0);
    }

    private bool GetInput()
    {
        switchSelectedButton = Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow);
        pressSelectedButton = Input.GetKeyDown(KeyCode.Return);
        return switchSelectedButton || pressSelectedButton;
    }

    private void ControlUI()
    {
        if(switchSelectedButton) ToggleSelectedButton();
        if(pressSelectedButton) selectedButton.onClick.Invoke();
    }
    
    private void ToggleSelectedButton(int selectedIndex = -1)
    {
        selectedButtonIndex = selectedIndex is < 0 or > 1  ? 1 - selectedButtonIndex : selectedIndex;
        selectedButton = activeButtons[selectedButtonIndex];
        eventSystem.SetSelectedGameObject(selectedButton.gameObject);
    }

    public void DisableControls()
    {
        activeButtons.Clear();
        eventSystem.SetSelectedGameObject(null);
    }
}
