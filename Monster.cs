using UnityEngine;
using System.Collections;

public class Monster : MonoBehaviour
{
    public string myName="Zombie";            // 몬스터의 이름
    public float speed = 2.0f;     // 이동 속도
    public int maxHealth = 10;     // 최대 체력
    public int damage = 5;         // 피해량

    private int currentHealth;
    private Transform player;      // 플레이어의 위치 정보

    // 스프라이트의 색상을 바꾸기 위한 컴포넌트 참조
    private SpriteRenderer spriteRenderer;
    private Color originalColor; // 원래 색상 저장용
    void Awake()
    {
        // 시작할 때 자신의 SpriteRenderer 컴포넌트를 가져옴
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color; // 보통은 흰색(Color.white)
    }

    void OnEnable()
    {
        currentHealth = maxHealth; // 다시 꺼내 쓸 때마다 체력 초기화
        spriteRenderer.color = originalColor; // 큐에서 다시 꺼낼 때 색상 원상복구

        // 플레이어 찾기 
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }
    }

    void Update()
    {
        // 1. 플레이어 추적 이동
        if (player != null)
        {
            // 현재 위치에서 플레이어를 향하는 방향 벡터를 구하고 정규화(길이를 1로 만듦)
            Vector3 direction = (player.position - transform.position).normalized;

            // 초당 speed만큼 해당 방향으로 이동
            transform.position += direction * speed * Time.deltaTime;
        }
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
        yield return new WaitForSeconds(0.1f);

        // 3. 원래 색상으로 복구
        spriteRenderer.color = originalColor;
    }

    // 3. 사망 처리 로직
    private void Die()
    {
        // 나중에 여기에 경험치 보석 드랍 이벤트를 추가할 수 있어.
        // 지금은 단순히 자신(게임 오브젝트)을 파괴하도록 구현.
        EnemySpawner.Instance.ReturnEnemy(myName, gameObject);
    }

    // 4. 플레이어와 충돌했을 때 데미지를 주는 로직

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // 닿은 오브젝트(플레이어)의 Player 스크립트에 접근해서 데미지 전달
            // Player playerScript = collision.GetComponent<Player>();
            // if (playerScript != null)
            // {
            //     playerScript.TakeDamage(damage);
            // }
        }
    }
}