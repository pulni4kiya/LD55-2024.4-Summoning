using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargerMobBrain : MonoBehaviour {
	[SerializeField] private Mob mob;
	[SerializeField] private float chargeTimePower;
	[SerializeField] private float chargeSpeedMp;
	[SerializeField] private Vector2 chargeUpTime;
	[SerializeField] private Vector2 waitTime;
	[SerializeField] private Vector2 walkTime;
	[SerializeField] private AnimationCurve chargeCurve;
	[SerializeField] private float overshootPercent;

	private Coroutine coroutine;

	private void Update() {
		if (this.mob.MobGroup.Targets.Count > 0) {
			if (this.coroutine == null) {
				this.coroutine = StartCoroutine(this.ChasePlayer());
			}
		} else {
			if (this.coroutine != null) {
				StopCoroutine(this.coroutine);
				this.coroutine = null;
			}
		}

		if (this.coroutine != null) {
			GameManager.Instance.ActiveCharger = this.mob;
		}
	}

	private IEnumerator ChasePlayer() {
		var player = GameManager.Instance.player.transform;

		while (true) {
			// Walk
			var walkDt = 0f;
			var walkT = GetRandom(this.walkTime);
			while (walkDt < walkT) {
				walkDt += Time.deltaTime;
				this.mob.Rigidbody.velocity = ((Vector2)player.position - this.mob.Rigidbody.position).normalized * this.mob.Stats.Speed;
				yield return null;
			}
			this.mob.Rigidbody.velocity = Vector2.zero;

			// Charge Up
			var chargeUpT = GetRandom(this.chargeUpTime);
			yield return new WaitForSeconds(chargeUpT);

			// Charge
			var currentPosition = this.mob.Rigidbody.position;
			var dirToPlayer = (Vector2)player.position - this.mob.Rigidbody.position;
			var dirOvershoot = dirToPlayer * (1f + this.overshootPercent);
			var targetPosition = currentPosition + dirOvershoot;

			var distance = dirOvershoot.magnitude;
			var dt = 0f;
			var t = Mathf.Pow(distance / (this.mob.Stats.Speed * this.chargeSpeedMp) * 100, this.chargeTimePower) / 10;
			while (dt < t) {
				yield return new WaitForFixedUpdate();
				dt += Time.deltaTime;
				this.mob.Rigidbody.MovePosition(Vector2.Lerp(currentPosition, targetPosition, this.chargeCurve.Evaluate(dt / t)));
			}

			// Wait
			var waitT = GetRandom(this.waitTime);
			yield return new WaitForSeconds(waitT);
		}
	}

	private float GetRandom(Vector2 minMax) {
		return UnityEngine.Random.Range(minMax.x, minMax.y);
	}
}
