using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GroundCrewLumberJack : GroundCrew
{
    protected override bool isCandidateTree(Tree tree)
    {
        return !tree.isSmoking && !tree.isTargeted && !tree.isLevelCritical;
    }

    protected override void handleTree()
    {
        //kill tree :)
        closestTree.isDead = true;
    }
}