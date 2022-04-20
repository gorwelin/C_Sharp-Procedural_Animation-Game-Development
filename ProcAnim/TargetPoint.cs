using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Object which is used as reference for where leg should move to relative to it's current position
 *
 */
public class TargetPoint : MonoBehaviour {

    public GameObject IKtarget;
    private Vector3 dir;

    void Start() {
        transform.position = IKtarget.transform.position;
        dir = new Vector3(0, -1, 0);
    }

    void FixedUpdate() {

        RaycastHit hit;
        //Height of the raycast
        dir = Vector3.down * 20;
   
        Debug.DrawRay(transform.position + new Vector3(0, 10, 0), dir, Color.green);

        //Raycast to check change in height and set y position of TargetPoint to hit point
        if (Physics.Raycast(transform.position + new Vector3(0, 10, 0), dir, out hit)) {
            
            Vector3 temp = transform.position;
            temp.y = hit.point.y;
            transform.position =  temp;
        }



    }

}
