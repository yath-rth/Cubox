using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(gunObject))]
public class resetButtonEditor1 : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var script = (gunObject)target;

        if (GUILayout.Button("reset upgrades", GUILayout.Height(20)))
        {
            script.resetUpgrades();
        }

    }
}
