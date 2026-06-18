using UnityEngine;
using System.Collections.Generic; // [�߰�] ���� �ߺ� ���� ó���� ���� List ���

public class Manage_Item_Selection : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject itemSelectionPanel; // ������ â �θ�
    [SerializeField] private GameObject darker;       // ��� ��Ӱ� �г�

    // [�߰�] �Ŵ��� ������Ʈ�� ������ ���� ������ ����Ʈ�� ȭ���� ī�� ������Ʈ��
    [Header("Item Pool & Cards")]
    [SerializeField] private Manage_Item[] allGameItems;
    [SerializeField] private Manage_Item_Card_Panel[] itemCards;

    void Start()
    {
        // ������ ���� �� ���α�
        CloseUI();
    }

    // ����ġ ��ũ��Ʈ���� ȣ���� �Լ�
    public void ShowItemSelection()
    {
        // 1. ������ �ƴ� �����۵鸸 �����Ͽ� �ĺ��� ����
        List<Manage_Item> availableItems = new List<Manage_Item>();
        foreach (Manage_Item item in allGameItems)
        {
            if (item != null && item.level < item.data.levelValues.Length)
            {
                availableItems.Add(item);
            }
        }

        // �� ���⿡ �߰�: ��ȭ ������ �������� �ϳ��� ������ ����â�� ����� ����
        if (availableItems.Count == 0)
        {
            Debug.Log("��ȭ ������ �������� ���� ����â�� ����� �ʽ��ϴ�.");
            return;
        }

        // �ð� ����
        Time.timeScale = 0f;

        // UI �ѱ�
        if (darker != null) darker.SetActive(true);
        if (itemSelectionPanel != null) itemSelectionPanel.SetActive(true);

        Debug.Log("������ �Ŵ���: â�� ���� �ð��� ������ϴ�.");


        // 2. ī�� ������ŭ �����ϰ� ��ġ (������ �ĺ��� ī�庸�� ���� ��Ȳ ����)
        int cardsToFill = Mathf.Min(itemCards.Length, availableItems.Count);

        // ��� ī�带 �ϴ� ����
        foreach (var card in itemCards)
        {
            card.gameObject.SetActive(false);
        }

        // 3. �ߺ� ���� ���� ��Ī �� ī�� �ѱ�
        for (int i = 0; i < cardsToFill; i++)
        {
            int randomIndex = Random.Range(0, availableItems.Count);
            Manage_Item selectedItem = availableItems[randomIndex];

            itemCards[i].SetupCard(selectedItem);
            itemCards[i].gameObject.SetActive(true);

            // �ߺ� ������ ���� ���� �ĺ� ��Ͽ��� ����
            availableItems.RemoveAt(randomIndex);
        }
    }

    // ������ ��ư�� ������ �� ȣ���� �Լ�
    public void OnSelectItem()
    {
        CloseUI();

        // �ð� �ٽ� �帣��
        Time.timeScale = 1f;

        Debug.Log("������ �Ŵ���: ���� �Ϸ�! �ð��� �ٽ� ����մϴ�.");
    }

    private void CloseUI()
    {
        if (darker != null) darker.SetActive(false);
        if (itemSelectionPanel != null) itemSelectionPanel.SetActive(false);
    }
}
