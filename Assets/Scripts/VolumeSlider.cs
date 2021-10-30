using DigitalRuby.SoundManagerNamespace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeSlider : MonoBehaviour
{   public void OnVolumeChanged(float volume)
    {
        AudioManager.Instance.SetVolume(volume);
    }
}
