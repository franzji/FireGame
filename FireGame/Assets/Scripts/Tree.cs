using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEditor;
using UnityEngine;

public partial class Tree : MonoBehaviour {
    [Header("Properties")]
    [SerializeField]
    private float affectingRadius = 30f;

    private List<Tree> affectingTrees = new List<Tree>();

    [SerializeField]
    private float _dryness;
    public float dryness
    {
        get { return _dryness; }
        set { _dryness = value; }
    }

    [SerializeField]
    private float _burnTime = 30f;
    public float burnTime
    {
        get { return _burnTime; }
        private set { _burnTime = value; }
    }
    [SerializeField]
    private GameObject burntTreePrefab;

    private float timeStartedBurning;

    [SerializeField]
    private bool _isFireStarter;
    public bool isfireStarter
    {
        get { return _isFireStarter; }
        private set { _isFireStarter = value; }
    }

    [SerializeField]
    private bool _isLevelCritical = false;
    public bool isLevelCritical { get { return _isLevelCritical; } }

    [Header("Temperatures")]
    [SerializeField]
    private float _temperature;
    private float temperature
    {
        get { return _temperature; }
        set
        {
            bool isBurningBeforeSet = isOnFire;

            _temperature = Mathf.Min(maxTemperature, value);
            _temperature = Mathf.Max(minTemperature, value);

            if (isOnFire)
            {
                ignitionParticles.Play();
                //Only play sound on random trees rather than all trees so there is no sound "spamming"
                if (!audioSource.isPlaying && (!isBurningBeforeSet && Random.Range(1, 15) == 1))
                    audioSource.Play();
                //Case where tree started burning as a result of this set
                if (!isBurningBeforeSet)
                    timeStartedBurning = Time.time;
                //Case where tree has been burning longer than death time
                if (Time.time - timeStartedBurning > burnTime)
                    isDead = true;
            }
            else
            {
                ignitionParticles.Stop();
                audioSource.Stop();
            }

            if (_temperature >= smokeTemperature)
            {
                smokeParticles.Play();
            }
            else smokeParticles.Stop();
        }
    }
    public bool isCold { get { return temperature < 100f; } }
    public bool isSmoking { get { return temperature >= smokeTemperature; } }
    public bool isOnFire { get { return temperature >= ignitionTemperature; } }

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
    private GameObject smokeParticlesPrefab;
    private ParticleSystem smokeParticles;
    private ParticleSystem.EmissionModule smokeParticlesEmission;
    [SerializeField]
    private GameObject ignitionParticlesPrefab;
    private ParticleSystem ignitionParticles;
    private ParticleSystem.EmissionModule ignitionParticlesEmission;
    [SerializeField]
    private AnimationCurve particleEmissionRate;
    private Vector3 affectingPosition;

    public bool isDead;

    [Header("Gizmos")]
    [SerializeField]
    private bool drawGizmos;

    [HideInInspector]
    public int prototypeIndex;

    [HideInInspector]
    public int targetCount = 0;
    public bool isTargeted { get { return targetCount > 0; } }

    private AudioSource audioSource;

    private void Start()
    {
        GameObject smokeParticles = Instantiate(smokeParticlesPrefab, transform.position, smokeParticlesPrefab.transform.rotation);
        this.smokeParticles = smokeParticles.GetComponent<ParticleSystem>();
        this.smokeParticles.transform.parent = transform;
        smokeParticlesEmission = this.smokeParticles.emission;
        GameObject ignitionParticles = Instantiate(ignitionParticlesPrefab, transform.position, smokeParticlesPrefab.transform.rotation);
        this.ignitionParticles = ignitionParticles.GetComponent<ParticleSystem>();
        ignitionParticlesEmission = this.ignitionParticles.emission;
        this.ignitionParticles.transform.parent = transform;

        //Random variation to the duration of time that a Tree burns.
        burnTime *= Random.Range(.8f, 1.2f);

        audioSource = GetComponent<AudioSource>();

        loopAffectingTrees(addAffectingTree);
        // if (isfireStarter)
        //     temperature = ignitionTemperature;
    }

    // Sets fire starter trees on fire. Called from BurnManager.
    public void igniteFireStarter()
    {
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
        removeDeadAffectingTrees();
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

    private void removeDeadAffectingTrees()
    {
        for (int i = affectingTrees.Count - 1; i >= 0; i--)
            if (affectingTrees[i].isDead)
                affectingTrees.RemoveAt(i);
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

    public void die()
    {
        //isDead = true;
        //Instantiate burnt tree prefab
        GameObject burntTree = Instantiate(burntTreePrefab, transform.position, transform.rotation);
        burntTree.transform.parent = transform.parent;
        //Destroy the gameobject
        Destroy(gameObject);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (drawGizmos)
        {
            Gizmos.color = Color.red;

            Handles.Label(transform.position, "Tree: " + Mathf.Round(_temperature) + " degrees");
            if (isfireStarter)
                Gizmos.DrawSphere(transform.position, 3f);
            //For debugging
            //Gizmos.DrawWireSphere(transform.position + BurnManager.windVector * BurnManager.windVector.magnitude * affectingRadius / 2, affectingRadius);
        }
    }
#endif
}