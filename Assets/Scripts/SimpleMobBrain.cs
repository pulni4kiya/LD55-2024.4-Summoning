using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMobBrain : MonoBehaviour {
	[SerializeField] private Mob mob;
	[SerializeField] private float thinkCooldown;

	private float cooldown;

	private void Start() {
		this.cooldown = Random.Range(0f, this.thinkCooldown);
	}

	private void Update() {
		this.cooldown -= Time.deltaTime;
		if (this.cooldown > 0f) return;
		this.cooldown += this.thinkCooldown;

		var closestDistance = float.PositiveInfinity;
		var closestDirection = Vector2.zero;
		//Mob closestTarget = null;
		foreach (var target in mob.MobGroup.Targets) {
			if (target == null) continue;

			var direction = target.Rigidbody.position - this.mob.Rigidbody.position;
			var distance = direction.sqrMagnitude;
			if (distance < closestDistance) {
				closestDistance = distance;
				//closestTarget = target;
				closestDirection = direction;
			}
		}

		this.mob.Rigidbody.velocity = closestDirection.normalized * this.mob.Stats.Speed;
	}
}
