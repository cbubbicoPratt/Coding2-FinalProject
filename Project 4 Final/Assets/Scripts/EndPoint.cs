using UnityEngine;

public class EndPoint : MonoBehaviour
{
    private CCPlayer player;
    private RoundManager roundManager;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<CCPlayer>();
        roundManager = GameObject.FindFirstObjectByType<RoundManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            player.ResetPosition();
            roundManager.NewRound();
            Destroy(gameObject);
        }
    }
}
