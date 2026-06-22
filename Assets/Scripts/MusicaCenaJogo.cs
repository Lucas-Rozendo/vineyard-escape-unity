using UnityEngine;

public class MusicaCenaJogo : MonoBehaviour
{
    void Start()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.TocarMusicaJogo();
        }
    }
}