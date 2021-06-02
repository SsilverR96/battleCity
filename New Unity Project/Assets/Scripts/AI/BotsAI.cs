using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class BotsAI : MonoBehaviour
{
    [Header("Gun")]
    public GunType currentGun;
    public int gunlvl;
    public int firingrange;

    [Header("Tank Hull")]
    public int tankhulllvl;
    public int health;
    public float speed;
    private float speedInitial;

    [Header("Tank Tower")]
    public int tanktowerlvl;

    [Header("Prefabs")]
    public GameObject modulePrefab;
    public Transform botGFX;
    public GameObject WallDetecter;
    public GameObject enemyBullet;

    [Header("Sprite Renderer")]
    public SpriteRenderer sr;

    [Header("Target")]
    public Transform targetPosition;
    public Path path; 
    public float nextWaypointDistance = 3;
    public bool reachedEndOfPath;
    public bool canMove = true;
    public BotType behavior;
    public int x, y;

    private int currentWaypoint = 0;
    private Seeker seeker;
    private Rigidbody2D myRigidbody;

    void Awake()
    {
        int rGunType = Random.Range(0, 2);
        gunlvl = Random.Range(1, 4);
        switch (rGunType)
        {
            case 0:
                currentGun = GunType.rapid_fire;
                break;
            case 1:
                currentGun = GunType.armor_piercing;
                break;
        }
        SetFiringRange();
        speedInitial = speed;
        tankhulllvl = Random.Range(1, 4);
        CheckTankHull();
        tanktowerlvl = Random.Range(1, 4);

        int rBotType = Random.Range(0, 2);
        switch (rBotType)
        {
            case 0:
                behavior = BotType.Destroyer;
                break;
            case 1:
                behavior = BotType.Stormtropper;
                break;
        }
        ConfigBehavior();
    }

    void ConfigBehavior()
    {
        switch (behavior)
        {
            case (BotType.Destroyer):
                if (this.gameObject.CompareTag("Enemy"))
                {
                    targetPosition = GameObject.FindWithTag("PlayerBase").transform;
                }
                else if (this.gameObject.CompareTag("Teammate"))
                {
                    targetPosition = GameObject.FindWithTag("EnemyBase").transform;
                }
                break;
            case (BotType.Stormtropper):
                if (this.gameObject.CompareTag("Enemy"))
                {
                    targetPosition = GameObject.FindWithTag("Player").transform;
                }
                else if (this.gameObject.CompareTag("Teammate"))
                {
                    targetPosition = GameObject.FindWithTag("Enemy").transform;
                }
                break;
        }
    }
    void Start()
    {
        seeker = GetComponent<Seeker>();
        myRigidbody = GetComponent<Rigidbody2D>();
        InvokeRepeating("UpdatePath", 0f, 0.5f);
    }
    void UpdatePath()
    {
        if (seeker.IsDone())
            seeker.StartPath(transform.position, targetPosition.position, OnPathComplete);
    }
    public void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }
    void FixedUpdate()
    {
        if (path == null)
        {
            return;
        }
        reachedEndOfPath = false;
        float distanceToWaypoint;
        while (true)
        {
            distanceToWaypoint = Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]);
            if (distanceToWaypoint < nextWaypointDistance)
            {
                if (currentWaypoint + 1 < path.vectorPath.Count)
                {
                    currentWaypoint++;
                }
                else
                {
                    Attack();
                    reachedEndOfPath = true;
                    break;
                }
            }
            else
            {
                break;
            }
        }
        if (canMove)
        {
            Vector3 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
            Vector3 velocity = dir * speed * Time.deltaTime;
            UpdateAnim(velocity);
            myRigidbody.transform.position += velocity * Time.deltaTime;
        }
    }

    private void LateUpdate()
    {
        if (targetPosition == null) 
        {
            ConfigBehavior();
        }
    }
    void UpdateAnim(Vector3 velocity)
    {
        if (velocity.y >= 0.01f)
        {
            botGFX.rotation = Quaternion.Euler(0, 0, 0);
            WallDetecter.transform.rotation = Quaternion.Euler(0, 0, 0);
            x = 0;
            y = 1;
        }
        else if (velocity.y <= -0.01f)
        {
            botGFX.rotation = Quaternion.Euler(0, 0, 180);
            WallDetecter.transform.rotation = Quaternion.Euler(0, 0, 180);
            x = 0;
            y = -1;
        }
        else if (velocity.x >= 0.01f)
        {
            botGFX.rotation = Quaternion.Euler(0, 0, 270);
            WallDetecter.transform.rotation = Quaternion.Euler(0, 0, 270);
            x = 1;
            y = 0;
        }
        else if (velocity.x <= -0.01f)
        {
            botGFX.rotation = Quaternion.Euler(0, 0, 90);
            WallDetecter.transform.rotation = Quaternion.Euler(0, 0, 90);
            x = -1;
            y = 0;
        }
    }
    void CheckTankHull()
    {
        switch (tankhulllvl)
        {
            case 1:
                health = 4;
                speed = speedInitial;
                break;
            case 2:
                health = 8;
                speed /= 1.5f;
                break;
            case 3:
                health = 12;
                speed /= 2f;
                break;
        }
    }
    void SetFiringRange()
    {
        if (currentGun == GunType.rapid_fire)
            switch (gunlvl)
            {
                case 1:
                    firingrange = 3;
                    break;
                case 2:
                    firingrange = 4;
                    break;
                case 3:
                    firingrange = 5;
                    break;
            }
        else if (currentGun == GunType.armor_piercing)
            switch (gunlvl)
            {
                case 1:
                    firingrange = 4;
                    break;
                case 2:
                    firingrange = 6;
                    break;
                case 3:
                    firingrange = 8;
                    break;
            }
    }
    public void takeDamage(int damage)
    {
        StartCoroutine(SpriteFlash(sr));
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }
    void Die()
    {
        Module module = Instantiate(modulePrefab, transform.position, Quaternion.identity).GetComponent<Module>();
        int RndM = Random.Range(0, 3);
        switch (RndM)
        {
            case 0:
                module.Setter("Gun", gunlvl);
                module.takeGunType(currentGun);
                break;
            case 1:
                module.Setter("TankHull", tankhulllvl);
                break;
            case 2:
                module.Setter("TankTower", tanktowerlvl);
                break;
        }
        module.Setup();
        Destroy(this.gameObject);
    }
    void Attack()
    {
        StartCoroutine(AttackCo());
    }

    private IEnumerator AttackCo()
    {
        canMove = false;
        new WaitForSeconds(.1f);
        if (currentGun == GunType.armor_piercing)
            MakeBullet();
        else
        {
            MakeBullet();
            yield return new WaitForSeconds(.2f);
            MakeBullet();
        }
        yield return new WaitForSeconds(.3f);
        canMove = true;
    }
    public virtual void MakeBullet()
    {
        Vector2 temp = new Vector2(x, y);
        EnemyBullet bullet = Instantiate(enemyBullet, transform.position, Quaternion.identity).GetComponent<EnemyBullet>();
        bullet.Setup(temp, ChooseBulletDirection(), tanktowerlvl, firingrange);
        SetBulletDamage(bullet);
    }
    public Vector3 ChooseBulletDirection()
    {
        float temp = Mathf.Atan2(y, x) * Mathf.Rad2Deg;
        return new Vector3(0, 0, temp);
    }
    void SetBulletDamage(EnemyBullet bullet)
    {
        if (currentGun == GunType.rapid_fire)
        {
            switch (gunlvl)
            {
                case 1:
                    bullet.SetDamage(1);
                    break;
                case 2:
                    bullet.SetDamage(2);
                    break;
                case 3:
                    bullet.SetDamage(3);
                    break;
            }
        }
        else if (currentGun == GunType.armor_piercing)
        {
            switch (gunlvl)
            {
                case 1:
                    bullet.SetDamage(2);
                    break;
                case 2:
                    bullet.SetDamage(4);
                    break;
                case 3:
                    bullet.SetDamage(6);
                    break;
            }
        }
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("CanBreak"))
        {
            Attack();
        }
        if ( col.gameObject.CompareTag("Module") && (this.CompareTag("Enemy") || this.CompareTag("Teammate")))
        {
            if (CheckModule(col))
            {
                ReplaceModule(col);
                col.gameObject.GetComponent<Module>().DestroyModuleItem();
            }
            else
            {
                col.gameObject.GetComponent<Module>().DestroyModuleItem();
            }
        }
    }
    bool CheckModule(Collider2D col)
    {
        string name = col.gameObject.GetComponent<Module>().getModuleName();

        switch (name)
        {
            case ("Gun"):
                if(gunlvl < col.gameObject.GetComponent<Module>().getModuleLvl())
                {
                    return true;
                }
                break;

            case ("TankHull"):
                if (tankhulllvl < col.gameObject.GetComponent<Module>().getModuleLvl())
                {
                    return true;
                }
                break;

            case ("TankTower"):
                if (tanktowerlvl < col.gameObject.GetComponent<Module>().getModuleLvl())
                {
                    return true;
                }
                break;
        }
        return false;
    }
    void ReplaceModule(Collider2D col)
    {
        string name = col.gameObject.GetComponent<Module>().getModuleName();

        switch (name)
        {
            case ("Gun"):
                currentGun = col.gameObject.GetComponent<Module>().getGunType();
                gunlvl = col.gameObject.GetComponent<Module>().getModuleLvl();
                SetFiringRange();
                break;
            case ("TankHull"):
                tankhulllvl = col.gameObject.GetComponent<Module>().getModuleLvl();
                CheckTankHull();
                break;
            case ("TankTower"):
                tanktowerlvl = col.gameObject.GetComponent<Module>().getModuleLvl();
                break;
        }
    }
    IEnumerator SpriteFlash(SpriteRenderer sr)
    {
        sr.color = Color.red;
        yield return new WaitForSeconds(0.05f);
        sr.color = Color.white;
    }
}