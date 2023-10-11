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
    public bool canLook = true;  // �̷��� ����� ������ ����?

    Rigidbody _rigidbody;

    public static PlayerController instance;
    private void Awake()
    {
        instance = this;
        _rigidbody = GetComponent<Rigidbody>(); 
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;  // Ŀ�� �ᱸ��
    }


    private void FixedUpdate()  // ���� �������� �۾��� ���� ��
    {
        Move();
    }

    private void LateUpdate()  // ���� ī�޶� �۾��� ���� ��
    {
        if(canLook)
        {
            CameraLook();
        }
    }

    void Move()
    {
        Vector3 dir = transform.forward * curMovementInput.y + transform.right * curMovementInput.x; // �÷��̾ ���ִ� ���¿����� forward�� right�� �Է°��� ���Ѵ�.
        dir *= moveSpeed;  // �̵��ӵ� ����
        dir.y = _rigidbody.velocity.y; // ���� ������ �������� �����Ѵ�.

        _rigidbody.velocity = dir; // dir ���͸� ����Ͽ� ĳ������ Rigidbody ������Ʈ�� �ӵ��� ����. ĳ���͸� ������ �����̰� �ϴ� �κ��̴�.
                                   // Rigidbody�� �ӵ��� �����ϸ� ���� ������ �̵��� ó���ϰ� �浹 ���� ���� �����Ѵ�.
    }

    void CameraLook()  // ī�޶� �����̴� "����"���� ��������
    {
        camCurXRot += mouseDelta.y * lookSensitivity; // ���콺�� �� �Ʒ��� �����̸�(mouseDelta�� y�� �����ϸ�) �������� ȸ���ؾ� �ϱ� ������ x�� Rotation�� ��ȭ�Ѵ�. (���� �����̴� ����)
        camCurXRot = Mathf.Clamp(camCurXRot, minXLook, maxXLook); // camCurXRot�� Clamp�� minXLook�� maxXLook��ŭ ������ - ������ �����ϱ� ���� �ڵ�
        cameraContainer.localEulerAngles = new Vector3(-camCurXRot, 0, 0); // ī�޶� �� �Ʒ��� = ������ �� �Ʒ��� �����ϴ� �ڵ�

        transform.eulerAngles += new Vector3(0, mouseDelta.x * lookSensitivity, 0); // ���콺�� �¿�� ������ �� ������ �¿�� ����ǰ��ϴ� �ڵ� mouseDelta.x�� �� ������ �� �� �ڵ�� ����.
    }

    public void OnLookInput(InputAction.CallbackContext context)  // ���콺 �Է��� ��ȭ�� �����ϰ� mouseDelta ������ �ش� ���� �����Ͽ� ���߿� ���� ���� � ����� �� �ְ� �Ѵ�.
    {
        mouseDelta = context.ReadValue<Vector2>();
    }

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        // InputActionPhase�� Ű�� � ������ �������� ���� �����ϴ�. �������� �ִ� ���̸� Performed, �����ٰ� �� ���¸� Canceled, ��ư�� ������ �� ó�� �� �������� Started
        if (context.phase == InputActionPhase.Performed) // "Performed" �ܰ��� �� = ��ư�� �������� ���� ��, ���� �Է� �׼��� ���� Vector2 ������ curMovementInput ������ �����Ѵ�.
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
        if(context.phase == InputActionPhase.Started) // ������ �� ���̱� ������ Started
        {
            if(IsGrounded())
            _rigidbody.AddForce(Vector2.up * jumpForce, ForceMode.Impulse); // ForceMode.Impulse�� ª�� �ð� ���� ū ���� �����Ѵ�. ����(Mass)�� ����Ͽ� �ӵ��� �����ϸ�, �������� ���� �����Ѵ�.
                                                                            // �ַ� ������ ���� �������� ���ۿ� ���ȴ�.
        }
    }

    private bool IsGrounded()
    {
        // ���� �ִ��� üũ�ϱ� ���� Ray�� 4������ �Ʒ���(Vector3.down) ���. ������� ��, ��, ������, ����
        Ray[] rays = new Ray[4]
        {
            new Ray(transform.position + (transform.forward * 0.2f) + (Vector3.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.forward * 0.2f)+ (Vector3.up * 0.01f), Vector3.down),
            new Ray(transform.position + (transform.right * 0.2f)+ (Vector3.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.right * 0.2f)+ (Vector3.up * 0.01f), Vector3.down),
        };

        for(int i = 0; i < rays.Length; i++)  // �޸������� �������� �̾����ִ� �ֵ��� Length��� �����ϸ� �ȴ�. List���� �ֵ��� Count 
        {
            if (Physics.Raycast(rays[i], 0.1f, groundLayerMask)) // 4���� Ray�߿� �ϳ��� ���� ������ true(���� ����) ��ȯ
            {
                return true;
            }
        }

        return false;
    }

    private void OnDrawGizmos()  // ���̶� ����ִ��� üũ�ϴ� ����� �׸���
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
        canLook = !toggle; // ����� ������ �þ߸� ���� �� ���� ����
    }
}
