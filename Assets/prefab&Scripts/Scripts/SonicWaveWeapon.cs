using UnityEngine;
using UnityEngine.InputSystem;

public class SonicWaveWeapon : MonoBehaviour
{
    [Header("���� ���� �⺻ ����")]
    public float attackRadius = 5f;
    public float attackAngle = 90f;
    public float slowDuration = 2f;
    public float lifeTime = 0.5f;
    public float spawnOffset = 1.5f;

    // �Ŵ����κ��� ���޹��� �������Դϴ�.
    private float currentDamage;
    private float currentSlowRatio;

    // ���Ⱑ �����Ǵ� ���� WeaponManager�� ȣ��
    public void Setup(float damage, float slowRatio)
    {
        currentDamage = damage;
        currentSlowRatio = slowRatio;
    }

    private void Start()
    {
        GameObject player = GameObject.Find("player");

        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
        mousePos.z = 0f;

        Vector2 direction = Vector2.zero;

        if (player != null)
        {
            direction = (mousePos - player.transform.position).normalized;
            transform.SetParent(player.transform);
            transform.localPosition = direction * spawnOffset;
        }
        else
        {
            direction = (mousePos - transform.position).normalized;
        }

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        Vector2 attackDirection = transform.right;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, attackRadius);

        foreach (Collider2D coll in colliders)
        {
            if (coll.CompareTag("Enemy"))
            {
                Vector2 dirToTarget = (coll.transform.position - transform.position).normalized;

                if (Vector2.Angle(attackDirection, dirToTarget) < attackAngle / 2f)
                {
                    HitEnemy(coll.gameObject);
                }
            }
        }

        Destroy(gameObject, lifeTime);
    }

   private void HitEnemy(GameObject enemy)
    {
        Debug.Log($"음파 명중! {enemy.name}에게 데미지 {currentDamage} 부여!");

        Monster m = enemy.GetComponent<Monster>();
        if (m != null) m.TakeDamage((int)currentDamage);

        RangedMonster r = enemy.GetComponent<RangedMonster>();
        if (r != null) r.TakeDamage((int)currentDamage);

        // ★ 보스 몬스터 체력 깎기 (여기를 추가하세요!) ★
        BossMonster boss = enemy.GetComponent<BossMonster>();
        if (boss != null) boss.TakeDamage((int)currentDamage);
    }
}