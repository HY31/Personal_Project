using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IInteractable
{
    string GetInteractPrompt();
    void OnInteract();
}

public class InteractionManager : MonoBehaviour
{
    public float checkRate = 0.05f;
    private float lastCheckTime;
    public float maxCheckDistance;
    public LayerMask layerMask;

    private GameObject curInteractGameObject;
    private IInteractable curInteractable;

    public TextMeshProUGUI promptText;
    private Camera camera;

    void Start()
    {
        camera = Camera.main; // main �±׸� �������ִ� ī�޶� �ϳ��� �ڵ������� ������ �� - �̱���� ���� ����
    }

    // Update is called once per frame
    void Update()
    {
        // Ÿ�� üũ
        if(Time.time - lastCheckTime > checkRate)
        {
            lastCheckTime = Time.time;

            Ray ray = camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));  // ȭ���� ���߾ӿ��� ���̸� ��ڴٴ� ��
            RaycastHit hit; // Raycast�� ���� ���ؼ� ������ Ray�� �־�ߵ� - ���̸� ��� �浹�� �Ͼ�� ������ �޾ƿ��� ����̱� �����̴�.

            if (Physics.Raycast(ray, out hit, maxCheckDistance, layerMask)) // out�� ������ ���� ä���� ������ - hit�� null�̰ų� ��� �����Ͱ� ä�������� ���̶� ���� ����
            {
                if(hit.collider.gameObject != curInteractGameObject)  // ���̿� ���� ������Ʈ�� �����ߴ� ������Ʈ�� �ٸ��ٸ� ������Ʈ�� ������
                {
                    curInteractGameObject = hit.collider.gameObject;
                    curInteractable = hit.collider.GetComponent<IInteractable>();
                    SetPromptText();
                }
            }  // ������� �ٶ� ������Ʈ�� ������ �� ������Ʈ�� ȭ�� ���߾ӿ� ��ġ�ϴ� ���� ����, PromptText�� ���� �۾��̴�.
            else
            {
                curInteractGameObject = null;
                curInteractable = null;
                promptText.gameObject.SetActive(false);
            }
        }
    }

    private void SetPromptText()
    {
        promptText.gameObject.SetActive(true);
        promptText.text = string.Format("<b>[E]</b> {0}", curInteractable.GetInteractPrompt()); // ""�ȿ� �ִ� ���ڴ� ��ũ�ٿ� ������ �����.
    }

    public void OnInteractInput(InputAction.CallbackContext callbackContext)
    {
        if(callbackContext.phase == InputActionPhase.Started && curInteractable != null)  // ������ �Ա�(E Ű�� ������ �� curInteractable�� null�� �ƴ϶��)
        {
            curInteractable.OnInteract();  // �����ۿ� ���� ��ȣ�ۿ��� ���� ��
            curInteractGameObject = null;  // ����� �������� �ʱ�ȭ
            curInteractable = null;
            promptText.gameObject.SetActive(false);
        }
    }
}
