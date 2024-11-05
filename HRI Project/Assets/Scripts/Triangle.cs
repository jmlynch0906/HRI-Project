using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Triangle : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
      Mesh mesh = new Mesh();

        // Define the vertices
        Vector3[] vertices = {
            new Vector3(0, 1, 0), // Top vertex
            new Vector3(-0.5f, 0, -0.5f), // Bottom left
            new Vector3(0.5f, 0, -0.5f), // Bottom right
            new Vector3(0.5f, 0, 0.5f), // Top right
            new Vector3(-0.5f, 0, 0.5f) // Top left
        };

        // Define the triangles
        int[] triangles = {
            0, 2, 1, // Side 1 (flipped)
            0, 3, 2, // Side 2 (flipped)
            0, 4, 3, // Side 3 (flipped)
            0, 1, 4, // Side 4 (flipped)
            1, 2, 3, 3, 4, 1 // Base (optional, can leave as is)
        };

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = mesh;  

        
        GetComponent<MeshCollider>().sharedMesh = mesh; // Assign the mesh to the collider
        GetComponent<MeshCollider>().convex = true; // Set to true if you need it to be convex
    }

}
