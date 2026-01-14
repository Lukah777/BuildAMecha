using UnityEngine;
using UnityEngine.SceneManagement;

public class TheEnd : MonoBehaviour
{
    // Play button, loads the game scene
    public void MainMenu() { SceneManager.LoadScene("MainMenu", LoadSceneMode.Single); }

    // Quit button closes the application
    public void Quit() { Application.Quit(); }
}
