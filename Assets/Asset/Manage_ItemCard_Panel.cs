using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Manage_Item_Card_Panel : MonoBehaviour
{
    [Header("바인딩할 UI 컴포넌트들")]
    public Image itemIcon;           // 아이템 이미지 오브젝트
    public TextMeshProUGUI itemName; // 아이템 이름 텍스트
    public TextMeshProUGUI itemDesc; // 아이템 설명 텍스트
    public TextMeshProUGUI statChangeText; // ★ 새로 추가된 레벨별 능력치 변화 텍스트 ★

    // ★ 부모 찾기 대신 인스펙터에서 직접 꽂아줄 사령탑 매니저 변수 추가!
    [Header("사령탑 매니저")]
    public Manage_Item_Selection selectionManager;

    private Manage_Item targetItem; // 현재 이 카드가 보여주고 있는 아이템 데이터

    // 선택창이 뜰 때 랜덤으로 뽑힌 아이템 정보를 카드에 세팅해주는 함수
    public void SetupCard(Manage_Item item)
    {
        targetItem = item;

        // ScriptableObject(ItemData)에서 기본 정보 로드
        itemIcon.sprite = item.data.itemIcon;
        itemName.text = item.data.itemName;
        itemDesc.text = item.data.itemDescription;

        // ★ [핵심] Item 스크립트에서 실시간 레벨업 변화 문자열을 가져와서 UI에 적용!
        statChangeText.text = item.GetStatChangeText();
    }

    // 유저가 이 아이템 카드(버튼)를 클릭했을 때 실행될 함수
    public void OnClickCard()
    {
        if (targetItem != null)
        {
            targetItem.LevelUp(); // 아이템 레벨업 실행

            Debug.Log($"{targetItem.data.itemName}을(를) 선택하여 레벨업했습니다.");

            // ★ 인스펙터에 연결된 매니저에게 바로 신호를 보냅니다!
            if (selectionManager != null)
            {
                selectionManager.OnSelectItem();
            }
            else
            {
                Debug.LogError($"{gameObject.name} 카드에 Selection Manager가 연결되지 않았습니다!");
            }
        }
    }
}