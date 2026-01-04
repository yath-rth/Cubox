using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(1)]
public class GameColorManager : MonoBehaviour
{
    Player player;

    public Color[] colors;
    [Range(0, 1)] public float playerTint, SkyboxTint, EnemyTint;
    public Material playerColor;
    public Material[] enemys;

    Color[] mapColors = new Color[2];
    Color colorPlayer;

    private void Awake()
    {
        player = Player.playerInstance;
    }

    public Color[] getColor()
    {
        int colorIndex = Random.Range(0, colors.Length);
        mapColors[0] = colors[colorIndex];

        int tmp = Random.Range(0, colors.Length);
        while (tmp == colorIndex)
        {
            tmp = Random.Range(0, colors.Length);
        }
        mapColors[1] = colors[tmp];

        //setColor();

        return mapColors;
    }

    void setColor()
    {
        colorPlayer = new Color(mapColors[1].r - playerTint, mapColors[1].g - playerTint, mapColors[1].b - playerTint, mapColors[1].a);
        playerColor.SetColor("_color", colorPlayer);

        if (player != null) player.setPlayerColor(colorPlayer);
    }
}
