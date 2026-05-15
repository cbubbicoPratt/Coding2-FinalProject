using TMPro;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI roundText;
    public PathGenerator pathGen;

    [Header("Variables")]
    public int round = 0;


    private void Awake()
    {
        NewRound();
    }

    public void NewRound()
    { 
        //increase round and update text
        //new round = new path generation
        round++;
        roundText.text = "Round: " + round;
        pathGen.GenerateMap();
    }
}
