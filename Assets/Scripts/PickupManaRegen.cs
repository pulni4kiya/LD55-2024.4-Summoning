using UnityEngine;

public class PickupManaRegen : MonoBehaviour
{
    [SerializeField] private float ManaRegenUpdate;

    Player player;

    private void Awake()
    {
        player = FindObjectOfType<Player>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Destroy(gameObject);
        player.manaRegen += ManaRegenUpdate;
    }
    

}
