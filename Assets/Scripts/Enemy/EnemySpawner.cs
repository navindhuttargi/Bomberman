using System.Collections.Generic;
using UnityEngine;

public interface IEnemySpawner
{
	bool areAllEnemyDied { get; }
	void InitializeEnemySpawner(GameObject _enemyPrefab, uint _enemyCount);
	void SpawnEnemy(Vector3 spawnPosition);
	void RemoveEnemy(GameObject _enemy);
}
public class EnemySpawner : IEnemySpawner
{
	private GameObject enemyPrefab;
	private List<GameObject> enemies;
	private uint enemyCount = 0;
	public bool areAllEnemyDied { get; set; }
	~EnemySpawner()
	{
		GameManager.instance.restartGame -= RestartGame;
	}
	public EnemySpawner()
	{
		GameManager.instance.restartGame += RestartGame;
	}
	public void InitializeEnemySpawner(GameObject _enemyPrefab, uint _enemyCount)
	{
		enemyPrefab = _enemyPrefab;
		enemyCount = _enemyCount;
		enemies = new List<GameObject>();
		areAllEnemyDied = false;
	}
	public void SpawnEnemy(Vector3 spawnPosition)
	{
		enemies.Add(MonoBehaviour.Instantiate(enemyPrefab, spawnPosition, Quaternion.identity));
	}
	public void RemoveEnemy(GameObject _enemy)
	{
		enemies.Remove(_enemy);
		GameManager.instance.UpdateScore();
		if (enemies.Count == 0)
		{
			//GameManager.instance.GameStatus(true);
			ServiceLocator.GetService<IGridHandler>().EnableHomePortal();
			areAllEnemyDied = true;
			return;
		}
	}
	void RestartGame()
	{
		areAllEnemyDied = false;
		for (int i = 0; i < enemies.Count; i++)
		{
			MonoBehaviour.Destroy(enemies[i]);
		}
		enemies.Clear();
	}
}
