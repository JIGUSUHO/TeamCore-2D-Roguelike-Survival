using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private Vector2 moveDirection;
    private int damageAmount;
    private float moveSpeed;

    // RangedMonster 스크립트에서 생성 시 호출하여 정보를 전달하는 함수
    public void Setup(Vector2 direction, int damage, float speed)
    {
        moveDirection = direction.normalized;
        damageAmount = damage;
        moveSpeed = speed;

        // 발사체가 날아가는 방향을 바라보게 회전 (화살, 마법탄 등)
        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // 5초 뒤에 자동으로 파괴 (화면 밖으로 나갔을 때 메모리 누수 방지)
        // 투사체용 오브젝트 풀이 있다면 Return 로직으로 바꿔주세요.
        Destroy(gameObject, 5f);
    }

    void Update()
    {
        // 몬스터의 Setup()에서 각도를 돌려놓았기 때문에 transform.right 방향으로 직진시키면 됩니다.
        transform.Translate(Vector3.right * moveSpeed * Time.deltaTime, Space.Self);
    }

    // 투사체는 물리적 충돌로 밀어내지 않고 뚫고/닿고 사라져야 하므로 Trigger를 사용합니다. (Collider의 Is Trigger 체크 필수)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // 플레이어에게 데미지 입히기
            PlayerStats playerScript = collision.GetComponent<PlayerStats>();
             if (playerScript != null)
             {
                  playerScript.TakeDamage(damageAmount);
             }

            // 플레이어에 적중했으므로 발사체 파괴
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Wall") || collision.CompareTag("Obstacle"))
        {
            Destroy(gameObject); // 벽에 닿아도 파괴
        }
    }
}