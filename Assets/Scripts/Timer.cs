using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [SerializeField] private Text m_timer;
    private float m_time = 0;

    private bool m_paused = false;

    public CanvasGroup canvasGroup;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (Savedata.Continue) m_time = Savedata.SavedTime;
        Savedata.OnSave += saveTime;
    }

    private void OnDestroy()
    {
        Savedata.OnSave -= saveTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_paused == false)
        {
            m_time += Time.deltaTime * 1000;
            m_timer.text = FormatTime(m_time);
        }
    }
    private void saveTime()
    {
        Savedata.SaveTime(m_time);
    }
    public void Pause()
    {
        m_paused = !m_paused;
    }
    string FormatTime(float time)
    {
        int minutes = (int)time / 60000;
        int hours = (int)minutes / 60;
        int seconds = (int)time / 1000 - 60 * minutes;
        int milliseconds = (int)time - minutes * 60000 - 1000 * seconds;
        return string.Format("{0:00}:{1:00}:{2:00}:{3:000}", hours, minutes % 60, seconds, milliseconds);
    }

    public string GetTimeString()
    {
        return FormatTime(m_time * 1000);
    }
}
