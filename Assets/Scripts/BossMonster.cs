using UnityEngine;
using System.Collections;

public class BossMonster : MonoBehaviour
{
    public string myName = "Boss";         // ������ �̸�
    public float speed = 1.5f;       // �̵� �ӵ� (������ �����ϰ�)
    public int maxHealth = 1000;     // �ִ� ü��
    public int damage = 15;          // ���ط�
    public int dropExpAmount = 50;  // ����� ����ġ ��
    public float attackCooldown = 0.5f;        // ���� ��Ÿ�� (0.5�ʸ��� ������)

    // �÷��̾�� �� �Ÿ� �̻� �־����� ���� (ȭ�� �� ó��) 
    // �� ������ �־����� �� ������� �Ÿ��� ���� ũ��(100f) ��� ���� ��õ�մϴ�.
    public float despawnDistance = 100f;

    [Header("Boss Patterns")]
    public float patternInterval = 4.0f; // �� �ʸ��� ������ ����� ������
    public GameObject projectilePrefab;  // ���� 2�� ����� �߻�ü ������
    public string minionName = "Zombie"; // ���� 3�� ��ȯ�� ��� �̸�

    private int currentHealth;
    private Transform player;      // �÷��̾��� ��ġ ����
    private float lastAttackTime;              // ���������� ������ �ð�

    // ��������Ʈ�� ������ �ٲٱ� ���� ������Ʈ ����
    private SpriteRenderer spriteRenderer;
    private Color originalColor; // ���� ���� �����
    private Rigidbody2D rb; // Rigidbody2D �߰�

    // [�߰���] ���� ������ �ð������� �׷��� ���� ������
    private LineRenderer indicatorLine;

    // [����ȭ] WaitForSeconds ĳ�� (�޸� ������ ���� ����)
    private WaitForSeconds flashDuration = new WaitForSeconds(0.1f);

    // �˹� ���� ��� ������ ����(���� ��, ���)�� Ȯ���ϴ� ����
    private bool isAttacking = false;
    private bool isDead = false;

    void Awake()
    {
        // ������ �� �ڽ��� SpriteRenderer ������Ʈ�� ������
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color; // ������ ���(Color.white)
        rb = GetComponent<Rigidbody2D>();

        // [�߰���] ���� ������ ������Ʈ�� ���ٸ� �ڵ�� ���� �߰��ϰ� �ʱ� ����
        indicatorLine = GetComponent<LineRenderer>();
        if (indicatorLine == null)
        {
            indicatorLine = gameObject.AddComponent<LineRenderer>();
        }
        // ���׸����� ������ ��ũ������ �����Ƿ�, ����Ƽ �⺻ 2D ��������Ʈ ���׸����� �Ҵ�
        indicatorLine.material = new Material(Shader.Find("Sprites/Default"));
        indicatorLine.sortingOrder = 10; // ������ �÷��̾� ���� �������ǰ� ����
        indicatorLine.enabled = false;   // ��ҿ��� ���ܵ�
    }

    void OnEnable()
    {
        currentHealth = maxHealth; // �ٽ� ���� �� ������ ü�� �ʱ�ȭ
        spriteRenderer.color = originalColor; // ť���� �ٽ� ���� �� ���� ���󺹱�
        lastAttackTime = 0f;                  // ���� ��Ÿ�� �ʱ�ȭ

        // Ǯ���� ���� �� ���� �ʱ�ȭ
        isAttacking = false;
        isDead = false;
        if (rb != null) rb.velocity = Vector2.zero;
        if (indicatorLine != null) indicatorLine.enabled = false; // Ȱ 

        // �÷��̾� ã�� --> �÷��̾ �ı��Ǵ� ���� �ƴѸ� Awake�� �ø���
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }

        // ������ Ȱ��ȭ�Ǹ� ���� ��Ÿ�� ����
        StartCoroutine(PatternRoutine());
    }

    void OnDisable()
    {
        // ��Ȱ��ȭ�� �� ���� ���� �������̳� ���� �ڷ�ƾ�� ������ ���� (���� ����)
        StopAllCoroutines();
        spriteRenderer.color = originalColor;
    }

    //  Update������ �Ÿ� üũ(����)�� ���
    void Update()
    {
        if (player != null && !isDead)
        {
            // ����ȭ: Vector2.Distance ��� sqrMagnitude ��� (��Ʈ ��� �������� CPU ���� ����)
            float sqrDistance = (player.position - transform.position).sqrMagnitude;

            // despawnDistance�� �������� ��
            if (sqrDistance > (despawnDistance * despawnDistance))
            {
                // �Ÿ��� �ʹ� �־����� Ǯ�� ���ư�
                EnemySpawner.Instance.ReturnEnemy(myName, gameObject);
            }
        }
    }

    void FixedUpdate()
    {
        // ������ ���� ���� ���̰ų� �׾��� ���� �Ʒ��� ���� �̵� ������ �������� �ʰ� ��������
        if (isAttacking || isDead || player == null) return;

        // 1. �÷��̾� ���� �̵�
        // ���� ��ġ���� �÷��̾ ���ϴ� ���� ���͸� ���ϰ� ����ȭ(���̸� 1�� ����)
        Vector2 direction = (player.position - transform.position).normalized;

        // ���⿡ ���� ��������Ʈ �¿� ����
        if (direction.x != 0)
        {
            spriteRenderer.flipX = direction.x < 0;
        }

        // �ʴ� speed��ŭ �ش� �������� �̵�
        Vector2 nextPosition = rb.position + direction * speed * Time.fixedDeltaTime;
        rb.MovePosition(nextPosition);
    }

    // ==========================================
    // ���� ���� �Ŵ���
    // ==========================================
    private IEnumerator PatternRoutine()
    {
        yield return new WaitForSeconds(2.0f); // �����ϰ� 2�� ��� �� ���� ����

        while (!isDead)
        {
            int pattern = Random.Range(1, 5); // 1~4�� ���� �� ���� ����

            isAttacking = true; // �̵�(����) ����
            rb.velocity = Vector2.zero;

            switch (pattern)
            {
                case 1: yield return StartCoroutine(DashAttack()); break;
                case 2: yield return StartCoroutine(BulletHell()); break;
                case 3: yield return StartCoroutine(SummonMinions()); break;
                case 4: yield return StartCoroutine(GroundSmash()); break;
            }

            isAttacking = false; // ���� ���� �� �ٽ� ���� ����
            yield return new WaitForSeconds(patternInterval);
        }
    }

    // ���� 1: �ͷ��� ����
    private IEnumerator DashAttack()
    {
        spriteRenderer.color = Color.red;
        Vector2 targetDir = (player.position - transform.position).normalized;

        float dashSpeed = speed * 2f;
        float dashDuration = 3.0f;

        // [�߰���] ����(���� ����) �׸���
        indicatorLine.positionCount = 2; // �� 2���� ���� ����
        indicatorLine.startWidth = 0.6f; // �� �β�
        indicatorLine.endWidth = 0.6f;
        indicatorLine.startColor = new Color(1f, 0f, 0f, 0.4f); // ������ ����
        indicatorLine.endColor = new Color(1f, 0f, 0f, 0.4f);

        indicatorLine.SetPosition(0, transform.position); // ������: ���� ��ġ
        indicatorLine.SetPosition(1, (Vector2)transform.position + targetDir * (dashSpeed * dashDuration)); // ����: ���� ������
        indicatorLine.enabled = true; // ȭ�鿡 ǥ��

        yield return new WaitForSeconds(0.8f);

        indicatorLine.enabled = false; // ���� ������ �� �����
        spriteRenderer.color = originalColor;

        float timer = 0f;

        while (timer < dashDuration)
        {
            rb.MovePosition(rb.position + targetDir * dashSpeed * Time.fixedDeltaTime);
            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
    }

    // ���� 2: ����� ź�� �߻� (EnemyProjectile ��ũ��Ʈ ����)
    private IEnumerator BulletHell()
    {
        spriteRenderer.color = Color.blue;
        yield return new WaitForSeconds(0.5f);

        int projectileCount = 20;
        float angleStep = 360f / projectileCount;

        if (projectilePrefab != null)
        {
            for (int i = 0; i < projectileCount; i++)
            {
                float angle = i * angleStep;
                Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

                GameObject bullet = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
                EnemyProjectile bulletScript = bullet.GetComponent<EnemyProjectile>();

                if (bulletScript != null)
                {
                    bulletScript.Setup(dir, damage, 5f);
                }
            }
        }

        spriteRenderer.color = originalColor;
        yield return new WaitForSeconds(0.5f);
    }

    // ���� 3: ���� ��ȯ
    private IEnumerator SummonMinions()
    {
        for (int i = 0; i < 3; i++)
        {
            spriteRenderer.color = Color.black;
            yield return new WaitForSeconds(0.2f);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(0.2f);
        }

        if (EnemySpawner.Instance != null)
        {
            for (int i = 0; i < 4; i++)
            {
                Vector2 randomPos = (Vector2)transform.position + Random.insideUnitCircle * 2f;
                // EnemySpawner�� �߰��ߴ� ��ġ ���� ���� �Լ� ȣ��
                EnemySpawner.Instance.SpawnEnemyAtPosition(minionName, randomPos);
            }
        }
    }

    // ���� 4: ���� �� ���
    private IEnumerator GroundSmash()
    {
        float smashRadius = 3.5f;

        // [�߰���] ����(Ÿ�� �ݰ�) �׵θ� �׸���
        int segments = 40; // ���� ������ ������ ���� (Ŭ���� �ε巯�� ���� ��)
        indicatorLine.positionCount = segments + 1; // ���� ������ ���� + 1
        indicatorLine.startWidth = 0.1f; // �� �׵θ� ����
        indicatorLine.endWidth = 0.1f;
        indicatorLine.startColor = new Color(1f, 0.5f, 0f, 0.8f); // �ѷ��� ��Ȳ��
        indicatorLine.endColor = new Color(1f, 0.5f, 0f, 0.8f);

        // 360���� �߰� �ɰ��� �������� ���� ����ϴ�.
        for (int i = 0; i <= segments; i++)
        {
            float angle = i * (360f / segments);
            Vector2 point = (Vector2)transform.position + new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * smashRadius;
            indicatorLine.SetPosition(i, point);
        }
        indicatorLine.enabled = true; // ȭ�鿡 �� ǥ��

        Vector3 originalScale = transform.localScale;
        transform.localScale = originalScale * 1.5f;
        spriteRenderer.color = new Color(1f, 0.5f, 0f);

        yield return new WaitForSeconds(3.0f); // 3�ʰ� ���� �� Ÿ��!

        indicatorLine.enabled = false; // Ÿ�� ������ �׵θ� �����

        if (Vector2.Distance(transform.position, player.position) <= smashRadius)
        {
            Manage_Hp playerScript = player.GetComponent<Manage_Hp>();
            if (playerScript != null) 
                playerScript.TakeDamage(damage * 2); 
        }

        transform.localScale = originalScale;
        spriteRenderer.color = originalColor;

        yield return new WaitForSeconds(1.0f);
    }

    // 2. 외부(무기 등)에서 호출하여 데미지를 입히는 함수
    public void TakeDamage(int damageAmount)
    {
        if (isDead) return;

        currentHealth -= damageAmount;

        // ★ [추가됨] 보스 체력이 진짜 깎이고 있는지 콘솔창에서 확인!
        Debug.Log($"{myName} 피격! 받은 데미지: {damageAmount} / 남은 체력: {currentHealth}");

        // 피격 시 하얗게 깜빡이는 코루틴 실행
        StartCoroutine(FlashRoutine());

        // 체력이 0 이하가 되면 사망 처리
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // ������ ȿ���� �ִ� �ڷ�ƾ �Լ�
    private IEnumerator FlashRoutine()
    {
        // 1. ������ �Ͼ��(�ǰ� ����)���� ���� (������ �Ͼ�� ������ ����)
        spriteRenderer.color = Color.white;

        // 2. 0.1�� ���� ���
        yield return flashDuration;

        // 3. ���� �������� ���� (���� ���� ���� �ƴ� ���� ����)
        if (!isAttacking) spriteRenderer.color = originalColor;
    }

    // 3. ��� ó�� ����
    // 3. 사망 처리 로직
    private void Die()
    {
        isDead = true;
        StopAllCoroutines();
        if (indicatorLine != null) indicatorLine.enabled = false; 

        // ★ [핵심 추가] 게임 매니저를 찾아서 승리(WinGame) 명령을 내립니다! ★
        Manage_Exp_Level gameManager = FindObjectOfType<Manage_Exp_Level>();
        if (gameManager != null)
        {
            Debug.Log("보스 처치 완료! 게임 클리어!");
            gameManager.WinGame();
        }

        // 아래는 기존에 있던 보석 소환 및 스포너 복귀 로직입니다. (그대로 둡니다)
        if (GemSpawner.Instance != null)
        {
            int gemCount = 8;
            // ... (생략) ...
        }

        if (EnemySpawner.Instance != null)
        {
            EnemySpawner.Instance.ReturnEnemy(myName, gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 4. �÷��̾�� �浹���� �� �������� �ִ� ����
    // 4. 플레이어와 충돌했을 때 데미지를 주는 로직
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isDead)
        {
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                // ★ [수정됨] PlayerStats를 Manage_Hp로 변경!
                Manage_Hp playerScript = collision.gameObject.GetComponent<Manage_Hp>();
                if (playerScript != null)
                {
                    playerScript.TakeDamage(damage);
                    lastAttackTime = Time.time;
                }
            }
        }
    }
}