using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class RobotMovement : MonoBehaviour
{
    private NavMeshAgent agent;
    [SerializeField] Transform holdArea;
    [SerializeField] private Transform shapeDestination;
    [SerializeField] private Transform slotDestination;
    [SerializeField] private GameObject heldObject;
      [SerializeField] private float distance;
      [SerializeField] private GameObject obj;


    private Transform originalShapeParent;
     private Rigidbody objRB;

    [SerializeField] private bool pickedupShape = false;
   [SerializeField] private bool taskComplete = false;

    [SerializeField] private bool destinationSet = false;
    //return point
    private Vector3 originalPosition;
    
    // Start is called before the first frame update
    void Start()
    {
     originalPosition = transform.position;
     agent = GetComponent<NavMeshAgent>(); 
    }

    // Update is called once per frame
    void Update()
    {
        //robot returns home
        if(taskComplete){
            agent.SetDestination(originalPosition);
        }
        else{
            //robot moves to the slot
            if(pickedupShape){
                MoveObject();
                if(!destinationSet){
                    agent.SetDestination(slotDestination.position);
                    destinationSet = true;
                }
                checkDistanceToSlot();
            }
            //robot picks up the shape
            else{
                if(!destinationSet){
                    agent.SetDestination(shapeDestination.position);
                    obj = shapeDestination.gameObject;
                    destinationSet = true;
                }
                checkDistanceToObject();
            }
        }
    }
    //use this to issue commands to the robot.
    public void SetTask(Transform shapeDestination,Transform slotDestination){
        this.shapeDestination = shapeDestination;
        this.slotDestination = slotDestination;
        taskComplete = false;
    }



        //pickup code copied from player character and slghtly altered.
void GrabObject(GameObject obj)
{
    if(obj.GetComponentInChildren<Rigidbody>())
    {
        this.obj = obj;
        objRB = obj.GetComponentInChildren<Rigidbody>();
        GameObject shapeObject = objRB.transform.gameObject;

        objRB.velocity = Vector3.zero;
        objRB.angularVelocity = Vector3.zero;
        objRB.useGravity = false;
        objRB.drag = 10;
        objRB.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePosition;
        originalShapeParent = shapeObject.transform.parent;
        Debug.Log("original parent" + originalShapeParent.name);

        // Parent the object to the holdArea
        shapeObject.transform.SetParent(holdArea);
  

        obj.transform.localPosition = Vector3.zero; 
        obj.transform.localRotation = Quaternion.identity; 

        heldObject = shapeObject.gameObject;
        pickedupShape = true;
        destinationSet = false;
    }
}

         void DropObject(){
            objRB.useGravity = true;
            objRB.drag = 1;
            objRB.constraints = RigidbodyConstraints.None;
            objRB.transform.parent = originalShapeParent;
            obj.GetComponent<ExperimentObject>().SetPosition(slotDestination);
            obj = null;
            originalShapeParent = null;
            heldObject = null;
            pickedupShape = false;
            taskComplete = true;
   }
    void MoveObject(){
     if (heldObject != null)
    {   
        // Set the held object’s position to the hold area’s position
        heldObject.transform.localPosition = Vector3.zero; 
        heldObject.transform.localRotation = Quaternion.identity; 
        objRB.angularVelocity = Vector3.zero;
    }
   }
    //checks the distance between the bot and the object
    void checkDistanceToObject(){
                 distance = Vector3.Distance(transform.position, shapeDestination.position);
                if(distance <= 3.0f ){
                    Debug.Log("grabbing");
                    GrabObject(shapeDestination.gameObject);    
                }
    }

    void checkDistanceToSlot(){
        distance = Vector3.Distance(transform.position,slotDestination.position);
        if(distance <= 3.0f){
            Debug.Log("dropping");
            DropObject();
        }
    }


}
