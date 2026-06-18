using UnityEngine;

public class Explosion : MonoBehaviour
{
    [Header("���� ����")]
    public float explosionRadius = 1.5f; // ������ ��� ���� (�ݰ�)
    public float lifeTime = 0.5f;        // ���� ����Ʈ�� ȭ�鿡 �����ִ� �ð�

    private float _damage;

    // ����ü�� ������ ���� ��, �ڱ� �������� ���߿��� ������ �ִ� �Լ��Դϴ�.
    public void Setup(float damage)
    {
        _damage = damage;
        Explode(); // �������� ���޹��ڸ��� ��� �����մϴ�!
    }

    private void Explode()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (Collider2D coll in colliders)
        {
            if (coll.CompareTag("Enemy"))
            {
                Debug.Log($" 폭발 명중! {coll.name}에게 광역 데미지 {_damage} 부여!");

                Monster m = coll.GetComponent<Monster>();
                if (m != null) m.TakeDamage((int)_damage);

                RangedMonster r = coll.GetComponent<RangedMonster>();
                if (r != null) r.TakeDamage((int)_damage);

                // ★ 보스 몬스터 체력 깎기 (여기를 추가하세요!) ★
                BossMonster boss = coll.GetComponent<BossMonster>();
                if (boss != null) boss.TakeDamage((int)_damage);
            }
        }
        Destroy(gameObject, lifeTime);
    }

    // ����Ƽ �����Ϳ��� ���� ������ ������ ������ �̸� ���� ���ִ� ���� ����Դϴ�.
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
