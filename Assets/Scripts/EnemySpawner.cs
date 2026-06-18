using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PoolInfo
{
    public string enemyName;      
    public GameObject prefab;     
    public int poolSize;          
}

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance;

    [Header("Spawn Settings")]
    public Transform player;
    public float spawnRadius = 10f;

    [Header("Wave Settings")]
    public List<WaveData> waves; 
    private int currentWaveIndex = 0; 
    private float waveTimer = 0f;     
    private Dictionary<string, float> spawnTimers = new Dictionary<string, float>();

    // ★ [추가됨] 보스가 이미 소환되었는지 체크하는 변수
    private bool isBossSpawned = false; 

    [Header("Pool Settings")]
    public List<PoolInfo> poolSettings; 
    private Dictionary<string, Queue<GameObject>> poolDictionary;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        InitializePools();
    }

    private void InitializePools()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (PoolInfo info in poolSettings)
        {
            Queue<GameObject> objectQueue = new Queue<GameObject>();

            for (int i = 0; i < info.poolSize; i++)
            {
                GameObject enemy = Instantiate(info.prefab, transform);
                enemy.SetActive(false);
                objectQueue.Enqueue(enemy);
            }

            poolDictionary.Add(info.enemyName, objectQueue);
        }
    }

    void Update()
    {
        if (currentWaveIndex >= waves.Count) return;

        WaveData currentWave = waves[currentWaveIndex];
        waveTimer += Time.deltaTime;

        foreach (EnemySpawnInfo info in currentWave.spawnInfos)
        {
            // ★ [수정됨] 만약 이번에 소환할 적이 "Boss" 라면?
            if (info.enemyName == "Boss")
            {
                // 보스가 아직 소환 안 됐다면 딱 1마리만 소환하고, 체크(true)합니다.
                if (!isBossSpawned)
                {
                    SpawnEnemy(info.enemyName);
                    isBossSpawned = true; 
                }
                continue; // 보스는 타이머 계산(무한 소환)을 하지 않고 다음 몬스터로 넘어갑니다!
            }

            // 일반 몬스터는 기존처럼 계속 무한 소환합니다.
            if (!spawnTimers.ContainsKey(info.enemyName))
            {
                spawnTimers[info.enemyName] = 0f;
            }

            spawnTimers[info.enemyName] += Time.deltaTime;

            float spawnInterval = 1f / info.spawnRate;

            if (spawnTimers[info.enemyName] >= spawnInterval)
            {
                SpawnEnemy(info.enemyName); 
                spawnTimers[info.enemyName] -= spawnInterval; 
            }
        }

        if (waveTimer >= currentWave.waveDuration)
        {
            NextWave();
        }
    }

    private void NextWave()
    {
        currentWaveIndex++;
        waveTimer = 0f;
        spawnTimers.Clear(); 
        isBossSpawned = false; // ★ [추가됨] 다음 웨이브가 되면 보스 소환 기록을 초기화합니다.
    }

    public void SpawnEnemy(string enemyName)
    {
        if (player == null || !poolDictionary.ContainsKey(enemyName)) return;

        float randomAngle = Random.Range(0f, 360f);
        Vector2 spawnDirection = new Vector2(Mathf.Cos(randomAngle * Mathf.Deg2Rad), Mathf.Sin(randomAngle * Mathf.Deg2Rad));
        Vector2 spawnPosition = (Vector2)player.position + (spawnDirection * spawnRadius);

        GameObject enemyToSpawn = null;
        Queue<GameObject> targetQueue = poolDictionary[enemyName];

        if (targetQueue.Count > 0)
        {
            enemyToSpawn = targetQueue.Dequeue();
        }
        else
        {
            PoolInfo info = poolSettings.Find(x => x.enemyName == enemyName);
            enemyToSpawn = Instantiate(info.prefab, transform);
        }

        enemyToSpawn.transform.position = spawnPosition;
        enemyToSpawn.SetActive(true);
    }

    public void ReturnEnemy(string enemyName, GameObject enemy)
    {
        enemy.SetActive(false);
        poolDictionary[enemyName].Enqueue(enemy);
    }

    public void SpawnEnemyAtPosition(string enemyName, Vector2 spawnPosition)
    {
        if (!poolDictionary.ContainsKey(enemyName)) return;

        GameObject enemyToSpawn = null;
        Queue<GameObject> targetQueue = poolDictionary[enemyName];

        if (targetQueue.Count > 0)
        {
            enemyToSpawn = targetQueue.Dequeue();
        }
        else
        {
            PoolInfo info = poolSettings.Find(x => x.enemyName == enemyName);
            if (info != null) enemyToSpawn = Instantiate(info.prefab, transform);
        }

        if (enemyToSpawn != null)
        {
            enemyToSpawn.transform.position = spawnPosition;
            enemyToSpawn.SetActive(true);
        }
    }
}