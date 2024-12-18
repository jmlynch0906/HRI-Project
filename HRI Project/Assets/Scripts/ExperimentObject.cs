using System.Collections;
using System.Collections.Generic;
using TMPro;

//using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

public class ExperimentObject : MonoBehaviour
{
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

   private TextMeshPro text;

    //variables for resetting the position
    
    private Vector3 pos;
    private Quaternion ros;
    private Rigidbody rb;

    [SerializeField] private Texture image;

    
    //binds the strings to the specific objects/meterials
    public Dictionary<string,GameObject> shapeobjects;
    public Dictionary<string,Material> colorMaterials;
    
    // Start is called before the first frame update
    void Start()
    {
        //for resetting the position
     pos = transform.position;  
     Debug.Log("position: " + pos);
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

     rb = GetComponentInChildren<Rigidbody>();
     text = GetComponentInChildren<TextMeshPro>();
     text.text = room;
    }

    // Update is called once per frame
    void Update()
    {

    }

    //sets the prefab as the proper object
    void setAttributes(){
        if (sphereObject != null)
            sphereObject.SetActive(false);
        if (cubeObject != null)
            cubeObject.SetActive(false);
        if (triangleObject != null)
            triangleObject.SetActive(false);

        if(shapeobjects.TryGetValue(shape, out GameObject correctShape)){
            if (correctShape != null) {
                correctShape.SetActive(true);
                if (colorMaterials.TryGetValue(color, out Material correctMaterial))
                {
                    correctShape.GetComponent<Renderer>().material = correctMaterial;

                }
            }

        }

        
    }

    // Returns True if the provided object matches this one
    public bool Matches(ExperimentObject obj)
    {
        return room.Equals(obj.room) & color.Equals(obj.color) & shape.Equals(obj.shape);
    }

    // Allows us to programatically initialize an instance ExperimentObjeect
    public void Init(string room, string color, string shape)
    {
        this.room = room;
        this.color = color;
        this.shape = shape;
    }

    //assigns the attributes of this object to a tile
    public void AssignToTile(int tilenum)
    {
        Slot slot = GameObject.Find("Slot"+tilenum).transform.GetComponentInChildren<Slot>();
        if (slot)
        {
            slot.setGoal(color,shape,room,image);
        }
        else
        {
            Debug.LogError($"Slot is Null For Tile Number : {tilenum}");
        }

    }

    public void SetPosition(Transform location)
    {
        if (rb != null)
        //reset to original position
        {   if(location == null){
            rb.transform.position = pos;
            rb.transform.rotation = ros;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
            else{
                rb.transform.position = location.position;
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }

    
    
    // Returns this Object as a string in json format
    public string Serialize()
    {
        return $"{{ \"room\": \"{room}\", \"shape\": \"{shape}\", \"color\": \"{color}\" }}";
    }


}
