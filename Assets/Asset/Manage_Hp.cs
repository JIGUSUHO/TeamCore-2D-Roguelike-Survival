using UnityEngine;
using UnityEngine.UI;
using TMPro; 
using System.Collections; 

public class Manage_Hp : MonoBehaviour
{
    [Header("HP Settings")]
    public float maxHp = 100f;
    public float currentHp;
    private bool isDead = false; 

    [Header("UI References")]
    [SerializeField] private Slider hpSlider;      
    [SerializeField] private TextMeshProUGUI hpText; 
    [SerializeField] private GameObject gameOverPanel; 
    [SerializeField] private CanvasGroup gameOverCanvasGroup; 

    [Header("Settings")]
    [SerializeField] private float fadeDuration = 2f; 

    void Awake()
    {
        currentHp = maxHp;
        
        if (hpSlider != null)
        {
            hpSlider.minValue = 0f;
            hpSlider.maxValue = 1f; 
            hpSlider.value = 1f; 
        }
        
        UpdateHpText(); 
    }

    // Update() 함수는 이제 필요 없어서 완전히 삭제했습니다!

    public void TakeDamage(float damage)
    {
        if (isDead) return; 

        currentHp -= damage;

        // ★ [핵심] 데미지를 입는 그 순간, 슬라이더의 Value를 즉시 깎아버립니다!
        if (hpSlider != null)
        {
            hpSlider.value = currentHp / maxHp;
        }

        UpdateHpText(); // 글씨도 즉시 업데이트

        // 체력이 0 이하가 되면 사망 처리
        if (currentHp <= 0)
        {
            currentHp = 0;
            Die();
        }
    }

    private void UpdateHpText()
    {
        if (hpText != null)
        {
            hpText.text = $"{Mathf.CeilToInt(currentHp)} / {maxHp}"; 
        }
    }

    private void Die()
    {
        if (isDead) return; 
        isDead = true;      
        

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            
            if (gameOverCanvasGroup != null)
            {
                StartCoroutine(FadeInGameOver());
            }
            else
            {
                Time.timeScale = 0f;
            }
        }
        else
        {
            Time.timeScale = 0f;
        }
        Debug.Log("실험 종료: 대상 사망");
    }

    IEnumerator FadeInGameOver()
    {
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime; 
            if (gameOverCanvasGroup != null)
            {
                gameOverCanvasGroup.alpha = Mathf.Lerp(0, 1, timer / fadeDuration);
            }
            yield return null; 
        }
        Time.timeScale = 0f; 
    }
}