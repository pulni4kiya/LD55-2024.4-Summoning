using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SummonInfoButton : MonoBehaviour {
	[SerializeField] private Mob mob;
	[SerializeField] private Button button;
	[SerializeField] private TMP_Text mobName;

	private void Start() {
		this.button.onClick.AddListener(this.SelectMob);
		this.mobName.text = this.mob.DisplayName;
	}

	private void OnDestroy() {
		this.button.onClick.RemoveListener(this.SelectMob);
	}

	private void SelectMob() {
		GameManager.Instance.player.MobToSummon = this.mob;
	}
}
