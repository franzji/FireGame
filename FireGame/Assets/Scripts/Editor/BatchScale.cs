using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BatchScale : EditorWindow {

    private string folderPath = "Assets";
    private float scale;

    [MenuItem("Window/Batch Scale")]
    public static void ShowWindow()
    {
        GetWindow<BatchScale>();
    }

    void OnGUI()
    {
        GUILayout.Label("Scales all models to a desired scale in a given folder.",
            EditorStyles.largeLabel);

        folderPath = EditorGUILayout.TextField("Path", folderPath);
        if (GUILayout.Button("Browse..."))
        {
            folderPath = EditorUtility.OpenFolderPanel("Select folder", "", "");

            //Convert absolute to relative path.
            folderPath = "Assets" + folderPath.Substring(Application.dataPath.Length);
        }

        scale = EditorGUILayout.FloatField("Scale", scale);

        if (GUILayout.Button("Scale all (takes long time)"))
        {
            //Enumerate through all objects and scale
            string[] assets = AssetDatabase.FindAssets("t:model", new string[] { folderPath });
            Debug.Log("num assets = " + assets.Length);
            scaleModels(assets, scale);
        }
    }

    private static void scaleModels(string[] assets, float scale)
    {
        for(int index = 0; index < assets.Length; index++)
        {
            string path = AssetDatabase.GUIDToAssetPath(assets[index]);
            AssetPostprocessor assetPostprocessor = new AssetPostprocessor();
            assetPostprocessor.assetPath = path;
            if (assetPostprocessor.assetImporter != null)
            {
                ModelImporter modelImporter = assetPostprocessor.assetImporter as ModelImporter;
                modelImporter.useFileScale = false;
                //Scale that will be seen in the world when placed.
                modelImporter.globalScale = scale;
                //Takes long time...
                modelImporter.SaveAndReimport();
            }

        }
    }

    private static void scaleObjects(Object[] assets, float scaleFactor)
    {
        for(int index = 0; index < assets.Length; index++)
        {
            Debug.Log(assets[index].name);
        }
    }
}
