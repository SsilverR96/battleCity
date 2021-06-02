using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
	[Header("Basic Parameters")]
	public float speed;
	public Rigidbody2D myRigidbody;
	public int bulletDamage;
	public int towerlvl;
	public int firingrange;

    public void Setup(Vector2 velocity, Vector3 direction, int tanktowerlvl, int range)
	{
		myRigidbody.velocity = velocity.normalized * speed;
		transform.rotation = Quaternion.Euler(direction);
		towerlvl = tanktowerlvl;
		firingrange = range;
	}

	void Start()
	{
		Destroy(gameObject, firingrange / speed);
	}

	public void SetDamage(int damage)
	{
		bulletDamage = damage;
	}
	public bool IsHit()
	{
		float rcoh = Random.Range(1, 100);
		switch (towerlvl)
		{
			case 1:
				if (rcoh < 71)
					return true;
				break;
			case 2:
				if (rcoh < 81)
					return true;
				break;
			case 3:
				if (rcoh < 91)
					return true;
				break;
		}
		return false;
	}
}