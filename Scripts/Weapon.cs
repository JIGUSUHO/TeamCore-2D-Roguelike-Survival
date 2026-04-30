using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("회전 설정")]
    public float orbitSpeed = 180f; // 초당 회전 각도 (속도)
    public float orbitRadius = 2f;  // 캐릭터로부터의 거리 (반경)

    private Transform _target;      // 중심축이 될 플레이어
    private float _currentAngle = 0f;

    private void Update()
    {
        // 타겟이 없으면 에러가 나지 않도록 방어 코드 작성
        if (_target == null) return;

        // 1. 각도 계산 (시계 방향으로 돌기 위해 각도를 빼줍니다)
        _currentAngle -= orbitSpeed * Time.deltaTime;

        // 2. 삼각함수를 이용해 원형 궤도의 X, Y 좌표 계산 (Mathf는 기본적으로 라디안 값을 사용하므로 Deg2Rad로 변환)
        float x = Mathf.Cos(_currentAngle * Mathf.Deg2Rad) * orbitRadius;
        float y = Mathf.Sin(_currentAngle * Mathf.Deg2Rad) * orbitRadius;

        // 3. 검의 위치를 플레이어 위치 기준으로 업데이트
        transform.position = _target.position + new Vector3(x, y, 0);

        // 4. (선택) 검의 날이 항상 바깥쪽을 향하도록 회전
        // 스프라이트 원본 이미지가 위(↑)를 보고 있다고 가정할 때의 기준입니다.
        transform.rotation = Quaternion.Euler(0, 0, _currentAngle - 90f);
    }

    // 매니저 클래스에서 검을 생성할 때 초기값을 넣어줄 함수
    public void Setup(Transform playerTransform, float startAngle = 0f)
    {
        _target = playerTransform;
        _currentAngle = startAngle;
    }
}
