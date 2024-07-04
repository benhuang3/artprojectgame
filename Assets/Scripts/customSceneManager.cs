using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 
 
public class customSceneManager : MonoBehaviour
{
    public GameOverScript GameOverScript_reference;

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);


    }
    public void quitGame()
    {
        #if UNITY_EDITOR
            // Stop playing the scene
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            // Quit the application
            Application.Quit();
        #endif
    }
}
