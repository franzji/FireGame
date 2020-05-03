using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class UnitData : ScriptableObject
{
    public string unitName;
    public int unitCost;
    public int index;
    public float unitCooldown;
    public string buttonToSummon;
    public GameObject unitGhost;
    public GameObject unitObject;
}
