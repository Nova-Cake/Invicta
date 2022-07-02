using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "unamed_unit", menuName = "UnitObject")]
public class UnitObject : ScriptableObject
{
    public string id;
    public string unitType;
    public string unitSubtype;
    public SubunitObject subunit;

    public enum UnitType
    {
        melee,
        ranged,
        cavalry,
        elephant,
    }

    public enum UnitSubtype
    {
        sword
    }

}

