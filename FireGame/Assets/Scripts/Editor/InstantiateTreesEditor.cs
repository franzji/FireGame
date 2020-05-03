using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(InstantiateTrees))]
public class InstantiateTreesEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        InstantiateTrees instantiateTrees = (InstantiateTrees)target;

        if (GUILayout.Button("TreeInstances to GameObjects"))
        {
            treeInstancesToGameObjects();
        }

        if (GUILayout.Button("GameObjects to TreeInstances"))
        {
            instantiateTrees.gameObjectsToTreeInstances();
        }
    }

    public void treeInstancesToGameObjects()
    {
        InstantiateTrees instantiateTrees = (InstantiateTrees)target;
        instantiateTrees.init();

        GameObject treeParent = GameObject.Find("Trees");

        foreach (TreeInstance treeInstance in instantiateTrees.terrainData.treeInstances)
        {
            int prototypeIndex = treeInstance.prototypeIndex;
            Vector3 treePosition = Vector3.Scale(treeInstance.position, instantiateTrees.terrainData.size) + instantiateTrees.terrain.transform.position;
            instantiateTreePrefabAtPosition(treePosition, treeParent, prototypeIndex);
        }

        instantiateTrees.terrainData.treeInstances = new List<TreeInstance>().ToArray();
    }

    private void instantiateTreePrefabAtPosition(Vector3 treePosition, GameObject treeParent, int prototypeIndex = 0)
    {
        InstantiateTrees instantiateTrees = (InstantiateTrees)target;

        GameObject currentTreePrefab = (GameObject)PrefabUtility.InstantiatePrefab(instantiateTrees.getTreePrefab(prototypeIndex));
        currentTreePrefab.GetComponent<Tree>().prototypeIndex = prototypeIndex;
        currentTreePrefab.transform.position = treePosition;
        currentTreePrefab.transform.rotation = Quaternion.identity;
        if (treeParent != null)
            currentTreePrefab.transform.parent = treeParent.transform;
    }
}
