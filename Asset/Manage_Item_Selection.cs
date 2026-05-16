using UnityEngine;

public class Manage_Item_Selection : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject itemSelectionPanel; // 아이템 창 부모
    [SerializeField] private GameObject darker;       // 배경 어둡게 패널

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

        // 여기에 나중에 "랜덤 아이템 3개 배치" 로직을 넣으시면 됩니다!
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
