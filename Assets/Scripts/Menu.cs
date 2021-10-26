using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [SerializeField] private Color m_hoverOverColor;
    [SerializeField] private Color m_defaultColor;
    [SerializeField] private Text m_confirmText;
    [SerializeField] private Text m_quitText;

    private void Start()
    {
        AudioManager.Instance.PlayMenuMusic();
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
