using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathGenerator : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject emptyPrefab;
    public GameObject[] pathPrefabs;

    [Header("References")]
    public GameObject endPoint;
    public Transform startPoint;

    [Header("Grid")]
    public int borderWidth = 3;
    private Vector2 _gridSize = new Vector2(15, 10);

    //accessor variables
    public int pathTiles = 0;

    private void Awake()
    {
        //make sure width of border is actually within the size of the grid
        if (borderWidth * 2 >= _gridSize.x) throw new System.Exception("2 * border width must be less than grid x");
    }
    private enum Direction
    {
        //possible directions the next tile in the path can go
        Up = 0,
        Down = 1,
        Right = 2
    }

    private enum TileType
    {
        //tiles are either empty or filled
        Empty = 0,
        Path = 1
    }

    public void GenerateMap()
    {
        //get rid of old path in the case of round generation
        ResetPath();
        
        //new grid with map
        TileType[,] map = new TileType[(int)_gridSize.x, (int)_gridSize.y];

        //get location on grid
        int startPosY = map.GetLength(1) / 2;
        int currentY = startPosY;
        int currentX = 0;

        //previous two directions to calculate where we can go
        //default to right (path can always go right)
        Direction lastDirection = Direction.Right;
        Direction secondLastDirection = Direction.Right;

        //detect top and bottom of grid
        bool IsCurrentPosOnTop(int y) => y == borderWidth;
        bool IsCurrentPosOnBottom(int y) => y == map.GetLength(1) - 1 - borderWidth;

        //every tile on the path that is passed through is a path
        map[currentX, currentY] = TileType.Path;

        while (currentX < map.GetLength(0) - 1)
        {
            //make a list for possible directions to go for this tile
            //we can always go right because that would never cause issues with tiles packing too close
            List<Direction> possibleDirections = new List<Direction>() { Direction.Right };

            if (currentX > 0)
            {
                // Right direction cases
                if (lastDirection == Direction.Right && secondLastDirection == Direction.Right)
                {
                    //if we're on top we can't go further up
                    //likewise if we're on the bottom we can't go further down
                    if (IsCurrentPosOnTop(currentY)) possibleDirections.Add(Direction.Down);
                    else if (IsCurrentPosOnBottom(currentY)) possibleDirections.Add(Direction.Up);

                    //otherwise, any direction is fair game
                    else
                    {
                        possibleDirections.Add(Direction.Up);
                        possibleDirections.Add(Direction.Down);
                    }
                }

                //we don't want to go down if we just went up and then right
                //that would create a square of touching tiles
                else if (lastDirection == Direction.Right && secondLastDirection == Direction.Up)
                {
                    //don't add up if we're on top
                    if (!IsCurrentPosOnTop(currentY)) possibleDirections.Add(Direction.Up);
                }
                //same case for down
                else if (lastDirection == Direction.Right && secondLastDirection == Direction.Down)
                {
                    if (!IsCurrentPosOnBottom(currentY)) possibleDirections.Add(Direction.Down);
                }

                //Up direction cases
                else if (lastDirection == Direction.Up && secondLastDirection == Direction.Up || lastDirection == Direction.Up && secondLastDirection == Direction.Right)
                {
                    //obviously we can't go back down if we just went up, so we only add up if we aren't on top
                    if (!IsCurrentPosOnTop(currentY)) possibleDirections.Add(Direction.Up);
                }
                //Down direction cases
                else if (lastDirection == Direction.Down && secondLastDirection == Direction.Down || lastDirection == Direction.Down && secondLastDirection == Direction.Right)
                {
                    //likewise for down
                    if (!IsCurrentPosOnBottom(currentY)) possibleDirections.Add(Direction.Down);
                }

            }

            //now we randomly choose a direction from the list of possibilities on the enum
            Direction direction = possibleDirections.OrderBy(x => Guid.NewGuid()).Take(1).Single();

            //update position using a switch based on what direction we go
            switch (direction)
            {
                case Direction.Up:
                    currentY--; break;
                case Direction.Down:
                    currentY++; break;
                default:
                    currentX++; break;
            }

            //randomize generated tile starting at index 2 (first 2 items are set to be the beginning and end platforms)
            int randomIndex = UnityEngine.Random.Range(2, pathPrefabs.Length);

            //int emptyCheck = UnityEngine.Random.Range(0, 2);

            //set first platform to a flat plane
            if (currentX == 1) randomIndex = 0;
            //set last platform to a flat plane
            //(separate because end platform uses tag)
            else if (currentX == map.GetLength(0) - 1) randomIndex = 1;
            //otherwise, randomize prefab
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

            //now we instantiate platforms from our list using direction based on our enum
            if (direction == Direction.Up) Instantiate(prefab, new Vector3(currentY * 40, -0.5f, currentX * 40), Quaternion.Euler(0, -90, 0), transform);
            else if (direction == Direction.Down) Instantiate(prefab, new Vector3(currentY * 40, -0.5f, currentX * 40), Quaternion.Euler(0, 90, 0), transform);
            else Instantiate(prefab, new Vector3(currentY * 40, -0.5f, currentX * 40), Quaternion.identity, transform);

            //push forward our previous direction counts
            secondLastDirection = lastDirection;
            lastDirection = direction;
            
            //add endpoint at the end of map
            if (currentX == map.GetLength(0) - 1) Instantiate(endPoint, new Vector3(currentY * 40, 2, currentX * 40), Quaternion.identity, transform);
            pathTiles++;
        }
    }

    public void ResetPath()
    {
        //destroy all obstacles
        foreach(Transform child in transform)
        {
            if (child != null) Destroy(child.gameObject);
        }
    }
}
