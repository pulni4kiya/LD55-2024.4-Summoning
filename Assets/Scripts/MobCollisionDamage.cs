using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobCollisionDamage : MonoBehaviour {
	[SerializeField] private float damagePerSecond;
	[SerializeField] private float attacksPerSecond;
	[SerializeField] private bool attackAll;
	[SerializeField] private bool attackPlayerOnHit;
	[SerializeField] private float playerDamageMp = 1f;

	public bool AttackAll => this.attackAll;
	public float DamagePerSecond => this.damagePerSecond;

	private List<Mob> enemies = new();
	private float cooldown = 0f;
	private float playerCooldown = 0f;

	private void Update() {
		this.cooldown -= Time.deltaTime;
		this.playerCooldown -= Time.deltaTime;

		for (int i = this.enemies.Count - 1; i >= 0; i--) {
			if (this.enemies[i] == null) {
				this.enemies.RemoveAt(i);
			}
		}

		if (this.cooldown < 0f && this.enemies.Count > 0) {
			this.Attack();
			this.cooldown = 1f / this.attacksPerSecond;
		}
	}

	private void Attack() {
		if (this.attackAll) {
			for (int i = 0; i < this.enemies.Count; i++) {
				var mob = this.enemies[i];
				this.Attack(mob);
			}
		} else {
			var index = UnityEngine.Random.Range(0, this.enemies.Count);
			var mob = this.enemies[index];
			this.Attack(mob);
		}
	}

	private void Attack(Mob mob) {
		if (mob == null) return;

		var isPlayer = mob.gameObject == GameManager.Instance.player.gameObject;

		var damage = this.damagePerSecond / this.attacksPerSecond;
		if (isPlayer) {
			damage *= this.playerDamageMp;
		}

		mob.CurrentHealth -= damage;
		if (mob.CurrentHealth <= 0f && !isPlayer) {
			GameObject.Destroy(mob.gameObject);
		}
	}

	private void OnCollisionEnter2D(Collision2D collision) {
		var mob = collision.collider.GetComponentInParent<Mob>();
		if (mob == null || mob.gameObject.layer == this.gameObject.layer) return;

		if (this.attackPlayerOnHit && this.playerCooldown < 0f && mob.gameObject == GameManager.Instance.player.gameObject) {
			this.Attack(mob);
			this.playerCooldown = 1f / this.attacksPerSecond;
			return;
		}

		this.enemies.Add(mob);
	}

	private void OnCollisionExit2D(Collision2D collision) {
		var mob = collision.collider.GetComponentInParent<Mob>();
		if (mob == null || mob.gameObject.layer == this.gameObject.layer) return;

		this.enemies.Remove(mob);
	}
}
