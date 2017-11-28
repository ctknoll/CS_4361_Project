using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIUtils : MonoBehaviour
{
    CanvasGroup pauseMenu = null;
    public static bool isPaused;
    public bool isGameOver = false;
    public static bool playerDead = false;
    float timer = 0;

    public static readonly string[] notInGame = {"Main Screen"};

    void Start()
    {
        if (inGame())
        {
            pauseMenu = GameObject.Find("PlayerUI").GetComponentInChildren<CanvasGroup>();
            isPaused = false;
            SetPaused(isPaused);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playerDead)
        {
            if(!isGameOver)
                EndGame();
            return;
        }
        if (!inGame())
            Cursor.lockState = CursorLockMode.None;
        if (Input.GetButtonDown("Cancel") && inGame())
        {
            SetPaused(!isPaused);
        }
    }

    public void EndGame()
    {
        timer += Time.deltaTime;
        if(timer >= 1)
            if (isPaused != true)
            {
                SetPaused(true);
                isGameOver = true;
            }
    }

    public bool inGame()
    {
        int pos = Array.IndexOf(UIUtils.notInGame, SceneManager.GetActiveScene().name);
        if (pos < 0)
            return true;
        return false;
    }

    public void Restart()
    {
        if (!playerDead)
            SetPaused(false);
        playerDead = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadScene(string sceneName)
    {
        if(!playerDead)
            SetPaused(false);
        playerDead = false;
        SceneManager.LoadScene(sceneName);
    }

    public void ExitGame()
    {
        if (UnityEditor.EditorApplication.isPlaying)
            UnityEditor.EditorApplication.isPlaying = false;
        else
            Application.Quit();
    }

    public void SetPaused(bool pause)
    {
        if (pauseMenu)
        {
            isPaused = pause;
            pauseMenu.interactable = isPaused;
            if (isPaused)
                Time.timeScale = 0F;
            else
                Time.timeScale = 1F;

            foreach (Transform g in pauseMenu.GetComponentInChildren<Transform>())
            {
                g.gameObject.SetActive(isPaused);
            }
            if (playerDead)
            {
                pauseMenu.GetComponentInChildren<Text>().text = "Game Over";
                RectTransform rectPos = pauseMenu.GetComponentInChildren<Text>().GetComponent<RectTransform>();
                rectPos.localPosition = new Vector3(20, rectPos.localPosition.y, rectPos.localPosition.z);
            }
        }
    }
}
