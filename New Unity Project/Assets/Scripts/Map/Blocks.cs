using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blocks : MonoBehaviour
{
    [Header("Basic Parameters")]
    public int bricks_health;
    public SpriteRenderer sr;

    public void BricksTakeDamage(int damage)
    {
        StartCoroutine(SpriteFlash(sr));
        bricks_health -= damage;
        if (bricks_health <= 0)
        {
            Destroy(this.gameObject);
            AstarPath.active.Scan();
        }
    }

    IEnumerator SpriteFlash(SpriteRenderer sr)
    {
        sr.color = Color.red;
        yield return new WaitForSeconds(0.05f);
        sr.color = Color.gray;
    }
}
