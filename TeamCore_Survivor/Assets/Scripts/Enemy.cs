using UnityEngine;

public class Enemy : MonoBehaviour
{
    // 무언가와 부딪혔을 때 실행되는 함수
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 부딪힌 물체의 태그가 "Weapon"이라면
        if (collision.CompareTag("Weapon"))
        {
            // 나(몬스터)를 파괴한다!
            Destroy(gameObject);
            
            GameManager.Instance.AddKill();
            // (나중에 여기서 점수를 올리거나 이펙트를 생성하는 코드를 넣게 됩니다)
            Debug.Log("몬스터 처치!");
        }
    }
}