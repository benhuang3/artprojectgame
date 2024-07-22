using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;  
using UnityEngine.UI;              
using UnityEngine.Audio;            
using UnityEngine.EventSystems;    
using UnityEngine.Video;            

public class PlayerColliderScript : MonoBehaviour
{
    

    public HealthScript healthscript;
    public Rigidbody2D player;
    public CharacterController2D CharacterController2D_reference;
    public PlayerMovement PlayerMovementscript;
    bool inHitstun = false;

    void Awake()
    {
        healthscript  = GameObject.FindWithTag("HealthOverlay").GetComponent<HealthScript>();

    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Enemy"))
        {
			healthscript.takeDamage(1);
            StartCoroutine(hitstun());
            player.velocity = Vector3.zero;

            if (collider.transform.position.x <= gameObject.transform.position.x)
            {
                CharacterController2D_reference.playerSetVelocity(new Vector3(3.5f, 5, 0));
                Debug.Log("Collided with enemy from right");
                // Debug.Log(player.velocity);
		    }
            else if (collider.transform.position.x > gameObject.transform.position.x)
            {
                CharacterController2D_reference.playerSetVelocity(new Vector3(-3.5f, 5, 0));
                Debug.Log("Collided with enemy from left");
            }
        }
    }

    public bool returnInHitstun()
    {
        return inHitstun;
    }

    IEnumerator hitstun()
    {
        inHitstun = true;
        CharacterController2D_reference.changeInControl(false);
        yield return new WaitForSeconds(0.25f);
        inHitstun = false;
        CharacterController2D_reference.changeInControl(true);
    }

    


}


