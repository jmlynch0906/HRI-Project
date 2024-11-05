using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screen : MonoBehaviour
{

    [SerializeField] private string screnNum;

    private Material material;
    private void Start(){
        material = GetComponent<Renderer>().material;
    }    

    //sets the image that'll appear on the screen
    public void setImage(Texture image){
        material.mainTexture = image;
    }
    //changes the background color of the screen. will change to green for correctness.

    public void setColor(Color color){
        material.color = color;
    }
}
