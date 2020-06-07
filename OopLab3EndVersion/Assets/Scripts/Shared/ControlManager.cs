using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ControlManager : MonoBehaviour
{
    public Animator animator;

    public bool playerIsIdle = true;
    public int steps;
    public int playerDirectionX;
    public int playerDirectionY;

    public  Func<GameObject, bool> moveUp { get; set; }
    public  Func<GameObject, bool> moveDown { get; set; }
    public  Func<GameObject, bool> moveLeft { get; set; }
    public  Func<GameObject, bool> moveRight { get; set; }


    public void StepCount()
    {
        steps++;
    }


    private void Start()
    {
        steps = 0;
        playerDirectionX = 0;
        playerDirectionY = -1;
    }

    public void Movement(GameObject player)
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            if (moveUp(player))
            {
                playerIsIdle = false;
                //steps++;
            }
            else
                playerIsIdle = true;

            playerDirectionX = 0;
            playerDirectionY = 1;



        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            if (moveDown(player))
            {
                playerIsIdle = false;
                //steps++;
            }
            else
                playerIsIdle = true;

            playerDirectionX = 0;
            playerDirectionY = -1;



        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            if (moveLeft(player))
            {
                playerIsIdle = false;
                //steps++;
            }
            else
                playerIsIdle = true;

            playerDirectionX = -1;
            playerDirectionY = 0;



        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            if (moveRight(player))
            {
                playerIsIdle = false;
                //steps++;
            }
            else
                playerIsIdle = true;
            playerDirectionX = 1;
            playerDirectionY = 0;
        }

        if (!Input.anyKey)
            playerIsIdle = true;
    }
}
