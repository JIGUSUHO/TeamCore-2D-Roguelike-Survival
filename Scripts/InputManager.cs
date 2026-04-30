using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    
    [SerializeField]
    private float speed;
    Rigidbody2D rb;
    private Animator anim;
    private Vector2 direction;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }
    
    // Update is called once per frame
    void Update()
    {
        //프레임마다 Vertical 축과 Horizontal 축 가져오기
        float vertical = Input.GetAxisRaw("Vertical");
        float horizontal = Input.GetAxisRaw("Horizontal");
        //정규화까지
        direction = new Vector2(horizontal, vertical).normalized;

        if (anim != null)
        {
            anim.SetBool("isMoving", direction.magnitude > 0);
        }
    }
    void FixedUpdate()
    {
        rb.velocity = direction * speed;
    }
}
