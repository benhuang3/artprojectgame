using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LavaTrigger : MonoBehaviour
{
    public HealthScript healthscript;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collider belongs to the player
        if (other.gameObject.CompareTag("Player"))
        {
            // Handle the collision with the player, e.g., reduce player's health or trigger death
            Debug.Log("Player hit the lava!");
            other.transform.position = new Vector3(0, -2, 0);
            healthscript.takeDamage(1);

    

            // Here you can add your logic to deal damage or any other consequence
        }
    }
}
