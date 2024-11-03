using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Experiment : MonoBehaviour
{
    private bool running = false;
    private float startTime = 0.0f;
    private float sequenceStartTime = 0.0f;
    public Sequence currentSequence;
    public List<Sequence> completedSequences;

    // Start is called before the first frame update
    void Start()
    {
        if (currentSequence == null)
        {
            currentSequence = gameObject.AddComponent<Sequence>();
        }

        currentSequence.sequenceCompleteEvent += SequenceCompleteEvent; // Subscribe to completion event
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SequenceCompleteEvent(object sender)
    {
        print("Sequence Complete!");

        // Clean up old sequence
        completedSequences.Add(currentSequence);
        currentSequence.sequenceCompleteEvent -= SequenceCompleteEvent;

        // Add new seequence
        currentSequence = gameObject.AddComponent<Sequence>();
        currentSequence.sequenceCompleteEvent += SequenceCompleteEvent;

    }
}
