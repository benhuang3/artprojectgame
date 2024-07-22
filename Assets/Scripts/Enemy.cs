using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy : MonoBehaviour

{
    // Start is called before the first frame update
    [SerializeField] int health;
    
    public CharacterController2D CharacterController2D_reference;
    public PlayerMovement PlayerMovementscript;


    private Rigidbody2D player;


    private Rigidbody2D enemy_Rigidbody2D;
    private float enemyPosition_x;
    private float playerPosition_x;
    private bool enemyControl = true;


    void Awake()
    {
        // Get the Rigidbody2D component attached to this GameObject
        enemy_Rigidbody2D = GetComponent<Rigidbody2D>();
        player = GameObject.FindWithTag("Player").GetComponent<Rigidbody2D>();
        // player_Rigidbody2D
    }
    // PlayerMovementscript.playerObject.GetComponent<Rigidbody2D>()

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    void FixedUpdate()
    {
        enemyPosition_x = enemy_Rigidbody2D.transform.position.x;
        playerPosition_x = player.transform.position.x;

        // Debug.Log("player:" + playerPosition_x.ToString() + "/ enemy: " + enemyPosition_x.ToString());
        if (enemyControl)
        {
            if (enemyPosition_x < playerPosition_x)
            {
                enemy_Rigidbody2D.velocity = new Vector3(3,-1,0);
            }
            else if (enemyPosition_x > playerPosition_x)
            {
                enemy_Rigidbody2D.velocity = new Vector3(-3,-1,0);
            }
            else if (enemyPosition_x == playerPosition_x)
            {
                enemy_Rigidbody2D.velocity = new Vector3(0, -2.5f,0);
            }
        }
        
        
    }
    
    public void EnemyGetHit ( int damageDone, string direction)
    {
        health = health - damageDone;
        StartCoroutine(enemyHitstun());
        // Debug.Log("Enemy health: " + health.ToString());
        switch (direction)
        {
        //enemy knockback
            case "side":
                if (CharacterController2D_reference.m_FacingRight)
                {
                    enemy_Rigidbody2D.AddForce(new Vector2(30.0f, 0.0f), ForceMode2D.Impulse);
                    Debug.Log("Hit right");

                }
                else if (!(CharacterController2D_reference.m_FacingRight))
                {
                    enemy_Rigidbody2D.AddForce(new Vector2(-30.0f, 0.0f), ForceMode2D.Impulse);
                    Debug.Log("Hit left");

                }
                break;

            case "up":
                enemy_Rigidbody2D.AddForce(new Vector2(0.0f, 30.0f) , ForceMode2D.Impulse);
                Debug.Log("Hit up");
                break;

            case "down":
                enemy_Rigidbody2D.AddForce(new Vector2(0.0f, -30.0f), ForceMode2D.Impulse);
                    Debug.Log("Hit down");

                break;
        }
    }

    IEnumerator enemyHitstun()
    {
        Debug.Log("e hit");
        enemyControl = false;
        yield return new WaitForSeconds(0.2f);
        enemyControl = true;



    }
}
