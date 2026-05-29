using UnityEngine;

public class Explosion : MonoBehaviour
{
    [Header("폭발 설정")]
    public float explosionRadius = 1.5f; // 폭발이 닿는 범위 (반경)
    public float lifeTime = 0.5f;        // 폭발 이펙트가 화면에 남아있는 시간

    private float _damage;

    // 투사체가 폭발을 낳을 때, 자기 데미지를 폭발에게 전달해 주는 함수입니다.
    public void Setup(float damage)
    {
        _damage = damage;
        Explode(); // 데미지를 전달받자마자 즉시 폭발합니다!
    }

    private void Explode()
    {
        // 내 위치를 중심으로 폭발 범위(explosionRadius) 내의 모든 적을 찾습니다.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (Collider2D coll in colliders)
        {
            if (coll.CompareTag("Enemy"))
            {
                // 나중에 몬스터 스크립트가 완성되면 여기에 연결해 주세요!
                Debug.Log($" 폭발 명중! {coll.name}에게 광역 데미지 {_damage} 부여!");
            }
        }

        // 폭발 애니메이션이 끝날 즈음(lifeTime) 이펙트를 깔끔하게 삭제합니다.
        Destroy(gameObject, lifeTime);
    }

    // 유니티 에디터에서 폭발 범위를 붉은색 원으로 미리 보게 해주는 보조 기능입니다.
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
