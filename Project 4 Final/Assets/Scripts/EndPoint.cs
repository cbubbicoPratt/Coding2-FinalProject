using UnityEngine;

public class EndPoint : MonoBehaviour
{
    public CCPlayer player;
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
            Debug.Log(player.transform.position);
            roundManager.NewRound();
            Destroy(gameObject);
        }
    }
}
