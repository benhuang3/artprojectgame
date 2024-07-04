using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    // combine PlayerMovement and CharacterControlled2D scripts

    // Start is called before the first frame update
    public CharacterController2D controller;
    public float runSpeed = 40f;
    private float xAxis, yAxis;
    bool jump = false;
    bool attackBool = false;


    // Update is called once per frame
    void Update()
    {
        xAxis = Input.GetAxisRaw("Horizontal") ;
        yAxis = Input.GetAxisRaw("Vertical") ;
        if (Input.GetButtonDown("Jump"))
        {
            jump = true; 
        }
        if (Input.GetKeyDown("z"))
        {
            attackBool = true; 
        }

    }
    void FixedUpdate()
    {
        controller.Move((xAxis * runSpeed) * Time.fixedDeltaTime, false, jump);
        controller.Attack(attackBool, xAxis, yAxis);

        jump = false;
        attackBool = false;
    }
}
