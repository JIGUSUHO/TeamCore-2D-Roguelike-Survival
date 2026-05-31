using UnityEngine;
using UnityEngine.InputSystem;

public class SonicWaveWeapon : MonoBehaviour
{
    [Header("음파 공격 기본 설정")]
    public float attackRadius = 5f;
    public float attackAngle = 90f;
    public float slowDuration = 2f;
    public float lifeTime = 0.5f;
    public float spawnOffset = 1.5f;

    // 매니저로부터 전달받을 변수들입니다.
    private float currentDamage;
    private float currentSlowRatio;

    // 무기가 생성되는 순간 WeaponManager가 호출
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
        // 현재 설정된 currentDamage와 currentSlowRatio를 적에게 적용
        Debug.Log($"음파 명중! {enemy.name}에게 데미지 {currentDamage} 부여 및 이동속도 {currentSlowRatio * 100}% 슬로우!");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}