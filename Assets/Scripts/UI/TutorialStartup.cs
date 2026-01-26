using UnityEngine;

public class TutorialStartup : MonoBehaviour
{
    [SerializeField]
    private GameObject tutorialPanel, openTutorialButton;
    static bool tutorialShown = false;

    [SerializeField]
    private GameObject alwaysOnScreenHints;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        if (!tutorialShown)
        {
            OpenTutorial();
        }
#else
        if (PlayerPrefs.GetInt("FIRSTTIMEOPENING", 1) == 1)
        {
            OpenTutorial();
        }
        else
        {
            //Do your stuff here
        }
#endif
    }

    private void OpenTutorial()
    {
        Debug.Log("First Time Opening");

        tutorialPanel.SetActive(true);
        openTutorialButton.SetActive(false);
    }

    public void OnCloseTutorial()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        tutorialShown = true;
#else
        PlayerPrefs.SetInt("FIRSTTIMEOPENING", 0);
#endif

        alwaysOnScreenHints.SetActive(true);
    }
}
