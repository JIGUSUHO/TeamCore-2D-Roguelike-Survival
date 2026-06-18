using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitWeapon : MonoBehaviour
{
    [Header("회전 설정")]
    public float orbitSpeed = 180f; 
    public float orbitRadius = 2f;  
    public float duration = 4f; 

    private Transform _target;      
    private float _currentAngle = 0f;
    private float _damage; // [추가됨] 데미지를 저장할 변수

    private void Update()
    {
        if (_target == null) return;

        _currentAngle -= orbitSpeed * Time.deltaTime;
        float x = Mathf.Cos(_currentAngle * Mathf.Deg2Rad) * orbitRadius;
        float y = Mathf.Sin(_currentAngle * Mathf.Deg2Rad) * orbitRadius;

        transform.position = _target.position + new Vector3(x, y, 0);
        transform.rotation = Quaternion.Euler(0, 0, _currentAngle - 90f);
    }

    // [수정됨] 매니저에서 무기를 생성할 때 데미지도 같이 넘겨받도록 수정
    public void Setup(Transform playerTransform, float damage, float startAngle = 0f)
    {
        _target = playerTransform;
        _damage = damage; // 받아온 데미지 저장
        _currentAngle = startAngle;

        Destroy(gameObject, duration);
    }

    // [추가됨] 검이 적과 닿았을 때 데미지를 주는 로직
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy")) 
        {
            Debug.Log($"검으로 {collision.name}을(를) 베었습니다! 데미지: {_damage}");

            // 1. 근접 몬스터 체력 깎기
            Monster m = collision.GetComponent<Monster>();
            if (m != null) m.TakeDamage((int)_damage);

            // 2. 원거리 몬스터 체력 깎기
            RangedMonster r = collision.GetComponent<RangedMonster>();
            if (r != null) r.TakeDamage((int)_damage);

            // ★ 3. 보스 몬스터 체력 깎기 (여기를 추가하세요!) ★
            BossMonster boss = collision.GetComponent<BossMonster>();
            if (boss != null) boss.TakeDamage((int)_damage);
        }
    }
}