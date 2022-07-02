using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "unamed_subunit", menuName = "SubunitObject")]
public class SubunitObject : ScriptableObject
{
    public string id;
    public UnitObject.UnitType unitType;
    public UnitObject.UnitSubtype unitSubtype;
    public UnitObject unit;

    public int meleeAtk;
    public int rangedAtk;
    public int ammo;
    public int range;
    public int health;
    public int defence;
    public int shock;
    public int morale;
}
