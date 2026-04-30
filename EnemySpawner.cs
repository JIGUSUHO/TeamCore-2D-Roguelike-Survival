using System.Collections.Generic;
using UnityEngine;

// 인스펙터에서 몬스터 종류별 프리팹과 풀 크기를 설정할 수 있게 해주는 클래스
[System.Serializable]
public class PoolInfo
{
    public string enemyName;      // 적 이름 (예: "Bat", "Zombie")
    public GameObject prefab;     // 적 프리팹
    public int poolSize;          // 미리 생성해 둘 개수
}

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance;

    [Header("Spawn Settings")]
    public Transform player;
    public float spawnRadius = 10f;

    [Header("Wave Settings")]
    public List<WaveData> waves; // 에디터에서 만든 WaveData 파일들을 넣을 리스트
    private int currentWaveIndex = 0; // 현재 진행 중인 웨이브 번호
    private float waveTimer = 0f;     // 현재 웨이브가 얼마나 진행되었는지 측정하는 타이머
    // 몬스터 종류별로 각각의 스폰 주기를 계산하기 위한 타이머 딕셔너리
    private Dictionary<string, float> spawnTimers = new Dictionary<string, float>();

    [Header("Pool Settings")]
    public List<PoolInfo> poolSettings; // 에디터에서 여러 몬스터를 등록할 리스트

    // 문자열(이름)을 키값으로 해서 해당 몬스터의 Queue를 빠르게 찾는 딕셔너리
    private Dictionary<string, Queue<GameObject>> poolDictionary;

    private float timer = 0f;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        InitializePools();
    }

    // 1. 등록된 모든 몬스터 종류의 풀을 초기화
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

            // 딕셔너리에 "이름"을 키로 Queue를 저장
            poolDictionary.Add(info.enemyName, objectQueue);
        }
    }

    void Update()
    {
        // 모든 웨이브가 끝났다면 작동 정지
        if (currentWaveIndex >= waves.Count) return;

        WaveData currentWave = waves[currentWaveIndex];
        waveTimer += Time.deltaTime;

        // 현재 웨이브에 등록된 모든 몬스터 종류를 순회하며 스폰 처리
        foreach (EnemySpawnInfo info in currentWave.spawnInfos)
        {
            // 딕셔너리에 이 몬스터의 타이머가 없다면 0으로 초기화
            if (!spawnTimers.ContainsKey(info.enemyName))
            {
                spawnTimers[info.enemyName] = 0f;
            }

            spawnTimers[info.enemyName] += Time.deltaTime;

            // spawnRate가 2면 1/2 = 0.5초마다 1마리 스폰
            float spawnInterval = 1f / info.spawnRate;

            // 타이머가 스폰 주기를 넘어서면 스폰 실행
            if (spawnTimers[info.enemyName] >= spawnInterval)
            {
                SpawnEnemy(info.enemyName); // (기존 스폰 함수 호출)
                spawnTimers[info.enemyName] -= spawnInterval; // 타이머에서 주기만큼 빼서 오차 방지
            }
        }

        // 현재 웨이브의 지속 시간이 끝나면 다음 웨이브로 넘어감
        if (waveTimer >= currentWave.waveDuration)
        {
            NextWave();
        }
    }

    private void NextWave()
    {
        currentWaveIndex++;
        waveTimer = 0f;
        spawnTimers.Clear(); // 다음 웨이브를 위해 타이머 초기화
    }

    // 2. 원하는 종류의 몬스터를 딕셔너리에서 찾아 스폰
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
            // 풀이 비었다면 해당 프리팹을 찾아서 새로 생성
            PoolInfo info = poolSettings.Find(x => x.enemyName == enemyName);
            enemyToSpawn = Instantiate(info.prefab, transform);
        }

        enemyToSpawn.transform.position = spawnPosition;
        enemyToSpawn.SetActive(true);
    }

    // 3. 적이 죽었을 때 다시 맞는 큐로 반환
    public void ReturnEnemy(string enemyName, GameObject enemy)
    {
        enemy.SetActive(false);
        poolDictionary[enemyName].Enqueue(enemy);
    }
}