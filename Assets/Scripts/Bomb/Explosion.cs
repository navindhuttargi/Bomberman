using UnityEngine;

public class Explosion : MonoBehaviour
{
	// Start is called before the first frame update
	void Start()
	{
		Destroy(gameObject, .4f);
	}
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.GetComponent<IDamage>() != null)
			collision.GetComponent<IDamage>().Damage();
	}
}
