using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 실제로 이동이 일어날 컴포넌트

public class TopDownMovement : MonoBehaviour
{
    //탑다운컨트롤러 필요
    private TopDownController movementController;
    //중력도 추가, 이동해야하니까
    private Rigidbody2D movementRigidbody;
    //이동의 초기값 설정
    private Vector2 movementDirection = Vector2.zero;

    private void Awake() // 내 컴포넌트 안에서 끝나는 것
    {
        //movementController와 TopDownMovement는 같은 게임오브젝트 안에 있구나 라고 생각
        //get 자체가 여기 없으면 없다고 말해주기 때문
        movementController = GetComponent<TopDownController>();
        //Rigidbody2D도 마찬가지
        movementRigidbody = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        //이제 컨트롤러에 접근이 가능하니까 move라는 함수를 등록한다.
        movementController.onMoveEvent += Move;
    }

    private void FixedUpdate()
    {
        // 물리 업데이트
        // 리지드바디에서 값을 바꾸기 때문에 물리 업데이트
        ApplyMovement(movementDirection);
    }

    private void Move(Vector2 direction)
    {
        // 이동방향만 정해두고 실제로 움직이지는 않음.
        // 움직이는 것은 물리 업데이트에서 진행(rigidbody가 물리니까)
        movementDirection = direction;
    }

    private void ApplyMovement(Vector2 direction)
    {
        direction = direction * 5;

        movementRigidbody.velocity = direction;
    }
}