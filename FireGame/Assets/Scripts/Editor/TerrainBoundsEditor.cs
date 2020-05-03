using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TerrainBounds))]
public class TerrainBoundsEditor : Editor {
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        TerrainBounds terrainBounds = (TerrainBounds)target;

        if (GUILayout.Button("Generate Safe Zones"))
        {
            terrainBounds.destroyAndInstantiateSpawnAreas();
        }
    }
}
