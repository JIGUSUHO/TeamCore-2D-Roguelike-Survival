using UnityEngine;

public class Manage_Item : MonoBehaviour
{
    public Item_Data_Structure data; // 2단계에서 만든 아이템 에셋이 여기 등록됩니다.
    public int level = 0; // 현재 이 아이템의 레벨 (0이면 아직 안 배운 상태)

    // 이 아이템을 처음 획득하거나 레벨업할 때 호출되는 함수
    public void LevelUp()
    {
        // 만렙 체크 (레벨 설정 칸보다 더 많이 업할 수 없게 막음)
        if (level >= data.levelValues.Length) return;

        level++;

        // 현재 레벨에 맞는 효과 수치(예: 0.1f)를 배열에서 가져옴
        float currentEffectValue = data.levelValues[level - 1];

        // 디버그 로그로 잘 작동하는지 찍어볼 수 있습니다.
        Debug.Log($"{data.itemName} 레벨업! 현재 레벨: {level}, 효과 수치: {currentEffectValue}");

        ApplyItemEffect(currentEffectValue);
    }

    // 속성에 맞게 플레이어 능력치에 적용하는 뼈대 함수
    private void ApplyItemEffect(float value)
    {
        switch (data.itemType)
        {
            // 여기에 플레이어 스탯 연동 로직이 들어갈 예정입니다!
            // Player 스크립트 이름이 만약 'PlayerStatus'라면
            // 이런 식으로 플레이어 컴포넌트를 가져와서 수치를 넘겨주게 됩니다.
            // PlayerStatus player = GetComponentInParent<PlayerStatus>();
            case ItemType.MoveSpeed:
                // 합칠 곳: player.moveSpeed = player.baseSpeed * (1 + value);
                break;
            case ItemType.AttackArea:
                // 합칠 곳: 무기 범위 스탯 변경 로직
                break;
            case ItemType.DamageReduce:
                // 합칠 곳: 피해량 감소 스탯 변경 로직
                break;
            case ItemType.AttackBoost:
                // 합칠 곳: 모든 무기 대미지 버프 로직
                break;
            case ItemType.MaxHP:
                // 합칠 곳 1: player.maxHP = value; (최대 체력 누적 최종값 반영)
                // 합칠 곳 2: float healAmount = player.maxHP * 0.5f;
                //              player.currentHP = Mathf.Min(player.currentHP + healAmount, player.maxHP); (오버힐 방지) (회복 보너스)
                break;
            case ItemType.ProjectileSpeed:
                // 합칠 곳: 투사체 날아가는 속도 스탯 변경 로직
                break;

            case ItemType.ActiveWeapon:
                if (WeaponManager.Instance != null)
                {
                    // WeaponManager의 LevelUpWeapon 함수에 아이템의 '이름'을 그대로 전달합니다!
                    WeaponManager.Instance.LevelUpWeapon(data.itemName);
                }
                break;
        }
    }

    // UI 패널에서 "레벨별 수치 변화"를 띄워주기 위해 호출할 함수
    public string GetStatChangeText()
    {
        // 만렙 체크 (더 이상 강화 불가인 경우)
        if (level >= data.levelValues.Length) return "MAX LEVEL";

        float nextValue = data.levelValues[level]; // 다음 레벨이 가질 최종 누적 수치

        // 퍼센트(%) 단위를 쓸 아이템과 일반 숫자 단위를 쓸 아이템 구분
        string unit = "%";
        float scale = 100f; // 퍼센트는 0.1을 10으로 보여주기 위함

        if (level == 0)
        {
            // 아직 한 번도 안 배운 새 아이템인 경우
            return $"[NEW] Lv.1: +{nextValue * scale}{unit}";
        }
        else
        {
            // 이미 가지고 있는 아이템을 레벨업하는 경우 (수치 변화 추적)
            float currentValue = data.levelValues[level - 1];
            return $"Lv.{level} > Lv.{level + 1}\n+{currentValue * scale}{unit} > +{nextValue * scale}{unit}";
        }
    }
}