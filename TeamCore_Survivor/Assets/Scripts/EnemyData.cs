using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "ScriptableObjects/EnemyData", order = 1)]
public class EnemyData : ScriptableObject
{
    [Header("기본 스탯")]
    public string enemyName;      // 몬스터 이름 
    public float maxHealth;       // 최대 체력
    public float moveSpeed;       // 이동 속도
    public float damage;          // 플레이어에게 부딪혔을 때 주는 데미지

    [Header("보상")]
    public int dropExp;           // 죽었을 때 떨어뜨리는 경험치 보석 량
}