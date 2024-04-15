using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance { get; private set; }

	[SerializeField] private MobGroup mobGroupPrefab;
	[SerializeField] private MenuController menu;

	public Player player;

	public State GameState { get; private set; }

	private void Awake() {
		Instance = this;
		Time.timeScale = 1f;
	}

	private void OnDestroy() {
		Instance = null;
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