using UnityEngine;

// � �ɷ�ġ�� �÷��ִ��� �����ϱ� ���� ������(Enum)
public enum ItemType
{
    MoveSpeed,     // �̵� �ӵ� (�ֻ��)
    AttackArea,    // ���� ���� (���� ���� ����)
    DamageReduce,  // ���ط� ���� (�˾�)
    AttackBoost,   // ���ݷ� ���� (����)
    MaxHP,         // �ִ� ü�� (�ش�)
    ProjectileSpeed, // ����ü �ӵ� (�ٴ�)

    ActiveWeapon   // ★ [추가됨] 직접 발사되는 액티브 무기!
}

// ����Ƽ ������Ʈ â���� ���콺 ��Ŭ������ ������ ������ ���� �� �ְ� ���ݴϴ�.
[CreateAssetMenu(fileName = "NewItemData", menuName = "Scriptable Object/Item Data")]
public class Item_Data_Structure : ScriptableObject
{
    [Header("������ �⺻ ����")]
    public string itemName;       // ������ �̸� (��: �ֻ��)
    public ItemType itemType;     // ������ �Ӽ� ����
    public Sprite itemIcon;       // ������ �̹���(��������Ʈ)
    [TextArea]
    public string itemDescription; // ������ �⺻ ����

    [Header("������ �ɷ�ġ ��ġ (�迭) -> ������ ��������")]
    // ���� 1, ���� 2, ���� 3... ���� ����� ��ġ ����Ʈ�Դϴ�.
    // ��: �̵��ӵ� ��ġ�� [0.1f, 0.15f, 0.2f] �� ������ 10%, 15%, 20% ������ �˴ϴ�.
    [Tooltip("�����ǡ� �� ������ �������� �ƴ϶�, �ش� ���� �޼� ���� '���� ���� ���ʽ� ��ġ'�� �����ϴ�.\n" +
             "��(�̵��ӵ�): 1���� 10% ����, 2���� �� 15% ����, 3���� �� 20% ������ ��� -> [0.1, 0.15, 0.2]�� �Է���.\n" +
             "�÷��̾� ���� �ݿ� ���� © �� '���� ���� ���� ���� �����ְų�' �Ǵ� '�⺻ ���ȿ� �� ���ʽ� ������ �ٷ� �����(���ϴ�)' ������� ������ �ּ���!")]
    public float[] levelValues; 
}