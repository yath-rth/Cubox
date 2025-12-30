using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Points : MonoBehaviour
{
    public static Points instance;
    public static event Action pointsAdded;

    int points = 0;
    int highscore = 0;

    [SerializeField]int pointsMultiplier = 1;
    
    int EnemyWave = 1;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        Enemy.OnDeath += AddPoints;
    }

    void AddPoints(int point, bool a)
    {
        switch (point)
        {
            case 1:
                points += 10 * pointsMultiplier;
                break;

            case 2:
                points += 12 * pointsMultiplier;
                break;

            case 3:
                points += 30 * pointsMultiplier;
                break;
        }

        if(pointsAdded != null) pointsAdded();
    }

    public int getWave()
    {
        return EnemyWave;
    }

    public void setWave(int wave)
    {
        EnemyWave += wave;
    }

    public int getPoints(){
        return points;
    }

    public int getHighscore(){
        return highscore;
    }

    public void setHighscore(int high){
        highscore = high;
    }
}
