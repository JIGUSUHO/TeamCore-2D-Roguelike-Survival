using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : TopDownController
{
    private Camera _camera;
    private void Awake()
    {
        _camera = Camera.main;
    }
    //wasd 값 입력
    public void OnMove(InputValue value)
    {
        //크기가 1인 벡터생성
        Vector2 moveInput = value.Get<Vector2>().normalized;
        CallMoveEvent(moveInput);
    }
    public void OnLook(InputValue value)
    {
        //마우스 위치
        Vector2 newAim = value.Get<Vector2>();
        //마우스위치의 화면좌표를 월드로 변경
        Vector2 worldPos = _camera.ScreenToWorldPoint(newAim);
        //월드 값에서 벡터2로 설정된 현재위치를 뺀값 정규화
        newAim = (worldPos - (Vector2)transform.position).normalized;

        if (newAim.magnitude >= .9f)
        //벡터값을 실수로 변환
        {
            CallLookEvent(newAim);
        }
    }
    public void OnFire(InputValue value)
    {
        Debug.Log("OnFire" + value.ToString());
    }
}

