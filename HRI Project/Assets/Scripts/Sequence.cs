using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Sequence : MonoBehaviour
{
    // Replace GameObject here with data type for 
    public GameObject[] goal;
    public GameObject[] current;

    public event Action<Sequence> sequenceCompleteEvent;

    // Creates a randomized set of objects
    void Start()
    {
        // Initialize to emptty arrays
        goal = new GameObject[4];
        current = new GameObject[4];

        string[] colors = { "red", "blue", "green" };
        string[] shapes = { "sphere", "cube", "triangle" };

        List<GameObject> objects = new List<GameObject>();

        // Create a random object for each room
        // NOTE: This could be adjusted to find a random existing object from each room instead to improve performance
        for (int room = 1; room <= 4; room++)
        {
            GameObject roomGameObject = new GameObject(room.ToString());
            ExperimentObject roomExperimentObject = roomGameObject.AddComponent<ExperimentObject>();
            roomExperimentObject.Init(room.ToString(), colors[UnityEngine.Random.Range(0, colors.Length)], shapes[UnityEngine.Random.Range(0, shapes.Length)]); // Initialize the object to random color/shape

            objects.Add(roomGameObject);
        }

        // Randomly order the objects into the goal
        for (int i = 0; i < goal.Length; i++)
        {
            int randIndex = UnityEngine.Random.Range(0, objects.Count); // Choose one at random
            goal[i] = objects[randIndex]; // Add it to the goal
            objects.RemoveAt(randIndex); // Remove it from the list
        }
    }

    // Update is called once per frame. Invokes sequenceCompleteEvent if current matches goal
    void Update()
    {
        for (int i = 0; i < goal.Length; i++)
        {
            if (current[i] != null) {
                ExperimentObject currentObject = current[i].GetComponent<ExperimentObject>();
                ExperimentObject goalObject = goal[i].GetComponent<ExperimentObject>();

                if (currentObject == null | !currentObject.Matches(goalObject))
                {
                    // An object doesn't match, return so we stop checking
                    return;
                }
            } else {
                // The sequence is not full, return so we stop checking
                return;
            }
        }
        // If we reach this point, that means all objects matched, signal that the sequence is complete
        sequenceCompleteEvent?.Invoke(this);
    }

    // Adds an object to the current sequence. Returns true if it was correct
    public bool AddObject(GameObject obj, int loc)
    {
        current[loc] = obj;
        ExperimentObject currentObject = current[loc].GetComponent<ExperimentObject>();
        ExperimentObject goalObject = goal[loc].GetComponent<ExperimentObject>();
        return currentObject != null & currentObject.Matches(goalObject);
    }
}
