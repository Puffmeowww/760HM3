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
        heroState = HeroState.ConsideringTarget;
     
    }

    // Update is called once per frame
    void Update()
    {

        if((targetCoin != null && (targetCoin.transform.position - transform.position).magnitude <= 0.5f))
        {
            Debug.Log("get coin");
            coinGenerator.GetCoin(targetCoin);
            heroState = HeroState.ConsideringTarget;
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
                    Vector3 movement = targetCoin.transform.position - transform.position;
                    rb.velocity = movement * moveSpeed;
                }

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






}
