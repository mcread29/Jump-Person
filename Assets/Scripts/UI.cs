using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour
{

    private static UI m_instance;
    public static UI Instance
    {
        get => m_instance;
    }

    [SerializeField] private FinalScreen m_finalScreen;

    [SerializeField] private Timer m_timer;
    [SerializeField] private CanvasGroup m_overlay;
    [SerializeField] private Transform m_pauseScreen;

    [SerializeField] private Toggle m_muteToggle;

    private bool m_paused = false;
    public bool Paused { get => m_paused; }
    private bool m_settingsActive = false;

    public static System.Action<bool> PauseAction;

    private bool m_showingFinalScreen = false;

    public static bool CanPause = true;

    private void Awake()
    {
        if (m_instance != null)
        {
            Destroy(m_instance.gameObject);
        }
        m_instance = this;
    }

    private void Start()
    {
        m_muteToggle.isOn = AudioManager.Instance.Muted;
    }

    public void Pause()
    {
        if (m_showingFinalScreen || CanPause == false) return;

        if (m_settingsActive == false)
        {
            m_paused = !m_paused;
            m_timer.Pause();
            if (m_paused)
            {
                AudioManager.Instance.OpenMenu();
                Cursor.lockState = CursorLockMode.None;
                Go.to(m_overlay, 0.25f, new GoTweenConfig().addTweenProperty(new ActionTweenProperty(0, 1, (val) => m_overlay.alpha = val)));
                Go.to(m_pauseScreen, 0.225f, new GoTweenConfig().scale(1).setEaseType(GoEaseType.BackOut).setDelay(0.15f));
            }
            else
            {
                AudioManager.Instance.CloseMenu();
                Cursor.lockState = CursorLockMode.Locked;
                Go.to(m_overlay, 0.25f, new GoTweenConfig().addTweenProperty(new ActionTweenProperty(1, 0, (val) => m_overlay.alpha = val)));
                Go.to(m_pauseScreen, 0.225f, new GoTweenConfig().scale(0).setEaseType(GoEaseType.BackIn));
            }
            if (PauseAction != null) PauseAction(m_paused);
            Cursor.visible = m_paused;
        }
    }

    public void ShowTimer(bool show)
    {
        int finalAlpha = show ? 1 : 0;
        int startAlpha = show ? 0 : 1;

        Go.to(m_timer.canvasGroup, 0.25f, new GoTweenConfig().addTweenProperty(new ActionTweenProperty(startAlpha, finalAlpha, (val) => m_timer.canvasGroup.alpha = val)));
    }

    public void Quit()
    {
        AudioManager.Instance.SelectMenu();
        SceneManager.LoadScene("Menu");
    }

    public void ShowFinalScreen()
    {
        AudioManager.Instance.FinishGame();
        if (PauseAction != null) PauseAction(m_paused);
        m_showingFinalScreen = true;
        m_paused = true;
        m_timer.Pause();
        m_finalScreen.SetTime(m_timer.GetTimeString());
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = m_paused;

        Go.to(m_overlay, 0.25f, new GoTweenConfig().addTweenProperty(new ActionTweenProperty(0, 1, (val) => m_overlay.alpha = val)));
        Go.to(m_finalScreen.transform, 0.225f, new GoTweenConfig().scale(1).setEaseType(GoEaseType.BackOut).setDelay(0.15f));
    }
}
