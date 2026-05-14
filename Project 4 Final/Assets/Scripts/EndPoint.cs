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
            _player.ResetPosition();
            roundManager.NewRound();
            Debug.Log("Reset");
            Destroy(gameObject);
        }
    }
}
