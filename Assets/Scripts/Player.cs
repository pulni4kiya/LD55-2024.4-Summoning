using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Player : MonoBehaviour {
	private static Collider2D[] collidersCheck = new Collider2D[1];

	[SerializeField] private float speed;
	[SerializeField] private float maxHealth;
	[SerializeField] private float healthRegen;
	[SerializeField] private Image healthBar;
	[SerializeField] private float maxMana;
	[SerializeField] private float manaRegen;
	[SerializeField] private Image manaBar;
	[SerializeField] private InputActionReference moveAction;
	[SerializeField] private InputActionReference summonAction;
	[SerializeField] private Rigidbody2D rigidbody;

	public Mob MobToSummon { get; set; }

	private MobGroup mobGroup;

	private Vector2 moveDir;

	private float health;
	private float mana;


	private void Start() {
		this.mobGroup = new MobGroup();
		this.moveAction.action.actionMap.Enable();
		this.health = this.maxHealth;
		this.mana = this.maxMana;
	}

	private void Update() {
		this.UpdateHealth();
		this.UpdateMana();

		this.moveDir = this.moveAction.action.ReadValue<Vector2>();

		if (this.MobToSummon != null && this.summonAction.action.IsPressed()) {
			if (this.CheckManaCost()) {
				this.TrySummon();
			}
		}
	}

	private void FixedUpdate() {
		this.rigidbody.velocity = this.moveDir * this.speed;
	}

	private void SpawnMob(Vector3 worldPos) {
		var mob = GameObject.Instantiate(this.MobToSummon, worldPos, Quaternion.identity);
		mob.MobGroup = this.mobGroup;
		this.mobGroup.AllMobs.Add(mob);
	}

	private void UpdateHealth() {
		this.health = Mathf.Clamp(this.health + this.healthRegen * Time.deltaTime, 0, this.maxHealth);
		this.healthBar.fillAmount = this.health / this.maxHealth;
	}

	private void UpdateMana() {
		this.mana = Mathf.Clamp(this.mana + this.manaRegen * Time.deltaTime, 0, this.maxMana);
		this.manaBar.fillAmount = this.mana / this.maxMana;
	}


	private bool CheckManaCost() {
		return this.mana >= this.MobToSummon.Stats.ManaCost;
	}

	private void TrySummon() {
		var mousePos = (Vector3)Mouse.current.position.value;
		mousePos.z = 10f;
		var worldPos = Camera.main.ScreenToWorldPoint(mousePos);
		var filter = new ContactFilter2D();
		filter.useTriggers = false;
		if (Physics2D.OverlapPoint(worldPos, filter, collidersCheck) ==  0) {
			this.SpawnMob(worldPos);
			this.mana -= this.MobToSummon.Stats.ManaCost;
		}
	}
}
