using UnityEngine;
using System.Collections;

public class Monster : MonoBehaviour
{
    public string myName="Slime";            // 몬스터의 이름
    public float speed = 2.0f;     // 이동 속도
    public int maxHealth = 10;     // 최대 체력
    public int damage = 5;         // 피해량
    public int dropExpAmount = 1; // 드랍할 경험치 양
    public float attackCooldown = 0.5f;        // 공격 쿨타임 (0.5초마다 데미지)


    // 플레이어와 이 거리 이상 멀어지면 디스폰 (화면 밖 처리)
    public float despawnDistance = 20f;

    private int currentHealth;
    private Transform player;      // 플레이어의 위치 정보
    private float lastAttackTime;              // 마지막으로 공격한 시간

    // 스프라이트의 색상을 바꾸기 위한 컴포넌트 참조
    private SpriteRenderer spriteRenderer;
    private Color originalColor; // 원래 색상 저장용
    private Rigidbody2D rb; // Rigidbody2D 추가

    // [최적화] WaitForSeconds 캐싱 (메모리 가비지 생성 방지)
    private WaitForSeconds flashDuration = new WaitForSeconds(0.1f);
    // 넉백 상태를 확인하는 변수
    private bool isKnockedBack = false;
    // 현재 실행 중인 넉백 코루틴을 기억할 변수
    private Coroutine knockbackCoroutine;

    void Awake()
    {
        // 시작할 때 자신의 SpriteRenderer 컴포넌트를 가져옴
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color; // 보통은 흰색(Color.white)
        rb = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        currentHealth = maxHealth; // 다시 꺼내 쓸 때마다 체력 초기화
        spriteRenderer.color = originalColor; // 큐에서 다시 꺼낼 때 색상 원상복구
        lastAttackTime = 0f;                  // 공격 쿨타임 초기화

        // 풀에서 꺼낼 때 상태 초기화
        isKnockedBack = false;
        if (rb != null) rb.velocity = Vector2.zero;
        knockbackCoroutine = null;

        // 플레이어 찾기 --> 플레이어가 파괴되는 것이 아닌면 Awake로 올리기
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }
    }
    void OnDisable()
    {
        // 비활성화될 때 진행 중인 깜빡임이나 넉백 코루틴을 강제로 멈춤 (버그 방지)
        StopAllCoroutines();
        spriteRenderer.color = originalColor;
    }

    //  Update에서는 거리 체크(디스폰)를 담당
    void Update()
    {
        if (player != null)
        {
            // 최적화: Vector2.Distance 대신 sqrMagnitude 사용 (루트 계산 생략으로 CPU 연산 절약)
            float sqrDistance = (player.position - transform.position).sqrMagnitude;

            // despawnDistance의 제곱값과 비교
            if (sqrDistance > (despawnDistance * despawnDistance))
            {
                // 거리가 너무 멀어지면 풀로 돌아감
                EnemySpawner.Instance.ReturnEnemy(myName, gameObject);
            }
        }
    }

    void FixedUpdate()
    {
        // 넉백 중일 때는 아래의 추적 이동 로직을 실행하지 않고 빠져나감
        if (isKnockedBack) return;

        // 1. 플레이어 추적 이동
        if (player != null)
        {
            // 현재 위치에서 플레이어를 향하는 방향 벡터를 구하고 정규화(길이를 1로 만듦)
            Vector2 direction = (player.position - transform.position).normalized;

            // 방향에 따른 스프라이트 좌우 반전
            if (direction.x != 0)
            {
                spriteRenderer.flipX = direction.x < 0;
            }

            // 초당 speed만큼 해당 방향으로 이동
            Vector2 nextPosition = rb.position + direction * speed * Time.fixedDeltaTime;
            rb.MovePosition(nextPosition);
        }
    }
    //  무기(외부)에서 호출하여 넉백을 적용하는 함수
    // direction: 밀려날 방향 (보통 플레이어 -> 몬스터 방향)
    // thrust: 밀려나는 힘의 크기
    // duration: 밀려나는 시간
    public void ApplyKnockback(Vector2 direction, float thrust, float duration = 0.15f)
    {
        // 오브젝트가 활성화 상태일 때만 코루틴 실행 (버그 방지)
        if (gameObject.activeInHierarchy)
        {
            // 이미 넉백 중이라면 기존 넉백을 취소하고 새로운 넉백 적용 (버그 방지)
            if (knockbackCoroutine != null)
            {
                StopCoroutine(knockbackCoroutine);
            }
            knockbackCoroutine = StartCoroutine(KnockbackRoutine(direction, thrust, duration));
        }
    }

    // 넉백 처리를 하는 코루틴
    private IEnumerator KnockbackRoutine(Vector2 direction, float thrust, float duration)
    {
        isKnockedBack = true; // 넉백 상태 켜기 (FixedUpdate 추적 중지)

        rb.velocity = Vector2.zero; // 기존에 받던 힘 초기화
        // 밀어내는 방향으로 순간적인 힘(Impulse)을 가함
        rb.AddForce(direction.normalized * thrust, ForceMode2D.Impulse);

        // 지정된 넉백 시간만큼 대기
        yield return new WaitForSeconds(duration);

        // 넉백이 끝나면 속도를 다시 0으로 만들고 추적 재개
        rb.velocity = Vector2.zero;
        isKnockedBack = false;
    }

    // 2. 외부(무기 등)에서 호출하여 데미지를 입히는 함수
    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;

        // 데미지를 입을 때 깜빡이는 코루틴 실행
        StartCoroutine(FlashRoutine());

        // 체력이 0 이하가 되면 사망 처리
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // 깜빡임 효과를 주는 코루틴 함수
    private IEnumerator FlashRoutine()
    {
        // 1. 색상을 빨간색(피격 느낌)으로 변경
        spriteRenderer.color = Color.red;

        // 2. 0.1초 동안 대기
        yield return flashDuration;

        // 3. 원래 색상으로 복구
        spriteRenderer.color = originalColor;
    }

    // 3. 사망 처리 로직
    private void Die()
    {
        //  GemSpawner를 통해 오브젝트 풀에서 보석을 꺼내옴
        if (GemSpawner.Instance != null)
        {
            GemSpawner.Instance.SpawnGem(transform.position, dropExpAmount);
        }
        EnemySpawner.Instance.ReturnEnemy(myName, gameObject);
    }

    // 4. 플레이어와 충돌했을 때 데미지를 주는 로직

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                 PlayerStats playerScript = collision.gameObject.GetComponent<PlayerStats>();
                 if (playerScript != null)
                 {
                      playerScript.TakeDamage(damage);
                      lastAttackTime = Time.time; // 타격 성공 시 공격 시간 갱신
                 }
            }
        }
    }
}