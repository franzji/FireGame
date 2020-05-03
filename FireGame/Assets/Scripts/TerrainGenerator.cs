using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    private Terrain terrain;

    public enum sizes
    {
        small = 256,
        medium = 512,
        large = 1024,
        xLarge = 2048
    }
    [SerializeField]
    private sizes size = sizes.medium;
    public sizes getSize() { return size; }

    [SerializeField]
    private int heightMapResolution = 513;
    [SerializeField]
    private float baseAmplitude = 10f;
    [SerializeField]
    private float baseScaleFactor = 1f;
    [SerializeField]
    [Range(1, 10)]
    private int octaves = 2;
    [SerializeField]
    [Range(.1f, .9f)]
    private float octaveAmplitudeFactor = 2;
    [SerializeField]
    [Range(1f, 5f)]
    private float octaveScaleFactor = 2;
    [SerializeField]
    [Range(0f, 1f)]
    private float highFactor = .5f;
    [SerializeField]
    [Range(0f, 1f)]
    private float lowFactor = .3f;
    [SerializeField]
    [Range(0f, 5f)]
    private float normalFactor = .3f;

    public float seed;

    public void generateTerrain()
    {
        terrain = GameObject.Find("Terrain").GetComponent<Terrain>();

        TerrainData terrainData = terrain.terrainData;

        terrainData.heightmapResolution = heightMapResolution;
        terrainData.size = new Vector3(2048, baseAmplitude, 2048);

        //Set the height map
        float[,] heightMap = TerrainGeneratorUtils.generateTerrainDataHeightmap(
            heightMapResolution, baseScaleFactor, octaves, octaveAmplitudeFactor,
            octaveScaleFactor, seed);
        terrainData.SetHeights(0, 0, heightMap);

        //Set the alpha map
        float[,,] alphaMap = TerrainGeneratorUtils.generateTerrainDataAlphaMap(
            terrainData, highFactor, lowFactor, normalFactor);
        terrainData.SetAlphamaps(0, 0, alphaMap);

        setBorderLines();
    }

    private void setBorderLines()
    {
        LineRenderer lineRenderer = GetComponent<LineRenderer>();

        if (lineRenderer)
        {
            lineRenderer.positionCount = 4;

            TerrainGenerator terrainGenerator = GetComponents<TerrainGenerator>()[0];
            Vector3[,] boundsArray = TerrainBounds.playerBounds(terrain);

            lineRenderer.SetPosition(0, boundsArray[0, 0]);
            lineRenderer.SetPosition(1, boundsArray[1, 0]);
            lineRenderer.SetPosition(2, boundsArray[1, 1]);
            lineRenderer.SetPosition(3, boundsArray[0, 1]);
        }
    }
}
