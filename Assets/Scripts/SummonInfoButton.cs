using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SummonInfoButton : MonoBehaviour {
	[SerializeField] private Mob mob;
	[SerializeField] private Button button;
	[SerializeField] private Image buttonBackground;
	[SerializeField] private TMP_Text mobName;
	[SerializeField] private TMP_Text mobInfo;
	[SerializeField] private InputActionReference switchAction;

	private void Start() {
		this.button.onClick.AddListener(this.SelectMob);
		this.mobName.text = this.mob.DisplayName;

		var damagePerSecond = 0f;
		var damageType = "";
		if (this.mob.TryGetComponent<MobCollisionDamage>(out var cdmg)) {
			damageType = cdmg.AttackAll ? "Melee All" : "Melee";
			damagePerSecond = cdmg.DamagePerSecond;
		} else if (this.mob.TryGetComponent<MobRangedDamage>(out var rdmg)) {
			damageType = "Ranged";
			damagePerSecond = rdmg.DamagePerSecond;
		}

		this.mobInfo.text = $"Health: {this.mob.Stats.StartingHealth:F0}\nDPS: {damagePerSecond:F0} ({damageType})\nSpeed: {this.mob.Stats.Speed:F0}\nMana cost: {this.mob.Stats.ManaCost:F0}\nLifetime: {this.mob.Stats.Lifetime:F0}s";
	}

	private void OnDestroy() {
		this.button.onClick.RemoveListener(this.SelectMob);
	}

	private void Update() {
		if (this.switchAction.action.WasPerformedThisFrame()) {
			this.SelectMob();
		}
	}

	private void SelectMob() {
		GameManager.Instance.player.MobToSummon = this.mob;
		foreach (var btn in GameObject.FindObjectsOfType<SummonInfoButton>()) {
			btn.buttonBackground.color = Color.white;
		}
		this.buttonBackground.color = Color.yellow;
	}
}
