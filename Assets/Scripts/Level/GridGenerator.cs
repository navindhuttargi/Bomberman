using System.Collections.Generic;
using UnityEngine;

public interface IGridGenerator
{
	GameObject home { get; }
	GameObject[,] gridCells { get; set; }
	void InitializeLevelController(Vector2Int gridSize, GameObject _brick, GameObject _block, GameObject _homePrefab);
	void GenerateGrid();
}
public class GridGenerator : IGridGenerator
{
	private GameObject brickPrefab;
	private GameObject solidBlockPrefab;
	private GameObject homePrefab;
	private int gridWidth = 0;
	private int gridHeight = 0;
	private const int edgeValue = 2;
	private List<Vector2> emptyCells;
	private GameObject gridParent;
	public GameObject[,] gridCells { get; set; }
	public GameObject home { get; private set; }
	IPlayerSpawner playerSpawner;
	IEnemySpawner enemySpawner;
	~GridGenerator()
	{
		GameManager.instance.restartGame -= ResetGrid;
	}
	public GridGenerator()
	{
		GameManager.instance.restartGame += ResetGrid;
	}
	public void InitializeLevelController(Vector2Int gridSize, GameObject _brickPrefab, GameObject _solidBlockPrefab, GameObject _homePrefab)
	{
		gridWidth = gridSize.x;
		gridHeight = gridSize.y;
		brickPrefab = _brickPrefab;
		solidBlockPrefab = _solidBlockPrefab;
		playerSpawner = ServiceLocator.GetService<IPlayerSpawner>();
		enemySpawner = ServiceLocator.GetService<IEnemySpawner>();
		homePrefab = _homePrefab;
	}

	private void ResetGrid()
	{
		for (int i = 0; i < gridWidth + edgeValue; i++)
		{
			for (int j = 0; j < gridHeight + edgeValue; j++)
			{
				if (gridCells[i, j])
				{
					GameObject gameObject = gridCells[i, j];
					MonoBehaviour.DestroyImmediate(gameObject);
					gridCells[i, j] = null;
				}
			}
		}
		gridCells = null;
		MonoBehaviour.Destroy(gridParent);
		//GenerateGrid();
	}
	public void GenerateGrid()
	{
		gridParent = new GameObject();
		gridParent.transform.position = Vector3.zero;
		gridParent.name = "GridParent";
		InitializeGrid();
		GenerateBorder();
		GenerateSolidBlocks();
		SpawnPlayer();
		GenerateBricks();
		SpawnEnemies();
		ServiceLocator.GetService<IGridHandler>().InitializeGridHandler();
	}


	private void InitializeGrid()
	{
		gridCells = new GameObject[gridWidth + edgeValue, gridHeight + edgeValue];
		emptyCells = new List<Vector2>();
		for (int i = 0; i < gridWidth + edgeValue; i++)
		{
			for (int j = 0; j < gridHeight + edgeValue; j++)
			{
				Vector2Int cellposition = new Vector2Int(i, j);
				emptyCells.Add(cellposition);
			}
		}
	}
	private void GenerateBorder()
	{
		for (int i = 0; i < gridWidth + edgeValue; i++)
		{
			DrawEdge(new Vector2Int(i, 0));
			DrawEdge(new Vector2Int(i, gridHeight + edgeValue - 1));
		}
		for (int j = 0; j < gridHeight + edgeValue; j++)
		{
			DrawEdge(new Vector2Int(0, j));
			DrawEdge(new Vector2Int(gridWidth + edgeValue - 1, j));
		}
	}
	private void DrawEdge(Vector2Int pos)
	{
		gridCells[pos.x, pos.y] = MonoBehaviour.Instantiate(solidBlockPrefab, new Vector3(pos.x, pos.y), Quaternion.identity, gridParent.transform);
		emptyCells.Remove(pos);
	}
	private void GenerateSolidBlocks()
	{
		for (int i = 2; i < gridWidth; i += 2)
		{
			for (int j = 2; j < gridHeight; j += 2)
			{
				Vector2 cellPosition = new Vector2(i, j);
				gridCells[i, j] = MonoBehaviour.Instantiate(solidBlockPrefab, cellPosition, Quaternion.identity, gridParent.transform);
				emptyCells.Remove(cellPosition);
			}
		}
	}
	private void SpawnPlayer()
	{
		Vector2 playerSpawnPos = new Vector2(1, gridHeight);
		ServiceLocator.GetService<IPlayerSpawner>().SpawnPlayer(playerSpawnPos);
		emptyCells.Remove(playerSpawnPos);
		emptyCells.Remove(new Vector2(1, gridHeight - 1));
		emptyCells.Remove(new Vector2(2, gridHeight));
	}
	private void GenerateBricks()
	{
		int minValue = (int)((emptyCells.Count / 100f) * 30);
		int maxValue = (int)((emptyCells.Count / 100f) * 40);
		int brickCount = UnityEngine.Random.Range(minValue, maxValue);
		int num = UnityEngine.Random.Range(0, emptyCells.Count);
		for (int i = 0; i < brickCount; i++)
		{
			int cellNumber = UnityEngine.Random.Range(0, emptyCells.Count);
			Vector2Int vectorIndex = Vector2Int.FloorToInt(emptyCells[cellNumber]);
			GameObject brickGameobject = gridCells[vectorIndex.x, vectorIndex.y] = MonoBehaviour.Instantiate(brickPrefab, emptyCells[cellNumber], Quaternion.identity, gridParent.transform);
			if (i == brickCount - 1)
				home = MonoBehaviour.Instantiate(homePrefab, brickGameobject.transform.position, Quaternion.identity, gridParent.transform);
			emptyCells.Remove(vectorIndex);
		}
	}
	private void SpawnEnemies()
	{
		for (int i = 0; i < GameManager.instance.enemyCount; i++)
		{
			int num = Random.Range(0, emptyCells.Count);
			Vector2 spawnPos = emptyCells[num];
			enemySpawner.SpawnEnemy(spawnPos);
			emptyCells.Remove(spawnPos);
		}
	}
}
