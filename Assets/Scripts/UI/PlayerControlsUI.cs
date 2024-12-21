using System;
using TMPro;
using UnityEngine;

public class PlayerControlsUI : MonoBehaviour
{
    [SerializeField] private string[] controlTexts;
    private TMP_Text textField;
    private Animator animator;

    private void Start()
    {
        textField = GetComponent<TMP_Text>();
        animator = GetComponentInParent<Animator>();
    }

    public int CurrentTextIndex
    {
        get => _currentTextIndex;
        private set
        {
            _currentTextIndex = value;
            OnIndexChanged();
        }
    }

    private int _currentTextIndex = 1;

    private void OnIndexChanged()
    {
        textField.text = controlTexts[_currentTextIndex];
    }
    
    public void DisableControls()
    {
        textField.enabled = false;
        animator.SetTrigger("Hide");
    }

    public void EnableControls()
    {
        textField.enabled = true;
        animator.SetTrigger("Show");
    }

    public void Next()
    {
        CurrentTextIndex = 1 - CurrentTextIndex;
    }

    public void GameStart()
    {
        animator.SetTrigger("Start");
    }
}