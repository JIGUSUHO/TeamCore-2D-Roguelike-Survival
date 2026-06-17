using UnityEngine;

public class Gem : MonoBehaviour
{
    private int expAmount = 1;

    // [추가됨] 자석 연출을 위한 변수
    private Transform player;
    private bool isMagnetic = false;
    private float moveSpeed = 5f;

    // [추가됨] 오브젝트 풀에서 꺼내질 때마다 상태를 초기화
    void OnEnable()
    {
        isMagnetic = false;
        moveSpeed = 5f;
    }

    // 스포너에서 보석을 꺼낼 때 경험치량을 세팅해주는 함수
    public void SetExp(int amount)
    {
        expAmount = amount;
    }

    // [추가됨] 자석 상태가 되면 플레이어를 향해 날아가는 이동 로직
    void Update()
    {
        if (isMagnetic && player != null)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
            moveSpeed += Time.deltaTime * 15f; // 갈수록 속도가 빨라지는 가속도 효과
        }
    }

    // 플레이어의 콜라이더와 닿았을 때 (보석의 Collider2D는 Is Trigger가 켜져 있어야 합니다)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. [추가됨] 플레이어의 '자석 범위'에 먼저 닿았을 때 끌려가기 시작
        if (collision.CompareTag("MagneticRadius"))
        {
            // 자석 범위 콜라이더가 플레이어의 자식 오브젝트라고 가정하고 부모(Player)를 저장
            player = collision.transform.parent;
            isMagnetic = true;
        }
        // 2. 플레이어 본체에 완전히 닿았을 때 (기존 로직)
        else if (collision.CompareTag("Player"))
        {
            // Player 스크립트 쪽에 "AddExp"라는 함수가 있다고 가정하고 경험치 전달
            // (에러 방지를 위해 DontRequireReceiver 옵션 사용)
            collision.SendMessage("AddExp", expAmount, SendMessageOptions.DontRequireReceiver);

            // 경험치를 줬으니 자신은 풀(대기열)로 돌아감
            if (GemSpawner.Instance != null)
            {
                GemSpawner.Instance.ReturnGem(gameObject);
            }
            else
            {
                // 스포너가 혹시나 없다면 그냥 파괴 (에러 대비용)
                Destroy(gameObject);
            }
        }
    }
}