using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * FloatingText manipulates the TextMesh shown when the AI notices/sees the player.
 * 1. Destroys the mesh after an amount of time
 * 2. Rotates the mesh to always face camera
 */


public class FloatingText : MonoBehaviour
{

    public float DestroyTime = 6f;
    public Transform textMeshTransform;
    public GameObject player;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        // After mesh is created, use DestroyTime to remove 
        Destroy(gameObject, DestroyTime);
        textMeshTransform = GetComponent<TextMesh>().transform;
    }


    void Update()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        // Rotate the text mesh every frame to face the camera
        textMeshTransform.rotation = Quaternion.LookRotation(textMeshTransform.position - Camera.main.transform.position);
    }
}
