using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Module : MonoBehaviour
{
    private string moduleName;
    private int moduleLvl;
    private GunType gunType;

    [Header("Sprites")]
    public SpriteRenderer sr;
    public Sprite[] modules;

    public void Setter(string name, int lvl)
    {
        moduleName = name;
        moduleLvl = lvl;
    }

    public void takeGunType(GunType gun)
    {
        gunType = gun;
    }

    public void Setup()
    {
        if (moduleName == "Gun")
            sr.sprite = modules[0];
        else if (moduleName == "TankHull")
            sr.sprite = modules[1];
        else if (moduleName == "TankTower")
            sr.sprite = modules[2];

        switch (moduleLvl)
        {
            case 1:
                sr.color = new Color(176, 115, 64);
                break;
            case 2:
                sr.color = Color.white;
                break;
            case 3:
                sr.color = new Color(250, 215, 84);
                break;
        }
    }

    public string getModuleName()
    {
        return moduleName;
    }

    public int getModuleLvl()
    {
        return moduleLvl;
    }

    public GunType getGunType()
    {
        return gunType;
    }

    public void DestroyModuleItem()
    {
        Destroy(this.gameObject);
    }
}