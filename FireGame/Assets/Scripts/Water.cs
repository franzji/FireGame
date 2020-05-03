using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour {
    [SerializeField]
    private float affectingRadius;
    [SerializeField]
    private float amountOfWater;

    private void OnTriggerEnter(Collider collider)
    {
        //When the Water collides with the terrain
        if (collider.gameObject.name == "Terrain")
        {
            //Cool down other trees
            cooldownTrees();
            //Destroy the Water
            Destroy(gameObject);
        }
    }

    private void cooldownTrees()
    {
        List<Tree> trees = Tree.overlapSphereTrees(transform.position, affectingRadius);

        foreach (Tree tree in trees)
            tree.cooldown(amountOfWater);
    }
}
