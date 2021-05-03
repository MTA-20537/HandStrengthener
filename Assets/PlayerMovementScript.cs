using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementScript : MonoBehaviour
{
    public GameObject[] phaseNodes;
    public float[] phaseDurations;
    public const float sceneSizeX = 245.5f;

    public GameObject playerAvatar;
    public GameObject pole;
    public Animator animator;
    private GameObject targetNode;
    private GameObject sourceNode;
    [SerializeField]
    private int sourceNodeIndex, targetNodeIndex, prevSourceIndex;
    private bool isReady;
    private float moveSpeed;
    // Start is called before the first frame update
    void Start()
    {
        prevSourceIndex = 4;
        isReady = false;
        animator = gameObject.GetComponent<Animator>();
        sourceNodeIndex = 0;
        targetNodeIndex = 1;
        if(phaseNodes[sourceNodeIndex]!=null && phaseNodes[targetNodeIndex] != null) {
            sourceNode = phaseNodes[sourceNodeIndex];
            targetNode = phaseNodes[targetNodeIndex];
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.x>=targetNode.transform.position.x) {
            Debug.Log("Player pos:" + transform.position.x + " Larger than target node " + targetNode.name + " pos " + targetNode.transform.position.x);
            Debug.Log("target node " + targetNode.name + " is ready?: " +isReady);
            //if(sourceNodeIndex==4)
            //    isReady = false;
            if(isReady) {
                NextNodes();
            }
        }
        float targetDistanceX = targetNode.transform.position.x - sourceNode.transform.position.x;
        switch(sourceNodeIndex) {
            //Add in mihai's thing on off
            //Background parallax
            case 0: //Inter-trial interval phase
                Debug.Log("Intertrial Phase");
                animator.speed = 1;
                IsNotReady();
                if(prevSourceIndex!=sourceNodeIndex) {
                    IsNotReady();
                }
                animator.SetTrigger("prep");
                break;
            case 1: //Cue phase
                Debug.Log("Cue Phase");
                animator.SetTrigger("run");
                animator.speed = 1/(phaseDurations[1] + phaseDurations[2]);
                moveSpeed = targetDistanceX/phaseDurations[sourceNodeIndex];
                break;
            case 2: //Prep phase
                Debug.Log("Prep Phase");
                moveSpeed = targetDistanceX/phaseDurations[sourceNodeIndex];
                break;
            case 3: //Task phase
                Debug.Log("Task Phase");
                animator.speed = 1/phaseDurations[3];
                animator.SetTrigger("rise");
                moveSpeed = targetDistanceX/phaseDurations[sourceNodeIndex];
                break;
            case 4: //Feedback phase
                Debug.Log("Feedback Phase");
                animator.speed = 1;
                animator.SetTrigger("midair");
                moveSpeed = targetDistanceX/phaseDurations[sourceNodeIndex];
                //pole.transform.position = new Vector3(sourceNode.transform.position.x - (transform.position.x - sourceNode.transform.position.x),pole.transform.localPosition.y,pole.transform.localPosition.z);
                break;
            default:
                Debug.LogError("Target Node Out of range");
                break;
        }
        prevSourceIndex = sourceNodeIndex;
        transform.position += new Vector3(moveSpeed*Time.deltaTime,0,0);
    }

    void NextNodes() {
        if(++targetNodeIndex>=phaseNodes.Length) {
            targetNodeIndex = 0;
        }
        if(++sourceNodeIndex>=phaseNodes.Length) {
            sourceNodeIndex = 0;
        }
        MoveNode(sourceNode);
        sourceNode = phaseNodes[sourceNodeIndex];
        targetNode = phaseNodes[targetNodeIndex];
    }

    void MoveNode(GameObject node, float xMove = sceneSizeX) {
        node.transform.position += new Vector3(xMove,0,0);
    }

    public void IsReady() {
        isReady = true;
    }

    public void IsNotReady() {
        isReady = false;
        moveSpeed = 0;
    }
}
