using UnityEngine;
public interface IGridHandler
{
	void InitializeGridHandler();
	void FillGrid(Vector2Int _pos, GameObject cell);
	void EmptyGrid(Vector2Int _pos);
	GameObject GetCellAtPosition(Vector2Int _pos);
	void EnableHomePortal();
}
public class GridHandler : IGridHandler
{
	private IGridGenerator gridGenerator;
	public void InitializeGridHandler()
	{
		gridGenerator = ServiceLocator.GetService<IGridGenerator>();
	}
	public void EmptyGrid(Vector2Int _pos)
	{
		gridGenerator.gridCells[_pos.x, _pos.y] = null;
	}

	public void FillGrid(Vector2Int _pos, GameObject cell)
	{
		if (gridGenerator.gridCells[_pos.x, _pos.y] == null)
			gridGenerator.gridCells[_pos.x, _pos.y] = cell;
	}

	public GameObject GetCellAtPosition(Vector2Int _pos)
	{
		GameObject gameObject = null;
		if (gridGenerator.gridCells[_pos.x, _pos.y])
			gameObject = gridGenerator.gridCells[_pos.x, _pos.y];
		return gameObject;
	}
	public void EnableHomePortal()
	{
		gridGenerator.home.GetComponent<CircleCollider2D>().isTrigger = false;
	}
}
