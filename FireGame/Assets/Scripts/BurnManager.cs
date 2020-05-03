using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnManager : MonoBehaviour {
    [Header("Level Goals")]
    [SerializeField]
    [Range(0f, 1f)]
    private float failureThreshold = .5f;

    [Header("Properties")]
    [SerializeField]
    private AnimationCurve burnCurve;
    private WindZone wind;
    [SerializeField]
    [Range(0f, 1f)]
    private float windInfluence = .5f;
    [SerializeField]
    private static Vector3 _windVector;
    public static Vector3 windVector
    {
        get { return _windVector; }
        private set { _windVector = value; }
    }

    [Header("Optimizations")]
    [SerializeField]
    private int numberOfTreeChunks;
    [SerializeField]
    private float burnCoroutineInterval;

    [Header("Level End")]
    [SerializeField]
    private GameObject levelEndPrefab;
    private bool isLevelEnd = false;

    private List<Tree> trees;
    private List<List<Tree>> treeChunks;
    private int initialNumberOfTrees;
    private int numberAliveTrees;
    private DisplayTreesRemaining treeDisplay;

    void Start()
    {
        //Convert tree array to a List of trees to initialize trees.
        trees = new List<Tree>(GetComponentsInChildren<Tree>());
        initialNumberOfTrees = trees.Count;
        numberAliveTrees = initialNumberOfTrees;
        treeDisplay = GameObject.Find("Trees Remaining").GetComponent<DisplayTreesRemaining>();
        treeDisplay.UpdateTreesRemaining(1.0f);

        treeChunks = chunkList(trees, numberOfTreeChunks);

        // int numTrees = 0;
        // for (int i = 0; i < treeChunks.Count; i++)
        //     numTrees += treeChunks[i].Count;
        //Debug.Log("Number of trees: " + trees.Count);
        //Debug.Log("Number of trees chunks: " + treeChunks.Count);
        //Debug.Log("Number of trees in chunks: " + numTrees);

        //Set up wind: points in the direction of the wind GameObject with magnitude the magnitude of Main.
        //windVector = wind.gameObject.transform.rotation * Vector3.forward * wind.windMain;

        //Amount of wind depends on the wind influence, not on the windMain in the wind zone.
        //Wind zone is just for fancy particle effects.
        wind = GameObject.Find("Wind").GetComponent<WindZone>();
        windVector = wind.gameObject.transform.rotation * Vector3.forward * windInfluence;
        wind.windMain = windInfluence * 15f;
    }

    public void startGame()
    {
        // Ignite the trees marked as "Is Fire Starter".
        foreach (Tree tree in trees)
            tree.igniteFireStarter();

        // Start the fire spreading logic.
        for (int i = 0; i < treeChunks.Count; i++)
        {
            float delay = i * (float)burnCoroutineInterval / treeChunks.Count;
            StartCoroutine(startBurnCoroutineAfterDelay(delay, treeChunks[i]));
        }
    }

    //Starts the burn coroutine after a delay.
    private IEnumerator startBurnCoroutineAfterDelay(float delay, List<Tree> trees)
    {
        yield return new WaitForSeconds(delay);
        StartCoroutine(burnCoroutine(trees));
    }

    private IEnumerator burnCoroutine(List<Tree> trees)
    {
        //While trees are still in the list
        while (trees.Count > 0)
        {
            List<Tree> deadTrees = new List<Tree>();
            //Warms up each tree and removes if the tree is dead.
            foreach (Tree tree in trees)
            {
                //A level critical object, such as a house, caught on fire.
                if (tree.isLevelCritical && tree.isOnFire)
                    instantiateLevelEnd(false);
                if (tree.isDead)
                    deadTrees.Add(tree);
                else tree.warmUp(burnCurve, burnCoroutineInterval);
            }

            foreach (Tree tree in deadTrees)
            {
                //Remove tree from coroutine list.
                trees.Remove(tree);
                //Remove tree from member lists.
                killandRemoveTree(tree);
            }

            //Fire is put out. Level won.
            if (isNoTreesSmoking)
                instantiateLevelEnd(true);

            //Too many trees died. Level lost.
            if ((float)this.trees.Count / (float)initialNumberOfTrees < failureThreshold)
                instantiateLevelEnd(false);

            yield return new WaitForSeconds(burnCoroutineInterval);
        }
    }

    private void instantiateLevelEnd(bool isWin)
    {
        if (!isLevelEnd)
        {
            //Set isLevelEnd to true to prevent duplicate spawning from coroutine calls
            isLevelEnd = true;

            GameObject levelEndInstance = Instantiate(levelEndPrefab, transform);
            LevelEnd levelEnd = levelEndInstance.GetComponent<LevelEnd>();
            //Set isWin in levelEnd to let it know how to handle message displaying
            levelEnd.isWin = isWin;
        }
    }

    //Breaks a list into multiple chunks
    private static List<List<T>> chunkList<T>(List<T> list, int numberofChunks)
    {
        List<List<T>> chunkList = new List<List<T>>();
        int chunkSize = (int)Mathf.Ceil((float)list.Count / numberofChunks);

        for (int i = 0; i < list.Count; i += chunkSize)
        {
            int count = Mathf.Min(chunkSize, list.Count - i);
            chunkList.Add(list.GetRange(i, count));
        }        

        return chunkList;
    }

    private void OnValidate()
    {
        if (numberOfTreeChunks < 1)
            numberOfTreeChunks = 1;
        if (burnCoroutineInterval < .1f)
            burnCoroutineInterval = .1f;
    }

    public void drawAffectingTreeDebugRays()
    {
        trees = new List<Tree>(GetComponentsInChildren<Tree>());
        foreach (Tree tree in trees)
            tree.loopAffectingTrees(tree.drawAffectingTreeDebugRay);
    }

    public void killandRemoveTree(Tree tree)
    {
        //Remove trees in burn manager list.
        trees.Remove(tree);
        //Update number of trees remaining display in Stats panel.
        numberAliveTrees--;
        treeDisplay.UpdateTreesRemaining((float) numberAliveTrees / initialNumberOfTrees);
        //Tree die method.
        tree.die();
    }

    private bool isNoTreesSmoking
    {
        get
        {
            foreach (Tree tree in trees)
                if (tree.isSmoking)
                    return false;

            return true;
        }
    }
}
