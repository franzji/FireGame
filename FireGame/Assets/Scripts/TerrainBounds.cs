using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TerrainBounds : MonoBehaviour
{
    [SerializeField]
    private Rect[] rects;
    public Rect[] getRects() { return rects; }
    [SerializeField]
    private int numRenderersPerRect = 3;
    [SerializeField]
    private GameObject spawnAreaPrefab;

    [HideInInspector]
    public Terrain terrain;
    [HideInInspector]
    public List<Vector3[,]> globalBoundsList;

    private void Start()
    {
        destroyAndInstantiateSpawnAreas();

        globalBoundsList = new List<Vector3[,]>();
        foreach (Rect rect in getRects())
        {
            Vector3[,] globalBounds = globalBoundsFromRect(rect);
            globalBoundsList.Add(globalBounds);
        }
    }

    public void destroyAndInstantiateSpawnAreas()
    {
        terrain = GameObject.Find("Terrain").GetComponent<Terrain>();

        GameObject spawnAreasParent = GameObject.Find("Spawn Areas");
        //Destroy all spawn areas
        while (spawnAreasParent.transform.childCount > 0)
            DestroyImmediate(spawnAreasParent.transform.GetChild(0).gameObject);

        //Instantiate all spawn areas
        foreach (Rect rect in getRects())
        {
            Vector3[,] globalBounds = globalBoundsFromRect(rect);
            instantiateSpawnAreas(globalBounds);
        }
    }

    public void instantiateSpawnAreas(Vector3[,] globalBounds)
    {
        float distanceBetweenRenderers = globalBounds[0, 0].y / numRenderersPerRect;

        for (int i = 0; i < numRenderersPerRect; i++)
        {
            //Modify global bounds on the y to be able to draw multiple sets of line renderers.
            for (int x = 0; x < globalBounds.GetLength(0); x++)
                for (int z = 0; z < globalBounds.GetLength(1); z++)
                    globalBounds[x, z].y -= distanceBetweenRenderers;

            instantiateSpawnArea(globalBounds);
        }
    }

    private void instantiateSpawnArea(Vector3[,] globalBounds)
    {
        GameObject gameObject = Instantiate(spawnAreaPrefab, transform.position, transform.rotation);
        GameObject spawnAreasParent = GameObject.Find("Spawn Areas");
        gameObject.transform.SetParent(spawnAreasParent.transform);

        LineRenderer lineRenderer = gameObject.GetComponent<LineRenderer>();
        lineRenderer.SetPosition(0, globalBounds[0, 0]);
        lineRenderer.SetPosition(1, globalBounds[1, 0]);
        lineRenderer.SetPosition(2, globalBounds[1, 1]);
        lineRenderer.SetPosition(3, globalBounds[0, 1]);
    }

    public Vector3[,] globalBoundsFromRect(Rect rect)
    {
        Vector3[,] boundsArray = new Vector3[2, 2];

        //[0, 0] in bounds array is lower-left.
        Vector3 boundsCorner = playerBounds(terrain)[0, 0];

        int terrainSize = TerrainBounds.getTerrainSize();

        boundsArray[0, 0] = boundsCorner + new Vector3(rect.position.x, 0, rect.position.y) * terrainSize;
        boundsArray[1, 0] = boundsCorner + new Vector3(rect.position.x + rect.size.x, 0, rect.position.y) * terrainSize;
        boundsArray[1, 1] = boundsCorner + new Vector3(rect.position.x + rect.size.x, 0, rect.position.y + rect.size.y) * terrainSize;
        boundsArray[0, 1] = boundsCorner + new Vector3(rect.position.x, 0, rect.position.y + rect.size.y) * terrainSize;

        return boundsArray;
    }

    //Calculates bounds from a terrain
    public static Vector3[,] playerBounds(Terrain terrain)
    {
        Vector3[,] boundsArray = new Vector3[2, 2];
        int terrainSize = TerrainBounds.getTerrainSize();

        TerrainData terrainData = terrain.terrainData;
        Vector3 position = terrain.transform.position;
        float terrainHeight = terrainData.size.y;

        float offset = (int)terrainSize / 2;
        float center = terrainData.size.x / 2;

        //Bottom left
        boundsArray[0, 0] = new Vector3(center + position.x - offset, position.y + terrainHeight, center + position.z - offset);
        //Bottom right
        boundsArray[1, 0] = new Vector3(center + position.x + offset, position.y + terrainHeight, center + position.z - offset);
        //Top left
        boundsArray[0, 1] = new Vector3(center + position.x - offset, position.y + terrainHeight, center + position.z + offset);
        //Top right
        boundsArray[1, 1] = new Vector3(center + position.x + offset, position.y + terrainHeight, center + position.z + offset);

        return boundsArray;
    }

    private static int getTerrainSize()
    {
        TerrainGenerator terrainGenerator = GameObject.Find("Terrain Tools").GetComponent<TerrainGenerator>();
        return (int)terrainGenerator.getSize();
    }

    //Determines whether a given position is in a spawn area.
    public bool pointInSpawnArea(Vector3 position)
    {
        int terrainSize = getTerrainSize();

        foreach (Vector3[,] globalBounds in globalBoundsList)
            //x is in bounds
            if (position.x >= globalBounds[0, 0].x && position.x <= globalBounds[1, 1].x)
                //z is in bounds
                if (position.z >= globalBounds[0, 0].z && position.z <= globalBounds[1, 1].z)
                    return true;

        return false;
    }
}
