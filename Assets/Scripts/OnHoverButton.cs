using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OnHoverButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] public UnityEvent m_hoverEnter;
    [SerializeField] public UnityEvent m_hoverExit;

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        if (m_hoverEnter != null) m_hoverEnter.Invoke();
    }
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        if (m_hoverExit != null) m_hoverExit.Invoke();
        // GetComponent<Button>().
    }
}
