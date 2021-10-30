using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Save{
    public Vector3 position = Vector3.zero;
    public Vector3 rotation = Vector3.zero;
    public float time = 0f;
}


public class Savedata : MonoBehaviour
{
    private static Save m_saveData = new Save();
    public static bool Continue = false;

    public static System.Action OnSave;

    public static Vector3 SavedPosition { get => m_saveData.position; }
    public static void SavePosition(Vector3 position) { m_saveData.position = position; }
    
    public static Vector3 SavedRotation { get => m_saveData.rotation; }
    public static void SaveRotation(Vector3 rotation) { m_saveData.rotation = rotation; }

    public static void SaveTransform(Transform transform)
    {
        m_saveData.position = transform.position;
        m_saveData.rotation = transform.rotation.eulerAngles;
    }

    public static float SavedTime { get => m_saveData.time; }
    public static void SaveTime(float time) { m_saveData.time = time; }

    public static bool Load()
    {
        string save = PlayerPrefs.GetString("SaveData");
        if (save == "") return false;
        else
        {
            m_saveData = JsonUtility.FromJson<Save>(save);
            return true;
        }
    }

    public static void Save()
    {
        if (OnSave != null) OnSave();
        PlayerPrefs.SetString("SaveData", JsonUtility.ToJson(m_saveData));
    }
}
