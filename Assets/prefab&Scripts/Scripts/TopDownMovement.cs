using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDownMovement : MonoBehaviour
{
    private TopDownController movementController;
    private Rigidbody2D movementRigidbody;
    private Vector2 movementDirection = Vector2.zero;

    // ★ [추가됨] 인스펙터 창에서 조절할 수 있는 속도 변수! 기본값은 3으로 낮췄습니다.
    [SerializeField] private float speed = 3f; 

    private void Awake() 
    {
        movementController = GetComponent<TopDownController>();
        movementRigidbody = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        movementController.onMoveEvent += Move;
    }

    private void FixedUpdate()
    {
        ApplyMovement(movementDirection);
    }

    private void Move(Vector2 direction)
    {
        movementDirection = direction;
    }

    private void ApplyMovement(Vector2 direction)
    {
        // ★ [수정됨] 고정된 숫자 5 대신 speed 변수를 곱해줍니다!
        direction = direction * speed; 

        movementRigidbody.velocity = direction;
    }
}