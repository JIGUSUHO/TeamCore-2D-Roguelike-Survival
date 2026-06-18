using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections; // �ڷ�ƾ�� ���� ���� �߰�!

public class Manage_Exp_Level : MonoBehaviour
{
    [Header("Leveling System")]
    public int currentLevel = 1;
    public float currentExp = 0f;
    public float targetExp = 100f;
    [SerializeField] private int targetSuccessLevel = 45; // ���� ��ǥ ���� �߰�

    [Header("UI References")]
    [SerializeField] private Slider expSlider;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Manage_Item_Selection itemManager; // ������ â ������ ����

    // ====== ���⿡ ���� ���� UI ���� �߰�! ======
    [Header("Game Success UI")]
    [SerializeField] private GameObject gameSuccessPanel; // ���� ���� â �г�
    [SerializeField] private CanvasGroup successCanvasGroup; // ������ �߰� �� ������Ʈ
    [SerializeField] private float fadeDuration = 2f; // ������ �ߴ� �ð� (2��)

    [Header("Settings")]
    [SerializeField] private float lerpSpeed = 5f;

    void Start()
    {
        // ===== [�߰�] �ٽ� �������� ���� ���� �ð��� ����(1)���� ��������! =====
        Time.timeScale = 1f;

        // ���� ���� �� �ʱ� UI ����
        UpdateLevelText();
        if (expSlider != null) expSlider.value = currentExp / targetExp;

        // ������ �� ���� �г��� ���α�
        if (gameSuccessPanel != null) gameSuccessPanel.SetActive(false);
    }

    void Update()
    {
        // ===== [����] ������ �̹� ���� ����(����/���� ��)��� �Ʒ� ������ �ǳʶٱ� =====
        if (Time.timeScale == 0f) return;

        /* �׽�Ʈ�� �ڵ�
        // [�ٽ�] �� ������ ����ġ�� �����ؼ� ��ġ�� ������ ����!
        // �̷��� �ϸ� �ν����Ϳ��� ���ڸ� ���� ���ĵ� ��� �����մϴ�.
        */
        if (currentExp >= targetExp)
        {
            LevelUp();
        }

        // �����̴� �ε巴�� ä���
        if (expSlider != null)
        {
            float targetValue = currentExp / targetExp;
            expSlider.value = Mathf.Lerp(expSlider.value, targetValue, Time.deltaTime * lerpSpeed);
        }
    }

    // �ܺ�(���� óġ ��)���� ����ġ�� �� �� ȣ���ϴ� �Լ�
    public void AddExp(float amount)
    {
        currentExp += amount;
        Debug.Log($"����ġ ȹ��: {amount}, ���� ����ġ: {currentExp}/{targetExp}");

        // ����ġ�� ��ǥġ�� �Ѵ� ���� ��� ������ (���� ������ ����)
        while (currentExp >= targetExp)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        currentLevel++;
        currentExp -= targetExp; // �ʰ��� �̿�
        targetExp += 50f;        // ���� ���� ���̵� ���

        // ������ ��� UI �ݿ�
        UpdateLevelText();

        if (levelText != null) levelText.text = $"LV. {currentLevel}";

        // [�ٽ�] 30���� �޼� �� ���� ó��!
        if (currentLevel >= targetSuccessLevel)
        {
            WinGame();
            return; // ���������Ƿ� �Ʒ� ������ ���� â�� ����� �ʰ� ����!
        }

        // 30���� �̸��� ���� ������ �Ŵ������� â�� ����� ��Ŵ!
        if (itemManager != null)
        {
            itemManager.ShowItemSelection();
        }

        Debug.Log($"�ڷ�����! ���� ����: {currentLevel}, ���� ��ǥ: {targetExp}");
    }

    private void UpdateLevelText()
    {
        if (levelText != null)
        {
            levelText.text = $"LV. {currentLevel}";
        }
    }

    // ���� ���� ���� ����
    public void WinGame()
    {
        Debug.Log($"{targetSuccessLevel}���� �޼�! ���� ����!");

        if (gameSuccessPanel != null)
        {
            gameSuccessPanel.SetActive(true);
            StartCoroutine(FadeInGameSuccess()); // ���� ȭ��ó�� ��- �ߴ� �ڷ�ƾ ����
        }
    }

    // ���� �ڵ�� �Ȱ��� ������ ��Ÿ���� �ϴ� ���� �ڷ�ƾ
    IEnumerator FadeInGameSuccess()
    {
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime; // ������ ���絵 ���ư��� unscaled ���
            if (successCanvasGroup != null)
            {
                // �ð��� �̿��� Alpha ���� 0���� 1�� ��- �ø��ϴ�.
                successCanvasGroup.alpha = Mathf.Lerp(0, 1, timer / fadeDuration);
            }
            yield return null;
        }

        // ������ �� �߸� �׶� ������ ����ϴ�.
        Time.timeScale = 0f;
    }
}
