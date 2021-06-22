using UnityEngine;
public class PlayerController : MonoBehaviour, IDamage
{
	[SerializeField]
	private float speed;
	[SerializeField]
	private GameObject bombPrefab;
	[SerializeField]
	private GameObject explosionPrefab;
	[SerializeField]
	private Sprite[] animationSprites;
	private SpriteRenderer playerSprite;
	private Rigidbody2D rigidbody2D;
	private float vertical;
	private float horizontal;
	private IBomb bomb;

	// Start is called before the first frame update
	void Start()
	{
		playerSprite = GetComponentInChildren<SpriteRenderer>();
		rigidbody2D = GetComponent<Rigidbody2D>();
		bomb = ServiceLocator.GetService<IBomb>();
		bomb.InitializeBomb(explosionPrefab, bombPrefab);
	}
	void Update()
	{
		GetInput();
		SetPlayerDirection();
		PlaceBomb();
	}

	private void PlaceBomb()
	{
		if (Input.GetKeyDown(KeyCode.Space) && bomb.bombExploded)
			StartCoroutine(bomb.LaunchBomb(transform.position));
	}

	void GetInput()
	{
		horizontal = Input.GetAxisRaw("Horizontal");
		vertical = Input.GetAxisRaw("Vertical");
	}
	void SetPlayerDirection()
	{
		if (Mathf.Abs(horizontal) > Mathf.Abs(vertical))
		{
			if (horizontal > 0 /*&& playerSprite.transform.rotation.z != 90*/)
				playerSprite.sprite = animationSprites[3];//playerSprite.transform.rotation =Quaternion.Euler(Vector3.forward * 90);
			else if (horizontal < 0 /*&& playerSprite.transform.rotation.z != 270*/)
				playerSprite.sprite = animationSprites[2];//playerSprite.transform.rotation = Quaternion.Euler(Vector3.forward * 270);
			vertical = 0;
		}
		else if (Mathf.Abs(vertical) > Mathf.Abs(horizontal))
		{
			if (vertical > 0 /*&& playerSprite.transform.rotation.z != 180*/)
				playerSprite.sprite = animationSprites[0];//playerSprite.transform.rotation = Quaternion.Euler(Vector3.forward * 180);
			else if (vertical < 0 /*&& playerSprite.transform.rotation.z != 0*/)
				playerSprite.sprite = animationSprites[1];//playerSprite.transform.rotation = Quaternion.identity;
			horizontal = 0;
		}
	}
	private void FixedUpdate()
	{
		Vector3 updatedVector = new Vector3(horizontal, vertical);

		rigidbody2D.MovePosition(transform.position + updatedVector * speed * Time.fixedDeltaTime);
	}
	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.GetComponent<EnemyController>())
			Damage();
		else if (collision.gameObject.GetComponent<Home>())
		{
			GameManager.instance.GameStatus(true);
		}
	}

	public void Damage()
	{
		GameManager.instance.GameStatus(false);
		Destroy(gameObject);
	}
}
