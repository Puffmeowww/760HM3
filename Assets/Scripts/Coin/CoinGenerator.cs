using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinGenerator : MonoBehaviour
{

    public GameObject coinPrefab;

    public int maxCoinNum;

    public Vector3 gridWorldSize;

    private float xRadius, zRadius;

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(gameObject.transform.position, gridWorldSize);
    }
    // Start is called before the first frame update
    void Start()
    {
        xRadius = gridWorldSize.x / 2.0f;
        zRadius = gridWorldSize.z / 2.0f;

        for(int i = 0; i< maxCoinNum; i++)
        {
            generateCoin();
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void generateCoin()
    {

        float randomX = Random.Range(transform.position.x - xRadius, transform.position.x + xRadius);
        float randomZ = Random.Range(transform.position.z - xRadius, transform.position.z + zRadius);

        Vector3 spawnLocation = new Vector3(randomX,0,randomZ);

        Instantiate(coinPrefab, spawnLocation, Quaternion.identity);
    }
}
