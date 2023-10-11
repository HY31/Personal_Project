using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footsteps : MonoBehaviour
{
    public AudioClip[] footstepClips;
    private AudioSource audioSource;
    private Rigidbody _rigidbody;
    public float footstepThreshold;  // 발자국 소리가 재생되기 위한 속도 임계값
    public float footstepRate;       // 발자국 소리의 재생 속도
    private float lastFootstepTime;  // 마지막으로 발자국 소리가 재생된 시간을 추적

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if(Mathf.Abs(_rigidbody.velocity.y) < 0.1f)  // 플레이어의 수직 속도(_rigidbody.velocity.y)가 0.1 미만이면(플레이어가 땅에 있다는 뜻) 발소리 재생
        {
            if(_rigidbody.velocity.magnitude > footstepThreshold) // 캐릭터의 현재 속도가 footstepThreshold보다 크다면 발자국 소리 재생
            {
                if (Time.time - lastFootstepTime > footstepRate)  // 현재 시간에서 이전 발자국 소리가 재생된 시간(lastFootstepTime)을 빼면 현재 시간과 이전 발자국 소리 재생 시간 사이의 시간 간격을 얻을 수 있다.
                                                                  // 이 간격은 마지막 발자국 소리 이후 얼마나 시간이 경과했는지를 나타내며, 이는 발자국 소리가 일정한 속도로 반복해서 재생되도록 제한하는 것이다.
                {
                    lastFootstepTime = Time.time;  // 현재 시간을 lastFootstepTime 변수에 저장하여 마지막 발자국 소리가 재생된 시간을 업데이트
                    audioSource.PlayOneShot(footstepClips[Random.Range(0, footstepClips.Length)]); // 발자국 소리는 footstepRate 간격으로 재생되지만, 항상 다른 발자국 소리가 선택된다.
                }
            }

        }
    }

}
