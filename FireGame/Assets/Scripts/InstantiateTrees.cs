using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class InstantiateTrees : MonoBehaviour
{
    [SerializeField]
    private GameObject[] treePrefabs;
    public GameObject getTreePrefab(int index) { return treePrefabs[index]; }

    public Terrain terrain { get; private set; }
    public TerrainData terrainData { get; private set; }
    private TreeInstance[] treeInstances;

    public void init()
    {
        if (terrain == null)
            terrain = GameObject.Find("Terrain").GetComponent<Terrain>();
        if (terrainData == null)
            terrainData = terrain.terrainData;
    }

    public void gameObjectsToTreeInstances()
    {
        init();

        GameObject treeParent = GameObject.Find("Trees");

        int childCount = treeParent.transform.childCount;
        for (int index = childCount - 1; index >= 0; index--)
        {
            GameObject currentTree = treeParent.transform.GetChild(index).gameObject;
            //If it's not a home
            if (!currentTree.name.Contains("Home"))
            {
                addTreeInstanceToTerrain(currentTree);
                DestroyImmediate(currentTree);
            }
        }
    }

    private void addTreeInstanceToTerrain(GameObject tree)
    {
        TreeInstance treeInstance = new TreeInstance();
        treeInstance.prototypeIndex = tree.GetComponent<Tree>().prototypeIndex;
        treeInstance.color = Color.white;
        treeInstance.lightmapColor = Color.white;
        treeInstance.heightScale = 1;
        treeInstance.widthScale = 1;

        Vector3 difference = tree.transform.position - terrain.transform.position;
        Vector3 size = terrainData.size;
        size.x = 1 / size.x;
        size.y = 1 / size.y;
        size.z = 1 / size.z;
        treeInstance.position = Vector3.Scale(difference, size);

        terrain.AddTreeInstance(treeInstance);
        terrain.Flush();
    }
}
