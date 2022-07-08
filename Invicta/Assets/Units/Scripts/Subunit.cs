using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Subunit : MonoBehaviour
{
    public Unit unit;
    public UnitData unitData;
    private int _health;
    public int health
    {
        get {return _health;}
        set
        {
            _health = value;
            if(_health <= 0)
            {
                Destroy(this.gameObject);
            }
        }
    }

    public NavMeshAgent agent;
    GameObject hitbox;

    public void Instantiate(Unit parent)
    {
        unit = parent;
        agent = gameObject.GetComponent<NavMeshAgent>();

        // Hitbox
        hitbox = GameObject.Find(this.gameObject.name + "/hitbox");
        GameObject.Find("UnitSelection").GetComponent<UnitSelection>().selectableObjects.Add(hitbox);
        hitbox.layer = 11;
    }
}
