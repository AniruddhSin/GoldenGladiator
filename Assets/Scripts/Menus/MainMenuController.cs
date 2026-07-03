using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject creditsPanel;
    [SerializeField] private GameObject tutorialPanel;
    public void OnStartClick()
    {
        SceneManager.LoadScene("PlayScene");
    }
    public void OnExitClick()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }

    public void OpenCredits()
    {
        mainMenuPanel.SetActive(false);
        creditsPanel.SetActive(true);
    }
    public void OpenMainMenu()
    {
        tutorialPanel.SetActive(false);
        creditsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
    public void OpenTutorial()
    {
        mainMenuPanel.SetActive(false);
        tutorialPanel.SetActive(true);
    }
}
