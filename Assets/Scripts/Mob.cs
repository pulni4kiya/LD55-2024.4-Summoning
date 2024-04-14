using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mob : MonoBehaviour {
	public string DisplayName;
	[SerializeField] private MobStats stats;
	[SerializeField] private Rigidbody2D rigidbody;
	[SerializeField] private CircleCollider2D detectionCollider;

	public MobGroup MobGroup { get; set; }

	public bool IsFriendly => this.gameObject.layer == 10;
	public Rigidbody2D Rigidbody => this.rigidbody;
	public MobStats Stats => this.stats;

	public float CurrentHealth { get; internal set; }

	private void Start() {
		this.CurrentHealth = this.stats.StartingHealth;
		this.detectionCollider.radius = this.stats.DetectionRange;
	}

	private void Update() {

	}

	private void OnTriggerEnter2D(Collider2D collider) {
		var mob = collider.GetComponentInParent<Mob>();
		if (mob == null || mob.gameObject.layer == this.gameObject.layer) return;

		if (!this.MobGroup.Targets.Contains(mob)) {
			this.MobGroup.Targets.Add(mob);
		}
	}

	[Serializable]
	public class MobStats {
		public float StartingHealth;
		public float Speed;
		public float DetectionRange;
		public float ManaCost;
	}
}
