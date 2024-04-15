using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MenuController : MonoBehaviour
{
	[SerializeField] private List<StateObjects> states;

	public void ShowMenu(GameManager.State state) {
		foreach (var config in this.states) {
			foreach (var obj in config.Objects) {
				obj.SetActive(false);
			}
		}

		var targetState = this.states.First(st => st.State == state);
		foreach (var obj in targetState.Objects) {
			obj.SetActive(true);
		}

		this.gameObject.SetActive(true);
	}

	public void HideMenu() {
		this.gameObject.SetActive(false);
	}

	[Serializable]
	public class StateObjects {
		public GameManager.State State;
		public List<GameObject> Objects;
	}
}
