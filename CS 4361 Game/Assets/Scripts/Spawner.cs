using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Enemy enemy;
    Player player;
    MapGenerator.Map map;

    int numOfEnemies, maxOnMap = 25;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        map = FindObjectOfType<MapGenerator>().currentMap;
        StartCoroutine(Spawn());
    }

    /// <summary>
    /// Spawns the enemies at a random position on the map
    /// </summary>
    /// <returns></returns>
    IEnumerator Spawn()
    {
        while (!player.isDead)
        {
            if (numOfEnemies < maxOnMap)
            {
                numOfEnemies++;
                Vector3 spawn = new Vector3(Random.Range(-map.sizeOfMap.x, map.sizeOfMap.x), 0.25f, Random.Range(-map.sizeOfMap.y, map.sizeOfMap.y));
                Debug.Log("Adding enemy" + spawn);
                Instantiate(enemy, spawn, Quaternion.identity).OnDeath += OnEnemyDeath;
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    /// <summary>
    /// Decrements the amount of enemies on the fields and increments the player's kill count
    /// </summary>
    void OnEnemyDeath()
    {
        numOfEnemies--;
        player.enemyCount++;
    }
}
