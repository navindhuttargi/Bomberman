using UnityEngine;

public class Brick : MonoBehaviour, IDamage
{
	private IGridHandler gridHandler;
	private void Start()
	{
		gridHandler = ServiceLocator.GetService<IGridHandler>();
	}
	public void Damage()
	{
		gridHandler.EmptyGrid(new Vector2Int((int)transform.position.x, (int)transform.position.y));
		Destroy(gameObject);
	}
}
