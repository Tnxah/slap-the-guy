using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuAudioController : MonoBehaviour
{
    private AudioSource audioSource;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        audioSource = GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
    }

    void OnDisable()
    {
        SceneManager.activeSceneChanged -= OnActiveSceneChanged;
    }

    void OnActiveSceneChanged(Scene oldScene, Scene newScene)
    {
        if(!audioSource.isPlaying && newScene.name != "Battleground")
        {
            audioSource.Play();
        }

        if(newScene.name == "Battleground")
        {
            audioSource.Stop();
        }
    }
}
