using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Sequence : MonoBehaviour
{
    public ExperimentObject[] goal;

    //arrays to store each room's objects as well as the slots
    public ExperimentObject[] room0;
    public ExperimentObject[] room1;
    public ExperimentObject[] room2;
    public ExperimentObject[] room3;

    public Slot[] slots;



    public event Action<Sequence> sequenceCompleteEvent;
    public float completionTime = 0;

    [SerializeField] private int currentScore = 0;
    private int goalScore = 4;
    private bool complete;

    // Creates a randomized set of objects
    void Start()
    {
        complete = false;
        // Initialize to empty arrays
        goal = new ExperimentObject[4];
        List<ExperimentObject> objects = new List<ExperimentObject>();
        Experiment experiment = GameObject.Find("ExperimentManager").transform.GetComponent<Experiment>();
        //initializes arrays with all the gameobjects on the map using the arrays set up in the experiment file.
        setArrays(experiment.GetRoom(0),experiment.GetRoom(1), experiment.GetRoom(2), experiment.GetRoom(3), experiment.GetSlots());

        //randomly select one object from each object list and add them to objects list
         objects.Add(room0[UnityEngine.Random.Range(0,room0.Length)]);
         objects.Add(room1[UnityEngine.Random.Range(0,room1.Length)]);
         objects.Add(room2[UnityEngine.Random.Range(0,room2.Length)]);
         objects.Add(room3[UnityEngine.Random.Range(0,room3.Length)]);
             // Randomly order the objects into the goal
        for (int i = 0; i < goal.Length; i++)
        {
            int randIndex = UnityEngine.Random.Range(0, objects.Count); // Choose one at random
            goal[i] = objects[randIndex]; // Add it to the goal
            goal[i].AssignToTile(i+1); //assigns the object to the appropriate tile. adds 1 to i since the tiles start at 1 instead of 0.
            objects.RemoveAt(randIndex); // Remove it from the list
        }

        foreach (Slot slot in slots)
        {
            slot.slotStateEvent += onSlotUpdate;
        }
    }

    // Update is called once per frame. Invokes sequenceCompleteEvent if current matches goal
    //changed the scoring system to not rely on checking arrays and constant for loops and instead a single if statement. 
    void Update()
    {
        if(!complete & currentScore >= goalScore){
            // If we reach this point, that means all objects matched, signal that the sequence is complete
            complete = true;
            completionTime = Time.time;
            sequenceCompleteEvent?.Invoke(this);
        }
    }

    private void setArrays(ExperimentObject[] r0, ExperimentObject[] r1, ExperimentObject[] r2, ExperimentObject[] r3, Slot[] s){
        room0 = r0;
        room1 = r1;
        room2 = r2;
        room3 = r3;
        slots = s;
    }

    private void onSlotUpdate(object sender, bool correct)
    {
        if (correct)
        {
            currentScore++;
        }
        else
        {
            currentScore--;
        }
    }

    // Returns this Sequence as a string in json format
    public string Serialize()
    {
        string json = "{ \"sequence\":[\n";

        for(int i = 0; i < goal.Length; i++)
        {
            json += goal[i].Serialize();
            if (i+1 != goal.Length)
            {
                json += ",";
            }
            json += "\n";
        }

        json += $"],\n \"completionTime\": {completionTime} }}";

        return json;
    }
}
