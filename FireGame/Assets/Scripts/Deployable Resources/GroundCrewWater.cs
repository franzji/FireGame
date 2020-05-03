using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GroundCrewWater : GroundCrew
{
    public float waterPower = 570f;

    protected override bool isCandidateTree(Tree tree)
    {
        return tree.isSmoking;
    }

    protected override void handleTree()
    {
        closestTree.cooldown(waterPower);
    }
}