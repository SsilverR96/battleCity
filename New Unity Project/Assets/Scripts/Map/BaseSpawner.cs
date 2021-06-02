using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject[] pointsForSpawn;
    public GameObject playerPrefab;
    public GameObject teammatePrefab;
    public GameObject enemyPrefab;

    [Header("Basic Parameters")]
    public float health;
    public int tanksLimit;

    [Header("Sprite Renderer")]
    public SpriteRenderer sr;

    void Awake()
    {
        if (this.gameObject.CompareTag("PlayerBase"))
        {
            SpawnPlayer();
            tanksLimit--;
        }
    }
    void Update()
    {
        if (this.gameObject.CompareTag("EnemyBase"))
        {
            GameObject[] Enemies = GameObject.FindGameObjectsWithTag("Enemy");
            if ((Enemies.Length < 2) && tanksLimit != 0)
            {
                SpawnEnemy();
                tanksLimit--;
            }
        }
        else if (this.gameObject.CompareTag("PlayerBase"))
        {
            if (GameObject.FindGameObjectWithTag("Teammate") == null)
            {
                SpawnTeammate();
                tanksLimit--;
            }
        }
    }
    void SpawnEnemy() 
    {
        int rs = Random.Range(0, pointsForSpawn.Length);
        Instantiate(enemyPrefab, pointsForSpawn[rs].transform.position, Quaternion.identity);
    }
    void SpawnPlayer()
    {
        int rs = Random.Range(0, pointsForSpawn.Length);
        Instantiate(playerPrefab, pointsForSpawn[rs].transform.position, Quaternion.identity);
    }
    void SpawnTeammate()
    {
        int rs = Random.Range(0, pointsForSpawn.Length);
        Instantiate(teammatePrefab, pointsForSpawn[rs].transform.position, Quaternion.identity);
    }
    public void TakeDamage(int damage) 
    {
        StartCoroutine(SpriteFlash(sr));
        health -= damage;
        if (health <= 0)
        {
            Destroy(this.gameObject);
            if (this.gameObject.CompareTag("PlayerBase"))
                Game.g.GameOver();
            else
                Game.g.Win();
        }
    }
    IEnumerator SpriteFlash(SpriteRenderer sr)
    {
        sr.color = Color.red;
        yield return new WaitForSeconds(0.05f);
        sr.color = Color.white;
    }
}