using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections; // 코루틴을 쓰기 위해 추가!

public class Manage_Exp_Level : MonoBehaviour
{
    [Header("Leveling System")]
    public int currentLevel = 1;
    public float currentExp = 0f;
    public float targetExp = 100f;
    [SerializeField] private int targetSuccessLevel = 30; // 성공 목표 레벨 추가

    [Header("UI References")]
    [SerializeField] private Slider expSlider;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Manage_Item_Selection itemManager; // 아이템 창 관리자 연결

    // ====== 여기에 게임 성공 UI 직접 추가! ======
    [Header("Game Success UI")]
    [SerializeField] private GameObject gameSuccessPanel; // 게임 성공 창 패널
    [SerializeField] private CanvasGroup successCanvasGroup; // 서서히 뜨게 할 컴포넌트
    [SerializeField] private float fadeDuration = 2f; // 서서히 뜨는 시간 (2초)

    [Header("Settings")]
    [SerializeField] private float lerpSpeed = 5f;

    void Start()
    {
        // ===== [추가] 다시 시작했을 때를 위해 시간을 정상(1)으로 돌려놓기! =====
        Time.timeScale = 1f;

        // 게임 시작 시 초기 UI 세팅
        UpdateLevelText();
        if (expSlider != null) expSlider.value = currentExp / targetExp;

        // 시작할 때 성공 패널은 꺼두기
        if (gameSuccessPanel != null) gameSuccessPanel.SetActive(false);
    }

    void Update()
    {
        // ===== [수정] 게임이 이미 멈춘 상태(성공/실패 후)라면 아래 로직은 건너뛰기 =====
        if (Time.timeScale == 0f) return;

        /* 테스트용 코드
        // [핵심] 매 프레임 경험치를 감시해서 넘치면 레벨업 실행!
        // 이렇게 하면 인스펙터에서 숫자를 직접 고쳐도 즉시 반응합니다.
        if (currentExp >= targetExp)
        {
            LevelUp();
        }
        */

        // 슬라이더 부드럽게 채우기
        if (expSlider != null)
        {
            float targetValue = currentExp / targetExp;
            expSlider.value = Mathf.Lerp(expSlider.value, targetValue, Time.deltaTime * lerpSpeed);
        }
    }

    // 외부(몬스터 처치 등)에서 경험치를 줄 때 호출하는 함수
    public void AddExp(float amount)
    {
        currentExp += amount;
        Debug.Log($"경험치 획득: {amount}, 현재 경험치: {currentExp}/{targetExp}");

        // 경험치가 목표치를 넘는 동안 계속 레벨업 (연속 레벨업 가능)
        while (currentExp >= targetExp)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        currentLevel++;
        currentExp -= targetExp; // 초과분 이월
        targetExp += 50f;        // 다음 레벨 난이도 상승

        // 레벨업 즉시 UI 반영
        UpdateLevelText();

        if (levelText != null) levelText.text = $"LV. {currentLevel}";

        // [핵심] 30레벨 달성 시 성공 처리!
        if (currentLevel >= targetSuccessLevel)
        {
            WinGame();
            return; // 성공했으므로 아래 아이템 선택 창은 띄우지 않고 종료!
        }

        // 30레벨 미만일 때만 아이템 매니저에게 창을 띄우라고 시킴!
        if (itemManager != null)
        {
            itemManager.ShowItemSelection();
        }

        Debug.Log($"★레벨업! 현재 레벨: {currentLevel}, 다음 목표: {targetExp}");
    }

    private void UpdateLevelText()
    {
        if (levelText != null)
        {
            levelText.text = $"LV. {currentLevel}";
        }
    }

    // 게임 성공 연출 시작
    private void WinGame()
    {
        Debug.Log($"{targetSuccessLevel}레벨 달성! 게임 성공!");

        if (gameSuccessPanel != null)
        {
            gameSuccessPanel.SetActive(true);
            StartCoroutine(FadeInGameSuccess()); // 실패 화면처럼 슥- 뜨는 코루틴 실행
        }
    }

    // 실패 코드랑 똑같이 서서히 나타나게 하는 성공 코루틴
    IEnumerator FadeInGameSuccess()
    {
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime; // 게임이 멈춰도 돌아가게 unscaled 사용
            if (successCanvasGroup != null)
            {
                // 시간을 이용해 Alpha 값을 0에서 1로 슥- 올립니다.
                successCanvasGroup.alpha = Mathf.Lerp(0, 1, timer / fadeDuration);
            }
            yield return null;
        }

        // 완전히 다 뜨면 그때 게임을 멈춥니다.
        Time.timeScale = 0f;
    }
}
