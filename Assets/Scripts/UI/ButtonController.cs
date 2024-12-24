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

    private void Start()
    {
        eventSystem = EventSystem.current;
        activeButtons = new List<Button>();
        GetEnabledButtons();
    }
    

    public void GetEnabledButtons()
    {
        activeButtons.Clear();
        foreach (var button in buttons)
        {
            if (button.gameObject.activeInHierarchy)
            {
                activeButtons.Add(button);
            }
        }
        
        ToggleSelectedButton(0);
    }

    private void ToggleSelectedButton(int selectedIndex = -1)
    {
        selectedButtonIndex = selectedIndex is < 0 or > 1 ? 1 - selectedButtonIndex : selectedIndex;
        selectedButton = activeButtons[selectedButtonIndex];
        eventSystem.SetSelectedGameObject(selectedButton.gameObject);
    }

    public void DisableControls()
    {
        activeButtons.Clear();
        eventSystem.SetSelectedGameObject(null);
    }
}
