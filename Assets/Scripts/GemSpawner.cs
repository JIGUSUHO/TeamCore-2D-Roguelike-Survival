using System.Collections.Generic;
using UnityEngine;

public class GemSpawner : MonoBehaviour
{
    // 어디서든 GemSpawner.Instance 로 접근할 수 있게 싱글톤 처리
    public static GemSpawner Instance;

    [Header("Pool Settings")]
    public GameObject gemPrefab; // 인스펙터에서 경험치 보석 프리팹 연결

    // 보석들을 담아둘 대기열(Queue)
    private Queue<GameObject> gemPool = new Queue<GameObject>();

    void Awake()
    {
        // 싱글톤 초기화
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 몬스터가 죽을 때 이 함수를 호출하여 보석을 꺼냄
    public void SpawnGem(Vector2 position, int expAmount)
    {
        GameObject gem;

        // 1. 풀에 남은 보석이 있다면 꺼내 씀
        if (gemPool.Count > 0)
        {
            gem = gemPool.Dequeue();
            gem.SetActive(true);
        }
        // 2. 풀이 비어있다면 새로 하나 만듦
        else
        {
            gem = Instantiate(gemPrefab, transform); // Spawner의 자식으로 깔끔하게 정리
        }

        // 보석의 위치와 경험치량 설정
        gem.transform.position = position;

        Gem gemScript = gem.GetComponent<Gem>();
        if (gemScript != null)
        {
            gemScript.SetExp(expAmount);
        }
    }

    // 플레이어가 보석을 먹었을 때 이 함수를 호출하여 풀로 반납함
    public void ReturnGem(GameObject gem)
    {
        gem.SetActive(false); // 화면에서 숨김
        gemPool.Enqueue(gem); // 대기열에 다시 넣음
    }
}