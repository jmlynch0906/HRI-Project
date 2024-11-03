using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object : MonoBehaviour
{
    private Vector3 pos;
    public string room;
    public string color;
    public string shape;

    //objects and materials set in inspector
    public GameObject sphereObject;
    public GameObject cubeObject;
    public GameObject triangleObject;

    public Material red;
    public Material green;
    public Material blue;

    
    //binds the strings to the specific objects/meterials
    public Dictionary<string,GameObject> shapeobjects;
    public Dictionary<string,Material> colorMaterials;
    
    // Start is called before the first frame update
    void Start()
    {
        //for resetting the position
     pos = transform.position;  
        //dictionaries must be assigned in Start because inspector references are not avaliable when the dictionaries are first initialized
     shapeobjects = new Dictionary<string, GameObject>{
        {"sphere", sphereObject},
        {"cube", cubeObject},
        {"triangle", triangleObject}
     };
     colorMaterials = new Dictionary<string, Material>{
        {"red",red},
        {"blue",blue},
        {"green",green}
     };
     setAttributes();
    }

    // Update is called once per frame
    void Update()
    {
    }

    //sets the prefab as the proper object
    void setAttributes(){
        sphereObject.SetActive(false);   
        cubeObject.SetActive(false);
        triangleObject.SetActive(false);

        if(shapeobjects.TryGetValue(shape, out GameObject correctShape)){
            correctShape.SetActive(true);
            if(colorMaterials.TryGetValue(color, out Material correctMaterial)){
                correctShape.GetComponent<Renderer>().material = correctMaterial;
            }
        }

        
    }
}
