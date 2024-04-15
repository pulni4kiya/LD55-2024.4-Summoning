using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedPlayerMobBrain : MonoBehaviour {
	[SerializeField] private Mob mob;
	[SerializeField] private MobRangedDamage rangedDamage;
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
		var range = this.rangedDamage.Range;
		var unsafeRangeSqr = (range - 1f) * (range - 1f);

		this.cooldown -= Time.deltaTime;
		if (this.cooldown > 0f) return;
		this.cooldown += this.thinkCooldown;

		var closestDistance = float.PositiveInfinity;
		var closestDirection = Vector2.zero;
		var runAwayDirection = Vector2.zero;
		this.rangedDamage.Target = null;
		foreach (var target in mob.MobGroup.Targets) {
			if (target == null) continue;

			var direction = target.Rigidbody.position - this.mob.Rigidbody.position;
			var distance = direction.sqrMagnitude;
			if (distance < closestDistance) {
				closestDistance = distance;
				closestDirection = direction;
				if (distance < range * range) {
					this.rangedDamage.Target = target;
				}
			}

			if (distance < unsafeRangeSqr) {
				runAwayDirection -= direction * 0.5f;
			}
		}

		runAwayDirection = Vector2.ClampMagnitude(runAwayDirection, this.mob.Stats.Speed);
		if (runAwayDirection != Vector2.zero) {
			closestDirection = runAwayDirection;
		} else if (closestDistance < range * range) {
			closestDirection = Vector2.zero;
		}

		if (this.mob.IsFriendly) {
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
		}

		this.mob.Rigidbody.velocity = closestDirection.normalized * this.mob.Stats.Speed;
	}
}
