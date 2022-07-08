using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class _Subunit : MonoBehaviour
{
    public UnitData unitData;
    public _Unit unit;
    [SerializeField] GameObject hitbox;

    public string id;
    public UnitData.Type unitType;
    public UnitData.Subtype unitSubtype;
    NavMeshAgent agent;

    public int health;
    public int morale;
    public int ammo;


    GameObject healthBarCanvas;
    
    public List<_Subunit> targeters = new List<_Subunit>();
    public _Subunit target;
    int tick = 0;
    int maxTick = 5;
    float attackTimer = 2.5f;


    public void Instantiate(_Unit parent)
    {
        unit = parent;
        hitbox = GameObject.Find(this.gameObject.name + "/hitbox");
        GameObject.Find("_UnitSelection").GetComponent<UnitSelection>().selectableObjects.Add(hitbox);
        agent = this.gameObject.GetComponent<NavMeshAgent>();
        hitbox.layer = 11;
        
        health = unitData.health;
        morale = unitData.morale;
        ammo = 5;

        this.gameObject.GetComponent<NavMeshAgent>().speed = unitData.speed;
        this.gameObject.name = unitData.id;
        unit = this.gameObject.GetComponentInParent<_Unit>();
        this.gameObject.AddComponent<Slider>();

        healthBarCanvas = Instantiate(parent.healthTemplate);
        healthBarCanvas.transform.position = this.transform.position + new Vector3(0, 1.2f, 0);
        healthBarCanvas.transform.SetParent(this.transform);
        GameObject healthBar = GameObject.Find(healthBarCanvas.gameObject.name + "/HealthBar");
        Slider healthSlider = healthBar.GetComponent<Slider>();
        healthSlider.maxValue = health;

        UpdateHealth();
    }

    void Update()
    {
        HandleAttack();
        tick = tick > maxTick ? 0 : tick + 1;
        attackTimer = attackTimer <= 0f ? 2.5f : attackTimer - Time.deltaTime;
    }

    void HandleAttack()
    {
        if(unit.target != null)
        {
            if(target == null || target.unit != this.unit.target)
            {
                target = GetClosest(unit.target);
            }

            if(Vector3.Distance(this.transform.position, target.transform.position) > 1000000 && tick == maxTick)
            {
                agent.SetDestination(target.transform.position);
            }

            if(Mathf.Abs(Vector3.Distance(this.transform.position, target.transform.position)) < 100000 && tick == maxTick)
            {
                MeleeAtk(target);
            }
        }
    }

    public void MeleeAtk(_Subunit subunit)
    {
        int damage = Mathf.RoundToInt(100000 * Random.Range(.75f, 1.25f));
        subunit.TakeDamage(damage);
    }

    public void TakeDamage(int damage)
    {
        damage = Mathf.RoundToInt(damage - unitData.defence * Random.Range(.25f, .75f));
        health -= damage;
        UpdateHealth();
    }

    public void UpdateHealth()
    {
        if(health < 1)
        {
            
            unit.alive_Subunits.Remove(this);
            Destroy(this.gameObject);
            return;
        }
        Slider healthSlider = GameObject.Find(
        healthBarCanvas.gameObject.name + "/HealthBar").GetComponent<Slider>();

        healthSlider.value = health;
        healthSlider.fillRect.gameObject.GetComponent<Image>().color = 
        Color.HSVToRGB((health / healthSlider.maxValue) / 2, 1, 1);
    }

    public _Subunit GetClosest(_Unit target_Unit)
    {
        _Subunit closest = target_Unit.alive_Subunits[0];
        float distance = Vector3.Distance(this.transform.position, closest.transform.position);
        foreach(_Subunit subunit in target_Unit.alive_Subunits)
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
