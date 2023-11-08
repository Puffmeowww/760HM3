using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackEnemy : MonoBehaviour
{
    public GameObject attackTarget;
    public Hero hero;

    // Start is called before the first frame update
    void Start()
    {
        hero = GetComponentInParent<Hero>();
    }

    public void Attack()
    {
        if(attackTarget)
        {
            Enemy e = attackTarget.GetComponent<Enemy>();
            e.TakeDamage(hero.attackDamage);
        }
        
    }
}
