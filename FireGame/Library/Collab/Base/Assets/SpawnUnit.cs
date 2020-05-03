using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnUnit : MonoBehaviour {

    public LevelData LvlData;
    [SerializeField]
    private GameObject terrain;
    [SerializeField]
    private GameObject[] SpawnedObject;
    public CoolDown[] CoolDowns;

    private void Start()
    {
        terrain = GameObject.Find("Terrain");
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetMouseButtonDown(0) && LvlData.PlayerState == State.Selected)
        {
            //get cooldowns
            CoolDowns = GetComponentsInChildren<CoolDown>();

            //start cooldown
            CoolDowns[LvlData.UsedAction].StartCoolDown();


            LvlData.PlayerState = State.BeforeSelected;

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (terrain.GetComponent<Collider>().Raycast(ray, out hit, Mathf.Infinity))
            {
                Instantiate(SpawnedObject[LvlData.SpawnUnitType], hit.point, Quaternion.Euler(0, 0, 0), GameObject.Find("Units").transform);
            }
        }
    }
}