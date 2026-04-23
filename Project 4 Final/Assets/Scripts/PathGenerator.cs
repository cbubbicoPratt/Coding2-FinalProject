using UnityEngine;

public class PathGenerator : MonoBehaviour
{
    //whatever object this script is attached to will have a customizable path from the start
    //we do this with a list of transform points for platforms to spawn
    //may have to change up list for corner points

    public Transform startPoint;
    private Transform endPoint;
    private Vector2 gridSize = new Vector2(200, 500);

    private void Awake()
    {
        endPoint = GetComponent<Transform>();
    }
}
