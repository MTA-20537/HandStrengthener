using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateHand : MonoBehaviour
{
    public Animator animator;
    public GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponentInChildren<MeshRenderer>().material.color = new Color(1f, 1f, 1f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.inputWindow == InputWindowState.Open)
        {
            animator.SetBool("grab",true);
            gameObject.GetComponentInChildren<MeshRenderer>().material.color = new Color(1f, 1f, 1f, 1f);
        }
        else
        {
            animator.SetBool("grab", false);
            gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material.color = new Color(1f, 1f, 1f, 0f);
        }
       
    }
}
