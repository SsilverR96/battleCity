using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class TeammateAI : BotsAI
{
    [Header("Prefabs")]
    public GameObject friendlyBullet;

    public override void MakeBullet()
    {
        Vector2 temp = new Vector2(x, y);
        FriendlyBullet bullet = Instantiate(friendlyBullet, transform.position, Quaternion.identity).GetComponent<FriendlyBullet>();
        bullet.Setup(temp, ChooseBulletDirection(), tanktowerlvl, firingrange);
        SetBulletDamage(bullet);
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
}
