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
        camera = Camera.main; // main 태그를 가지고있는 카메라 하나만 자동적으로 잡히는 것 - 싱글톤과 같은 느낌
    }

    // Update is called once per frame
    void Update()
    {
        // 타임 체크
        if(Time.time - lastCheckTime > checkRate)
        {
            lastCheckTime = Time.time;

            Ray ray = camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));  // 화면의 정중앙에서 레이를 쏘겠다는 뜻
            RaycastHit hit; // Raycast를 쓰기 위해선 무조건 Ray가 있어야됨 - 레이를 쏘고 충돌이 일어나는 정보를 받아오는 기능이기 때문이다.

            if (Physics.Raycast(ray, out hit, maxCheckDistance, layerMask)) // out은 무조건 값을 채워서 돌려줌 - hit이 null이거나 어떠한 데이터가 채워져있을 것이라 예상 가능
            {
                if(hit.collider.gameObject != curInteractGameObject)  // 레이에 맞은 오브젝트가 저장했던 오브젝트와 다르다면 오브젝트를 저장함
                {
                    curInteractGameObject = hit.collider.gameObject;
                    curInteractable = hit.collider.GetComponent<IInteractable>();
                    SetPromptText();
                }
            }  // 여기까지 바라본 오브젝트가 있으면 그 오브젝트가 화면 정중앙에 위치하는 것을 인지, PromptText를 띄우는 작업이다.
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
        promptText.text = string.Format("<b>[E]</b> {0}", curInteractable.GetInteractPrompt()); // ""안에 있는 문자는 마크다운 문법을 사용함.
    }

    public void OnInteractInput(InputAction.CallbackContext callbackContext)
    {
        if(callbackContext.phase == InputActionPhase.Started && curInteractable != null)  // 아이템 먹기(E 키를 눌렀을 때 curInteractable이 null이 아니라면)
        {
            curInteractable.OnInteract();  // 아이템에 대한 상호작용을 진행 후
            curInteractGameObject = null;  // 저장된 정보들을 초기화
            curInteractable = null;
            promptText.gameObject.SetActive(false);
        }
    }
}
