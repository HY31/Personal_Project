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
        // Time.timeScale을 1로 설정
        Time.timeScale = 1.0f;
    }
}
