using UnityEngine;

// 어떤 능력치를 올려주는지 구분하기 위한 열거형(Enum)
public enum ItemType
{
    MoveSpeed,     // 이동 속도 (주사기)
    AttackArea,    // 공격 범위 (깨진 유리 조각)
    DamageReduce,  // 피해량 감소 (알약)
    AttackBoost,   // 공격력 증가 (물약)
    MaxHP,         // 최대 체력 (붕대)
    ProjectileSpeed // 투사체 속도 (바늘)
}

// 유니티 프로젝트 창에서 마우스 우클릭으로 아이템 파일을 만들 수 있게 해줍니다.
[CreateAssetMenu(fileName = "NewItemData", menuName = "Scriptable Object/Item Data")]
public class Item_Data_Structure : ScriptableObject
{
    [Header("아이템 기본 정보")]
    public string itemName;       // 아이템 이름 (예: 주사기)
    public ItemType itemType;     // 아이템 속성 선택
    public Sprite itemIcon;       // 아이템 이미지(스프라이트)
    [TextArea]
    public string itemDescription; // 아이템 기본 설명

    [Header("레벨별 능력치 수치 (배열) -> ★최종 누적값★")]
    // 레벨 1, 레벨 2, 레벨 3... 마다 상승할 수치 리스트입니다.
    // 예: 이동속도 수치를 [0.1f, 0.15f, 0.2f] 로 적으면 10%, 15%, 20% 증가가 됩니다.
    [Tooltip("★주의★ 각 레벨당 증가량이 아니라, 해당 레벨 달성 시의 '최종 누적 보너스 수치'를 적습니다.\n" +
             "예(이동속도): 1렙에 10% 증가, 2렙에 총 15% 증가, 3렙에 총 20% 증가인 경우 -> [0.1, 0.15, 0.2]로 입력함.\n" +
             "플레이어 스탯 반영 로직 짤 때 '이전 레벨 값을 빼고 더해주거나' 또는 '기본 스탯에 이 보너스 비율을 바로 덮어쓰는(곱하는)' 방식으로 연동해 주세요!")]
    public float[] levelValues; 
}