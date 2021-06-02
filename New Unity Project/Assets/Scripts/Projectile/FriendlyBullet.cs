using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendlyBullet : Projectile
{
	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.gameObject.CompareTag("Enemy"))
		{
			if (IsHit())
			{
				col.gameObject.GetComponent<BotsAI>().takeDamage(bulletDamage);
			}
			Destroy(this.gameObject);
		}
		else if (col.gameObject.CompareTag("EnemyBase"))
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