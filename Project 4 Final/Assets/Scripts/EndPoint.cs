using UnityEngine;

public class EndPoint : MonoBehaviour
{
    private ThirdPersonMovement _player;
    private RoundManager roundManager;

    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<ThirdPersonMovement>();
        roundManager = GameObject.FindFirstObjectByType<RoundManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            //reset round when making contact with player
            //bring player to start
            //start a new round to regenerate path
            //destroy self
            _player.ResetPosition();
            roundManager.NewRound();
            Debug.Log("Reset");
            Destroy(gameObject);
        }
    }
}
