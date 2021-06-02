using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : Projectile
{
	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.gameObject.CompareTag("Player"))
		{
			if (IsHit())
			{
				col.gameObject.GetComponent<Player>().TakeDamage(bulletDamage);
			}
			Destroy(this.gameObject);
		}
		else if (col.gameObject.CompareTag("Teammate"))
		{
			if (IsHit())
			{
				col.gameObject.GetComponent<BotsAI>().takeDamage(bulletDamage);
			}
			Destroy(this.gameObject);
		}
		else if (col.gameObject.CompareTag("PlayerBase"))
		{
			col.gameObject.GetComponent<BaseSpawner>().TakeDamage(bulletDamage);
			Destroy(this.gameObject);
		}
		else if (col.gameObject.CompareTag("CanBreak"))
		{
			col.gameObject.GetComponent<Blocks>().BricksTakeDamage(bulletDamage);
			Destroy(this.gameObject);
		}
		else if (col.gameObject.CompareTag("CantBreak"))
		{
			Destroy(this.gameObject);
		}
	}
}