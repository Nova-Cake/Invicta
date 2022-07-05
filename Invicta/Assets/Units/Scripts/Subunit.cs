using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Subunit : MonoBehaviour
{
    public SubunitObject unitData;
    public Unit unit;
    [SerializeField] GameObject hitbox;

    public string id;
    public SubunitObject.UnitType unitType;
    public SubunitObject.UnitSubtype unitSubtype;
    NavMeshAgent agent;

    public int meleeAtk;
    public int health;
    public int defence;
    public int shock;
    public int morale;
    public int rangedAtk;
    public int ammo;
    public int range;


    public void Instantiate()
    {
        hitbox = GameObject.Find(this.gameObject.name + "/hitbox");
        GameObject.Find("UnitSelection").GetComponent<UnitSelection>().selectableObjects.Add(hitbox);
        agent = this.gameObject.GetComponent<NavMeshAgent>();
        hitbox.layer = 11;
        
        health = unitData.health;
        morale = unitData.morale;
        ammo = unitData.ammo;

        this.gameObject.GetComponent<NavMeshAgent>().speed = unitData.speed;
        this.gameObject.name = unitData.id;
        unit = this.gameObject.GetComponentInParent<Unit>();
    }

    public void AttackUnit(Unit targetUnit)
    {
        Subunit target = GetClosest(targetUnit);
        agent.SetDestination(target.transform.position);
    }

    public Subunit GetClosest(Unit targetUnit)
    {
        Subunit closest = targetUnit.aliveSubunits[0];
        float distance = Vector3.Distance(this.transform.position, closest.transform.position);
        foreach(Subunit subunit in targetUnit.aliveSubunits)
        {
            if(Vector3.Distance(this.transform.position, subunit.transform.position) < distance)
            {
                closest = subunit;
                distance = Vector3.Distance(this.transform.position, subunit.transform.position);
            }
        }

        return closest;
    }
}
