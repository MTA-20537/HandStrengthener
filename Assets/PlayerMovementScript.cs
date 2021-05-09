using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovementScript : MonoBehaviour
{
    public RecordToArray recordToArrayRef;
    public GameObject signalHand, dontmoveText, moveText, victoryText, scoreText, pointGuy, bgGroup;
    public Text pointGuyText;

    public int maxRuns = 16;
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
    private bool[] taskResults;
    private float timer;
    private int runCounter = -1;
    private float scoreCounter;
    private bool isFirstRound;
    // Start is called before the first frame update
    void Start()
    {
        prevSourceIndex = 4;
        isReady = false;
        isFirstRound = true;
        taskResults = new bool[] {false,false,false};
        animator = gameObject.GetComponent<Animator>();
        sourceNodeIndex = 0;
        targetNodeIndex = 1;
        if(phaseNodes[sourceNodeIndex]!=null && phaseNodes[targetNodeIndex] != null) {
            sourceNode = phaseNodes[sourceNodeIndex];
            targetNode = phaseNodes[targetNodeIndex];
        }
        signalHand.SetActive(false);
        dontmoveText.SetActive(false);
        moveText.SetActive(false);
        victoryText.SetActive(false);
        pointGuy.SetActive(false);
        scoreText.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.x>=targetNode.transform.position.x) {
            //Debug.Log("Player pos:" + transform.position.x + " Larger than target node " + targetNode.name + " pos " + targetNode.transform.position.x);
            //Debug.Log("target node " + targetNode.name + " is ready?: " +isReady);
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
                //Debug.Log("Intertrial Phase");
                animator.speed = 1;
                if(prevSourceIndex!=sourceNodeIndex) {
                    IsNotReady();
                    runCounter++;
                }
                animator.SetTrigger("prep");
                animator.SetBool("trick_hard",false);
                animator.SetBool("trick_medium",false);
                animator.SetBool("trick_easy",false);
                animator.SetBool("CanTrick",true);
                if(isFirstRound)
                    pointGuy.SetActive(false);
                pointGuyText.text = calculateScore(taskResults).ToString();
                break;
            case 1: //Cue phase
                //Debug.Log("Cue Phase");
                if(runCounter<maxRuns) {
                    animator.SetTrigger("run");

                    animator.speed = 1/(phaseDurations[1]+phaseDurations[2]);
                    moveSpeed = targetDistanceX/((phaseDurations[1]+phaseDurations[2])/2f);
                } else {
                    IsNotReady();
                    victoryText.SetActive(true);
                    scoreText.SetActive(true);
                    scoreText.GetComponent<Text>().text = "Score:" + (Mathf.Round((scoreCounter/maxRuns) * 100)) / 100.0;
                }
                
                break;
            case 2: //Prep phase
                //Debug.Log("Prep Phase");
                if(isFirstRound) {
                    isFirstRound = false;
                    pointGuy.SetActive(true);
                }

                dontmoveText.SetActive(true);

                animator.speed = 1/(phaseDurations[1]+phaseDurations[2]);
                moveSpeed = targetDistanceX/((phaseDurations[1]+phaseDurations[2])/2f);

                break;
            case 3: //Task phase
                //Take input
                //Debug.Log("Task Phase");
                if(prevSourceIndex!=sourceNodeIndex) {
                    animator.SetTrigger("rise");
                    pointGuy.SetActive(true);
                    MoveNode(pointGuy);
                    MoveNode(bgGroup);
                }

                dontmoveText.SetActive(false);
                moveText.SetActive(true);
                signalHand.SetActive(true);

                animator.speed = 1/phaseDurations[3];
                moveSpeed = targetDistanceX/phaseDurations[sourceNodeIndex];

                pointGuyText.text = "";
                break;
            case 4: //Feedback phase
                //Debug.Log("Feedback Phase");
                if(prevSourceIndex!=sourceNodeIndex) {
                    if(recordToArrayRef.result.Length>=6) {
                        taskResults = new bool[] {recordToArrayRef.result[2],recordToArrayRef.result[3],recordToArrayRef.result[5]};
                    } else {
                        taskResults = new bool[] {false,false,false};
                    }

                    timer = Time.time;
                    animator.SetTrigger("midair");
                    scoreCounter+=calculateScore(recordToArrayRef.result);
                }
                float elapsedAnimationTime = (Time.time-timer)*animator.speed;
                animator.SetBool("CanTrick",false);
                if((elapsedAnimationTime>=0.37f)) {
                        animator.SetBool("trick_hard",taskResults[2]);
                } else if((elapsedAnimationTime>=0.18f)) {
                        animator.SetBool("trick_medium",taskResults[1]);
                } else if((elapsedAnimationTime>=0.0f)) {
                        animator.SetBool("trick_easy",taskResults[0]);
                }
                animator.SetTrigger("midair");

                moveText.SetActive(false);
                signalHand.SetActive(false);
                
                animator.speed = 1/phaseDurations[4];
                moveSpeed = targetDistanceX/phaseDurations[sourceNodeIndex];
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

    float calculateScore(bool[] results) {
        float score = 1f;
        if(results[0]) score+=2f;
        if(results[1]) score+=3f;
        if(results[2]) score+=4f;
        //if motor imagery, then double points 
        return score;
    }
}
