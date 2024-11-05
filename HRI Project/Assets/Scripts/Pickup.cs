using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Pickup : MonoBehaviour
{
   [SerializeField] Transform holdArea;
   [SerializeField] private GameObject heldObject;
   private Rigidbody objRB;
   [SerializeField] private float pickupRange = 5.0f;
   [SerializeField] private float pickupForce = 150.0f;
   private Transform originalParent;

   

   private void Update(){
    if(Input.GetMouseButtonDown(0)){
        if(heldObject == null){
            RaycastHit hit;
           if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, pickupRange, LayerMask.GetMask("Object"))){
                //pick up object
                GrabObject(hit.transform.gameObject);
            }
        }
        else{
            //drop object
            DropObject();
        }
    }
    if(heldObject != null){
        //move object
        MoveObject();
    }
   }

   void GrabObject(GameObject obj){
        if(obj.GetComponent<Rigidbody>()){
            originalParent = obj.transform.parent;
            objRB = obj.GetComponent<Rigidbody>();
            objRB.useGravity = false;
            objRB.drag = 10;
            objRB.constraints = RigidbodyConstraints.FreezeRotation;
            
            objRB.transform.parent = holdArea;
            heldObject = obj;
        }
   }

      void DropObject(){
            objRB.useGravity = true;
            objRB.drag = 1;
            objRB.constraints = RigidbodyConstraints.None;
            objRB.transform.parent = originalParent;
            originalParent = null;
            heldObject = null;
   }
   void MoveObject(){
     if (heldObject != null)
    {
        // Set the held object’s position to the hold area’s position
        objRB.MovePosition(holdArea.position);
    }
   }

}
