using UnityEngine;

public class PickupMaxHealth : MonoBehaviour
{
    [SerializeField] private float MaxHealthUpdate;

    Player player;

    private void Awake()
    {
        player = FindObjectOfType<Player>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Destroy(gameObject);
        //player.stats.StartingHealth += MaxHealthUpdate;
    }
}
