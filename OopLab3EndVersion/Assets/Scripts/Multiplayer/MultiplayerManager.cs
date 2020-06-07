using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using UnityEngine.Tilemaps;
using MazeDll;
using UnityEngine.SceneManagement;

public class MultiplayerManager : Multiplayer
{
   
 
    bool PlayerIsOff = false;

    bool isPaused = false;

    public int divisor = 20;

    public Animator playerAnimator;
    public Animator enemyAnimator;

    public bool playerIsIdle = true;
   
    private void Start()
    { 
        controlManager.moveUp = this.moveUp;
        controlManager.moveDown = this.moveDown;
        controlManager.moveRight = this.moveRight;
        controlManager.moveLeft = this.moveLeft;
        (controlManager as MultiplayerControlManager).pauseMake = GetPaused;

        playerIsIdle = true;
        playerDirectionX = 0;
        playerDirectionY = -1;

        enemyIsIdle = true;
        enemyDirectionX = 0;
        enemyDirectionY= -1;

       
        steps = 0;

       
    }

    // Update is called once per frame
    void Update()
    {

        enemyAnimator.SetFloat("Horizontal", enemyDirectionX);
        enemyAnimator.SetFloat("Vertical", enemyDirectionY);
        enemyAnimator.SetBool("Idling", enemyIsIdle);


        while (stream == null)
            return;

        spend_time += Time.deltaTime;

        Debug.Log(status);

        (controlManager as MultiplayerControlManager).canMove = true;

        playerDirectionX = controlManager.playerDirectionX;
        playerDirectionY = controlManager.playerDirectionY;
        playerIsIdle = controlManager.playerIsIdle;


        if (EnemyIsOff)
        {
            client.Close();
            writer.Close();
            reader.Close();
            stream = null;
            uiManager.SetInfoText(true, enemyName, EnemyIsOff);
            (controlManager as MultiplayerControlManager).canMove = false;
            return;
        }

        if (status == 1 || isWin)
        {
            if (isWin)
                SendMessage();
            uiManager.SetInfoText(isWin, enemyName);
            Debug.Log("here to iopen result panel");
            (controlManager as MultiplayerControlManager).canMove = false;
            Disconnect();

            return;
        }

        player.GetComponent<Transform>().position = new Vector3(palyerCurrentX, palyerCurrentY, 0);

        SendMessage();
        enemy.GetComponent<Transform>().position = new Vector3(enemyCurrentX, enemyCurrentY, 0);
        DepictUnpickedKey();
        DepictUnpickedDoor();

        if (PlayerIsOff)
        {
            Disconnect();
            SceneManager.LoadScene(0);
        }

    }
    private void FixedUpdate()
    {
        uiManager.UpdateSteps(Mathf.Round(controlManager.steps / divisor).ToString());
        uiManager.UpdateTime(Mathf.Round(spend_time).ToString());

    }

    void GetPaused()
    {
        isPaused = !isPaused;
    }

    void SendMessage()
    {
        if (writer == null)
            return;

        if (isWin)
            status = 1;
        else if (PlayerIsOff)
            status = 2;

        writer.Write(status);

        writer.Write(PickedKey);
        writer.Write(OpenedDoor);

        writer.Write(playerDirectionX);
        writer.Write(playerDirectionY);

        writer.Write(playerIsIdle);

        writer.Write(palyerCurrentX);
        writer.Write(palyerCurrentY);

    }

    void DepictUnpickedKey()
    {
        if (PickedKey != 'q')
        {
            Point key = (from List<Point> itmes in labr.maze
                                    from Point k in itmes
                                    where k.door_key == PickedKey
                                    select k).ToArray()[0];
            if (key != null)
                if (!enemyKeys.Contains(key))
                    enemyKeys.Add(key);
        }

        if (enemyKeys.Count > 0)
        {
            foreach (var en_key in enemyKeys)
            {
                if (!keys.Contains(en_key.door_key))
                {
                    switch (en_key.door_key)
                    {
                        case 'a':
                            fieldManager.decorMap.SetColor(fieldManager.passMap.WorldToCell(new Vector3(en_key.x + 1, en_key.y + 1, 0)), Color.red * new Color(1f, 1f, 1f, 0.5f));
                            break;
                        case 'b':
                            fieldManager.decorMap.SetColor(fieldManager.passMap.WorldToCell(new Vector3(en_key.x + 1, en_key.y + 1, 0)), Color.green * new Color(1f, 1f, 1f, 0.5f));
                            break;
                        case 'c':
                            fieldManager.decorMap.SetColor(fieldManager.passMap.WorldToCell(new Vector3(en_key.x + 1, en_key.y + 1, 0)), Color.yellow * new Color(1f, 1f, 1f, 0.5f));
                            break;
                    }
                }
            }

        }
    }

    void DepictUnpickedDoor()
    {

        if (OpenedDoor != 'Q')
        {
            var key = (from List<Point> itmes in labr.maze
                       from Point k in itmes
                       where k.door_key == OpenedDoor
                       select k).ToArray();
            Point dor;
            if (key.Length > 0)
                dor = key[0];
            else
                return;

            if (key != null)
                if (!enemyDoors.Contains(dor))
                    enemyDoors.Add(dor);
        }

        if (enemyDoors.Count > 0)
        {
            foreach (var en_key in enemyDoors)
            {
                if (!playerDoors.Contains(en_key.door_key))
                {
                    switch (en_key.door_key)
                    {
                        case 'A':
                            fieldManager.decorMap.SetColor(fieldManager.passMap.WorldToCell(new Vector3(en_key.x + 1, en_key.y + 1, 0)), Color.red * new Color(1f, 1f, 1f, 0.5f));
                            break;
                        case 'B':
                            fieldManager.decorMap.SetColor(fieldManager.passMap.WorldToCell(new Vector3(en_key.x + 1, en_key.y + 1, 0)), Color.green * new Color(1f, 1f, 1f, 0.5f));
                            break;
                        case 'C':
                            fieldManager.decorMap.SetColor(fieldManager.passMap.WorldToCell(new Vector3(en_key.x + 1, en_key.y + 1, 0)), Color.yellow * new Color(1f, 1f, 1f, 0.5f));
                            break;
                    }
                }
            }

        }
    }

    public void Exit()
    {
        if (!isWin && !isLoose)
        {
            PlayerIsOff = true;
        }
    }

    public void Disconnect()
    {
        client.Close();
        reader.Close();
        writer.Close();
        reader = null;
        writer = null;
        stream = null;

        SetResults();
    }
}
