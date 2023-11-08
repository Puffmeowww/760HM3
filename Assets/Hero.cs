using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{

    public enum HeroState
    {
        ConsideringTarget,
        Walking,
        ChasingEnemy,
        Flee,
        Attack,
    }

    public HeroState heroState;

    private GameObject targetCoin;

    CoinGenerator coinGenerator;

    //HeroMovement
    public float moveSpeed = 5.0f;
    private Rigidbody rb;

    public float rotationSpeed = 5.0f;

    private float maxHealth = 100f;
    private float currentHealth = 100f;
    HealthBar healthBar;

    public float attackDamage;
    public float attackRange = 2f;
    private GameObject attackTarget;
    public LayerMask enemyLayer;

    private Vector3 fleeDirection;

    //Animator
    Animator animator;

    public struct CoinScore
    {
        public GameObject coinObject;
        public float score;
    }
    private List<CoinScore> coinScoreList = new List<CoinScore>();

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        coinGenerator = GameObject.Find("CoinGenerator").GetComponent<CoinGenerator>();
        healthBar = GetComponentInChildren<HealthBar>();
        animator = GetComponentInChildren<Animator>();

        heroState = HeroState.ConsideringTarget;   
    }

    // Update is called once per frame
    void Update()
    {

        //Check if hero gets the coin
        if((targetCoin != null && (targetCoin.transform.position - transform.position).magnitude <= 0.5f))
        {
            coinGenerator.GetCoin(targetCoin);
            heroState = HeroState.ConsideringTarget;
            return;
        }

        switch (heroState)
        {
            case HeroState.ConsideringTarget:

                targetCoin = GetHighestScoreCoin();

                if(targetCoin != null)
                {
                    heroState = HeroState.Walking;
                    animator.SetTrigger("Move");
                }
                break;


            case HeroState.Walking:

                if (targetCoin != null)
                {
                    MoveTo(targetCoin.transform.position, moveSpeed);
                }

                //Check if there is enemy around
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.forward, out hit, 2f, enemyLayer))
                {
                    if (hit.collider.CompareTag("Enemy"))
                    { 
                        AttackEnemy ae = GetComponentInChildren<AttackEnemy>();
                        ae.attackTarget = hit.collider.gameObject;
                        attackTarget = hit.collider.gameObject;
                        ConsiderAttackOrFlee(attackTarget);
                    }
                }

                break;

            case HeroState.ChasingEnemy:

                if(attackTarget == null)
                {
                    heroState = HeroState.ConsideringTarget;
                    return;
                }
                //If enemy in attack range, attack
                if ((attackTarget.transform.position - transform.position).magnitude < attackRange)
                {
                    heroState = HeroState.Attack;
                    animator.SetTrigger("Attack");
                    return;
                }

                //if too far, go back to the coin
                if((targetCoin.transform.position - transform.position).magnitude <= 7f)
                {
                    heroState = HeroState.ConsideringTarget;
                    return;
                }

                //else, keep chasing enemy
                MoveTo(attackTarget.transform.position, moveSpeed);

                break;

            case HeroState.Attack:

                if (attackTarget == null)
                {
                    heroState = HeroState.ConsideringTarget;
                    return;
                }

                //If too far, chasing enemy
                if ((attackTarget.transform.position - transform.position).magnitude > attackRange)
                {
                    heroState = HeroState.ChasingEnemy;
                    animator.SetTrigger("Move");
                }

                break;

            case HeroState.Flee:

                if (attackTarget == null)
                {
                    heroState = HeroState.ConsideringTarget;
                    return;
                }

                //if far enough, go back to find coin
                if ((attackTarget.transform.position - transform.position).magnitude > 5f)
                {
                    heroState = HeroState.ConsideringTarget;
                    return;
                }

                MoveTo(fleeDirection, moveSpeed*2);

                break;

            default:

                break;
        }
    }

    private GameObject GetHighestScoreCoin()
    {

        foreach(GameObject coin in coinGenerator.coinList)
        {
            //Normalize the score
            float normalizedDistance = Mathf.Clamp01(1 - ((coin.transform.position - transform.position).magnitude / 100));

            CoinScore newCoinScore = new CoinScore();
            newCoinScore.coinObject = coin; 
            newCoinScore.score = normalizedDistance;
            coinScoreList.Add(newCoinScore);
        }

        CoinScore highestCoinScore = coinScoreList[0];
        foreach(CoinScore cs in coinScoreList)
        {
           if(cs.score > highestCoinScore.score)
            {
                highestCoinScore = cs;
            }
        }

        coinScoreList.Clear();
        return highestCoinScore.coinObject;
    }

    public void TakeDamage(float damage, GameObject enemy)
    {
        currentHealth -= damage;
        healthBar.UpdateHealthBar(currentHealth, maxHealth);
        attackTarget = enemy;

        //When getting damaged, consider attack or flee
        ConsiderAttackOrFlee(attackTarget);
    }

    private void MoveTo(Vector3 tgPos, float speed)
    {
        rb.velocity = (tgPos - transform.position).normalized * speed;
        Quaternion rotation = Quaternion.LookRotation((tgPos - transform.position).normalized);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
    }



    //According to current hp and distance to the coin
    private void ConsiderAttackOrFlee(GameObject tg)
    {
        float normalizedHp = Mathf.Clamp01(currentHealth/maxHealth);
        float normalizedDistance = Mathf.Clamp01(1 - ((tg.transform.position - transform.position).magnitude / 100));
        float score = normalizedDistance + normalizedHp;
        if (score >= 1.7)
        {
            animator.SetTrigger("Move");
            heroState = HeroState.ChasingEnemy;
        }
        else
        {
            animator.SetTrigger("Move");
            fleeDirection = -attackTarget.transform.position;
            heroState = HeroState.Flee;
        }

    }

}
