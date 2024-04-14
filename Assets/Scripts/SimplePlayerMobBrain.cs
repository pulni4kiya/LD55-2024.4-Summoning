using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplePlayerMobBrain : MonoBehaviour {
	[SerializeField] private Mob mob;
	[SerializeField] private float thinkCooldown;
	[SerializeField] private AnimationCurve followChance;
	[SerializeField] private float minDistance;
	[SerializeField] private float maxDistance;

	private Transform player;
	private float cooldown;

	private void Start() {
		this.player = GameManager.Instance.player.transform;
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

		var dir = (Vector2)this.player.position - this.mob.Rigidbody.position;
		if (float.IsPositiveInfinity(closestDistance)) {
			var t = Mathf.InverseLerp(0f, this.maxDistance, dir.magnitude);
			var chance = this.followChance.Evaluate(t);
			if (Random.value < chance) {
				closestDirection = dir + Random.insideUnitCircle * this.minDistance;
			}
		} else {
			if (dir.magnitude > this.maxDistance) {
				closestDirection = dir + Random.insideUnitCircle * this.minDistance;
			}
		}

		this.mob.Rigidbody.velocity = closestDirection.normalized * this.mob.Stats.Speed;
	}
}
