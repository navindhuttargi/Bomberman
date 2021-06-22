using UnityEngine;

public class GameManager : MonoBehaviour
{
	[Tooltip("Grid size Should odd order")]
	[SerializeField]
	private Vector2Int _gridSize;

	[SerializeField]
	private GameObject brickPrefab;
	[SerializeField]
	private GameObject solidBlockPrefab;
	[SerializeField]
	private GameObject homePrefab;
	public uint enemyCount;
	[SerializeField]
	private GameObject enemyPrefab;
	[SerializeField]
	private GameObject playerPrefab;

	private static GameManager _instance;
	public static GameManager instance
	{
		get
		{
			if (_instance == null)
				_instance = FindObjectOfType<GameManager>();
			return _instance;
		}
	}
	public System.Action restartGame;
	public System.Action scoreUpdate;
	public System.Action<bool> gameStatus;
	public UIManager uIManager;
	private void Awake()
	{
		if (_instance != null)
			Destroy(gameObject);
		else
		{
			_instance = this;
		}
	}
	void Start()
	{
		ServiceLocator.InitializeContainer();
		GameReady();
		ServiceLocator.GetService<IGridGenerator>().InitializeLevelController(_gridSize, brickPrefab, solidBlockPrefab, homePrefab);
		ServiceLocator.GetService<IPlayerSpawner>().InitializePlayerSpawner(playerPrefab);
		ServiceLocator.GetService<IEnemySpawner>().InitializeEnemySpawner(enemyPrefab, enemyCount);
	}
	public void GameReady()
	{
		StartCoroutine(uIManager.LevelIntro(ServiceLocator.GetService<IGridGenerator>().GenerateGrid));
	}
	public void RestartGame()
	{
		StartCoroutine(uIManager.LevelIntro(() =>
		{
			restartGame?.Invoke();
			ServiceLocator.GetService<IGridGenerator>().GenerateGrid();
		}));
	}
	public void GameStatus(bool isWon)
	{
		gameStatus?.Invoke(isWon);
	}
	public void UpdateScore()
	{
		scoreUpdate?.Invoke();
	}
}
