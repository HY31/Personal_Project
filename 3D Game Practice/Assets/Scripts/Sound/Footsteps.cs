using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footsteps : MonoBehaviour
{
    public AudioClip[] footstepClips;
    private AudioSource audioSource;
    private Rigidbody _rigidbody;
    public float footstepThreshold;  // ���ڱ� �Ҹ��� ����Ǳ� ���� �ӵ� �Ӱ谪
    public float footstepRate;       // ���ڱ� �Ҹ��� ��� �ӵ�
    private float lastFootstepTime;  // ���������� ���ڱ� �Ҹ��� ����� �ð��� ����

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if(Mathf.Abs(_rigidbody.velocity.y) < 0.1f)  // �÷��̾��� ���� �ӵ�(_rigidbody.velocity.y)�� 0.1 �̸��̸�(�÷��̾ ���� �ִٴ� ��) �߼Ҹ� ���
        {
            if(_rigidbody.velocity.magnitude > footstepThreshold) // ĳ������ ���� �ӵ��� footstepThreshold���� ũ�ٸ� ���ڱ� �Ҹ� ���
            {
                if (Time.time - lastFootstepTime > footstepRate)  // ���� �ð����� ���� ���ڱ� �Ҹ��� ����� �ð�(lastFootstepTime)�� ���� ���� �ð��� ���� ���ڱ� �Ҹ� ��� �ð� ������ �ð� ������ ���� �� �ִ�.
                                                                  // �� ������ ������ ���ڱ� �Ҹ� ���� �󸶳� �ð��� ����ߴ����� ��Ÿ����, �̴� ���ڱ� �Ҹ��� ������ �ӵ��� �ݺ��ؼ� ����ǵ��� �����ϴ� ���̴�.
                {
                    lastFootstepTime = Time.time;  // ���� �ð��� lastFootstepTime ������ �����Ͽ� ������ ���ڱ� �Ҹ��� ����� �ð��� ������Ʈ
                    audioSource.PlayOneShot(footstepClips[Random.Range(0, footstepClips.Length)]); // ���ڱ� �Ҹ��� footstepRate �������� ���������, �׻� �ٸ� ���ڱ� �Ҹ��� ���õȴ�.
                }
            }

        }
    }

}
