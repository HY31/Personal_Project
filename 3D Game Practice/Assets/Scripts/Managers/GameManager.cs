using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public void GameStart()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadSceneAsync("MainScene").completed += OnSceneLoaded;
    }

    private void OnSceneLoaded(AsyncOperation asyncOperation)
    {
        // Time.timeScale�� 1�� ����
        Time.timeScale = 1.0f;
    }
}
