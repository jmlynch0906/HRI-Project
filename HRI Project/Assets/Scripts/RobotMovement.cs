using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    [SerializeField] private Transform originalShapeParent;
    [SerializeField] private Rigidbody objRB;

    [SerializeField] private bool pickedupShape = false;
    [SerializeField] private bool taskComplete = true;

    [SerializeField] private bool shapeDestinationSet = false;

    [SerializeField] private ExperimentObject[] roomObjects;

    [SerializeField] private String robotNum;


    //return point
    private Vector3 originalPosition;
    
    void Start()
    {
        originalPosition = transform.position;
        agent = GetComponent<NavMeshAgent>();
        agent.SetDestination(originalPosition);
        taskComplete = true;
        TextMeshPro text = GetComponentInChildren<TextMeshPro>();
        text.text = robotNum;
    }

    // Update is called once per frame
    void Update()
    {
        //robot returns home
        if(taskComplete){
            if (agent.destination != originalPosition)
            {
                agent.SetDestination(originalPosition);
            }
        }
        else{
            //robot moves to the slot
            if(pickedupShape){
                MoveObject();
                if(!shapeDestinationSet){
                    agent.SetDestination(slotDestination.position);
                    shapeDestinationSet = true;
                }
                checkDistanceToSlot();
            }
            //robot picks up the shape
            else{
                if(!shapeDestinationSet){
                    agent.SetDestination(shapeDestination.position);
                    obj = shapeDestination.gameObject;
                    shapeDestinationSet = true;
                }
                checkDistanceToObject();
            }
        }
    }

    //use this to issue commands to the robot.
    public void SetTask(Transform shapeDestination,Transform slotDestination)
    {
        DropObject();
        this.shapeDestination = shapeDestination;
        this.slotDestination = slotDestination;
        taskComplete = false;
    }

    //use this to issue commands to the robot.
    public void SetTask(int shapeIndex, Transform slotDestination)
    {
        DropObject();
        this.SetShapeDestinationTransform(shapeIndex);
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

            heldObject = shapeObject.gameObject;
            pickedupShape = true;
            shapeDestinationSet = false;
        }
    }

    void DropObject()
    {
        if (objRB != null & obj != null)
        {
            objRB.useGravity = true;
            objRB.drag = 1;
            objRB.constraints = RigidbodyConstraints.None;
            objRB.transform.parent = originalShapeParent;
            obj.GetComponent<ExperimentObject>().SetPosition(slotDestination);
        }
        ResetState();
    }

    void MoveObject()
    {
        if (heldObject != null)
        {   
            // Set the held object’s position to the hold area’s position
            heldObject.transform.localPosition = Vector3.zero; 
            heldObject.transform.localRotation = Quaternion.identity; 
            objRB.angularVelocity = Vector3.zero;
        }
    }

    //checks the distance between the bot and the object
    void checkDistanceToObject()
    {
        distance = Vector3.Distance(transform.position, shapeDestination.position);
        if(distance <= 3.0f ){
            Debug.Log("grabbing");
            GrabObject(shapeDestination.gameObject);    
        }
    }

    void checkDistanceToSlot()
    {
        distance = Vector3.Distance(transform.position,slotDestination.position);
        if(distance <= 3.0f){
            Debug.Log("dropping");
            DropObject();
        }
    }

    public void SetSlotDestinationTransform(Transform slotTransform)
    {
        slotDestination = slotTransform;
    }

    public void SetShapeDestinationTransform(int shapeIndex)
    {
        if(shapeIndex > roomObjects.Length)
        {
            shapeIndex = 0;
        }
        shapeDestination = roomObjects[shapeIndex].transform;
    }

    private void ResetState()
    {
        agent.ResetPath();
        agent.SetDestination(originalPosition);
        obj = null;
        objRB = null;
        originalShapeParent = null;
        heldObject = null;
        pickedupShape = false;
        shapeDestinationSet = false;
        taskComplete = true;

        shapeDestination = null;
        slotDestination = null;
    }
}
