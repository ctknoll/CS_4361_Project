using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof (MapGenerator))]
public class MapEditor : Editor {
	public override void OnInspectorGUI()
	{
        MapGenerator map = target as MapGenerator;
        if (DrawDefaultInspector())
        {
            map.CreateMap();
        }

        if (GUILayout.Button("Generate Map"))
        {
            map.CreateMap();
        }
    }
}