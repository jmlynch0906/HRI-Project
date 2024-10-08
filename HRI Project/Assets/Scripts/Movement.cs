using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Movement : MonoBehaviour

{
public GameObject target_tile;
public float speed = 2.0f;
public float rotationSpeed = 5.0f;
private KeyCode left = KeyCode.A;
private KeyCode right = KeyCode.D;
private bool moving = false;


    // this is a barebones script for getting an object to move between two tiles. This is just to get the basic ideas of movement down
    void Update()
    {   //only two options, done with keys. In the final product, we can have buttons that call the move function for different objects and tiles. or voice commands that call it.
        if(Input.GetKeyDown(left)){
            moving = true;
            target_tile = GameObject.Find("L1");
        }
        if(Input.GetKeyDown(right)){
            moving = true;
            target_tile = GameObject.Find("R1");
        }

        if (moving){
            move(target_tile.transform.position);
        }
    }
    //function that handles moveing the object to the tile
    void move(Vector3 tile_pos){
        //movement
        transform.position = Vector3.MoveTowards(transform.position, tile_pos, speed * Time.deltaTime);
        //rotation (purely for style)
        Vector3 directionToTarget = tile_pos - transform.position;
        directionToTarget.y = 0; 
        if (directionToTarget != Vector3.zero) 
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, speed * Time.deltaTime);
        }
        //target reached
        if(Vector3.Distance(transform.position, tile_pos) < 1f){

            moving = false;
        }
    }
}
