using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;  
using UnityEngine.UI;              
using UnityEngine.Audio;            
using UnityEngine.EventSystems;    
using UnityEngine.Video;            

public class GameOverScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (gameObject != null)
        {
            gameObject.SetActive(false);
        }
    }

    public void showGameOver(bool show)
    {
        gameObject.SetActive(show);
    }
}
