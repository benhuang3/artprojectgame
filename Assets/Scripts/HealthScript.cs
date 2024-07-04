using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 


public class HealthScript : MonoBehaviour
{
    public Slider slider;
    public GameOverScript GameOverScript_reference;

    public void setHealth(int health)
    {
        slider.value = health;
    }
    public void takeDamage(int damage)
    {
        slider.value -= damage;
        if (slider.value <= 0)
        {
            GameOverScript_reference.showGameOver(true);

        }
    }
}
