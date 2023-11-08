using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGenerator : MonoBehaviour
{
    public GameObject enemyPrefab;

    //Max coin number
    public int maxEnemyNum;

    //Generate area
    public Vector3 worldSize;
    private float xRadius, zRadius;

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(gameObject.transform.position, worldSize);
    }


    // Start is called before the first frame update
    void Start()
    {
        xRadius = worldSize.x / 2.0f;
        zRadius = worldSize.z / 2.0f;

        for (int i = 0; i < maxEnemyNum; i++)
        {
            GenerateEnemy();
        }

    }

    // Update is called once per frame
    void Update()
    {

    }


    public void GenerateEnemy()
    {

        float randomX = Random.Range(transform.position.x - xRadius, transform.position.x + xRadius);
        float randomZ = Random.Range(transform.position.z - xRadius, transform.position.z + zRadius);

        Vector3 spawnLocation = new Vector3(randomX, 0, randomZ);

        GameObject enemy = Instantiate(enemyPrefab, spawnLocation, Quaternion.identity);

        enemy.GetComponentInChildren<Enemy>().enemyGenerator = this;
    }

}
