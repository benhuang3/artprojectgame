using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;  
using UnityEngine.UI;              
using UnityEngine.Audio;            
using UnityEngine.EventSystems;    
using UnityEngine.Video;

public class PlayerMovement : MonoBehaviour
{
    // combine PlayerMovement and CharacterControlled2D scripts

    // Start is called before the first frame update
    public CharacterController2D controller;
    public GameObject m_player;



    public float runSpeed = 40f;
    private float xAxis, yAxis;
    bool jump = false;
    bool attackBool = false;
    bool dashBool = false;

    void Start()
    {
        m_player = gameObject; //player is gameboject bird2
    }


    // Update is called once per frame
    void Update()
    {
        xAxis = Input.GetAxisRaw("Horizontal") ;
        yAxis = Input.GetAxisRaw("Vertical") ;
        if (Input.GetButtonDown("Jump"))
        {
            jump = true; 
        }
        if (Input.GetButtonDown("Attack"))
        {
            attackBool = true; 
        }
        if (Input.GetKeyDown("c") )
        {
            dashBool = true; 
        }
    }
    void FixedUpdate()
    {
      
        controller.Move((xAxis * runSpeed) * Time.fixedDeltaTime,  jump);
        controller.Attack(attackBool, xAxis, yAxis);
        controller.Dash(dashBool);

        dashBool = false;
        jump = false;
        attackBool = false;
        
        
    }
}
