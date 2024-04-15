using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(10)]
public class MobSpawner : MonoBehaviour {
	[field: SerializeField] public List<MobSpawn> Spawns { get; set; }

	private void Start() {
		var mobGroup = new MobGroup();
		var mobsList = new List<Mob>();
		mobGroup.AllMobs = mobsList;
		foreach (var spawn in this.Spawns) {
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
		foreach (var mobs in this.Spawns) {
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
