using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mob : MonoBehaviour {
	public const int FriendlyLayer = 10;
	public const int EnemyLayer = 11;

	public string DisplayName;

	public bool IsFriendly => this.gameObject.layer == 10;

	private void Start() {

	}

	private void Update() {

	}
}
