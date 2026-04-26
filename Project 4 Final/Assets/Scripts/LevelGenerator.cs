using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class TileGenerator : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject[] prefabs;

    [Header("Path Generator")]
    public PathGenerator generator;

    private List<GameObject> _emptyPlatforms;
    private List<GameObject> _platforms;
    private int _level;

    public GameObject RandomTile()
    {
        //we want 
        int tempRandom;
        _level = generator.pathTiles;
        for (int i = 1; i <= _level; i++)
        {
            if (i == 0 || i == _level)
            {
                
            }
            else if (_level[i - 1] == false)
            {
                _level[i] = true;
            }
            else
            {
                tempRandom = Random.Range(0, 2);
                if (tempRandom == 0) _level[i] = false;
                else _level[i] = true;
            }
            Debug.Log(_level[i]);
        }

        foreach (GameObject platform in prefabs)
        {
            if (platform == null) return;
            if (platform.tag == "PlatformEmpty")
            {
                _emptyPlatforms.Add(platform);
            }
            else if (platform.tag == "Platform")
            {
                _platforms.Add(platform);
            }
        }
    }
}
