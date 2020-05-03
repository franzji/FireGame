using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostScript : MonoBehaviour {

    public GlobalLevelData LvlData;
    [SerializeField]
    private GameObject terrain;
    [SerializeField]
    private TerrainBounds terrainBounds;
    [SerializeField]
    private Material ghostMaterial;
    [SerializeField]
    private Material ghostNotAllowedMaterial;
    [SerializeField]
    private GameObject[] spawnedObjectGhost;
    private GameObject _currentGhost;
    private GameObject currentGhost
    {
        get
        {
            if (_currentGhost == null)
                _currentGhost = Instantiate(spawnedObjectGhost[LvlData.SpawnUnitType]);
            return _currentGhost;
        }
        set
        {
            if (value == null && _currentGhost != null)
            {
                Destroy(_currentGhost);
                _currentGhost = null;
            }
        }
    }

    // Use this for initialization
    void Start () {
        currentGhost = null;
        terrain = GameObject.Find("Terrain");
    }
	
	// Update is called once per frame
	void Update () {
        //CREATING ghost
        if (LvlData.PlayerState == State.Selected)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (terrain.GetComponent<Collider>().Raycast(ray, out hit, Mathf.Infinity))
            {
                //spawn ghost and set its transform position to the hit point.
                currentGhost.transform.position = hit.point;
                Ghost ghostProps = currentGhost.GetComponent<Ghost>();
                if (!ghostProps.placableOutsideSafeZone)
                {
                    if (terrainBounds.pointInSpawnArea(hit.point))
                    {
                        foreach (MeshRenderer renderer in ghostProps.renderers)
                        {
                            renderer.material = ghostMaterial;
                        }
                    }
                    else
                    {
                        foreach (MeshRenderer renderer in ghostProps.renderers)
                        {
                            renderer.material = ghostNotAllowedMaterial;
                        }
                    }
                }
            }
        }

        //ESC button if you want to cancel selection of a calldown
        if (Input.GetKeyDown(KeyCode.Escape) && LvlData.PlayerState == State.Selected)
        {
            //cancel the dropping.
            LvlData.PlayerState = State.BeforeSelected;
            //delete current ghost
            RemoveGhost();
        }
    }

    public void RemoveGhost()
    {
        currentGhost = null;
    }
}
