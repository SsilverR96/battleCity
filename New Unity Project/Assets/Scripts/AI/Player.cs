using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D myRigidbody;
    private Animator myAnim;
    private Vector3 change;
    public bool triggerModule = false;
    private GameObject ModuleGmObj;

    [Header("Player state machine")]
    public PlayerState currentState;

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
    public GameObject projectilePrefab;

    [Header("Sprite Renderer")]
    public SpriteRenderer sr;

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
    }
    void Start()
    {
        currentState = PlayerState.walk;
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnim = GetComponent<Animator>();
        myAnim.SetFloat("moveX", 0);
        myAnim.SetFloat("moveY", 1);
    }
    void Update()
    {
        change = Vector3.zero;
        change.x = Input.GetAxis("Horizontal");
        change.y = Input.GetAxis("Vertical");
        if (Input.GetButtonDown("Attack") && (currentState != PlayerState.attack) && (currentState != PlayerState.stagger))
        {
            StartCoroutine(AttackCo());
        }
        else if (Input.GetButtonDown("PickupItem") && triggerModule)
        {
            string name = ModuleGmObj.gameObject.GetComponent<Module>().getModuleName();
            switch (name)
            {
                case ("Gun"):
                    currentGun = ModuleGmObj.gameObject.GetComponent<Module>().getGunType();
                    gunlvl = ModuleGmObj.gameObject.GetComponent<Module>().getModuleLvl();
                    SetFiringRange();
                    break;
                case ("TankHull"):
                    tankhulllvl = ModuleGmObj.gameObject.GetComponent<Module>().getModuleLvl();
                    CheckTankHull();
                    break;
                case ("TankTower"):
                    tanktowerlvl = ModuleGmObj.gameObject.GetComponent<Module>().getModuleLvl();
                    break;
            }
            ModuleGmObj.gameObject.GetComponent<Module>().DestroyModuleItem();
        }
        else if (Input.GetButtonDown("DestroyItem") && triggerModule)
        {
            ModuleGmObj.gameObject.GetComponent<Module>().DestroyModuleItem();
        }
        else if (currentState == PlayerState.walk)
        {
            UpdateAnimationAndMove();
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
    void UpdateAnimationAndMove()
    {
        if (change != Vector3.zero)
        {
            MovePlayer();
            myAnim.SetFloat("moveX", change.x);
            myAnim.SetFloat("moveY", change.y);
        }
    }
    void MovePlayer()
    {
        change.Normalize();
        myRigidbody.MovePosition(transform.position + change * speed * Time.deltaTime);
    }
    private IEnumerator AttackCo()
    {
        currentState = PlayerState.attack;
        yield return null;
        if (currentGun == GunType.armor_piercing)
            MakeBullet();
        else
        {
            MakeBullet();
            yield return new WaitForSeconds(.2f);
            MakeBullet();
        }
        yield return new WaitForSeconds(.3f);
        currentState = PlayerState.walk;
    }
    private void MakeBullet()
    {
        Vector2 temp = new Vector2(myAnim.GetFloat("moveX"), myAnim.GetFloat("moveY"));
        FriendlyBullet bullet = Instantiate(projectilePrefab, transform.position, Quaternion.identity).GetComponent<FriendlyBullet>();
        bullet.Setup(temp, ChooseBulletDirection(), tanktowerlvl, firingrange);
        SetBulletDamage(bullet);
    }
    Vector3 ChooseBulletDirection()
    {
        float temp = Mathf.Atan2(myAnim.GetFloat("moveY"), myAnim.GetFloat("moveX")) * Mathf.Rad2Deg;
        return new Vector3(0, 0, temp);
    }
    void SetBulletDamage(FriendlyBullet bullet)
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
    public void TakeDamage(int damage)
    {
        StartCoroutine(TakeDamageCo(damage));
        StartCoroutine(SpriteFlash(sr));
    }
    private IEnumerator TakeDamageCo(int damage)
    {
        currentState = PlayerState.stagger;
        yield return null;
        health -= damage;
        if (health > 0)
        {
            yield return new WaitForSeconds(.3f);
            currentState = PlayerState.walk;
        }
        else
        {
            Die();
        }
    }
    void Die()
    {
        Destroy(this.gameObject);
        Game.g.GameOver();
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Module") && (this.CompareTag("Player")))
        {
            ModuleGmObj = col.gameObject;
            triggerModule = true;
        }
    }
    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Module") && (this.CompareTag("Player")))
        {
            triggerModule = false;
        }
    }
    IEnumerator SpriteFlash(SpriteRenderer sr)
    {
        sr.color = Color.red;
        yield return new WaitForSeconds(0.05f);
        sr.color = Color.white;
    }
}