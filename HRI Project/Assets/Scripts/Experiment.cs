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

    
    private ExperimentObject[] room0;
    private ExperimentObject[] room1;
    private ExperimentObject[] room2;
    private ExperimentObject[] room3;

   
    private Slot[] slots;
    // switched to awake to ensure that a sequence is added before other references to it are called.
    void Awake()
    {
        if (currentSequence == null)
        {
            currentSequence = gameObject.AddComponent<Sequence>();
        }

        currentSequence.sequenceCompleteEvent += SequenceCompleteEvent; // Subscribe to completion event

        //initializes arrays of objects to be used in the sequences. Since the sequence component has to run it more than once, it is more efficient to just have it ran once.

        Transform room0transform = GameObject.Find("Room0_Objects").transform;
        Transform room1transform = GameObject.Find("Room1_Objects").transform;
        Transform room2transform = GameObject.Find("Room2_Objects").transform;
        Transform room3transform = GameObject.Find("Room3_Objects").transform;
        Transform slottransform = GameObject.Find("Slots").transform;
        room0 = room0transform.GetComponentsInChildren<ExperimentObject>(); 
        Debug.Log("room0 array size: " + room0.Length);
        room1  = room1transform.GetComponentsInChildren<ExperimentObject>();
        Debug.Log ("room1 array size: " + room1.Length);
        room2 = room2transform.GetComponentsInChildren<ExperimentObject>();
        Debug.Log("room2 array size: " + room2.Length);
        room3 = room3transform.GetComponentsInChildren<ExperimentObject>();
        Debug.Log("room3 array size: "+ room3.Length);
        slots = slottransform.GetComponentsInChildren<Slot>();
        Debug.Log("slots array size"+ slots.Length);

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

    //getters for the arrays
    public ExperimentObject[] GetRoom(int roomnum){
        switch(roomnum){
            case 0:
                return room0;
            case 1:
                return room1;
            case 2:
                return room2;
            case 3: 
                return room3;
            default:
                return null;
        }
    }
    public Slot[] GetSlots(){
        return slots;
    }
}
