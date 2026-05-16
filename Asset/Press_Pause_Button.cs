using UnityEngine;
using UnityEngine.UI; // Image 컴포넌트 제어용
using UnityEngine.InputSystem; // 신규 입력 시스템

public class Press_Pause_Button : MonoBehaviour
{
    [Header("Button Image Settings")]
    [SerializeField] private Image targetButtonImage; // 교체될 Image 컴포넌트
    [SerializeField] private Sprite playSprite;       // 재생 모양 이미지 (▶)
    [SerializeField] private Sprite pauseSprite;      // 일시정지 모양 이미지 (||)

    [Header("Pause Overlay")]
    [SerializeField] private GameObject pauseOverlayPanel;

    private bool isPaused = false;

    void Start()
    {
        // 처음에 버튼 이미지를 일시정지(||) 상태로 초기화
        if (targetButtonImage != null && pauseSprite != null)
        {
            targetButtonImage.sprite = pauseSprite;
        }
        // 시작할 때는 어두운 화면을 꺼둡니다.
        if (pauseOverlayPanel != null) pauseOverlayPanel.SetActive(false);
    }

    // 버튼을 눌렀을 때 실행될 함수
    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f; // 시간 정지
            if (targetButtonImage != null) targetButtonImage.sprite = playSprite;

            // [추가] 일시정지 시 화면 어둡게 하기
            if (pauseOverlayPanel != null) pauseOverlayPanel.SetActive(true);
            
            Debug.Log("시스템 일시정지: 재생 아이콘으로 변경");
        }
        else
        {
            Time.timeScale = 1f; // 시간 재개
            if (targetButtonImage != null) targetButtonImage.sprite = pauseSprite;

            // [추가] 재개 시 어두운 화면 치우기
            if (pauseOverlayPanel != null) pauseOverlayPanel.SetActive(false);

            Debug.Log("시스템 재개: 일시정지 아이콘으로 변경");
        }
    }
}