using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(upgradeStats))]
public class resetButtonEditor2 : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var script = (upgradeStats)target;

        if (GUILayout.Button("upgrade", GUILayout.Height(20)))
        {
            script.DoUpgrade();
        }

    }
}
