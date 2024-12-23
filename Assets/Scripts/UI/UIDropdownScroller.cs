using System.Diagnostics;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIDropdownScroller : MonoBehaviour, ISelectHandler
{
    private ScrollRect scrollRect;
    private float scrollPosition = 1;
    void Start()
    {
        scrollRect = GetComponentInParent<ScrollRect>(true);
        
        int childCount = scrollRect.content.transform.childCount;
        int childIndex = transform.GetSiblingIndex();
        
        if (childIndex == 1)
        {
            scrollPosition = 1; return;
        }
        scrollPosition  = 1 - ((float)childIndex) / (childCount-1);
    }

    private void OnEnable()
    {
        if(scrollRect) scrollRect.verticalScrollbar.value = scrollPosition;
    }

    public void OnSelect(BaseEventData eventData)
    {
        if(scrollRect) scrollRect.verticalScrollbar.value = scrollPosition;
    }
}
