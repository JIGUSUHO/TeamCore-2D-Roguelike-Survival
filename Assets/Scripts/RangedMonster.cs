using UnityEngine;
using System.Collections;

public class RangedMonster : MonoBehaviour
{
    public string myName = "Bat";         // ������ �̸�
    public float speed = 1.5f;                 // �̵� �ӵ� (�ٰŸ����� ���� ����)
    public int maxHealth = 8;                  // �ִ� ü��
    public int damage = 5;                     // ���ط�
    public int dropExpAmount = 1;              // ����� ����ġ ��
    public float attackCooldown = 2.0f;        // ���� ��Ÿ�� (���Ÿ� ��Ÿ��)

    public float despawnDistance = 20f;        // ȭ�� �� ���� �Ÿ�

    // --- ���Ÿ� ���ݿ� �߰� ���� ---
    public float attackRange = 7f;             // ���� ��Ÿ� (�� �Ÿ� ������ ���� ����)
    public GameObject projectilePrefab;        // �߻�ü(����ü) ������
    public float projectileSpeed = 5f;         // �߻�ü ���ư��� �ӵ�

    private int currentHealth;
    private Transform player;
    private float lastAttackTime;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Rigidbody2D rb;

    private WaitForSeconds flashDuration = new WaitForSeconds(0.1f);
    private bool isKnockedBack = false;
    private Coroutine knockbackCoroutine;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        rb = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        currentHealth = maxHealth;
        spriteRenderer.color = originalColor;
        lastAttackTime = Time.time; // ���� ���ڸ��� ���� �ʵ��� �ʱ�ȭ

        isKnockedBack = false;
        if (rb != null) rb.velocity = Vector2.zero;
        knockbackCoroutine = null;

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }
    }

    void OnDisable()
    {
        StopAllCoroutines();
        spriteRenderer.color = originalColor;
    }

    void Update()
    {
        if (player != null)
        {
            float sqrDistance = (player.position - transform.position).sqrMagnitude;
            if (sqrDistance > (despawnDistance * despawnDistance))
            {
                EnemySpawner.Instance.ReturnEnemy(myName, gameObject);
            }
        }
    }

    void FixedUpdate()
    {
        if (isKnockedBack) return;

        if (player != null)
        {
            float sqrDistance = (player.position - transform.position).sqrMagnitude;
            Vector2 direction = (player.position - transform.position).normalized;

            // ��������Ʈ �¿� ����
            if (direction.x != 0)
            {
                spriteRenderer.flipX = direction.x < 0;
            }

            // 1. �÷��̾ ��Ÿ� ���̶�� ���� �̵�
            if (sqrDistance > (attackRange * attackRange))
            {
                Vector2 nextPosition = rb.position + direction * speed * Time.fixedDeltaTime;
                rb.MovePosition(nextPosition);
            }
            else // 2. �÷��̾ ��Ÿ� �ȿ� �ִٸ� ���缭 ����
            {
                // �̲������� ���� �����ϱ� ���� �ӵ� 0���� (�ɼ�)
                rb.velocity = Vector2.zero;

                // ��Ÿ�� üũ �� �߻�
                if (Time.time >= lastAttackTime + attackCooldown)
                {
                    FireProjectile(direction);
                    lastAttackTime = Time.time;
                }
            }
        }
    }

    // �߻�ü ���� �� �߻� �Լ�
    private void FireProjectile(Vector2 direction)
    {
        if (projectilePrefab == null) return;

        // [����] �߻�ü�� ����ó�� ������Ʈ Ǯ���� ���� ���� ����ȭ�� �����ϴ�.
        // ���⼭�� �������� ���ظ� ���� Instantiate�� ����߽��ϴ�.
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

        EnemyProjectile projScript = projectile.GetComponent<EnemyProjectile>();
        if (projScript != null)
        {
            // �߻�ü�� ����, ������, �ӵ� ������ �Ѱ���
            projScript.Setup(direction, damage, projectileSpeed);
        }
    }

    public void ApplyKnockback(Vector2 direction, float thrust, float duration = 0.15f)
    {
        if (gameObject.activeInHierarchy)
        {
            if (knockbackCoroutine != null) StopCoroutine(knockbackCoroutine);
            knockbackCoroutine = StartCoroutine(KnockbackRoutine(direction, thrust, duration));
        }
    }

    private IEnumerator KnockbackRoutine(Vector2 direction, float thrust, float duration)
    {
        isKnockedBack = true;
        rb.velocity = Vector2.zero;
        rb.AddForce(direction.normalized * thrust, ForceMode2D.Impulse);
        yield return new WaitForSeconds(duration);
        rb.velocity = Vector2.zero;
        isKnockedBack = false;
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        StartCoroutine(FlashRoutine());
        if (currentHealth <= 0) Die();
    }

    private IEnumerator FlashRoutine()
    {
        spriteRenderer.color = Color.red;
        yield return flashDuration;
        spriteRenderer.color = originalColor;
    }

    private void Die()
    {
        if (GemSpawner.Instance != null)
        {
            GemSpawner.Instance.SpawnGem(transform.position, dropExpAmount);
        }
        EnemySpawner.Instance.ReturnEnemy(myName, gameObject);
    }

    // ���Ÿ� ���Ͷ� �÷��̾�� ���� �ε����� �� �������� �ַ��� ���� ������ �����մϴ�.
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
                    lastAttackTime = Time.time;
                }
            }
        }
    }
}