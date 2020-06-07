using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using MazeDll;

public class FieldManager : MonoBehaviour
{
    public Camera Camera;

    public Tile floorTile;
    public Tile wallTile;
    public Tile doors;
    public Tile key;
    public Tilemap wallMap;
    public Tilemap passMap;
    public Tilemap decorMap;

    public List<List<Point>> maze;
    MazeGenerator labr;
    public int realSizeX;
    public int realSizeY;

    public int startX;
    public int startY;

    public Vector3 startPlayerPos;

    [HideInInspector]
    public PlayerManager plm;

    private void Start()
    {
        plm = FindObjectOfType<SinglePlayerManager>();
        if (plm!=null)
            InitField(FieldConfig.Width, FieldConfig.Height,plm);
    }

    public void InitField(int width, int height, PlayerManager playerManager)
    {
        if (!CustomField.isCustom)
        {
            if (width != 0 && height != 0)
            {
                realSizeX = 2 * width - 1;
                realSizeY = 2 * height - 1;
            }
            else
            {
                realSizeX = 19;
                realSizeY = 19;
            }
            labr = new MazeGenerator(realSizeX, realSizeY);
            this.maze = labr.maze;

            startX = labr.startX;
            startY = labr.startY;

            Camera.GetComponent<Transform>().transform.position = new Vector3(startX, startY, -10);

            startPlayerPos = wallMap.CellToWorld(new Vector3Int(startX + 1, startY + 1, 0));

            startPlayerPos += new Vector3(0.5f, 0.5f, 0);
        }
        else
        {
            maze = CustomField.field;
            startPlayerPos = new Vector3(CustomField.startX+1, CustomField.startY+1, 0);
            Camera.GetComponent<Transform>().transform.position = new Vector3(startX, startY, -10);
            realSizeX = CustomField.realSizeX;
            realSizeY = CustomField.realSizeY;
        }
        if (plm!=null)
        {
            DisplayMaze(maze, realSizeX, realSizeY);
            playerManager.PlayerInit(this);
        }
    }
   

    public void DisplayMaze(List<List<Point>> maze, int real_size_x, int real_size_y)
    {
        for (int i = 0; i < real_size_x + 2; i++)
        {

            wallMap.SetTile(wallMap.WorldToCell(new Vector3(i, 0, 0)), wallTile);
        }


        for (int i = 0; i < maze.Count; i++)
        {
            wallMap.SetTile(wallMap.WorldToCell(new Vector3(0, i + 1, 0)), wallTile);
            for (int j = 0; j < maze[i].Count; j++)
            {   
                if (maze[i][j].symbol == '#')
                    wallMap.SetTile(wallMap.WorldToCell(new Vector3(j + 1, i + 1, 0)), wallTile);
                else
                {
                    passMap.SetTile(passMap.WorldToCell(new Vector3(j + 1, i + 1, 0)), floorTile);
                    if (maze[i][j].door_key != ' ')
                    {
                        if (!char.IsLower(maze[i][j].door_key))
                        {
                            decorMap.SetTile(passMap.WorldToCell(new Vector3(j + 1, i + 1, 0)), doors);

                        }
                        else
                        {
                            decorMap.SetTile(passMap.WorldToCell(new Vector3(j + 1, i + 1, 0)), key);
                        }
                        decorMap.SetTileFlags(passMap.WorldToCell(new Vector3(j + 1, i + 1, 0)), TileFlags.None);
                        switch (char.ToUpper(maze[i][j].door_key))
                        {
                            case 'A':
                                decorMap.SetColor(passMap.WorldToCell(new Vector3(j + 1, i + 1, 0)), Color.red);
                                break;
                            case 'B':
                                decorMap.SetColor(passMap.WorldToCell(new Vector3(j + 1, i + 1, 0)), Color.green);
                                break;
                            case 'C':
                                decorMap.SetColor(passMap.WorldToCell(new Vector3(j + 1, i + 1, 0)), Color.yellow);
                                break;
                        }
                    }
                }
            }
            wallMap.SetTile(wallMap.WorldToCell(new Vector3(maze[i].Count + 1, i + 1, 0)), wallTile);
        }

        for (int i = 0; i < real_size_x + 2; i++)
        {
            wallMap.SetTile(wallMap.WorldToCell(new Vector3(i, maze.Count + 1, 0)), wallTile);
        }

        wallMap.GetComponent<TilemapCollider2D>().enabled = false;
        wallMap.GetComponent<TilemapCollider2D>().enabled = true;
    }

}
