using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIUtils : MonoBehaviour
{
    CanvasGroup pauseMenu = null;
    private static bool isPaused;

    public static readonly string[] notInGame = {"Main Screen"};

    void Start()
    {
        if (inGame())
        {
            pauseMenu = GameObject.Find("UI").GetComponentInChildren<CanvasGroup>();
            isPaused = false;
            SetPaused(isPaused);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!inGame())
            Cursor.lockState = CursorLockMode.None;
        if (Input.GetButtonDown("Escape") && inGame())
        {
            SetPaused(!isPaused);
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
        SetPaused(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
        }
    }
}
