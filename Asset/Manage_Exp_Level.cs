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
    [SerializeField] private GameObject itemPanel; // 자동 팝업용

    [Header("Settings")]
    [SerializeField] private float lerpSpeed = 5f;
    [SerializeField] private Press_Pause_Button pauseScript; // 일시정지 연동

    void Update()
    {
        // 실시간으로 바를 부드럽게 채움
        if (expSlider != null)
        {
            float targetValue = currentExp / targetExp;
            expSlider.value = Mathf.Lerp(expSlider.value, targetValue, Time.deltaTime * lerpSpeed);
        }
    }

    // 외부(적 처치 시)에서 호출할 함수
    public void AddExp(float amount)
    {
        currentExp += amount;
        if (currentExp >= targetExp)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        currentLevel++;
        currentExp -= targetExp; // 초과분 이월
        targetExp += 50f;        // 다음 레벨 난이도 상승

        if (levelText != null) levelText.text = $"LV. {currentLevel}";

        // [자동 로직] 레벨업 시 창 띄우고 시간 멈춤
        if (itemPanel != null) itemPanel.SetActive(true);
        if (pauseScript != null) pauseScript.TogglePause(); // 일시정지 연동

        Debug.Log("시스템: 레벨업 자동 트리거 발생!");
    }
}
