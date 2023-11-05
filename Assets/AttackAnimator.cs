using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAnimator : MonoBehaviour
{

    Hero hero;
    Enemy enemy;

    // Start is called before the first frame update
    void Start()
    {
        hero = GameObject.Find("Human").GetComponent<Hero>();
        enemy = GetComponentInParent<Enemy>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Attack()
    {
        hero.TakeDamage(enemy.attackDamage);
    }
}
