using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager uiManagerInstance;
    public bool TestMode = false;

    Player player;
    Points points;

    public GameObject allUI;
    public GameObject reloadingUI;
    public GameObject DeathMenu;
    public TextMeshProUGUI highScoreUI;

    public TextMeshProUGUI[] pointsUI = new TextMeshProUGUI[2];

    private void Start()
    {
        player = Player.playerInstance;
        points = Points.instance;

        //if (!TestMode) Time.timeScale = 0;
    }

    private void Awake()
    {
        if (uiManagerInstance != null && uiManagerInstance != this)
        {
            Destroy(this);
        }
        else
        {
            uiManagerInstance = this;
        }
    }

    private void OnDestroy()
    {
        if (uiManagerInstance == this)
        {
            uiManagerInstance = null;
        }
    }

    void Update()
    {
        for (int i = 0; i < pointsUI.Length; i++)
        {
            pointsUI[i].text = points.getPoints().ToString();
        }

        if (player.gun.isReloading)
        {
            reloadingUI.SetActive(true);
        }
        else
        {
            reloadingUI.SetActive(false);
        }

        if (!player.Alive)
        {
            highScoreUI.text = Save.playerData.HighScore.ToString();
            allUI.SetActive(false);
            DeathMenu.SetActive(true);
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1;
    }

    public void ButtonOpen(GameObject ui)
    {
        ui.SetActive(true);
    }

    public void ButtonClose(GameObject ui)
    {
        ui.SetActive(false);
    }

    public void Pause()
    {
        Time.timeScale = 0;
    }

    public void Resume()
    {
        Time.timeScale = 1;
    }

    public void debug()
    {
        Debug.Log("debug");
    }

    public void Exit()
    {
        Application.Quit();
    }
}
