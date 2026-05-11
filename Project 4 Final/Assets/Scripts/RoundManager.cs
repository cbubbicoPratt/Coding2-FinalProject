using TMPro;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI roundText;
    public GameObject endObject;
    private GameObject endPoint;
    public PathGenerator pathGen;

    [Header("Variables")]
    public int round = 0;


    private void Awake()
    {
        NewRound();
    }

    public void NewRound()
    {
        round++;
        roundText.text = "Round: " + round;
        pathGen.GenerateMap();
        endPoint = GameObject.FindGameObjectWithTag("EndPoint");
        Instantiate(endObject, new Vector3(endPoint.transform.position.x, 2, endPoint.transform.position.z), Quaternion.identity, endPoint.transform);
    }
}
