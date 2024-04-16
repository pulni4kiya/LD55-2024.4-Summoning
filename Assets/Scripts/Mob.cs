using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mob : MonoBehaviour {
	public string DisplayName;
	[SerializeField] private MobStats stats;
	[SerializeField] private Rigidbody2D rigidbody;
	[SerializeField] private CircleCollider2D detectionCollider;
	[SerializeField] private Transform visualRoot;
	[SerializeField] private Animator animator;
	[SerializeField] private AudioSource soundPlayer;
	[SerializeField] private List<AudioClip> spawnSounds;

	public MobGroup MobGroup { get; set; }

	public bool IsFriendly => this.gameObject.layer == 10;
	public Rigidbody2D Rigidbody => this.rigidbody;
	public MobStats Stats => this.stats;

	public float CurrentHealth { get; internal set; }

	private void Start() {
		this.CurrentHealth = this.stats.StartingHealth;
		this.detectionCollider.radius = this.stats.DetectionRange;

		if (this.stats.Lifetime > 0f) {
			GameObject.Destroy(this.gameObject, this.stats.Lifetime);
		}

		if (this.spawnSounds.Count > 0) {
			//this.soundPlayer.PlayOneShot(this.spawnSounds[GameManager.Instance.VoicePack]);
			this.soundPlayer.PlayOneShot(this.spawnSounds[UnityEngine.Random.Range(0, this.spawnSounds.Count)]);
		}
	}

	private void Update() {
		if (this.visualRoot != null) {
			if (this.Rigidbody.velocity.x != 0f) {
				this.visualRoot.localScale = new Vector3(-Mathf.Sign(this.Rigidbody.velocity.x), 1f, 1f);
			}
		}

		if (this.animator != null) {
			this.animator.SetBool("IsMoving", this.Rigidbody.velocity != Vector2.zero);
		}
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
		public float Lifetime;
	}
}
