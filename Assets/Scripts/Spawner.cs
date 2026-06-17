using System.Collections.Generic; // Queue를 사용하기 위해 필요
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public static Spawner Instance; // 싱글톤 패턴 (다른 스크립트에서 쉽게 접근 가능)

    public GameObject enemyPrefab;      //  적 프리팹
    public Transform player;            // 플레이어의 위치
    public float spawnInterval = 1.0f;  // 적이 생성되는 주기 (초 단위)
    public float spawnRadius = 10f;     // 플레이어로부터 떨어져서 생성될 반경 (카메라 밖으로 설정)

    public int poolSize = 50; // 처음에 미리 만들어둘 적의 수

    // 비활성화된 적 게임 오브젝트를 담아둘 큐(Queue)
    private Queue<GameObject> enemyPool = new Queue<GameObject>();

    private float timer = 0f;

    void Awake()
    {
        // 싱글톤 세팅
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // 게임 시작 시 큐에 적을 미리 생성해서 넣어둠
        for (int i = 0; i < poolSize; i++)
        {
            GameObject enemy = Instantiate(enemyPrefab, transform);
            enemy.SetActive(false); // 화면에 안 보이게 비활성화
            enemyPool.Enqueue(enemy); // 큐에 삽입
        }
    }

    void Update()
    {
        // 매 프레임마다 시간을 누적함
        timer += Time.deltaTime;

        // 누적된 시간이 설정한 주기(spawnInterval)를 넘으면 스폰 실행
        if (timer >= spawnInterval)
        {
            SpawnEnemy();
            timer = 0f; // 타이머 초기화
        }
    }

    private void SpawnEnemy()
    {
        // 플레이어가 없거나 파괴되었다면 스폰하지 않음
        if (player == null) return;

        // 1. 0도에서 360도 사이의 랜덤한 각도 추출
        float randomAngle = Random.Range(0f, 360f);

        // 2. 추출한 각도(Degree)를 라디안(Radian)으로 변환 후 방향 벡터 계산 (x = cos, y = sin)
        Vector2 spawnDirection = new Vector2(Mathf.Cos(randomAngle * Mathf.Deg2Rad), Mathf.Sin(randomAngle * Mathf.Deg2Rad));

        // 3. 플레이어 위치 + (방향 * 반지름) = 최종 스폰 위치
        Vector2 spawnPosition = (Vector2)player.position + (spawnDirection * spawnRadius);

        GameObject enemy = null;

        // 1. 큐에 남은 적이 있다면 꺼내서 쓴다 (Dequeue)
        if (enemyPool.Count > 0)
        {
            enemy = enemyPool.Dequeue();
        }
        else
        {
            // 2. 만약 큐가 비었다면 (풀 사이즈보다 적이 더 많이 필요해지면) 새로 생성해서 보충한다
            enemy = Instantiate(enemyPrefab, transform);
        }

        // 위치를 스폰 위치로 옮기고 활성화
        enemy.transform.position = spawnPosition;
        enemy.SetActive(true);
    }

    // 적이 죽었을 때 파괴하는 대신 다시 큐로 돌려보내는 함수
    public void ReturnEnemy(GameObject enemy)
    {
        enemy.SetActive(false); // 다시 숨김
        enemyPool.Enqueue(enemy); // 큐에 반환
    }
}