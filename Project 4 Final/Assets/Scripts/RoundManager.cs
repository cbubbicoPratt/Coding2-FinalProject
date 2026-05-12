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
        round++;
        roundText.text = "Round: " + round;
        pathGen.GenerateMap();
    }
}
