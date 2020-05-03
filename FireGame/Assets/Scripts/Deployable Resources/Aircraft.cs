using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aircraft : GameUnit
{
    [SerializeField]
    private GameObject deployPrefab;
    private Transform deployParentTransform;

    void Start()
    {
        spawnUnit = GameObject.Find("UI").GetComponent<AIManager>();
        deployParentTransform = GameObject.Find("Units").transform;

        Vector3 parentRotation = transform.parent.rotation.eulerAngles;
        transform.parent.rotation = Quaternion.Euler(parentRotation.x, Random.Range(0f, 360f), parentRotation.z);
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
