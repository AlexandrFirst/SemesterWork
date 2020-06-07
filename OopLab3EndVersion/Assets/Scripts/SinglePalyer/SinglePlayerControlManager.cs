using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinglePlayerControlManager : ControlManager
{
    int palyerCurrentX;
    int palyerCurrentY;

    Func<bool> CheckAndPick_Key;
   

    float horizontal;
    float vertiacal;

    public void InitDels( params Func<GameObject, bool>[] actions)
    {
        moveUp = actions[0];
        moveDown = actions[1];
        moveLeft = actions[2];
        moveRight = actions[3];
      

    }

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

        horizontal = 0;
        vertiacal = -1;
        animator.SetBool("Idling", true);
    }

    // Update is called once per frame
    void Update()
    {

        animator.SetFloat("Horizontal", playerDirectionX);
        animator.SetFloat("Vertical", playerDirectionY);
        animator.SetBool("Idling", playerIsIdle);
        Movement(gameObject);
    }
}
