using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using URan = UnityEngine.Random;

public class MapGeneratorSquares : MonoBehaviour {
	[SerializeField] private float cellSize;
	[SerializeField] private float mapSize;
	[SerializeField] private GameObject obstaclePrefab;
	[SerializeField] private Vector2Int sizeMinMax;
	[SerializeField] private int count;
	[SerializeField] private Vector2Int distanceMinMax;
	[SerializeField] private int corridorWidth;

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

	private async void GenerateMap() {
		for (int i = this.transform.childCount - 1; i >= 0; i--) {
			GameObject.Destroy(this.transform.GetChild(i).gameObject);
		}

		var desiredSize = this.mapSize;

		var rooms = new List<Room>();

		for (int i = 0; i < this.count; i++) {
			var attempts = 0;
			Room newRoom;
			do {
				attempts++;
				if (attempts == 20) {
					attempts = 0;
					desiredSize += 2f;
				}

				var centerF = URan.insideUnitCircle * desiredSize;
				var center = new Vector2Int(Mathf.RoundToInt(centerF.x), Mathf.RoundToInt(centerF.y));

				var w = URan.Range(this.sizeMinMax.x, this.sizeMinMax.y);
				var h = URan.Range(this.sizeMinMax.x, this.sizeMinMax.y);
				var size = new Vector2Int(w, h);

				newRoom = new Room() {
					Rect = new RectInt(center - new Vector2Int(w / 2, h / 2), size)
				};

				
			} while (!IsValid(rooms, newRoom, this.distanceMinMax));

			rooms.Add(newRoom);
		}

		// Prim! Or Hruskal. I don't remember.
		var originalRooms = new List<Room>(rooms);
		for (int i = 0; i < rooms.Count; i++) {
			rooms[i].Group = new List<Room>() { rooms[i] };
		}

		var pairs = rooms
			.SelectMany((x, i) => rooms.Skip(i + 1).Select(y => new { x, y }))
			.OrderBy(pair => pair.x.DistanceTo(pair.y))
			.ToList();

		var connections = 0;
		foreach (var pair in pairs) {
			if (pair.x.Group == pair.y.Group) {
				if (URan.value < 0.3f) {
					Connect(rooms, pair.x, pair.y);
				}
			} else {
				Connect(rooms, pair.x, pair.y);
				connections++;
			}

			if (connections == count - 1) break;
		}

		foreach (var room in rooms) {
			var rect = room.Rect;
			for (int x = rect.xMin - 2; x < rect.xMax + 2; x++) {
				for (int y = rect.yMin - 2; y < rect.yMax + 2; y++) {
					if (rooms.Any(r => r.Rect.Contains(new Vector2Int(x, y)))) continue;

					var position = new Vector3(x, y, 0f) * this.cellSize;
					var obstacle = Instantiate(this.obstaclePrefab, position, Quaternion.identity, this.transform);
					obstacle.transform.localScale = Vector3.one * this.cellSize;
				}
			}
		}

		var orderedRooms = originalRooms.OrderBy(r => r.Rect.center.x).ToList();

		var bossRoom = orderedRooms.Last();
		this.AddSpawner(bossRoom.Rect.center * this.cellSize, this.bossSpawnSet);

		var playerRoom = orderedRooms.First();
		GameManager.Instance.player.transform.position = playerRoom.Rect.center * this.cellSize;

		for (int i = 1; i < orderedRooms.Count; i++) {
			var randomIndex = UnityEngine.Random.Range(0, this.spawnerSets.Count);
			AddSpawner(orderedRooms[i].Rect.center * this.cellSize, this.spawnerSets[randomIndex]);
		}
	}

	private void Connect(List<Room> rooms, Room room1, Room room2) {
		var p1 = new Vector2Int(URan.Range(room1.Rect.xMin + 1, room1.Rect.xMax - 1), URan.Range(room1.Rect.yMin + 1, room1.Rect.yMax - 1));
		var p2 = new Vector2Int(URan.Range(room2.Rect.xMin + 1, room2.Rect.xMax - 1), URan.Range(room2.Rect.yMin + 1, room2.Rect.yMax - 1));

		Debug.DrawLine((Vector2)p1, (Vector2)p2, Color.red, 5f);

		var delta = p2 - p1;
		if (URan.value > 0.5f) {
			delta.x = 0;
		} else {
			delta.y = 0;
		}
		var p3 = p1 + delta;

		AddRoom(rooms, p1, p3);
		AddRoom(rooms, p3, p2);

		room1.Group.AddRange(room2.Group);
		room2.Group.ForEach(grp => grp.Group = room1.Group);
	}

	private void AddRoom(List<Room> rooms, Vector2Int p1, Vector2Int p2) {
		var bottomLeft = new Vector2Int(Mathf.Min(p1.x, p2.x) - 1, Mathf.Min(p1.y, p2.y) - 1);
		var delta = p2 - p1;
		var size = new Vector2Int(Mathf.Abs(delta.x) + this.corridorWidth, Mathf.Abs(delta.y) + this.corridorWidth);

		var room = new Room() { Rect = new RectInt(bottomLeft, size) };
		rooms.Add(room);
	}

	private bool IsValid(List<Room> rooms, Room newRoom, Vector2Int distanceMinMax) {
		var minDistance = 1000000;
		foreach (var room in rooms) {
			var dist = newRoom.DistanceTo(room);
			if (dist < distanceMinMax.x) return false;
			if (dist < minDistance) {
				minDistance = dist;
			}
		}
		return rooms.Count == 0 || minDistance < distanceMinMax.y;
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

	private class Room {
		public RectInt Rect;
		public List<Room> Group;

		public int DistanceTo(Room other) {
			if (Rect.Overlaps(other.Rect)) {
				return 0;
			}

			int distanceX = Mathf.Max(0, Mathf.Abs(Mathf.CeilToInt(Rect.center.x) - Mathf.CeilToInt(other.Rect.center.x)) - (Rect.width + other.Rect.width) / 2);
			int distanceY = Mathf.Max(0, Mathf.Abs(Mathf.CeilToInt(Rect.center.y) - Mathf.CeilToInt(other.Rect.center.y)) - (Rect.height + other.Rect.height) / 2);

			return Mathf.Max(0, distanceX + distanceY);
		}

		public int DistanceTo(Vector2Int point) {
			var nearestPoint = new Vector2Int(
				Mathf.Clamp(point.x, Rect.xMin, Rect.xMax),
				Mathf.Clamp(point.y, Rect.yMin, Rect.yMax)
			);

			var delta = point - nearestPoint;

			return Mathf.Abs(delta.x) + Mathf.Abs(delta.y);
		}
	}
}
