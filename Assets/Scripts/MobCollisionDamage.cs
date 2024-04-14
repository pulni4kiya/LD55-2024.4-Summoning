using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobCollisionDamage : MonoBehaviour {
	[SerializeField] private float damagePerSecond;
	[SerializeField] private float attacksPerSecond;
	[SerializeField] private bool attackAll;

	private List<Mob> enemies = new();
	private float cooldown = 0f;

	private void Update() {
		this.cooldown -= Time.deltaTime;
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
		mob.CurrentHealth -= this.damagePerSecond / this.attacksPerSecond;
		if (mob.CurrentHealth < 0f) {
			GameObject.Destroy(mob.gameObject);
		}
	}

	private void OnCollisionEnter2D(Collision2D collision) {
		var mob = collision.collider.GetComponentInParent<Mob>();
		if (mob == null || mob.gameObject.layer == this.gameObject.layer) return;

		this.enemies.Add(mob);
	}

	private void OnCollisionExit2D(Collision2D collision) {
		var mob = collision.collider.GetComponentInParent<Mob>();
		if (mob == null || mob.gameObject.layer == this.gameObject.layer) return;

		this.enemies.Remove(mob);
	}
}
