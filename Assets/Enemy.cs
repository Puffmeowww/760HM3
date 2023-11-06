using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    public enum EnemyState
    {
        ConsideringTarget,
        Walking,
        ConsideringPatrol,
        PatrollingAroundCoin,
        ChasingPlayer,
        AttackingPlayer
    }
    public EnemyState enemyState;

    //Current target coin
    private GameObject targetCoin;

    CoinGenerator coinGenerator;

    //Enemy Movement and rotation
    public float moveSpeed = 5.0f;
    public float patrollingSpeed = 0.5f;
    Rigidbody rb;
    public float rotationSpeed = 5.0f;

    //Patrolling Area Settings
    private float patrollingRadius;
    public float patrollingMaxRadius = 5.0f;
    private Vector3 randomDirection;
    private Vector3 patrolTargetPosition;

    //Detect player
    public float detectRange = 2.0f;
    public GameObject hero;

    //Attack Player
    public float attackRange = 1.0f;
    public float attackDamage = 20f;


    //HP
    private float maxHealth = 100f;
    private float currentHealth = 100f;
    HealthBar healthBar;

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
        animator = GetComponentInChildren<Animator>();
        healthBar = GetComponentInChildren<HealthBar>();
        enemyState = EnemyState.ConsideringTarget;
    }

    // Update is called once per frame
    void Update()
    {
        //If the target coin is got by hero, go to next target
        if(targetCoin == null)
        {
            enemyState = EnemyState.ConsideringTarget;
        }

        switch (enemyState)
        {
            case EnemyState.ConsideringTarget:

                targetCoin = GetHighestScoreCoin();

                if (targetCoin != null)
                {
                    enemyState = EnemyState.Walking;
                }
                break;


            case EnemyState.Walking:

                //Check if enemy is near to the coin
                if ((targetCoin != null && (targetCoin.transform.position - transform.position).magnitude <= 5f))
                {
                    //Start patrolling
                    enemyState = EnemyState.ConsideringPatrol;
                }

                //If target != null, walk towards the target
                if (targetCoin != null)
                {
                    MoveTo(targetCoin.transform.position, moveSpeed);
                }

                break;

            case EnemyState.ConsideringPatrol:
                randomDirection = Random.insideUnitSphere.normalized;
                randomDirection.y = 0;
                patrollingRadius = Random.Range(0.0f, patrollingMaxRadius);
               
                patrolTargetPosition = targetCoin.transform.position + randomDirection * patrollingRadius;
                patrolTargetPosition.y = 0;

                enemyState = EnemyState.PatrollingAroundCoin;
                break;

            
            case EnemyState.PatrollingAroundCoin:
                RandomPatrolling();
                break;


            case EnemyState.ChasingPlayer:

                animator.SetTrigger("Move");
                //If too far, go back to protect coin
                if ((targetCoin.transform.position - transform.position).magnitude >= patrollingMaxRadius)
                {

                    enemyState = EnemyState.ConsideringTarget;
                }

                if ((hero.transform.position - transform.position).magnitude <= attackRange)
                {
                    enemyState = EnemyState.AttackingPlayer;
                    animator.SetTrigger("Attack");
                }

                MoveTo(hero.transform.position, patrollingSpeed);
                break;

            case EnemyState.AttackingPlayer:

                if ((hero.transform.position - transform.position).magnitude > attackRange)
                {
                    enemyState = EnemyState.ChasingPlayer;
                    animator.SetTrigger("Move");
                }

                break;

            default:

                break;
        }
    }


    private GameObject GetHighestScoreCoin()
    {

        foreach (GameObject coin in coinGenerator.coinList)
        {
            //Normalize the score
            float normalizedDistance = Mathf.Clamp01(1 - ((coin.transform.position - transform.position).magnitude / 100));

            CoinScore newCoinScore = new CoinScore();
            newCoinScore.coinObject = coin;
            newCoinScore.score = normalizedDistance;
            coinScoreList.Add(newCoinScore);
        }

        CoinScore highestCoinScore = coinScoreList[0];
        foreach (CoinScore cs in coinScoreList)
        {
            if (cs.score > highestCoinScore.score)
            {
                highestCoinScore = cs;
            }
        }

        coinScoreList.Clear();
        return highestCoinScore.coinObject;
    }


    private void RandomPatrolling()
    {

        //Detect player
        if ((hero.transform.position - transform.position).magnitude <= detectRange)
        {
            enemyState = EnemyState.ChasingPlayer;
        }

        //Get patrolling point
        if ((patrolTargetPosition - transform.position).magnitude <= 0.1f)
        {
            enemyState = EnemyState.ConsideringPatrol;
            return;
        }


        MoveTo(patrolTargetPosition, patrollingSpeed);

    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        healthBar.UpdateHealthBar(currentHealth, maxHealth);
        enemyState = EnemyState.ChasingPlayer;
    }


    private void MoveTo(Vector3 tgPos, float speed)
    {
        rb.velocity = (tgPos - transform.position).normalized * speed;

        Quaternion rotation = Quaternion.LookRotation((tgPos - transform.position).normalized);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
    }


}
