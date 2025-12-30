using System.Collections.Generic;
using UnityEngine;

public class XPManager : MonoBehaviour
{
    public static XPManager instance;
    public List<XPObject> xpObjects;

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

    public void AddPoints(int index, float value)
    {
        if (index < 0 || index >= xpObjects.Count)
        {
            Debug.LogError("Index out of range for XPObjects list.");
            return;
        }

        XPObject xpObject = xpObjects[index];
        xpObject.currentState += value;
    }

    public float GetPoints(int index)
    {
        if (index < 0 || index >= xpObjects.Count)
        {
            Debug.LogError("Index out of range for XPObjects list.");
            return 0;
        }

        float points = xpObjects[index].xpCurve.Evaluate(xpObjects[index].currentState / 100f) * (xpObjects[index].maxValue - xpObjects[index].minValue);

        points = Mathf.Clamp(points, xpObjects[index].minValue, xpObjects[index].maxValue);
        return points;
    }
}
