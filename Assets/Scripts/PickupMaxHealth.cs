using UnityEngine;

public class PickupMaxHealth : MonoBehaviour
{
    [SerializeField] private float MaxHealthUpdate;

    Player player;

    private void Awake()
    {
		player = GameManager.Instance.player;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Destroy(gameObject);
        player.GetComponent<Mob>().Stats.StartingHealth += MaxHealthUpdate;
		player.GetComponent<Mob>().CurrentHealth += MaxHealthUpdate;
	}
}
