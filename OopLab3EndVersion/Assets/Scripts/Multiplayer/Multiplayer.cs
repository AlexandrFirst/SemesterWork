using MazeDll;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class Multiplayer : PlayerManager
{
    public MultiplayerUIManager uiManager;
    protected TcpClient client;
    protected NetworkStream stream;

    protected BinaryWriter writer;
    protected BinaryReader reader;

    protected float enemyCurrentX;
    protected float enemyCurrentY;

    public GameObject player;
    public GameObject enemy;

    bool isHost;

    // protected List<char> player_keys;
    protected List<char> playerDoors;
    protected List<Point> enemyKeys;
    protected List<Point> enemyDoors;
    List<char> OpenedDoors;

    protected char OpenedDoor;
    protected char PickedKey;

    protected Labr labr;

    int LabrSize;
    int realSizeX = 19;
    int realSizeY = 19;

    protected string yourName;
    protected string enemyName;



    protected int playerDirectionX = 0;
    protected int playerDirectionY = 0;

    protected int enemyDirectionX = 0;
    protected int enemyDirectionY = 0;

    protected int status = 0;
    protected bool enemyIsIdle = true;
    protected bool isLoose = false;
    protected bool EnemyIsOff = false;



    public void Init(TcpClient client, bool isHost)
    {
        this.client = client;
        this.isHost = isHost;
        stream = this.client.GetStream();
        writer = new BinaryWriter(stream);
        reader = new BinaryReader(stream);

        keys = new List<char>();
        playerDoors = new List<char>();
        OpenedDoors = new List<char>();

        enemyKeys = new List<Point>();
        enemyDoors = new List<Point>();

        OpenedDoor = 'Q';
        PickedKey = 'q';

        realSizeX = MultiplayerConfig.Width;
        realSizeY = MultiplayerConfig.Height;
        
        if (isHost)
        {
            labr = new Labr();

            Debug.Log(realSizeX);
            Debug.Log(realSizeY);

            fieldManager.InitField(realSizeX, realSizeY, this);

            labr.maze = fieldManager.maze;
            labr.start_x = fieldManager.startX;
            labr.start_y = fieldManager.startY;

            byte[] array = ConvertToByteArray(labr);
            LabrSize = array.Length;
            Debug.Log(LabrSize);
            writer.Write(LabrSize);

            yourName = MultiplayerConfig.Name;

            writer.Write(array, 0, array.Length);

        }
        else
        {
            yourName = MultiplayerConfig.Name;

            LabrSize = reader.ReadInt32();
            Debug.Log(LabrSize);
            labr = GetLabr(LabrSize);
            realSizeX = labr.maze[0].Count;
            realSizeY = labr.maze.Count;
        }

        maze = labr.maze;

        palyerCurrentX = labr.start_x + 1.5f;
        palyerCurrentY = labr.start_y + 1.5f;

        enemyCurrentX = labr.start_x + 1.5f;
        enemyCurrentY = labr.start_y + 1.5f;

        fieldManager.DisplayMaze(labr.maze, realSizeX, realSizeY);

        if (yourName != null)
            writer.Write(yourName);

        enemyName = reader.ReadString();


        player.GetComponent<Transform>().position = new Vector3(palyerCurrentX, palyerCurrentY, 0);

        enemy.GetComponent<Transform>().position = new Vector3(enemyCurrentX, enemyCurrentY, 0);

        Thread thread = new Thread(new ThreadStart(receiveMessage));
        thread.Start();

        uiManager.SetNames(yourName, enemyName);

    }

    byte[] ConvertToByteArray(Labr labr)
    {
        byte[] data = Serializator.ObjectToByteArray(labr);
        return data;
    }

    Labr GetLabr(int LabrSize)
    {
        byte[] data = new byte[LabrSize];

        reader.Read(data, 0, LabrSize);

        Labr result = Serializator.ByteArrayToObject(data) as Labr;

        return result;
    }

    protected override bool CheckIfWin()
    {
        return OpenedDoors.Count == 3;
    }

    public override void PlayerInit(FieldManager fieldManager)
    {
        return;
    }
    protected override bool NotifyOpenedDoor(Vector3Int coords, int i)
    {
        OpenedDoor = labr.maze[coords.y - 1][coords.x - 1].door_key;
        fieldManager.decorMap.SetTile(coords, null);
        if (!OpenedDoors.Contains(OpenedDoor))
            OpenedDoors.Add(OpenedDoor);

        countDoorsOpen++;
        return true;
    }

    protected override void SetResults()
    {
        DateTime dateTime = DateTime.UtcNow.Date;
        ResultStatConfig.Date = dateTime.ToString("d");
        ResultStatConfig.Name = MultiplayerConfig.Name;
        ResultStatConfig.OpponetName = enemyName;
        ResultStatConfig.Result = isWin ? "Win" : "Lose";
        ResultStatConfig.Steps = controlManager.steps.ToString();
        ResultStatConfig.Time = spend_time.ToString();
        ResultStatConfig.Type = "Multiplayer";

        StatisticHelper.WriteResult();
    }

    void receiveMessage()
    {
        while (true)
        {
            if (reader != null)
            {


                status = reader.ReadInt32();

                if (status == 1)
                    isLoose = true;
                else if (status == 2)
                    EnemyIsOff = true;

                PickedKey = reader.ReadChar();
                OpenedDoor = reader.ReadChar();


                enemyDirectionX = reader.ReadInt32();
                enemyDirectionY = reader.ReadInt32();

                enemyIsIdle = reader.ReadBoolean();

                enemyCurrentX = reader.ReadSingle();
                enemyCurrentY = reader.ReadSingle();
            }
        }
    }

    protected override void AssignPickedKey(char key)
    {
        PickedKey = key;
    }
}
