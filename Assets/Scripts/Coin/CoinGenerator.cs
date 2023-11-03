using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinGenerator : MonoBehaviour
{

    public GameObject coinPrefab;

    //Max coin number
    public int maxCoinNum;

    //Generate area
    public Vector3 worldSize;
    private float xRadius, zRadius;

    //A list to store all coins
    public List<GameObject> coinList = new List<GameObject>();

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(gameObject.transform.position, worldSize);
    }


    // Start is called before the first frame update
    void Start()
    {
        xRadius = worldSize.x / 2.0f;
        zRadius = worldSize.z / 2.0f;

        for(int i = 0; i< maxCoinNum; i++)
        {
            GenerateCoin();
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void GenerateCoin()
    {

        float randomX = Random.Range(transform.position.x - xRadius, transform.position.x + xRadius);
        float randomZ = Random.Range(transform.position.z - xRadius, transform.position.z + zRadius);

        Vector3 spawnLocation = new Vector3(randomX,0,randomZ);

        GameObject coin = Instantiate(coinPrefab, spawnLocation, Quaternion.identity);
        coinList.Add(coin);
    }

    public void GetCoin(GameObject coin)
    {
        coinList.Remove(coin);
        Destroy(coin);
        GenerateCoin();
    }
}
