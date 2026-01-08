using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    Player player;
    GameObject[] healthBars;
    int count, gap = 20, num;
    float colourPercent;

    public Color Red;

    private void Start()
    {
        healthBars = new GameObject[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            healthBars[i] = transform.GetChild(i).gameObject;
            healthBars[i].SetActive(true);
        }

        num = healthBars.Length;
    }

    public void UpdateHealthBar(int health)
    {
        count = num - (int)System.Math.Ceiling((double)health / gap);

        for (int i = 0; i < count; i++)
        {
            if (i < num) healthBars[i].SetActive(false);
        }

        for (int i = num - 1; i >= count; i--)
        {
            if (i < num && i > -1) healthBars[i].SetActive(true);
        }

        for (int i = 0; i < healthBars.Length; i++)
        {
            Renderer render = healthBars[i].GetComponent<Renderer>();
            Material obstacleMaterial = new Material(render.sharedMaterial);
            colourPercent = (5 - count) / num;
            obstacleMaterial.color = Color.Lerp(Red, Color.white, colourPercent);
            render.sharedMaterial = obstacleMaterial;
        }
    }
}
