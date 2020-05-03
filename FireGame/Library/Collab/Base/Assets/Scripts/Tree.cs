using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public partial class Tree : MonoBehaviour {
    [Header("Properties")]
    [SerializeField]
    private float affectingRadius;

    private List<Tree> affectingTrees = new List<Tree>();

    [SerializeField]
    private float _dryness;
    public float dryness
    {
        get { return _dryness; }
        set { _dryness = value; }
    }

    [SerializeField]
    private bool _isFireStarter;
    public bool isfireStarter
    {
        get { return _isFireStarter; }
        private set { _isFireStarter = value; }
    }

    [Header("Temperatures")]
    [SerializeField]
    private float _temperature;
    private float temperature
    {
        get { return _temperature; }
        set
        {
            _temperature = Mathf.Min(maxTemperature, value);
            _temperature = Mathf.Max(minTemperature, value);

            if (_temperature > ignitionTemperature)
                ignitionParticles.Play();
            else ignitionParticles.Stop();

            if (_temperature > smokeTemperature)
                smokeParticles.Play();
            else smokeParticles.Stop();
        }
    }

    [SerializeField]
    private float _minTemperature = 50f;
    public float minTemperature { get { return _minTemperature; } }
    [SerializeField]
    private float _smokeTemperature = 450f;
    public float smokeTemperature { get { return _smokeTemperature; } }
    [SerializeField]
    private float _ignitionTemperature = 572f;
    public float ignitionTemperature { get { return _ignitionTemperature; } }
    [SerializeField]
    private float _maxTemperature = 1200f;
    public float maxTemperature { get { return _maxTemperature; } }

    [Header("Particles")]
    [SerializeField]
    private ParticleSystem smokeParticles;
    [SerializeField]
    private ParticleSystem ignitionParticles;
    private Vector3 affectingPosition;

    private void Start()
    {
        loopAffectingTrees(addAffectingTree);

        if (isfireStarter)
            temperature = ignitionTemperature;
    }

    /// <summary>
    /// Loops through all affecting Trees to perform an action on those trees.
    /// </summary>
    public void loopAffectingTrees(singleTreeDelegate singleTreeDelegate)
    {
        //Looks for other trees to affect.
        affectingPosition = transform.position + BurnManager.windVector * affectingRadius / 2;
        overlapSphereTrees(affectingPosition, affectingRadius);
        //Get all trees in the affecting radius.
        List<Tree> trees = overlapSphereTrees(affectingPosition, affectingRadius);
        foreach (Tree tree in trees)
            singleTreeDelegate(tree);
    }

    public delegate void singleTreeDelegate(Tree tree);
    public void drawAffectingTreeDebugRay(Tree tree)
    {
        Vector3 offest = Vector3.up * 4f;
        Debug.DrawLine(transform.position + offest, tree.transform.position + offest, Color.blue, 2f);
    }
    private void addAffectingTree(Tree tree)
    {
        //Add this Tree to the other Tree's affectingTrees List.
        tree.affectingTrees.Add(this);
    }

    private void OnValidate()
    {
        dryness = Mathf.Max(dryness, 0);
    }

    public void warmUp(AnimationCurve burnCurve, float deltaTime)
    {
        //Warms up 'this' Tree by looking at all other affecting trees
        foreach (Tree tree in affectingTrees)
        {
            if (tree.temperature > tree.smokeTemperature)
            {
                //Evaluate position on curve
                float burnCurveVal = burnCurve.Evaluate(
                    temperature / maxTemperature
                    );
                //Evaluate distance between 'this' Tree and affecting tree
                float distance = Vector3.Distance(transform.position , tree.affectingPosition);
                float distanceWeight = (tree.affectingRadius - distance) / tree.affectingRadius;
                //Increase the temperature of 'this' Tree
                temperature += dryness * burnCurveVal * distanceWeight * deltaTime;
            }
        }
    }

    public void cooldown(float amountOfWater)
    {
        temperature -= amountOfWater;
    }

    public static List<Tree> overlapSphereTrees(Vector3 position, float radius)
    {
        List<Tree> trees = new List<Tree>();

        Collider[] array = Physics.OverlapSphere(position, radius);

        for (int i = 0; i < array.Length; i++)
        {
            Tree tree = array[i].GetComponent<Tree>();
            //If there is a tree compoment
            if (tree)
                trees.Add(tree);
        }

        return trees;
    }
}