using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaminaBar : MonoBehaviour
{
    Dash dash;
    GameObject[] staminaBars;
    int count, gap, num;

    public Color Red;

    private void Start()
    {
        dash = Player.playerInstance.GetComponent<Dash>();
        staminaBars = new GameObject[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            staminaBars[i] = transform.GetChild(i).gameObject;
            staminaBars[i].SetActive(true);
        }

        num = staminaBars.Length;

        if (dash != null) gap = (int)dash.dashCoolDown / num;
    }

    private void LateUpdate()
    {
        if (dash != null)
        {
            if (gap != 0) count = num - Mathf.Clamp((int)System.Math.Ceiling((double)(dash.getCurrTime() - Time.time) / gap), 0, num);

            for (int i = 0; i < count; i++)
            {
                if(i < num) staminaBars[i].SetActive(true);
            } 

            for (int i = num - 1; i >= count; i--)
            {
                if(i < num && i > -1) staminaBars[i].SetActive(false);
            }
        }
    }
}
