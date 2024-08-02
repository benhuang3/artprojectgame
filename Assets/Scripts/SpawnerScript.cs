using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;  
using UnityEngine.UI;              
using UnityEngine.Audio;            
using UnityEngine.EventSystems;    
using UnityEngine.Video;            

public class SpawnerScript : MonoBehaviour
{

    
    public float spawnInterval = 3f;
    private float nextSpawnTime;

    public GameObject enemyPrefab;

    public GameObject playerPrefab;
    private GameObject playerInstance;

    void Start()
    {

        Vector2 spawnPosition = new Vector2(0, 0);
        Vector2 healthSpawnPosition = new Vector3(1.6f, 0, 0);


        // playerInstance = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);

        Debug.Log( playerInstance);

        // Initialize the time for the next spawn
        nextSpawnTime = Time.time + spawnInterval;
    }

    void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            // SpawnEnemy();
            nextSpawnTime = Time.time + spawnInterval;
        }
    }

    void SpawnEnemy()
    {
        // Generate a random position within the defined area
        // Vector2 spawnPosition = new Vector2(
        //     Random.Range(spawnAreaMin.x, spawnAreaMax.x),
        //     Random.Range(spawnAreaMin.y, spawnAreaMax.y)
        // );
        // Generate a random position within the defined area
        Vector2 spawnPosition = new Vector2(0, 0);

        // Instantiate the enemy prefab at the spawn position
        GameObject spawned_enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }

}   
