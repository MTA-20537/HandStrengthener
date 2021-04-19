using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class moveBall : MonoBehaviour
{

    private OpenBCIInput controller;
    // Start is called before the first frame update
    void Start()
    {
        controller = GameObject.Find("InputManager").GetComponent<OpenBCIInput>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnBCIEvent(float value)
    {
        gameObject.transform.localPosition  = ( new Vector3 (0, 1 + 4 * value * Time.deltaTime, -11.54f));
    }

}
