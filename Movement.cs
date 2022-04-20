using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// movement manager
public class Movement : MonoBehaviour {
    public CharacterController controller;

    public float speed = 10f;
    public float rotateSpeed = 30f;
    public float legSpeed;
    public float legDist;

    public float turnSmoothTime = 0.1f;
    public float turnSmoothVelocity;


    // controls to manage and manipulate movement. Attatched to body controller
    void Update() {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 move = (transform.forward * -1) * vertical + (transform.right * -1) * horizontal;
        controller.Move(speed * Time.deltaTime * move);

        if (Input.GetKey(KeyCode.E))
            transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
        if (Input.GetKey(KeyCode.Q))
            transform.Rotate(-Vector3.up * rotateSpeed * Time.deltaTime);
        
    }
}
