using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.tag + ", " + other.gameObject.name);
        if(other.tag == "Player")
        {
            UI.Instance.ShowFinalScreen();
        }
    }
}
