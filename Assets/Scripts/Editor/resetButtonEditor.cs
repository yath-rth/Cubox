using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Stats))]
public class resetButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var script = (Stats)target;

        if (GUILayout.Button("reset upgrades", GUILayout.Height(20)))
        {
            script.resetUpgrades();
        }

    }
}
