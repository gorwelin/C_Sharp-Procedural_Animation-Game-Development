using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class BodyController : MonoBehaviour
{
    public GameObject body;
    public LegControl[] legs = new LegControl[8];
    public TargetPoint[] targetPoints = new TargetPoint[8];
    public GameObject centerPoint;
    public Vector3 bodyOffset;
    private Vector3 sum;

    private Vector3 GroundDir;
    private float GravityRotationSpeed = 10f;

    private bool groundedPlayer;


    // Start is called before the first frame update
    void Start() {

 
        sum = Vector3.zero;
        GroundDir = transform.up;

    }

    private void Awake() {
        centerPoint.transform.position = LegCenter();
        StartCoroutine(TransBody());
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        centerPoint.transform.position = LegCenter();

        //Vector3.Lerp(GroundDir, SetGroundDir, Time.deltaTime * GravityRotationSpeed);
        RotateSelf(FloorAngleCheck(), Time.deltaTime, GravityRotationSpeed);
       


        /*
        for (int i = 1; i < legs.Length-1; i++) {
            if (legs[i - 1].distance >= legs[i - 1].maxDistance) {
                if (legs[i].isGrounded) {
                    StartCoroutine(legs[i-1].MoveLeg());
                }
            }
        }
        */



        /*
            if(legs[i].distance >= legs[i].maxDistance) {

                switch(i) {
                    case 0:
                        if (legs[i + 1].isGrounded) {
                            StartCoroutine(legs[i].MoveLeg());
                        }
                        break;
                    case 7:
                        if (legs[i - 1].isGrounded) {
                            StartCoroutine(legs[i].MoveLeg());
                        }
                        break;
                    default:
                        if (legs[i - 1].isGrounded && legs[i + 1].isGrounded) {
                            StartCoroutine(legs[i].MoveLeg());
                        }
                        break;
                }

            }
        */





    }


    private Vector3 LegCenter() {
        sum = Vector3.zero;
        foreach (LegControl leg in legs) {
            sum += leg.transform.position;
        }

        return sum / legs.Length;
    }

    private Vector3 TargetCenter() {
        sum = Vector3.zero;
        foreach (TargetPoint tp in targetPoints) {
            sum += tp.transform.position;
        }

        return sum / targetPoints.Length;
    }

    private IEnumerator TransBody() {
        while(true) {
            float bodyHeight = LegCenter().y + bodyOffset.y;
            transform.position = new Vector3(transform.position.x, bodyHeight, transform.position.z);
            yield return null;
        }
    }

    //https://www.youtube.com/watch?v=QSDUA9YpVwQ&ab_channel=SlugGlove code reference from this video
    Vector3 FloorAngleCheck() {

        RaycastHit hitF;
        RaycastHit hitM;
        RaycastHit hitB;

        Physics.Raycast(transform.position + (transform.forward * 10), -transform.up, out hitF);
        Physics.Raycast(transform.position, -transform.up, out hitM);
        Physics.Raycast(transform.position - (transform.forward * 10), -transform.up, out hitB);

        Debug.DrawRay(transform.position + (transform.forward * 10), Vector3.down * 20, Color.red);
        Debug.DrawRay(transform.position, Vector3.down * 20, Color.red);
        Debug.DrawRay(transform.position - (transform.forward * 10), Vector3.down * 20, Color.red);

        Vector3 hitDir = transform.up;

        if (hitF.transform != null) {
            hitDir += hitF.normal;
        }
        if (hitM.transform != null) {
            hitDir += hitM.normal;
        }
        if (hitB.transform != null) {
            hitDir += hitB.normal;
        }

        Debug.DrawLine(transform.position, transform.position + (hitDir.normalized * 5f), Color.red);

        return hitDir.normalized;

    }

    void RotateSelf(Vector3 Direction, float d, float GravitySpd) {
        Vector3 LerpDir = Vector3.Lerp(transform.up, Direction, d * GravitySpd);
        Quaternion targetRot = Quaternion.Euler(LerpDir.x, LerpDir.y, LerpDir.z);

        transform.rotation = Quaternion.FromToRotation(transform.up, LerpDir) * transform.rotation;
    }
}
