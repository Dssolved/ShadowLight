using UnityEngine;

public class SaveOnPause : MonoBehaviour
{
    void OnApplicationPause(bool paused)
    {
        if (paused) SaveSystem.Save();
    }
}
