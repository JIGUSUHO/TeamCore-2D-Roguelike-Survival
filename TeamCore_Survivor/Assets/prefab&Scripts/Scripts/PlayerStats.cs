using System;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("플레이어 상태 (HP)")]
    public float maxHp = 100f;
    public float currentHp;

    [Header("플레이어 상태 (레벨 및 경험치)")]
    public int level = 1;
    public float currentExp = 0f;
    public float maxExp = 100f; // 레벨업에 필요한 경험치 통

    // UI나 다른 스크립트에서 변화를 감지할 수 있도록 돕는 이벤트들입니다.
    public event Action<float, float> OnHpChanged; // 현재 체력, 최대 체력 전달
    public event Action<float, float> OnExpChanged; // 현재 경험치, 최대 경험치 전달
    public event Action<int> OnLevelUp;             // 새로운 레벨 전달
    public event Action OnDie;                      // 사망 이벤트

    private void Awake()
    {
        // 게임 시작 시 체력을 꽉 채워줍니다.
        currentHp = maxHp;
    }

    private void Start()
    {
        // 시작 시 초기 UI 업데이트를 위해 이벤트를 한 번 호출해 줍니다.
        OnHpChanged?.Invoke(currentHp, maxHp);
        OnExpChanged?.Invoke(currentExp, maxExp);
    }

    // 1. 데미지를 입을 때 호출되는 함수
    public void TakeDamage(float damage)
    {
        if (currentHp <= 0) return; // 이미 죽었다면 무시

        currentHp -= damage;
        currentHp = Mathf.Clamp(currentHp, 0, maxHp); // 체력이 0 미만으로 내려가지 않게 고정

        OnHpChanged?.Invoke(currentHp, maxHp); // UI 업데이트 알림

        if (currentHp <= 0)
        {
            Die();
        }
    }

    // 2. 경험치를 얻을 때 호출되는 함수
    public void AddExp(float amount)
    {
        currentExp += amount;

        // 경험치가 꽉 찼다면 레벨업 진행
        while (currentExp >= maxExp)
        {
            currentExp -= maxExp;
            LevelUp();
        }

        OnExpChanged?.Invoke(currentExp, maxExp); // UI 업데이트 알림
    }

    // 3. 레벨업 처리 함수
    private void LevelUp()
    {
        level++;
        maxExp = 100f + (level * 50f); // 다음 레벨업 요구치를 레벨*50 만큼 Linear Scaling (차후 밸런스 수정)
        maxHp += 20f;   // 레벨업 시 최대 체력 증가
        currentHp = maxHp; // 체력 회복

        OnLevelUp?.Invoke(level);
        OnHpChanged?.Invoke(currentHp, maxHp); // 최대 체력이 올랐으니 UI 다시 갱신
    }

    // 4. 사망 처리 함수
    private void Die()
    {
        Debug.Log("플레이어 사망!");
        OnDie?.Invoke();
        // 여기에 게임 오버 처리나 애니메이션 연결 로직을 추가할 수 있습니다.
    }
}
