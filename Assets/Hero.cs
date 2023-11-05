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

        if((targetCoin != null && (targetCoin.transform.position - transform.position).magnitude <= 0.5f))
        {
            coinGenerator.GetCoin(targetCoin);
            heroState = HeroState.ConsideringTarget;
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 2f, enemyLayer))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                ConsiderAttackOrFlee(hit.collider.gameObject);
                AttackEnemy ae = GetComponentInChildren<AttackEnemy>();
                ae.attackTarget = hit.collider.gameObject;
                attackTarget = hit.collider.gameObject;

            }
        }

        switch (heroState)
        {
            case HeroState.ConsideringTarget:

                targetCoin = GetHighestScoreCoin();

                if(targetCoin != null)
                {
                    heroState = HeroState.Walking;
                }
                break;


            case HeroState.Walking:

                if (targetCoin != null)
                {
                    Vector3 direction = (targetCoin.transform.position - transform.position).normalized;
                    rb.velocity = direction * moveSpeed;

                    Quaternion rotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
                }

                break;

            case HeroState.Attack:
                
                if((attackTarget.transform.position - transform.position).magnitude > attackRange)
                {
                    heroState = HeroState.ConsideringTarget;
                    animator.SetTrigger("Move");
                    Debug.Log("stop");
                }

                break;

            case HeroState.Flee:

                if ((attackTarget.transform.position - transform.position).magnitude > 5f)
                {
                    heroState = HeroState.ConsideringTarget;
                }

                Vector3 reverseDirection = -(attackTarget.transform.position - transform.position).normalized;
                rb.velocity = reverseDirection * moveSpeed;
                Debug.Log("Flee");
                Quaternion rotation1 = Quaternion.LookRotation(reverseDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation1, rotationSpeed * Time.deltaTime);

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

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        healthBar.UpdateHealthBar(currentHealth, maxHealth);
    }


    //According to current hp and distance to the coin
    private void ConsiderAttackOrFlee(GameObject tg)
    {
        float normalizedHp = Mathf.Clamp01(currentHealth/maxHealth);
        float normalizedDistance = Mathf.Clamp01(1 - ((tg.transform.position - transform.position).magnitude / 100));
        float score = normalizedDistance + normalizedHp;
        if (score >= 1.7)
        {
            animator.SetTrigger("Attack");
            heroState = HeroState.Attack;
        }
        else
        {
            animator.SetTrigger("Move");
            heroState = HeroState.Flee;
        }

    }

}
