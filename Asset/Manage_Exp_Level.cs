using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Manage_Exp_Level : MonoBehaviour
{
    [Header("Leveling System")]
    public int currentLevel = 1;
    public float currentExp = 0f;
    public float targetExp = 100f;

    [Header("UI References")]
    [SerializeField] private Slider expSlider;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Manage_Item_Selection itemManager; // 아이템 창 관리자 연결

    [Header("Settings")]
    [SerializeField] private float lerpSpeed = 5f;

    void Start()
    {
        // 게임 시작 시 초기 UI 세팅
        UpdateLevelText();
        if (expSlider != null) expSlider.value = currentExp / targetExp;
    }

    void Update()
    {
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

        // [핵심] 아이템 매니저에게 창을 띄우라고 시킴!
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
}
