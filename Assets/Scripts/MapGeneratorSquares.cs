using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class MapGeneratorSquares : MonoBehaviour {
	[SerializeField] private Vector2Int mapSize;
	[SerializeField] private float cellSize;
	[SerializeField] private float fillRate;
	[SerializeField] private float noiseScale;
	//[SerializeField] private float noiseOffset;
	[SerializeField] private GameObject obstaclePrefab;
	[SerializeField] private int extraPoints;

	[SerializeField] private List<MobSpanwerSet> spawnerSets;
	[SerializeField] private MobSpanwerSet bossSpawnSet;

	private Vector2 noiseOffset;

	private void Start() {
		this.GenerateMap();
		this.noiseOffset = UnityEngine.Random.insideUnitCircle * 50f;
	}

	private void Update() {
		if (Keyboard.current.rKey.wasPressedThisFrame) {
			this.GenerateMap();
		}
	}

	private void GenerateMap() {
		for (int i = this.transform.childCount - 1; i >= 0; i--) {
			GameObject.Destroy(this.transform.GetChild(i).gameObject);
		}

		for (int i=0; i < this.mapSize.x; i++) {
			for (int j = 0; j < this.mapSize.y; j++) {
				var noisePos = (new Vector2(i, j) + this.noiseOffset) * this.noiseScale;
				var noisevalue = Mathf.PerlinNoise(noisePos.x, noisePos.y);
				if (noisevalue < this.fillRate || i == 0 || i == this.mapSize.x - 1 || j == 0 || j == this.mapSize.y - 1) {
					var position = this.transform.position + new Vector3(this.mapSize.x / 2f - i, this.mapSize.y / 2f - j, 0f) * this.cellSize;
					var obstacle = GameObject.Instantiate(this.obstaclePrefab, position, Quaternion.identity, this.transform);
					obstacle.transform.localScale = Vector3.one * this.cellSize;
				}
			}
		}

		var points = new List<Vector2>();
		points.Add(new Vector2(0.1f, 0.1f));
		for (int i = 0; i < this.extraPoints; i++) {
			points.Add(new Vector2(UnityEngine.Random.Range(0.2f, 0.8f), UnityEngine.Random.Range(0.2f, 0.8f)));
		}
		points.Add(new Vector2(0.9f, 0.9f));

		for (int i = 0; i < points.Count; i++) {
			points[i] = Vector2.Scale((points[i] - Vector2.one * 0.5f), this.mapSize) * this.cellSize;
		}

		for (int i = 1; i < points.Count; i++) {
			var p1 = points[i - 1];
			var p2 = points[i];
			var hits = Physics2D.CircleCastAll(p1, this.cellSize, p2 - p1, (p2 - p1).magnitude, 1 << Const.ObstacleLayer);
			foreach (var hit in hits) {
				GameObject.Destroy(hit.collider.transform.parent.gameObject);
			}

			if (i != points.Count - 1) {
				var randomIndex = UnityEngine.Random.Range(0, this.spawnerSets.Count);
				AddSpawner(p2, this.spawnerSets[randomIndex]);
			}
		}

		AddSpawner(points.Last(), this.bossSpawnSet);
		//AddSpawner(points[1], this.bossSpawnSet);

		GameManager.Instance.player.transform.position = points[0];
	}

	private void AddSpawner(Vector3 position, MobSpanwerSet spawnSet) {
		var go = new GameObject();
		go.transform.position = position;
		var spawner = go.AddComponent<MobSpawner>();
		spawner.Spawns = spawnSet.Spawns;
	}

	[Serializable]
	public class MobSpanwerSet {
		public List<MobSpawner.MobSpawn> Spawns;
	}
}
