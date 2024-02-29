using Core;
using Creature;
using Currency;
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
    [SerializeField] private CoinDrop _coinDropPrefab;
    [SerializeField] private ShardDrop _shardDropPrefab;

    private Room room;
    List<Vector3> spawnablePositions;

    private int maxEnemies;
    private float timeBetweenEnemySpawns;

    private bool bossRoomComplete;


    //Stage 1: Mouse
    //Stage 2: Imp
    //Stage 3: Bird

    private void Awake()
    {
        if (_coinDropPrefab == null) _coinDropPrefab = Resources.Load<CoinDrop>("Prefabs/CoinDrop");
        if (_shardDropPrefab == null) _shardDropPrefab = Resources.Load<ShardDrop>("Prefabs/Shard");
    }

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
            bossRoomComplete = true;
            if (Core.Locator.StageManager.getStage() < Core.Locator.StageManager.getMaxStage()) {
                Core.Locator.StageManager.NextStage();
                DropMoneyAndShards();
            }
            else {
                //Win screen or smth
            }
        });

        StartCoroutine(nameof(RecurrentSpawn));
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

    void DropMoneyAndShards()
    {
        List<CoinDrop> _coinDrops = new List<CoinDrop>();
        List<ShardDrop> _shardDrops = new List<ShardDrop>();
        var totalValue = Locator.GameplaySettingsManager.ChestDropValue.GetValue() * 4 * Core.Locator.StageManager.getStage(); //Multiply by 4 and by stage for boss

        var droppedValue = 0f;
        while (droppedValue < totalValue)
        {
            var offset = transform.up * Random.Range(-0.25f, -0.75f) + transform.right * Random.Range(-0.5f, 0.5f);
            Debug.DrawRay(transform.position, offset, Color.red, 60f);

            var coinDrop = Instantiate(_coinDropPrefab, transform.position + offset, Quaternion.identity);
            coinDrop.transform.parent = transform;
            coinDrop.coinValue = Mathf.RoundToInt(Locator.GameplaySettingsManager.CoinDropValue.GetValue());
            _coinDrops.Add(coinDrop);
            droppedValue += coinDrop.coinValue;

            coinDrop.OnPickup.AddListener(() =>
            {
                _coinDrops.Remove(coinDrop);
            });

            if (Random.value > 0.8f)
            {
                var shardDrop = Instantiate(_shardDropPrefab, transform.position + offset, Quaternion.identity);
                shardDrop.transform.parent = transform;
                shardDrop.shardValue = Random.Range(1, 4);
                _shardDrops.Add(shardDrop);
                droppedValue += shardDrop.shardValue;
                shardDrop.OnPickupShard.AddListener(() =>
                {
                    _shardDrops.Remove(shardDrop);
                });
            }
        }
    }
}