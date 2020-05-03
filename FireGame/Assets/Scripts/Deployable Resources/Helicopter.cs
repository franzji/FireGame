using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helicopter : GameUnit {
    [SerializeField]
    private GameObject deployPrefab;

    private Transform deployParentTransform;
    void Start()
    {
        spawnUnit = GameObject.Find("UI").GetComponent<AIManager>();
        deployParentTransform = GameObject.Find("Units").transform;
    }

    public void deployPrefabAtPosition()
    {
        Instantiate(deployPrefab, transform.position, transform.rotation, deployParentTransform);
    }

    public void animationComplete()
    {
        Destroy(transform.parent.gameObject);
    }
}
