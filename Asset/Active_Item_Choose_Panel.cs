using UnityEngine;
using UnityEngine.InputSystem; // 신규 입력 시스템 필수 네임스페이스

public class Active_Item_Choose : MonoBehaviour
{
    [SerializeField] private GameObject levelUpPanel; // 인스펙터에서 패널 드래그 앤 드롭

    void Update()
    {
        // Keyboard.current.lKey.wasPressedThisFrame 이 구식 GetKeyDown(KeyCode.L)과 같습니다.
        if (Keyboard.current != null && Keyboard.current.lKey.wasPressedThisFrame)
        {
            ToggleLevelUpPanel();
        }
    }

    public void ToggleLevelUpPanel()
    {
        if (levelUpPanel != null)
        {
            // 현재 활성화 상태를 반전 (켜져있으면 끄고, 꺼져있으면 켬)
            bool isActive = !levelUpPanel.activeSelf;
            levelUpPanel.SetActive(isActive);

            // 패널이 켜질 때 게임 일시정지, 꺼질 때 재개
            Time.timeScale = isActive ? 0f : 1f;

            Debug.Log(isActive ? "연구소 시스템 각성: 레벨업 창 활성화" : "각성 완료: 게임 재개");
        }
    }

    // 아이템 선택 버튼 등에 연결할 닫기 함수
    public void ClosePanel()
    {
        levelUpPanel.SetActive(false);
        Time.timeScale = 1f;
    }
}