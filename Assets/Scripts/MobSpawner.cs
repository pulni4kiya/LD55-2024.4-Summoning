using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobSpawner : MonoBehaviour {
	[SerializeField] private List<MobSpawn> spawns;

	private void Start() {
		var mobGroup = new MobGroup();
		var mobsList = new List<Mob>();
		mobGroup.AllMobs = mobsList;
		foreach (var spawn in this.spawns) {
			for (int i = 0; i < spawn.Amount; i++) {
				var position = this.transform.position + (Vector3)UnityEngine.Random.insideUnitCircle * spawn.Radius;
				var mob = GameObject.Instantiate(spawn.Prefab, position, Quaternion.identity, this.transform);
				mob.MobGroup = mobGroup;
				mobsList.Add(mob);
			}
		}
	}

	private void OnDrawGizmos() {
		Gizmos.color = Color.red;
		foreach (var mobs in this.spawns) {
			Gizmos.DrawWireSphere(this.transform.position, mobs.Radius);
		}
	}

	[Serializable]
	public class MobSpawn {
		public Mob Prefab;
		public int Amount;
		public float Radius;
	}
}
