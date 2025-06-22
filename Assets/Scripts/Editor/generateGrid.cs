using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (grid))]
public class generateGrid : Editor
{
   public override void OnInspectorGUI ()
	{

		grid map = target as grid;

		if (DrawDefaultInspector ()) {
			map.GenerateGrid ();
		}

		if (GUILayout.Button("Generate Grid")) {
			map.GenerateGrid ();
		}


	}
}
