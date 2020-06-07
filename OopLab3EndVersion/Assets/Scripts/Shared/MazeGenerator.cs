using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MazeDll;

public class MazeGenerator
{
    public List<List<Point>> maze = new List<List<Point>>();
    Stack<char> doorsKeys;

    public int Initial_door_count { get; private set; }
    public int startX { get; private set; }
    public int startY { get; private set; }

    int realSizeX;
    int realSizeY;

    public MazeGenerator(int size_x, int size_y)
    {
        realSizeX = size_x;
        realSizeY = size_y;

        FillArea();
        CreateMaze();
    }

    void FillArea()
    {
        for (int i = 0; i < realSizeY; i++)
        {
            List<Point> row = new List<Point>();
            for (int j = 0; j < realSizeX; j++)
            {
                row.Add(new PointPassOrWall('#', j, i));
            }
            maze.Add(row);
        }

        doorsKeys = new Stack<char>();

        //manage the number of keys(custom set)
        doorsKeys.Push('a');
        doorsKeys.Push('A');
        doorsKeys.Push('b');
        doorsKeys.Push('B');
        doorsKeys.Push('c');
        doorsKeys.Push('C');

        Initial_door_count = doorsKeys.Count / 2;
    }

    void CreateMaze()
    {
        Random random = new Random();

        int prevX = random.Next(realSizeX);
        int prevY = random.Next(realSizeY);

        startX = prevX;
        startY = prevY;

        Stack<Point> visited = new Stack<Point>();
        Point current = maze[prevY][prevX];
        current.symbol = ' ';
        visited.Push(current);
        maze[prevY][prevX] = current;
        while (visited.Count > 0)
        {
            List<int> direction = new List<int>() { 0, 1, 2, 3 }.OrderBy(x => Guid.NewGuid()).ToList();
            bool foundWay = false;

            prevX = visited.Peek().x;
            prevY = visited.Peek().y;

            foreach (int item in direction)
            {
                if (item == 0)//down
                {
                    if (prevY + 2 < realSizeY && maze[prevY + 2][prevX].symbol != ' ')
                    {
                        foundWay = true;

                        visited.Peek().down = true;

                        current = maze[prevY + 2][prevX];
                        current.symbol = ' ';
                        current.up = true;

                        maze[prevY + 1][prevX].symbol = ' ';

                        visited.Push(current);
                        break;
                    }
                }
                else if (item == 1)//up
                {
                    if (prevY - 2 >= 0 && maze[prevY - 2][prevX].symbol != ' ')
                    {
                        foundWay = true;


                        visited.Peek().up = true;

                        current = maze[prevY - 2][prevX];
                        current.symbol = ' ';
                        current.down = true;

                        maze[prevY - 1][prevX].symbol = ' ';

                        visited.Push(current);
                        break;
                    }
                }
                else if (item == 2)//left
                {
                    if (prevX - 2 >= 0 && maze[prevY][prevX - 2].symbol != ' ')
                    {
                        foundWay = true;

                        visited.Peek().left = true;

                        current = maze[prevY][prevX - 2];
                        current.symbol = ' ';
                        current.right = true;

                        maze[prevY][prevX - 1].symbol = ' ';

                        visited.Push(current);
                        break;
                    }
                }
                else if (item == 3)//right
                {
                    if (prevX + 2 < realSizeX && maze[prevY][prevX + 2].symbol != ' ')
                    {
                        foundWay = true;



                        visited.Peek().right = true;

                        current = maze[prevY][prevX + 2];
                        current.symbol = ' ';
                        current.left = true;

                        maze[prevY][prevX + 1].symbol = ' ';

                        visited.Push(current);
                        break;
                    }
                }
            }

            if (!foundWay)
            {
                if ((current.left && !current.right && !current.up && !current.down) ||
                    (!current.left && !current.right && current.up && !current.down) ||
                    (current.left && !current.right && current.up && !current.down))
                {
                    if (maze[prevY][prevX].door_key == ' ' && (current.y != prevY || current.x != prevX))
                    {
                        if (doorsKeys.Count > 0)
                        {
                            if (char.IsLower(doorsKeys.Peek()))
                            {
                                maze[prevY][prevX] = new PointKey(doorsKeys.Pop(), prevX, prevY);
                            }
                            else
                            {
                                maze[prevY][prevX] = new PointDoor(doorsKeys.Pop(), prevX, prevY);
                            }

                        }
                    }
                }
                current = visited.Pop();
            }
        }
    }
}
