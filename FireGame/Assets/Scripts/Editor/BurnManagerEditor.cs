using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BurnManager))]
public class BurnManagerEditor : Editor {
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        BurnManager burnManager = (BurnManager)target;

        if (GUILayout.Button("View affecting trees"))
            burnManager.drawAffectingTreeDebugRays();
    }
}
