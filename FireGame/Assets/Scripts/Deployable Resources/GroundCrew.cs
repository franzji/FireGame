using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GroundCrew : GameUnit {

    public enum AI_State { Walking, Cutting, FindNewTree, Idle, WalkingToClick };
    public Tree closestTree;
    public AI_State State = AI_State.Walking;
    public float IdleTime;
    public float IdleTimer;
    public int FindTreeRadius;
    public List<Tree> alltrees = new List<Tree>();
    public Vector3 StartingPosition;
    private NavMeshAgent agent;
    private Vector3 destination;
    private bool movingTowardsDestination;
    protected AudioSource audioSource;

    void Start()
    {
        State = AI_State.FindNewTree;
        IdleTimer = IdleTime;

        StartingPosition = transform.position;
        //OnDrawGizmos();

        agent = GetComponent<NavMeshAgent>();
        spawnUnit = GameObject.Find("UI").GetComponent<AIManager>();

        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        //basic AI stuff? sure
        if (State == AI_State.FindNewTree)
        {
            FindClosestTree();
        }
        else if (State == AI_State.Walking)
        {
            MoveToTree();
        }
        else if (State == AI_State.Idle)
        {
            SittingIdle();
        }
        else if (State == AI_State.WalkingToClick)
        {
            MoveToClick();
        }
    }

    protected virtual void handleTree() { }
    private void SittingIdle()
    {
        IdleTimer -= Time.deltaTime;

        if (IdleTimer < 0)
        {
            IdleTimer = IdleTime;
            if (closestTree)
            {
                handleTree();
                closestTree.targetCount--;
                audioSource.pitch = Random.Range(.8f, 1.2f);
                audioSource.Play();
            }
            State = AI_State.FindNewTree;
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(StartingPosition, 20);
    }

    public void MoveToTree()
    {
        if (destination == null)
        {
            movingTowardsDestination = false;
            State = AI_State.FindNewTree;
        }
        else
        {
            if (!movingTowardsDestination)
            {
                agent.SetDestination(destination);
                movingTowardsDestination = true;
            }
            if (!agent.pathPending && agent.remainingDistance < 2)
            {
                IdleTimer = IdleTime;
                State = AI_State.Idle;
            }
        }
    }

    public void MoveToClick()
    {
        //Detarget the closest tree
        if (closestTree)
        {
            closestTree.targetCount--;
            closestTree = null;
        }

        if (!movingTowardsDestination)
        {
            agent.SetDestination(destination);
            movingTowardsDestination = true;
        }
        if (!agent.pathPending && agent.remainingDistance < 20)
        {
            State = AI_State.Idle;
        }
    }

    protected virtual bool isCandidateTree(Tree tree) { return false; }

    public void FindClosestTree()
    {
        float distanceToClosestTree = Mathf.Infinity;

        List<Tree> alltrees = new List<Tree>();

        Collider[] array = Physics.OverlapSphere(StartingPosition, FindTreeRadius);

        for (int i = 0; i < array.Length; i++)
        {
            Tree tree = array[i].GetComponent<Tree>();
            //If there is a tree compoment
            if (tree)
                //Tree cannot be already targeted
                if (isCandidateTree(tree))
                alltrees.Add(tree);
        }

        foreach (Tree currentTree in alltrees)
        {
            float distanceToTree = (currentTree.transform.position - this.transform.position).sqrMagnitude;
            if (distanceToTree < distanceToClosestTree)
            {
                distanceToClosestTree = distanceToTree;
                closestTree = currentTree;
            }
        }
        if (closestTree)
        {
            closestTree.targetCount++;
            destination = closestTree.gameObject.transform.position;
            movingTowardsDestination = false;
            State = AI_State.Walking;
        }
        else
        {
            IdleTimer = IdleTime;
            State = AI_State.Idle;
        }
    }

    public void SetHome(Vector3 hit)
    {
        StartingPosition = hit;
        State = AI_State.WalkingToClick;
        destination = StartingPosition;
        movingTowardsDestination = false;
    }
}
