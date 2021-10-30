using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuteToggle : MonoBehaviour
{
    public void MuteAudio(bool mute)
    {
        if (mute)
        {
            AudioManager.Instance.Mute();
        }
        else
        {
            AudioManager.Instance.UnMute();
        }
    }
}
