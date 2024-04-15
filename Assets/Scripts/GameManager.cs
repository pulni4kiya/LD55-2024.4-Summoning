using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance { get; private set; }

	[SerializeField] private MobGroup mobGroupPrefab;
	[SerializeField] private MenuController menu;
	[SerializeField] private Image healthBar;

	public Player player;

	public State GameState { get; private set; }
	public Mob ActiveCharger { get; set; }

	private void Awake() {
		Instance = this;
		Time.timeScale = 1f;
	}

	private void OnDestroy() {
		Instance = null;
	}

	private void Update() {
		if (this.ActiveCharger != null) {
			if (this.ActiveCharger.CurrentHealth <= 0f) {
				this.ActiveCharger = null;
			} else {
				this.healthBar.fillAmount = this.ActiveCharger.CurrentHealth / this.ActiveCharger.Stats.StartingHealth;
			}
		}

		this.healthBar.transform.parent.gameObject.SetActive(this.ActiveCharger != null);
	}

	public void Pause() {
		Time.timeScale = 0f;
		this.menu.ShowMenu(this.GameState);
	}

	public void Unpause() {
		Time.timeScale = 1f;
		this.menu.HideMenu();
	}

	public void GameLost() {
		this.GameState = State.Loss;
		this.Pause();
	}

	public void GameWon() {
		this.GameState = State.Win;
		this.Pause();
	}

	public void NewGame() {
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	public enum State {
		InProgress,
		Win,
		Loss
	}
}

public static class Const {
	public const int FriendlyLayer = 10;
	public const int EnemyLayer = 11;
	public const int ObstacleLayer = 12;
}