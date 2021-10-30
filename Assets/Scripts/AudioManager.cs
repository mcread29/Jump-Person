using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.SoundManagerNamespace;

public class AudioManager : MonoBehaviour
{
    private static AudioManager m_instance;
    public static AudioManager Instance { get => m_instance; }

    private void Awake()
    {
        if(m_instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        m_instance = this;
        SoundManager.SoundVolume = 0.5f;
        SoundManager.MusicVolume = 0.65f;
        DontDestroyOnLoad(this.gameObject);
    }

    [SerializeField] private AudioSource m_jumpCharge;
    [SerializeField] private AudioSource m_jump;
    [SerializeField] private AudioSource m_openMenu;
    [SerializeField] private AudioSource m_closeMenu;
    [SerializeField] private AudioSource m_selectMenu;
    [SerializeField] private AudioSource m_finishGame;
    [SerializeField] private AudioSource m_menuMusic;
    [SerializeField] private AudioSource m_gameMusic;

    float m_soundVolume = 0.5f;
    float m_musicVolume = 0.65f;

    private bool m_muted = false;
    public bool Muted { get => m_muted; }

    public void Mute()
    {
        m_muted = true;
        SoundManager.SoundVolume = 0;
        SoundManager.MusicVolume = 0;
    }
    public void UnMute()
    {
        m_muted = false;
        SoundManager.SoundVolume = m_soundVolume;
        SoundManager.MusicVolume = m_musicVolume;
    }
    public void SetVolume(float volume)
    {
        m_soundVolume = 0.5f * volume;
        m_musicVolume = 0.65f * volume;

        SoundManager.SoundVolume = m_soundVolume;
        SoundManager.MusicVolume = m_musicVolume;
    }

    public void JumpCharge()
    {
        m_jumpCharge.PlayOneShotSoundManaged(m_jumpCharge.clip);
    }
    public void Jump()
    {
        m_jump.PlayOneShotSoundManaged(m_jump.clip);
    }
    public void OpenMenu()
    {
        m_openMenu.PlayOneShotSoundManaged(m_openMenu.clip);
    }
    public void CloseMenu()
    {
        m_closeMenu.PlayOneShotSoundManaged(m_closeMenu.clip);
    }
    public void SelectMenu()
    {
        m_selectMenu.PlayOneShotSoundManaged(m_selectMenu.clip);
    }
    public void FinishGame()
    {
        m_finishGame.PlayOneShotSoundManaged(m_finishGame.clip);
    }
    public void PlayMenuMusic()
    {
        m_menuMusic.PlayLoopingMusicManaged(1.0f, 1.0f, true);
    }
    public void PlayGameMusic()
    {
        m_gameMusic.PlayLoopingMusicManaged(1.0f, 1.0f, true);
    }
}
