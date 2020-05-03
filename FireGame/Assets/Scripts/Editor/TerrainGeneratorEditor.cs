using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TerrainGenerator))]
public class TerrainGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        TerrainGenerator terrainGenerator = (TerrainGenerator)target;

        if (GUILayout.Button("Generate terrain"))
        {
            terrainGenerator.generateTerrain();
        }
    }
}
