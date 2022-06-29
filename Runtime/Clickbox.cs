using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Clickbox : EventTrigger
{
    private UnityEvent onClick = new UnityEvent();

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);

        onClick?.Invoke();
    }

    public void AddListener(UnityAction listener)
    {
        onClick.AddListener(listener);
    }
    public void RemoveListener(UnityAction listener)
    {
        onClick.RemoveListener(listener);
    }
    public void RemoveAllListeners()
    {
        onClick.RemoveAllListeners();
    }
}
