using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;


public class Slot : MonoBehaviour
{
    
    [SerializeField] private string desiredcolor;
    [SerializeField] private string desiredshape;
    [SerializeField] private string desiredroom;
    [SerializeField] private string slotNum;
    private Screen screen;
    


    //boolean to prevent the score from incrementing when repeadedly putting a shape into a slot
    private bool correct = false;

//reference to the sequence so the tile can communicate score changes.
   private Sequence sequence;
    //resets the tile with a new goal

    private void Start(){
        sequence = GameObject.Find("ExperimentManager").GetComponent<Sequence>();
        Debug.Log("sequence: "+ sequence);
        screen = GameObject.Find("Screen_"+slotNum).transform.GetComponentInChildren<Screen>();
    }
    //sets the goal and the image on the screen
    public void setGoal(string color, string shape, string room, Texture image){
        correct = false;
        desiredcolor = color;
        desiredshape = shape;
        desiredroom = room;
        //sets the image that'll appear on the screen. also sets the background color to white in case it isnt already
        screen.setImage(image);
        screen.setColor(Color.white);
    }

    private void OnTriggerEnter(Collider other){
        ExperimentObject obj = other.GetComponentInParent<ExperimentObject>();
        Debug.Log("Collision with" + other.name);
        if(obj != null){
            if(checkCorrectness(obj) && !correct){
                Debug.Log("Correct Object Placed!");
                correct = true;
                sequence.OnCorrectObject();
                screen.setColor(Color.green);

            }
            else{
                Debug.Log("Wrong Object!");
            }

        }
    }

    private void OnTriggerExit(Collider other)
{
    ExperimentObject obj = other.GetComponentInChildren<ExperimentObject>();
       if(correct){
         // Reset the correct flag only for this slot when the correct object exits
            correct = false;
            sequence.onCorrectObjectRemoval();
            screen.setColor(Color.white);
       }
    
    else{
        Debug.Log("No ExperimentObject found. Checking components...");
        foreach (var component in other.GetComponents<Component>())
        {
            Debug.Log("Component: " + component.GetType());
        }
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
