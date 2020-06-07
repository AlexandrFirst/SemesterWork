using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using MazeDll;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Creator : MonoBehaviour
{
    public Button StartBtn;

    public GameObject PlayerGameObj;
    GameObject InstatiatedPlayer;

    public Tilemap floor;
    public Tilemap walls;
    public Tilemap decor;

    public Tile floorTile;
    public Tile wallTile;
    public Tile key;
    public Tile door;

    Tile currentTile;
    Tilemap currentTileMap;

    int sizeX = 10;
    int sizeY = 10;

    List<List<Point>> maze;

    int type = 0;


    int playerCount = 0;


    public Image Wall, Player, Key, Door,DeleteBut;

    Image currentSelected;

    Stack<char> doors = new Stack<char>();
    Stack<char> keys = new Stack<char>();
    // Start is called before the first frame update
    void Start()
    {

        sizeX = CustomField.realSizeX;
        sizeY = CustomField.realSizeY;

        StartBtn.gameObject.SetActive(false);
        maze = new List<List<Point>>();
        currentTile = floorTile;
        currentTileMap = floor;
        for (int i = 0; i < sizeY; i++)
        {
            List<Point> row = new List<Point>();
            for (int j = 0; j < sizeX; j++)
            {
                row.Add(new PointPassOrWall(' ', j, i));
                currentTileMap.SetTile(floor.WorldToCell(new Vector3(j, i, 0)), currentTile);
            }
            maze.Add(row);
        }
        currentTile = wallTile;
        currentTileMap = walls;
        type = 1;

        keys.Push('a');
        doors.Push('A');
        keys.Push('b');
        doors.Push('B');
        keys.Push('c');
        doors.Push('C');

        currentSelected = Wall;
        currentSelected.color = Color.green;
    }


    


    // Update is called once per frame
    void Update()
    {
        Vector3 posInst = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));


        if (Input.GetKey(KeyCode.Alpha1))//planting walls
        {
            currentTile = wallTile;
            currentTileMap = walls;
            type = 1;
            currentSelected.color = Color.white;
            currentSelected = Wall;
            currentSelected.color = Color.green;
        }
        if (Input.GetKey(KeyCode.Alpha2))//planting player
        {
            currentTile = null;
            currentTileMap = null;
            type = 2;
            currentSelected.color = Color.white;
            currentSelected = Player;
            currentSelected.color = Color.green;
        }
        if (Input.GetKey(KeyCode.Alpha3)) // planting key
        {

            currentTile = key;
            currentTileMap = decor;
            type = 3;

            currentSelected.color = Color.white;
            currentSelected = Key;
            currentSelected.color = Color.green;
        }
        if (Input.GetKey(KeyCode.Alpha4))//planting door
        {
            currentTile = door;
            currentTileMap = decor;
            type = 3;
            currentSelected.color = Color.white;
            currentSelected = Door;
            currentSelected.color = Color.green;
        }
        else if (Input.GetKey(KeyCode.Alpha5))//deleting element on clicking coords
        {
            type = -1;
            currentSelected.color = Color.red;
        }


        if (Input.GetMouseButtonDown(0)) //palnting...
        {


            if (((int)posInst.x < 0 || (int)posInst.x >= sizeX) || (int)posInst.y >= sizeY || (int)posInst.y < 0)
                return;


            if (type != -1)
            {
                switch (type)
                {
                    case 1://walls
                        if (maze[(int)posInst.y][(int)posInst.x] is PointPassOrWall && maze[(int)posInst.y][(int)posInst.x].symbol == ' ')
                        {
                            if (playerCount == 1)
                            {
                                if ((int)posInst.y == InstatiatedPlayer.transform.position.y-0.5f &&
                                    (int)posInst.x == InstatiatedPlayer.transform.position.x-0.5f)
                                {
                                    break;
                                }
                            }

                            currentTileMap.SetTile(currentTileMap.WorldToCell(new Vector3((int)posInst.x, (int)posInst.y, 0)), currentTile);
                            maze[(int)posInst.y][(int)posInst.x] = new PointPassOrWall('#', (int)posInst.x, (int)posInst.y);
                        }
                            //maybe make the check if it is not a doorkey and it is ' '
                        break;
                    case 2://player
                        if (playerCount == 1)
                            return;
                        if (maze[(int)posInst.y][(int)posInst.x] is PointPassOrWall && maze[(int)posInst.y][(int)posInst.x].symbol == ' ')
                        {
                            InstatiatedPlayer = Instantiate(PlayerGameObj, new Vector3((int)posInst.x + 0.5f, (int)posInst.y + 0.5f, 0), Quaternion.identity);
                            PlayerGameObj.name = "pl";
                            playerCount++;
                        }
                        break;
                    case 3://planting key and door
                        //maybe make the check if it is not a doorkey and it is ' '
                        if (maze[(int)posInst.y][(int)posInst.x] is PointPassOrWall && maze[(int)posInst.y][(int)posInst.x].symbol==' ')
                        {
                            if (playerCount == 1)
                            {
                                if ((int)posInst.y == InstatiatedPlayer.transform.position.y-0.5f &&
                                    (int)posInst.x == InstatiatedPlayer.transform.position.x - 0.5f)
                                {
                                    break;
                                }
                            }

                            char symbol = 'q';
                            if (currentTile == door)
                                if (doors.Count == 0)
                                    return;
                                else
                                {
                                    symbol = doors.Pop();
                                    maze[(int)posInst.y][(int)posInst.x] = new PointDoor(symbol, (int)posInst.x, (int)posInst.y);
                                }
                            if (currentTile == key)
                                if (keys.Count == 0)
                                    return;
                                else
                                {
                                    symbol = keys.Pop();
                                    maze[(int)posInst.y][(int)posInst.x] = new PointKey(symbol, (int)posInst.x, (int)posInst.y);
                                }

                            currentTileMap.SetTile(currentTileMap.WorldToCell(new Vector3((int)posInst.x, (int)posInst.y, 0)), currentTile);
                            currentTileMap.SetTileFlags(currentTileMap.WorldToCell(new Vector3((int)posInst.x, (int)posInst.y, 0)), TileFlags.None);
                            

                            switch (char.ToLower(symbol))
                            {
                                case 'a':
                                    currentTileMap.SetColor(currentTileMap.WorldToCell(new Vector3((int)posInst.x, (int)posInst.y, 0)), Color.red);
                                    break;
                                case 'b':
                                    currentTileMap.SetColor(currentTileMap.WorldToCell(new Vector3((int)posInst.x, (int)posInst.y, 0)), Color.green);
                                    break;
                                case 'c':
                                    currentTileMap.SetColor(currentTileMap.WorldToCell(new Vector3((int)posInst.x, (int)posInst.y, 0)), Color.yellow);
                                    break;
                            }
                        }
                        break;
                    default:
                        break;
                }



            }
            else if (currentTileMap != floor)
            {
                if (currentTileMap != null)
                {
                    if (currentTileMap != decor)
                    {
                        currentTileMap.SetTile(currentTileMap.WorldToCell(new Vector3((int)posInst.x, (int)posInst.y, 0)), null);
                        maze[(int)posInst.y][(int)posInst.x] = new PointPassOrWall(' ', (int)posInst.x, (int)posInst.y);
                        //delrting the # from the arraty
                    }
                    else
                    {
                        //next can happen if the currentTilMap is decor
                        char doorKeyEl = maze[(int)posInst.y][(int)posInst.x].door_key;
                        if (doorKeyEl != ' ')
                        {
                            if (char.IsUpper(doorKeyEl))
                            {
                                doors.Push(doorKeyEl);
                            }
                            else
                            {
                                keys.Push(doorKeyEl);
                            }
                        }
                        maze[(int)posInst.y][(int)posInst.x] = new PointPassOrWall(' ', (int)posInst.x, (int)posInst.y);
                        currentTileMap.SetTile(currentTileMap.WorldToCell(new Vector3((int)posInst.x, (int)posInst.y, 0)), null);
                    }
                }
                else if (playerCount == 1 && InstatiatedPlayer.transform.position == new Vector3((int)posInst.x + 0.5f, (int)posInst.y + 0.5f, 0))
                {
                    Destroy(InstatiatedPlayer);
                    InstatiatedPlayer = null;
                    playerCount--;
                }
            }

            Debug.Log(Input.mousePosition);

            if (keys.Count == 0 && doors.Count == 0 && playerCount == 1)
            {
                StartBtn.gameObject.SetActive(true);
            }
            else
            {
                StartBtn.gameObject.SetActive(false);
            }

        }


    }

    public void pressStartButton()
    {
        CustomField.isCustom = true;
        CustomField.field = maze;
        CustomField.startX = InstatiatedPlayer.GetComponent<Transform>().position.x;
        CustomField.startY = InstatiatedPlayer.GetComponent<Transform>().position.y;
        CustomField.realSizeX = sizeX;
        CustomField.realSizeY = sizeY;
        SceneManager.LoadScene(1);
    }
    public void pressExitButton()
    {
        CustomField.isCustom = false;
        SceneManager.LoadScene(0);
    }

}
