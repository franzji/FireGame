using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum State { BeforeSelected, Selected, Dragging, UnitSelected };

[CreateAssetMenu]
public class GlobalLevelData : ScriptableObject
{
    //[HideInInspector]
    public float[] actionmoneys = new float[10];
    //[HideInInspector]
    public int[] actionchoices = new int[10];
    //[HideInInspector]
    public State PlayerState = State.BeforeSelected;
    //[HideInInspector]
    public int UsedAction = 0;
    //[HideInInspector]
    public int SpawnUnitType;
}