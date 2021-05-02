using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowScript : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    public GameObject objectToFollow;
    private Vector3 followOffset;

    void Start()
    {
        followOffset = transform.position - objectToFollow.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = followOffset + objectToFollow.transform.position;
    }
}
