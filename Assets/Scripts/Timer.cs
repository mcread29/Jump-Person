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
        Debug.Log(canvasGroup.gameObject.name);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_paused == false)
        {
            m_time += Time.deltaTime;
            m_timer.text = FormatTime(m_time * 1000);
        }
    }
    public void Pause()
    {
        m_paused = !m_paused;
    }
    string FormatTime(float time)
    {
        int hours = (int)time / 6000;
        int minutes = (int)time / 60000;
        int seconds = (int)time / 1000 - 60 * minutes;
        int milliseconds = (int)time - minutes * 60000 - 1000 * seconds;
        return string.Format("{0:00}:{0:00}:{1:00}:{2:000}", hours, minutes, seconds, milliseconds);
    }
}
