using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Sequence : MonoBehaviour
{
    // Replace GameObject here with data type for 
    private GameObject[] goal = new GameObject[4];
    private GameObject[] current = new GameObject[4];

    public event Action<Sequence> sequenceCompleteEvent;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Adds an object to the current sequence. Invokes sequenceCompleteEvent if current matches goal
    void AddObject(GameObject obj, int loc)
    {
        current[loc] = obj;
        for (int i = 0; i < 4; i++)
        {
            if (!GameObject.ReferenceEquals(current[i], goal[i]))
            {
                // Some object doesn't match, return so we stop checking
                return;
            }
        }
        // If we reach this point, that means all objects matched, signal that the sequence is complete
        sequenceCompleteEvent?.Invoke(this);
    }
}
