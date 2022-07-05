using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "unamed_subunit", menuName = "SubunitObject")]
public class SubunitObject : ScriptableObject
{
    public string id;
    public UnitType unitType;
    public UnitSubtype unitSubtype;

    public int meleeAtk;
    public int health;
    public int defence;
    public int shock;
    public int morale;
    public int rangedAtk;
    public int ammo;
    public int range;
    public int speed;

    [SerializeField] GameObject prefab;

    public GameObject Instantiate()
    {
        GameObject gameObject = Instantiate<GameObject>(prefab);
        Subunit subunit = gameObject.AddComponent<Subunit>();
        subunit.unitData = this;
        subunit.Instantiate();
        return gameObject;
    }

    public enum UnitType
    {
        melee,
        ranged,
        cavalry,
        elephant
    }

    public enum UnitSubtype
    {
        sword,
        spear,
        pike,
        archer,
        javelin,
        slinger,
    }
}


