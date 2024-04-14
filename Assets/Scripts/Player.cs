using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour {
	[SerializeField] private float speed;
	[SerializeField] private InputActionReference moveAction;
	[SerializeField] private InputActionReference summonAction;
	[SerializeField] private Rigidbody2D rigidbody;

	public Mob MobToSummon { get; set; }

	private Vector2 moveDir;

	private void Start() {
		this.moveAction.action.actionMap.Enable();
	}

	private void Update() {
		this.moveDir = this.moveAction.action.ReadValue<Vector2>();

		if(this.MobToSummon != null && this.summonAction.action.IsPressed()) {
			var mousePos = (Vector3)Mouse.current.position.value;
			mousePos.z = 10f;
			var worldPos = Camera.main.ScreenToWorldPoint(mousePos);
			if (Physics2D.OverlapPoint(worldPos) == null) {
				this.SpawnMob(worldPos);
			}
		}
	}

	private void FixedUpdate() {
		this.rigidbody.velocity = this.moveDir * this.speed;
	}

	private void SpawnMob(Vector3 worldPos) {
		GameObject.Instantiate(this.MobToSummon, worldPos, Quaternion.identity);
	}
}
