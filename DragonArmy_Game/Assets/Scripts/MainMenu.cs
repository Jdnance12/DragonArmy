using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update

    public void PlayGame()
    {
        SceneManager.LoadScene("TheDungeon");
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    public void MainMenuButton()
    {
        SceneManager.LoadScene("Main Menu");
    }
    public void CreditsButton()
    {
        SceneManager.LoadScene("Credits");
    }
}