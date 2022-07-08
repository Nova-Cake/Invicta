using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "unamed_weapon", menuName = "WeaponData")]
public class WeaponData : ScriptableObject
{
    public string id;
    Type type;
    Subtype subtype;

    public int atkDamage;
    public int maxRange;
    public float minRange;
    public int ammo;
    public float atkAngle;
    

    public enum Type
    {
        melee,
        spear,
        ranged,
    }

    public enum Subtype
    {
        // melee
        sword,
        axe,
        mace,

        // spear
        spear,
        pike,

        // ranged
        bow,
        javelin,
        sling,
    }
}
