using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [SerializeField] private Color m_hoverOverColor;
    [SerializeField] private Color m_defaultColor;
    [SerializeField] private Color m_disabledColor;
    [SerializeField] private Text m_confirmText;
    [SerializeField] private Text m_quitText;

    [SerializeField] private Text m_continueText;
    private bool m_continueEnabled = false;

    private void Start()
    {
        AudioManager.Instance.PlayMenuMusic();
        m_continueEnabled = Savedata.Load();
        if (m_continueEnabled == false) m_continueText.color = m_disabledColor;
    }

    public void HoverOnContinue()
    {
        if (m_continueEnabled) m_continueText.color = m_hoverOverColor;
    }
    public void HoverOffContinue()
    {
        if (m_continueEnabled) m_continueText.color = m_defaultColor;
    }
    public void Continue()
    {
        if (m_continueEnabled)
        {
            AudioManager.Instance.SelectMenu();
            Savedata.Continue = true;
            SceneManager.LoadScene("Main");
        }
    }

    public void HoverOnYes()
    {
        m_confirmText.color = m_hoverOverColor;
    }
    public void HoverOffYes()
    {
        m_confirmText.color = m_defaultColor;
    }
    public void StartGame()
    {
        AudioManager.Instance.SelectMenu();
        Savedata.Continue = false;
        SceneManager.LoadScene("Main");
    }

    public void HoverOnNo()
    {
        m_quitText.color = m_hoverOverColor;
    }
    public void HoverOffNo()
    {
        m_quitText.color = m_defaultColor;
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
