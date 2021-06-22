using System.Collections;
using UnityEngine;
public interface IBomb
{
	bool bombExploded { get; }
	void InitializeBomb(GameObject _explosionPrefab, GameObject _bombPrefab);
	IEnumerator LaunchBomb(Vector3 bombPosition);
}
public class Bomb : IBomb
{
	private float explodeTime = 3;
	private GameObject explosionPrefab;
	private GameObject bombRefObj;
	private IGridHandler gridHandler;
	public bool bombExploded { get; private set; }
	public void InitializeBomb(GameObject _explosionPrefab, GameObject _bombPrefab)
	{
		explosionPrefab = _explosionPrefab;
		gridHandler = ServiceLocator.GetService<IGridHandler>();
		bombRefObj = MonoBehaviour.Instantiate(_bombPrefab, Vector3.zero, Quaternion.identity);
		bombRefObj.SetActive(false);
		bombExploded = true;
	}
	public Bomb()
	{
		GameManager.instance.restartGame += ResetBomb;
	}
	void ResetBomb()
	{
		MonoBehaviour.Destroy(bombRefObj);
	}
	~Bomb()
	{
		GameManager.instance.restartGame -= ResetBomb;
	}
	public IEnumerator LaunchBomb(Vector3 bombPosition)
	{
		bombPosition.x = Mathf.Round(bombPosition.x);
		bombPosition.y = Mathf.Round(bombPosition.y);
		gridHandler.FillGrid(Vector2Int.FloorToInt(bombPosition), bombRefObj);
		bombRefObj.SetActive(true);
		bombRefObj.transform.position = bombPosition;
		float tempExplodeTime = explodeTime;
		bombExploded = false;
		while (tempExplodeTime > 0)
		{
			tempExplodeTime -= Time.deltaTime;
			yield return null;
		}
		ExplodeCells(bombPosition);
		bombExploded = true;
		bombRefObj.SetActive(false);
		gridHandler.EmptyGrid(Vector2Int.FloorToInt(bombPosition));
		tempExplodeTime = .4f;
		while (tempExplodeTime > 0)
		{
			tempExplodeTime -= Time.deltaTime;
			yield return null;
		}
	}
	void ExplodeCells(Vector2 bombPosition)
	{
		ExplodeCell(bombPosition + Vector2.zero);
		ExplodeCell(bombPosition + Vector2.up);
		ExplodeCell(bombPosition + Vector2.down);
		ExplodeCell(bombPosition + Vector2.left);
		ExplodeCell(bombPosition + Vector2.right);
	}
	void ExplodeCell(Vector2 targetPosition)
	{
		GameObject cell = gridHandler.GetCellAtPosition(Vector2Int.FloorToInt(targetPosition));
		if (cell != null)
		{
			if (cell.GetComponent<SolidBlock>() != null)
				return;
			else
			{
				MonoBehaviour.Instantiate(explosionPrefab, targetPosition, Quaternion.identity);
				gridHandler.EmptyGrid(Vector2Int.FloorToInt(targetPosition));
			}
		}
		else
		{
			MonoBehaviour.Instantiate(explosionPrefab, targetPosition, Quaternion.identity);
		}
	}
}
