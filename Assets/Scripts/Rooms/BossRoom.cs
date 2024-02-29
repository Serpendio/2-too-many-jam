using Creature;
using Rooms;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class BossRoom : MonoBehaviour
{

    [SerializeField] private GameObject[] bossPrefabs;
    [SerializeField] private GameObject[] bossRoomEnemyPrefabs;

    private Room room;
    List<Vector3> spawnablePositions;

    private int maxEnemies;
    private float timeBetweenEnemySpawns;

    private bool bossRoomComplete;


    //Stage 1: Mouse
    //Stage 2: Imp
    //Stage 3: Bird

    private void Start()
    {

        room = GetComponent<Room>();
        spawnablePositions = room.GenerateSpawnablePositions();

        maxEnemies = 20;
        timeBetweenEnemySpawns = 5f;
        bossRoomComplete = false;

        //Spawn boss
        EnemyBase bossEnemy = Instantiate(bossPrefabs[Core.Locator.StageManager.getStage() - 1], room.EnemiesContainer).GetComponent<EnemyBase>();
        bossEnemy.transform.position = room.Tilemap.cellBounds.center; //Center of room
        bossEnemy.target = Core.Locator.Player;
        room.RegisterEnemy(bossEnemy);

        bossEnemy.OnDeath.AddListener(() =>
        {
            Debug.Log("YIppe");
            bossRoomComplete = true;
            if (Core.Locator.StageManager.getStage() < Core.Locator.StageManager.getMaxStage()) {
                Debug.Log("Wagoo");
                Core.Locator.StageManager.NextStage();
            }
            else {
                //Win screen or smth
            }
        });

        StartCoroutine("RecurrentSpawn");
    }


    IEnumerator RecurrentSpawn()
    {
        while (!bossRoomComplete) {
            //Ensure value of enemiesToSpawn will never result in enemy count going over maxEnemies
            int enemiesToSpawn = room.EnemiesContainer.childCount + 6 < maxEnemies ? Random.Range(2, 6) : maxEnemies - room.EnemiesContainer.childCount;

            for (int i = 0; i < enemiesToSpawn; i++) {
                EnemyBase enemy = Instantiate(bossRoomEnemyPrefabs[Core.Locator.StageManager.getStage() - 1], room.EnemiesContainer).GetComponent<EnemyBase>();
                Vector3 randomPos = spawnablePositions[Random.Range(0, spawnablePositions.Count)];
                enemy.transform.position = randomPos;
                enemy.target = Core.Locator.Player;

                room.RegisterEnemy(enemy);
            }

            yield return new WaitForSeconds(timeBetweenEnemySpawns);
        }
        yield break;
    }
}