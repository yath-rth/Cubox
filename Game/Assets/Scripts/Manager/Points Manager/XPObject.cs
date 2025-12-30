using UnityEngine;


[CreateAssetMenu(fileName = "XPObject", menuName = "XPObject", order = 0)]
public class XPObject : ScriptableObject
{
    public int maxValue = 100;
    public int minValue = 0;
    public AnimationCurve xpCurve;
    public AnimationCurve levelCurve;
    public int level = 1;
    public float currentState = 0f;
}
