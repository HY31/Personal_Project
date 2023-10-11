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
        // ����� ������ ������ ��ȭ���� ������� ������ ������ų� ��Ÿ������ �ϴ� �� ���Ǵ� �ڵ�

        if (!Mathf.Approximately(audioSource.volume, targetVolume)) // ���� ����� ������ ��ǥ ������ ���� �������� Ȯ���ϰ� ���� ����� ������ ��ǥ ������ ���� ���ٸ� �Ʒ��� �ڵ� ����� ����ȴ�.
        {
            audioSource.volume = Mathf.MoveTowards(audioSource.volume, targetVolume, (maxVolume / fadeTime) * Time.deltaTime);
            // Mathf.MoveTowards() �Լ��� ����Ͽ� ��ǥ ������ targetVolume���� ������ ����
            // (maxVolume / fadeTime) * Time.deltaTime)�� fadeTime���� ������ �ð� ���� �ִ� �������� ��ȭ�� �� �ֵ��� ���� �ӵ��̴�.
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
