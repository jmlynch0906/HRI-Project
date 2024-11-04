using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;


public class Slot : MonoBehaviour
{
    
    [SerializeField] private string desiredcolor;
    [SerializeField] private string desiredshape;
    [SerializeField] private string desiredroom;
    [SerializeField] private string slotNum;


    //boolean to prevent the score from incrementing when repeadedly putting a shape into a slot
    private bool correct = false;

//reference to the sequence so the tile can communicate score changes.
   private Sequence sequence;
    //resets the tile with a new goal

    private void Start(){
        sequence = GameObject.Find("ExperimentManager").GetComponent<Sequence>();
        Debug.Log("sequence: "+ sequence);
    }
    public void setGoal(string color, string shape, string room){
        correct = false;
        desiredcolor = color;
        desiredshape = shape;
        desiredroom = room;
    }

    private void OnTriggerEnter(Collider other){
        ExperimentObject obj = other.GetComponentInParent<ExperimentObject>();
        Debug.Log("Collision with" + other.name);
        if(obj != null){
            if(checkCorrectness(obj) && !correct){
                Debug.Log("Correct Object Placed!");
                correct = true;
                sequence.OnCorrectObject();

            }
            else{
                Debug.Log("Wrong Object!");
            }

        }
    }

    private void OnTriggerExit(Collider other)
{
    ExperimentObject obj = other.GetComponentInParent<ExperimentObject>();
    if (obj != null)
    {
        // Reset the correct flag only for this slot when object exits
        correct = false;
    }
}
    //creates a temp object then checks if it is equal to the current object
    bool checkCorrectness(ExperimentObject obj){
        GameObject tileGameObject = new GameObject();
        ExperimentObject tileExperiementObject = tileGameObject.AddComponent<ExperimentObject>();
        tileExperiementObject.Init(desiredroom,desiredcolor,desiredshape);
        bool correct = tileExperiementObject.Matches(obj);
        Destroy(tileGameObject);
        return correct;

    }

}
