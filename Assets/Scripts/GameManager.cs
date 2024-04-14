using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance { get; private set; }

	[SerializeField] private MobGroup mobGroupPrefab;

	public Player player;

	private void Awake() {
		Instance = this;
	}

	private void Start() {
		FindMobGroups();
	}

	private void OnDestroy() {
		Instance = null;
	}

	private void FindMobGroups() {
		//var group
		var mobs = GameObject.FindObjectsOfType<Mob>();
		for (int i = 0; i < mobs.Length; i++) {

		}
	}
}

public static class Const {
	public const int FriendlyLayer = 10;
	public const int EnemyLayer = 11;
	public const int ObstacleLayer = 12;
}