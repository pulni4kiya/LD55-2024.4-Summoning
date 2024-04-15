using UnityEngine;

public class PickupMaxMana : MonoBehaviour
{
    [SerializeField] private float MaxManaUpdate;

    Player player;

    private void Awake()
    {
        player = FindObjectOfType<Player>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Destroy(gameObject);
        player.maxMana += MaxManaUpdate;
    }
    

}
