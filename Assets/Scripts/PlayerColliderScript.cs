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
    public GameObject playerObject;
    public Rigidbody2D m_Rigidbody2D;
    public GameObject PlayerCollider;

    public CharacterController2D CharacterController2D_reference;
    public PlayerMovement PlayerMovementscript;
    bool inHitstun = false;
    bool isInvincible = false;
    private Color originalColor;


    void Awake()
    {
        healthscript  = GameObject.FindWithTag("HealthOverlay").GetComponent<HealthScript>();
        originalColor = playerObject.GetComponent<SpriteRenderer>().color;

    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("EnemyCollider") &&  ! isInvincible)
        {
			healthscript.takeDamage(1);
            m_Rigidbody2D.constraints &= ~RigidbodyConstraints2D.FreezePositionY; // stops dash antigravity

            StartCoroutine(startHitstun(0.25f));
            StartCoroutine(startInvincible(3.5f));
            m_Rigidbody2D.velocity = Vector3.zero;

            if (collider.transform.position.x <= gameObject.transform.position.x)
            {
                CharacterController2D_reference.playerSetVelocity(new Vector3(11f, 8, 0));
                Debug.Log("Collided with enemy from right");
                // Debug.Log(player.velocity);
		    }
            else if (collider.transform.position.x > gameObject.transform.position.x)
            {
                CharacterController2D_reference.playerSetVelocity(new Vector3(-11f, 8, 0));
                Debug.Log("Collided with enemy from left");
            }
        }
    }

    public bool returnInHitstun()
    {
        return inHitstun;
    }

    IEnumerator startHitstun(float time)
    {
        inHitstun = true;
        CharacterController2D_reference.changeInControl(false);

        yield return new WaitForSeconds(time);

        inHitstun = false;
        CharacterController2D_reference.changeInControl(true);
    }

    IEnumerator startInvincible(float time)
    {
        PlayerCollider.GetComponent<BoxCollider2D>().enabled = false;
        Debug.Log("invincible");
        ChangeColor(Color.blue);

        yield return new WaitForSeconds(time);

        PlayerCollider.GetComponent<BoxCollider2D>().enabled = true;
        Debug.Log("not invincible");
        ChangeColor(originalColor);


    }

    public void ChangeColor(Color newColor)
    {
        playerObject.GetComponent<SpriteRenderer>().color = newColor;
    }

    


}


