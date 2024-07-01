using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lavascript : MonoBehaviour
{
      // This function is called when another collider enters the trigger collider attached to the object
    // where this script is attached.
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collider belongs to the player
        if (other.gameObject.CompareTag("Player"))
        {
            // Handle the collision with the player, e.g., reduce player's health or trigger death
            Debug.Log("Player hit the lava!");
            // Here you can add your logic to deal damage or any other consequence
        }
    }
}
