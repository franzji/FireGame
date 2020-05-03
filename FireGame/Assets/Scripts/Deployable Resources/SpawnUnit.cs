using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnUnit : MonoBehaviour {

    public GlobalLevelData LvlData;
    [SerializeField]
    private GameObject terrain;
    [SerializeField]
    private TerrainBounds terrainBounds;
    private GameObject AOE;

    [SerializeField]
    private GameObject[] SpawnedObject;

    public Transform unitDisplayHolderTransform;
    private UnitSpawnController[] unitDisplays = new UnitSpawnController[4];


    private void Start()
    {
        terrain = GameObject.Find("Terrain");
        terrainBounds = GameObject.Find("Terrain Tools").GetComponent<TerrainBounds>();
    }

    public void EnumerateUnitDisplays()
    {
        foreach (Transform unitDisplay in unitDisplayHolderTransform)
        {
            UnitSpawnController unitSpawnController = unitDisplay.gameObject.GetComponent<UnitSpawnController>();
            UnitData unitData = unitSpawnController.unitData;
            unitDisplays[unitData.index] = unitSpawnController;
        }
    }

    // Update is called once per frame
    void Update () {

        //ACT of spawning the calldown action
        if (Input.GetMouseButtonDown(0) && LvlData.PlayerState == State.Selected)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (terrain.GetComponent<Collider>().Raycast(ray, out hit, Mathf.Infinity))
            {
                GameObject prefabToSpawn = SpawnedObject[LvlData.SpawnUnitType];
                GameUnit gameUnit = prefabToSpawn.GetComponent<GameUnit>();

                if (!(gameUnit is GroundCrew) || terrainBounds.pointInSpawnArea(hit.point))
                {
                    //delete current ghost
                    GetComponent<GhostScript>().RemoveGhost();

                    unitDisplays[LvlData.SpawnUnitType].decrementUnits();

                    LvlData.PlayerState = State.BeforeSelected;
                    Instantiate(prefabToSpawn, hit.point, Quaternion.Euler(0, 0, 0), GameObject.Find("Units").transform);
                }
            }
        }
    }
}