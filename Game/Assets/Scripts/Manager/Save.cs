using UnityEngine;
using System.Collections;

public class Save : MonoBehaviour
{
    Player player;
    Points points;

    public static PlayerData playerData;
    int highScore;

    private void FixedUpdate()
    {
        if (playerData != null)
        {
            if (points.getPoints() > playerData.HighScore)
            {
                highScore = points.getPoints();
            }

            if(!player.Alive){
                SavePlayer();
            }
        }
    }

    private void Start()
    {
        player = Player.playerInstance;
        points = Points.instance;

        playerData = SaveSystem.LoadPlayer();

        if (playerData != null)
        {
            points.setHighscore(playerData.HighScore);
            highScore = playerData.HighScore;
        }
    }

    public void SavePlayer()
    {
        if (points.getPoints() > playerData.HighScore)
        {
            playerData.HighScore = points.getPoints();
        }
        else
        {
            playerData.HighScore = highScore;
        }

        SaveSystem.SavePlayer(playerData);
    }
}