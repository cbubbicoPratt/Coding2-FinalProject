using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [Header("SpawnPoints")]
    public GameObject endPlat;
    public Transform[] endPoints;

    [Header("Prefabs")]
    public GameObject[] prefabs;

    public Transform endPlatform;
    private List<GameObject> _emptyPlatforms;
    private List<GameObject> _platforms;
    private bool[] _level;

    private void Awake()
    {
        SpawnPlatforms();
        int tempRandom;
        _level = new bool[Random.Range(3, 9)];
        for (int i = 0; i < _level.Length; i++)
        {
            if( i == 0 || i == _level.Length - 1 )
            {
                _level[i] = true;
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
        
        foreach(GameObject platform in prefabs)
        {
            if (platform == null) return;
            if(platform.tag == "PlatformEmpty")
            {
                _emptyPlatforms.Add(platform);
            } 
            else if (platform.tag == "Platform")
            {
                _platforms.Add(platform);
            }
        }
    }

    public void SpawnPlatforms()
    {
        Transform tempEnd = endPoints[Random.Range(0, endPoints.Length - 1)];
        Instantiate(endPlat, tempEnd);
    }
}
