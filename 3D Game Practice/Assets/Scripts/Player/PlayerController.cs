using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    Vector2 curMovementInput;
    public float jumpForce;
    public LayerMask groundLayerMask;

    [Header("Look")]
    public Transform cameraContainer;
    public float minXLook;
    public float maxXLook;
    float camCurXRot;
    public float lookSensitivity;

    Vector2 mouseDelta;

    [HideInInspector]
    public bool canLook = true;  // 이렇게 숨기는 이유가 뭘까?

    Rigidbody _rigidbody;

    public static PlayerController instance;
    private void Awake()
    {
        instance = this;
        _rigidbody = GetComponent<Rigidbody>(); 
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;  // 커서 잠구기
    }


    private void FixedUpdate()  // 보통 물리적인 작업에 많이 씀
    {
        Move();
    }

    private void LateUpdate()  // 보통 카메라 작업에 많이 씀
    {
        if(canLook)
        {
            CameraLook();
        }
    }

    void Move()
    {
        Vector3 dir = transform.forward * curMovementInput.y + transform.right * curMovementInput.x; // 플레이어가 서있는 상태에서의 forward와 right에 입력값을 더한다.
        dir *= moveSpeed;  // 이동속도 조절
        dir.y = _rigidbody.velocity.y; // 수직 방향의 움직임을 설정한다.

        _rigidbody.velocity = dir; // dir 벡터를 사용하여 캐릭터의 Rigidbody 컴포넌트의 속도를 설정. 캐릭터를 실제로 움직이게 하는 부분이다.
                                   // Rigidbody의 속도를 설정하면 물리 엔진이 이동을 처리하고 충돌 감지 등을 수행한다.
    }

    void CameraLook()  // 카메라를 움직이는 "패턴"으로 이해하자
    {
        camCurXRot += mouseDelta.y * lookSensitivity; // 마우스를 위 아래로 움직이면(mouseDelta의 y가 변동하면) 가로축이 회전해야 하기 때문에 x의 Rotation이 변화한다. (고개를 끄덕이는 느낌)
        camCurXRot = Mathf.Clamp(camCurXRot, minXLook, maxXLook); // camCurXRot을 Clamp로 minXLook과 maxXLook만큼 제한함 - 시점을 제한하기 위한 코드
        cameraContainer.localEulerAngles = new Vector3(-camCurXRot, 0, 0); // 카메라를 위 아래로 = 시점을 위 아래로 조절하는 코드

        transform.eulerAngles += new Vector3(0, mouseDelta.x * lookSensitivity, 0); // 마우스를 좌우로 움직일 때 시점이 좌우로 변경되게하는 코드 mouseDelta.x를 쓴 이유는 맨 위 코드와 같다.
    }

    public void OnLookInput(InputAction.CallbackContext context)  // 마우스 입력의 변화를 감지하고 mouseDelta 변수에 해당 값을 저장하여 나중에 시점 조절 등에 사용할 수 있게 한다.
    {
        mouseDelta = context.ReadValue<Vector2>();
    }

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        // InputActionPhase로 키를 어떤 식으로 눌렀는지 구분 가능하다. 눌러지고 있는 중이면 Performed, 눌렀다가 뗀 상태면 Canceled, 버튼을 눌렀을 때 처음 한 프레임은 Started
        if (context.phase == InputActionPhase.Performed) // "Performed" 단계일 때 = 버튼이 눌려지고 있을 때, 현재 입력 액션의 값을 Vector2 형식의 curMovementInput 변수에 저장한다.
        {
            curMovementInput = context.ReadValue<Vector2>();
        }
        else if(context.phase == InputActionPhase.Canceled)
        {
            curMovementInput = Vector2.zero;
        }
    }

    public void OnJumpInput(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started) // 점프는 한 번이기 때문에 Started
        {
            if(IsGrounded())
            _rigidbody.AddForce(Vector2.up * jumpForce, ForceMode.Impulse); // ForceMode.Impulse는 짧은 시간 동안 큰 힘을 적용한다. 질량(Mass)에 비례하여 속도를 변경하며, 순간적인 힘을 제공한다.
                                                                            // 주로 점프와 같은 순간적인 동작에 사용된다.
        }
    }

    private bool IsGrounded()
    {
        // 땅에 있는지 체크하기 위해 Ray를 4방향의 아래로(Vector3.down) 쏜다. 순서대로 앞, 뒤, 오른쪽, 왼쪽
        Ray[] rays = new Ray[4]
        {
            new Ray(transform.position + (transform.forward * 0.2f) + (Vector3.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.forward * 0.2f)+ (Vector3.up * 0.01f), Vector3.down),
            new Ray(transform.position + (transform.right * 0.2f)+ (Vector3.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.right * 0.2f)+ (Vector3.up * 0.01f), Vector3.down),
        };

        for(int i = 0; i < rays.Length; i++)  // 메모리적으로 선형으로 이어져있는 애들은 Length라고 생각하면 된다. List같은 애들은 Count 
        {
            if (Physics.Raycast(rays[i], 0.1f, groundLayerMask)) // 4개의 Ray중에 하나라도 땅에 닿으면 true(점프 가능) 반환
            {
                return true;
            }
        }

        return false;
    }

    private void OnDrawGizmos()  // 땅이랑 닿아있는지 체크하는 기즈모 그리기
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position + (transform.forward * 0.2f) + (Vector3.up * 0.01f), Vector3.down);
        Gizmos.DrawRay(transform.position + (-transform.forward * 0.2f) + (Vector3.up * 0.01f), Vector3.down);
        Gizmos.DrawRay(transform.position + (transform.right * 0.2f) + (Vector3.up * 0.01f), Vector3.down);
        Gizmos.DrawRay(transform.position + (-transform.right * 0.2f) + (Vector3.up * 0.01f), Vector3.down);
    }

    public void ToggleCursor(bool toggle)
    {
        Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
        canLook = !toggle; // 토글이 켜지면 시야를 돌릴 수 없게 설정
    }
}
