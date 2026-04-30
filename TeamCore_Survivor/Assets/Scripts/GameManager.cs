using UnityEngine;

public class GameManager : MonoBehaviour
{
    // [핵심] 싱글톤 패턴: 게임 내에 매니저는 딱 1개만 존재하며, 어디서든 쉽게 접근 가능하게 만듭니다.
    public static GameManager Instance;

    [Header("게임 데이터")]
    public float surviveTime; // 생존 시간
    public int killCount;     // 처치한 몬스터 수
    public int currentLevel = 1; // 현재 난이도(웨이브) 레벨

    private void Awake()
    {
        // 씬에 GameManager가 여러 개 생기는 것을 방지하는 세팅
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        // 1. 매 프레임마다 생존 시간 증가 (Time.deltaTime은 1프레임당 걸린 시간)
        surviveTime += Time.deltaTime;

        // 2. [난이도 조절 예시] 60초(1분)가 지날 때마다 레벨업!
        if (surviveTime > currentLevel * 60f)
        {
            LevelUpWave();
        }
    }

    // 몬스터가 죽을 때마다 다른 스크립트에서 이 함수를 부를 겁니다!
    public void AddKill()
    {
        killCount++;
        Debug.Log($"몬스터 처치! 현재 킬수: {killCount} / 생존 시간: {Mathf.FloorToInt(surviveTime)}초");
    }

    // 난이도 상승 함수 (승환 님이 튜닝하실 핵심 구간)
    private void LevelUpWave()
    {
        currentLevel++;
        Debug.Log($"🚨 난이도 상승! 현재 웨이브 레벨: {currentLevel} 🚨");
        
        // TODO: 나중에 여기에 "몬스터 스폰 속도 2배 증가", "몬스터 체력 증가" 등의 코드를 추가하면 됩니다.
    }
}