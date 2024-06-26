using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobRangedDamage : MonoBehaviour {
	[SerializeField] private float damagePerSecond;
	[SerializeField] private float attacksPerSecond;
	[SerializeField] private float range;

	private float cooldown = 0f;

	public float Range => this.range;
	public float DamagePerSecond => this.damagePerSecond;

	public Mob Target { get; set; }

	private void Update() {
		this.cooldown -= Time.deltaTime;
		if (this.cooldown < 0f && this.Target != null) {
			this.Attack(this.Target);
			this.cooldown = 1f / this.attacksPerSecond;
		}
	}

	private void Attack(Mob mob) {
		mob.CurrentHealth -= this.damagePerSecond / this.attacksPerSecond;
		if (mob.CurrentHealth <= 0f && mob.gameObject != GameManager.Instance.player.gameObject) {
			GameManager.Instance.OnMobKilled(mob);
		}
	}
}
