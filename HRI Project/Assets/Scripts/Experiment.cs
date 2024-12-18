using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.UI;

public class Experiment : MonoBehaviour
{
    private bool running = false;
    private float experimentStartTime = 0.0f;
    public Sequence currentSequence;
    public List<Sequence> completedSequences;

    private bool voiceControlEnabled = false;


    public ExperimentObject[] room0;
    public ExperimentObject[] room1;
    public ExperimentObject[] room2;
    public ExperimentObject[] room3;
    public ExperimentObject[] allObjects;

    [SerializeField] private Button startManual;
    [SerializeField] private Button startVoice;
    [SerializeField] private GameObject testCompletePopup;

    private int experimentNum = 0;


    private Slot[] slots;
    // switched to awake to ensure that a sequence is added before other references to it are called.
    void Awake()
    {
        //initializes arrays of objects to be used in the sequences. Since the sequence component has to run it more than once, it is more efficient to just have it ran once.

        Transform room0transform = GameObject.Find("Room0_Objects").transform;
        Transform room1transform = GameObject.Find("Room1_Objects").transform;
        Transform room2transform = GameObject.Find("Room2_Objects").transform;
        Transform room3transform = GameObject.Find("Room3_Objects").transform;
        Transform slottransform = GameObject.Find("Slots").transform;
        room0 = room0transform.GetComponentsInChildren<ExperimentObject>();
        Debug.Log("room0 array size: " + room0.Length);
        room1 = room1transform.GetComponentsInChildren<ExperimentObject>();
        Debug.Log("room1 array size: " + room1.Length);
        room2 = room2transform.GetComponentsInChildren<ExperimentObject>();
        Debug.Log("room2 array size: " + room2.Length);
        room3 = room3transform.GetComponentsInChildren<ExperimentObject>();
        Debug.Log("room3 array size: " + room3.Length);
        slots = slottransform.GetComponentsInChildren<Slot>();
        Debug.Log("slots array size" + slots.Length);
        //concat all the arrays together to get all objects for use in the resetPositions method
        allObjects = room0.Concat(room1).Concat(room2).Concat(room3).ToArray();


        startManual.transform.parent.parent.gameObject.SetActive(true);
        startManual.onClick.AddListener(StartExperiment);
        startVoice.onClick.AddListener(StartExperiment);

        voiceControlEnabled = false;
        GameObject.Find("Canvas").GetComponent<UISelection>().OnVoiceControlToggled(voiceControlEnabled);
        experimentNum = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Starts the experiment and adds the first sequence
    public void StartExperiment()
    {
        if (!running) // Only run through the start procedure if we haven't already started
        {
            startManual.transform.parent.parent.gameObject.SetActive(false);
            startVoice.transform.parent.parent.gameObject.SetActive(false);

            if (currentSequence != null)
            {
                // There is still a prior sequence here. Unsubscribe from it and clear the previous list
                currentSequence.sequenceCompleteEvent -= SequenceCompleteEvent;
                completedSequences.Clear();
            }

            currentSequence = gameObject.AddComponent<Sequence>();
            currentSequence.sequenceCompleteEvent += SequenceCompleteEvent; // Subscribe to completion event
            experimentStartTime = Time.time;

            running = true;
        }
    }

    void SequenceCompleteEvent(object sender)
    {
        print("Sequence Complete!");
        //reset all objects
        resetObjects();

        // Clean up old sequence
        completedSequences.Add(currentSequence);
        currentSequence.sequenceCompleteEvent -= SequenceCompleteEvent;

        if (completedSequences.Count() >= 5)
        {
            // Complete the experiment after 5 sequences
            running = false;
            experimentNum++;

            // Write results to an output file
            string path = Application.dataPath + $"/results_{experimentNum}.json";

            print(path);

            StreamWriter sw = File.CreateText(path);
            sw.WriteLine($"{{ \"experimentStartTime\": {experimentStartTime},");
            sw.WriteLine($"\"voiceControlEnabled\": {voiceControlEnabled},");
            sw.WriteLine("\"sequences\": [");
            for (int i = 0; i < completedSequences.Count(); i++)
            {
                Sequence seq = completedSequences[i];
                string json = seq.Serialize();
                if (i + 1 != completedSequences.Count())
                {
                    json += ",";
                }
                sw.WriteLine(json);
            }
            sw.WriteLine("]}");
            sw.Close();

            // Switch control type, reset sequences
            voiceControlEnabled = !voiceControlEnabled;
            GameObject.Find("Canvas").GetComponent<UISelection>().OnVoiceControlToggled(voiceControlEnabled);
            completedSequences.Clear();
            currentSequence = gameObject.AddComponent<Sequence>();
            currentSequence.sequenceCompleteEvent += SequenceCompleteEvent;

            if (voiceControlEnabled)
            {
                startVoice.transform.parent.parent.gameObject.SetActive(true);
            }
            else
            {
                testCompletePopup.SetActive(true);
            }
        }
        else
        {
            // Add new seequence
            currentSequence = gameObject.AddComponent<Sequence>();
            currentSequence.sequenceCompleteEvent += SequenceCompleteEvent;
        }

    }

    //getters for the arrays
    public ExperimentObject[] GetRoom(int roomnum)
    {
        switch (roomnum)
        {
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
    public Slot[] GetSlots()
    {
        return slots;
    }

    public ExperimentObject[] GetAllObjects()
    {
        return allObjects;
    }

    public void resetObjects()
    {

        foreach (ExperimentObject obj in allObjects)
        {
            obj.SetPosition(null);
        }
    }
}
