using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "unamed_subunit", menuName = "UnitData")]
public class UnitData : ScriptableObject
{
    public string id;

    // Unit Type
    public Type unitType;
    public Subtype unitSubtype;

    
    [SerializeField] List<WeaponData> weapons = new List<WeaponData>();

    public int health;
    public int defence;
    public int morale;
    public int speed;
    public int shock;


    [SerializeField] GameObject prefab;

    public GameObject Instantiate(Unit unit)
    {
        GameObject gameObject = Instantiate<GameObject>(prefab);
        Subunit subunit = gameObject.AddComponent<Subunit>();
        subunit.unitData = this;
        subunit.Instantiate(unit);
        return gameObject;
    }

    public enum Type
    {
        melee,
        ranged,
        cavalry,
        elephant
    }

    public enum Subtype
    {
        sword,
        spear,
        pike,
        archer,
        javelin,
        slinger,
        meleeCav,
        shockCav,
        missileCav,
        elephant
    }
}


