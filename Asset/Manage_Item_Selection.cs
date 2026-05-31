using UnityEngine;
using System.Collections.Generic; // [추가] 랜덤 중복 방지 처리를 위한 List 사용

public class Manage_Item_Selection : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject itemSelectionPanel; // 아이템 창 부모
    [SerializeField] private GameObject darker;       // 배경 어둡게 패널

    // [추가] 매니저 오브젝트가 가지고 있을 아이템 리스트와 화면의 카드 컴포넌트들
    [Header("Item Pool & Cards")]
    [SerializeField] private Manage_Item[] allGameItems;
    [SerializeField] private Manage_Item_Card_Panel[] itemCards;

    void Start()
    {
        // 시작할 때는 다 꺼두기
        CloseUI();
    }

    // 경험치 스크립트에서 호출할 함수
    public void ShowItemSelection()
    {
        // 시간 멈춤
        Time.timeScale = 0f;

        // UI 켜기
        if (darker != null) darker.SetActive(true);
        if (itemSelectionPanel != null) itemSelectionPanel.SetActive(true);

        Debug.Log("아이템 매니저: 창을 띄우고 시간을 멈췄습니다.");

        // 1. 만렙이 아닌 아이템들만 추출하여 후보군 생성
        List<Manage_Item> availableItems = new List<Manage_Item>();
        foreach (Manage_Item item in allGameItems)
        {
            if (item != null && item.level < item.data.levelValues.Length)
            {
                availableItems.Add(item);
            }
        }

        // 2. 카드 개수만큼 랜덤하게 배치 (아이템 후보가 카드보다 적을 상황 방지)
        int cardsToFill = Mathf.Min(itemCards.Length, availableItems.Count);

        // 모든 카드를 일단 끄기
        foreach (var card in itemCards)
        {
            card.gameObject.SetActive(false);
        }

        // 3. 중복 없이 랜덤 매칭 후 카드 켜기
        for (int i = 0; i < cardsToFill; i++)
        {
            int randomIndex = Random.Range(0, availableItems.Count);
            Manage_Item selectedItem = availableItems[randomIndex];

            itemCards[i].SetupCard(selectedItem);
            itemCards[i].gameObject.SetActive(true);

            // 중복 선택을 막기 위해 후보 목록에서 제거
            availableItems.RemoveAt(randomIndex);
        }
    }

    // 아이템 버튼을 눌렀을 때 호출할 함수
    public void OnSelectItem()
    {
        CloseUI();

        // 시간 다시 흐르게
        Time.timeScale = 1f;

        Debug.Log("아이템 매니저: 선택 완료! 시간을 다시 재생합니다.");
    }

    private void CloseUI()
    {
        if (darker != null) darker.SetActive(false);
        if (itemSelectionPanel != null) itemSelectionPanel.SetActive(false);
    }
}
