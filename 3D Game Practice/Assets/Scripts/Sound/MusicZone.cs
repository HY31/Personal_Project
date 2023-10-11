using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicZone : MonoBehaviour
{
    public AudioSource audioSource;
    public float fadeTime;
    public float maxVolume;
    private float targetVolume;

    private void Start()
    {
        targetVolume = 0f;
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = targetVolume;
        audioSource.Play();
    }

    private void Update()
    {
        // 오디오 볼륨을 서서히 변화시켜 오디오가 서서히 사라지거나 나타나도록 하는 데 사용되는 코드

        if (!Mathf.Approximately(audioSource.volume, targetVolume)) // 현재 오디오 볼륨과 목표 볼륨이 서로 근접한지 확인하고 만약 오디오 볼륨과 목표 볼륨이 거의 같다면 아래의 코드 블록이 실행된다.
        {
            audioSource.volume = Mathf.MoveTowards(audioSource.volume, targetVolume, (maxVolume / fadeTime) * Time.deltaTime);
            // Mathf.MoveTowards() 함수를 사용하여 목표 볼륨인 targetVolume으로 서서히 변경
            // (maxVolume / fadeTime) * Time.deltaTime)은 fadeTime에서 설정한 시간 내에 최대 볼륨으로 변화할 수 있도록 계산된 속도이다.
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            targetVolume = maxVolume;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            targetVolume = 0f;
    }
}
