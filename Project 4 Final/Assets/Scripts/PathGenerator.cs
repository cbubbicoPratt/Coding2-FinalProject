using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathGenerator : MonoBehaviour
{
    public GameObject emptyPrefab;
    public GameObject[] pathPrefabs;
    public Transform startPoint;
    public int borderWidth = 3;
    private Vector2 gridSize = new Vector2(25, 10);
    private GameObject player;

    //accessor variables
    public int pathTiles = 0;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (borderWidth * 2 >= gridSize.x) throw new System.Exception("2 * border width must be less than grid x");
        GenerateMap();
    }
    private enum Direction
    {
        Up = 0,
        Down = 1,
        Right = 2
    }

    private enum TileType
    {
        Empty = 0,
        Path = 1
    }

    private void GenerateMap()
    {
        TileType[,] map = new TileType[(int)gridSize.x, (int)gridSize.y];
        int startPosY = map.GetLength(1) / 2;
        int currentY = startPosY;
        int currentX = 0;

        Direction lastDirection = Direction.Right;
        Direction secondLastDirection = Direction.Right;

        bool IsCurrentPosOnTop(int y) => y == borderWidth;
        bool IsCurrentPosOnBottom(int y) => y == map.GetLength(1) - 1 - borderWidth;

        bool lastTileWasEmpty = false;

        map[currentX, currentY] = TileType.Path;

        while (currentX < map.GetLength(0) - 1)
        {
            List<Direction> possibleDirections = new List<Direction>() { Direction.Right };
            if (currentX > 0)
            {
                // Right direction cases
                if (lastDirection == Direction.Right && secondLastDirection == Direction.Right)
                {
                    if (IsCurrentPosOnTop(currentY)) possibleDirections.Add(Direction.Down);
                    else if (IsCurrentPosOnBottom(currentY)) possibleDirections.Add(Direction.Up);
                    else
                    {
                        possibleDirections.Add(Direction.Up);
                        possibleDirections.Add(Direction.Down);
                    }
                }
                else if (lastDirection == Direction.Right && secondLastDirection == Direction.Up)
                {
                    if (!IsCurrentPosOnTop(currentY)) possibleDirections.Add(Direction.Up);
                }
                else if (lastDirection == Direction.Right && secondLastDirection == Direction.Down)
                {
                    if (!IsCurrentPosOnBottom(currentY)) possibleDirections.Add(Direction.Down);
                }
                //Up direction cases
                else if (lastDirection == Direction.Up && secondLastDirection == Direction.Up || lastDirection == Direction.Up && secondLastDirection == Direction.Right)
                {
                    if (!IsCurrentPosOnTop(currentY)) possibleDirections.Add(Direction.Up);
                }
                //Down direction cases
                else if (lastDirection == Direction.Down && secondLastDirection == Direction.Down || lastDirection == Direction.Down && secondLastDirection == Direction.Right)
                {
                    if (!IsCurrentPosOnBottom(currentY)) possibleDirections.Add(Direction.Down);
                }

            }
            Direction direction = possibleDirections.OrderBy(x => Guid.NewGuid()).Take(1).Single();

            switch (direction)
            {
                case Direction.Up:
                    currentY--; break;
                case Direction.Down:
                    currentY++; break;
                default:
                    currentX++; break;
            }

            int randomIndex = UnityEngine.Random.Range(0, pathPrefabs.Length);
            //int emptyCheck = UnityEngine.Random.Range(0, 2);
            GameObject prefab = pathPrefabs[randomIndex];
            /*
            if(lastTileWasEmpty || emptyCheck == 0 || currentX == 1 || currentX == map.GetLength(0) - 1)
            {
                prefab = pathPrefabs[randomIndex];
                lastTileWasEmpty = false;
                map[currentX, currentY] = TileType.Path;
            } 
            else
            {
                prefab = emptyPrefab;
                lastTileWasEmpty = true;
                map[currentX, currentY] = TileType.Empty;
            }*/
            if (direction == Direction.Up) Instantiate(prefab, new Vector3(currentY * 40, -0.5f, currentX * 40), Quaternion.Euler(0, -90, 0));
            else if (direction == Direction.Down) Instantiate(prefab, new Vector3(currentY * 40, -0.5f, currentX * 40), Quaternion.Euler(0, 90, 0));
            else Instantiate(prefab, new Vector3(currentY * 40, -0.5f, currentX * 40), Quaternion.identity);
            secondLastDirection = lastDirection;
            lastDirection = direction;
            pathTiles++;
        }
    }
}
