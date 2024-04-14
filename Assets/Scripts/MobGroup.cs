using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobGroup {
	public List<Mob> AllMobs { get; set; } = new();
	public List<Mob> Targets { get; set; } = new();
}
