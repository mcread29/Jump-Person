using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SettingsText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Text m_quitText;
    [SerializeField] private Text m_quitExplain;
    [SerializeField] private Color m_quitHoverColor;
    [SerializeField] private Color m_quitOffColor;
    [SerializeField] private Color m_hoverColor;
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        m_quitText.color = m_hoverColor;
        m_quitExplain.color = m_quitHoverColor;
    }
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        m_quitExplain.color = m_quitOffColor;
        m_quitText.color = m_quitHoverColor;
    }
}
