using UnityEngine;
using UnityEngine.InputSystem;

public class ProjectileWeapon : MonoBehaviour
{
    [Header("투사체 설정")]
    public float speed = 25f;
    public float maxDistance = 40f;
    public GameObject explosionPrefab; // 폭발 프리팹 연결 빈칸

    private float currentDamage;
    private int currentLevel;
    private Vector2 startPosition;
    private Vector2 moveDirection;

    public void Setup(float damage, int level)
    {
        currentDamage = damage;
        currentLevel = level;
    }

    private void Start()
    {
        startPosition = transform.position;

        if (currentLevel >= 5)
        {
            TargetNearestEnemy();
        }
        else
        {
            TargetMouse();
        }

        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void Update()
    {
        transform.position += (Vector3)(moveDirection * speed * Time.deltaTime);

        // 1. 최대 사거리에 도달하면 허공이더라도 폭발 함수를 부릅니다!
        if (Vector2.Distance(startPosition, transform.position) >= maxDistance)
        {
            TriggerExplosion();
        }
    }

    private void TargetMouse()
    {
        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
        mousePos.z = 0f;

        moveDirection = (mousePos - transform.position).normalized;
    }

    private void TargetNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float closestDistance = Mathf.Infinity;
        GameObject closestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }

        if (closestEnemy != null)
        {
            moveDirection = (closestEnemy.transform.position - transform.position).normalized;
        }
        else
        {
            TargetMouse();
        }
    }

    // 2. 적과 부딪혔을 때도 폭발 함수를 부릅니다!
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            TriggerExplosion();
        }
    }

    // 3. 핵심: 폭발을 일으키고 사라지는 기능을 따로 빼서 깔끔하게 묶어주었습니다.
    private void TriggerExplosion()
    {
        if (explosionPrefab != null)
        {
            GameObject explosionObj = Instantiate(explosionPrefab, transform.position, Quaternion.identity);

            Explosion expScript = explosionObj.GetComponent<Explosion>();
            if (expScript != null)
            {
                expScript.Setup(currentDamage);
            }
        }

        Destroy(gameObject); 
    }
}