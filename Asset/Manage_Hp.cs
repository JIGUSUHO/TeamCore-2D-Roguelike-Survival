using UnityEngine;
using UnityEngine.UI;
using System.Collections; // [추가] 코루틴을 쓰기 위해 필요합니다!

public class Manage_Hp : MonoBehaviour
{
    [Header("HP Settings")]
    public float maxHp = 100f;
    public float currentHp;

    [Header("UI References")]
    [SerializeField] private Slider hpSlider;      // [cite: 2]
    [SerializeField] private GameObject gameOverPanel; // 게임 오버 창 [cite: 18]
    [SerializeField] private CanvasGroup gameOverCanvasGroup; // [추가] 서서히 뜨게 할 컴포넌트

    [Header("Settings")]
    [SerializeField] private float lerpSpeed = 5f;  // '슥' 줄어드는 속도
    [SerializeField] private float fadeDuration = 2f; // [추가] 서서히 뜨는 시간 (2초)

    void Awake()
    {
        currentHp = maxHp;
    }

    void Update()
    {
        // [테스트 1] 인스펙터에서 숫자를 0으로 바꿨을 때 바로 죽게 하고 싶다면?
        if (currentHp <= 0 && Time.timeScale != 0)
        {
            Die();
        }

        // 실시간으로 체력 바를 부드럽게 반영
        if (hpSlider != null)
        {
            float targetValue = currentHp / maxHp;
            hpSlider.value = Mathf.Lerp(hpSlider.value, targetValue, Time.deltaTime * lerpSpeed);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHp -= damage;
        if (currentHp <= 0)
        {
            currentHp = 0;
            Die();
        }
    }

    private void Die()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            // [수정] 바로 띄우는 대신 코루틴 시작!
            StartCoroutine(FadeInGameOver());
        }
        Debug.Log("실험 종료: 대상 사망");
    }

    // [추가] 서서히 나타나게 하는 마법의 코드
    IEnumerator FadeInGameOver()
    {
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime; // 게임이 멈춰도 돌아가게 unscaled 사용
            if (gameOverCanvasGroup != null)
            {
                // 시간을 이용해 Alpha 값을 0에서 1로 슥- 올립니다.
                gameOverCanvasGroup.alpha = Mathf.Lerp(0, 1, timer / fadeDuration);
            }
            yield return null; // 한 프레임 쉬고 다음 프레임 진행
        }

        // 완전히 다 뜨면 그때 게임을 멈춥니다.
        Time.timeScale = 0f;
    }
}
