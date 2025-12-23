using UnityEngine;

public class TutorialStartup : MonoBehaviour
{
    [SerializeField]
    private GameObject tutorialPanel, openTutorialButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (PlayerPrefs.GetInt("FIRSTTIMEOPENING", 1) == 1)
        {
            Debug.Log("First Time Opening");

            PlayerPrefs.SetInt("FIRSTTIMEOPENING", 0);

            OpenTutorial();
        }
        else
        {
            //Do your stuff here
        }
    }

    private void OpenTutorial()
    {
        tutorialPanel.SetActive(true);
        openTutorialButton.SetActive(false);
    }
}
