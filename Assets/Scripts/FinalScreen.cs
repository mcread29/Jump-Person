using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FinalScreen : MonoBehaviour
{
    [SerializeField] private Text m_timerText;

    [SerializeField] private Color m_hoverOverColor;
    [SerializeField] private Color m_defaultColor;
    [SerializeField] private Text m_confirmText;
    [SerializeField] private Text m_denyText;

    public void SetTime(string time)
    {
        m_timerText.text = time;
    }

    public void HoverOnYes()
    {
        m_confirmText.color = m_hoverOverColor;
    }
    public void HoverOffYes()
    {
        m_confirmText.color = m_defaultColor;
    }
    public void HoverOnNo()
    {
        m_denyText.color = m_hoverOverColor;
    }
    public void HoverOffNo()
    {
        m_denyText.color = m_defaultColor;
    }

    public void Replay()
    {
        AudioManager.Instance.SelectMenu();
        SceneManager.LoadScene("Main");
    }

    public void Quit()
    {
        AudioManager.Instance.SelectMenu();
        SceneManager.LoadScene("Menu");
    }
}
