using UnityEngine;

public class RoundManager : MonoBehaviour
{
    [Header("References")]
    public GameObject endObject;
    private GameObject endPoint;
    public PathGenerator pathGen;

    [Header("Variables")]
    public static int round = 0;


    private void Awake()
    {
        endPoint = GameObject.FindGameObjectWithTag("EndPoint");
    }

    public static void NewRound()
    {
        round++;
    }
}
