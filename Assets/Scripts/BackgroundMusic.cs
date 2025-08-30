using UnityEngine;

/// <summary>
/// Ensures that background music plays across scenes without interruption
/// and that there is only one instance of the music player.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class BackgroundMusic : MonoBehaviour
{
    private static BackgroundMusic instance;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            // If an instance of BackgroundMusic already exists,
            // destroy the old one to replace it with the new one.
            Destroy(instance.gameObject);
            instance = this;
        }
        else if (instance == null)
        {
            instance = this;
        }

        DontDestroyOnLoad(this.gameObject);
    }
}
