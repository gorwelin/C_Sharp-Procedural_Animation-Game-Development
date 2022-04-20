using System.Collections;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

/*
 * Attatched to every leg's IK controller
 * Performs all animation and checks to ensure procedural works as expected
 */
public class LegControl : MonoBehaviour {

    // Object the leg moves to
    public GameObject targetPoint;
    // Maximum distance a leg can be from its targetpoint before tryMove() is called
    public float maxDistance = 3;
    // distance between leg's IK(LegControl) and TargetPoint
    public float distance;
    // Bool to check if each leg is moving for LegPairControl
    public bool moving;
    // Overshoot how much leg moves
    public float stepOvershootFraction;
    // Time in seconds a leg takes to move
    public float moveDuration = 1;

    //always monitor the original, target position and direction between the two
    private Vector3 fromPosition;
    private Vector3 toPosition;
    private Vector3 direction;

    // Start is called before the first frame update
    void Start() {
        var hits = Physics.RaycastAll(transform.position + Vector3.up, Vector3.down, 10f);
        moving = false;

        // https://unity3d.college/2017/09/26/using-unity-editor-extensions-to-snap-to-ground-when-placing-gameobjects/
        /*
         * Editor script which snaps the position to highest point below GameObject
         */
        foreach (var hit in hits) {
            if (hit.collider.gameObject == transform.gameObject)
                continue;

            transform.position = hit.point;
            break;
        }

    }


    void FixedUpdate() {

        // constantly update positions, distance and direction per physics update
        fromPosition = transform.position;
        toPosition = targetPoint.transform.position;
        distance = Vector3.Distance(fromPosition, toPosition);
        direction = transform.position - targetPoint.transform.position;

        // Show each leg position and where its moving to, visual representation of maxDistance factor
        Debug.DrawLine(fromPosition, toPosition, Color.red);


    }

    // only allow coroutine to begin if it's not already moving
    public void TryMove() {
        if (moving) {
            return;
        }

        // Check distance before calling coroutine
        if(distance > maxDistance) {
            StartCoroutine(Move());
        }
    }


    /*
     * https://weaverdev.io/blog/bonehead-procedural-animation reference for movement script
     * 
     * Performs the animation of moving from its current position to targetPoint
     * originally used MoveTowards() but would never truly meet target so wasn't efficient, especially at speed
     */
    public IEnumerator Move() {
        moving = true;

        Vector3 startPoint = transform.position;
        Quaternion startRot = transform.rotation;
        Quaternion endRot = targetPoint.transform.rotation;

        // vector from the leg's position to TargetPoint
        Vector3 direction = (targetPoint.transform.position - transform.position);

        // Total distance to overshoot by using maxDistance
        float overshootDisance = maxDistance * stepOvershootFraction;
        Vector3 overshootVect = direction * overshootDisance;

        // Apply the overshoot
        Vector3 endPoint = targetPoint.transform.position + overshootVect;

        // pass through the point between start and end
        Vector3 centerPoint = (startPoint + endPoint) / 2;

        // lift the leg up in the center point to imitate typical leg movement
        centerPoint += targetPoint.transform.up * Vector3.Distance(startPoint, endPoint) / 2f;

        float timeElapsed = 0;
        do {
            timeElapsed += Time.deltaTime;
            float normalizedTime = timeElapsed / moveDuration;
            normalizedTime = Easing.InOutCubic(normalizedTime);

            // Quadratic Bezier curve
            transform.position =
                Vector3.Lerp(
                    Vector3.Lerp(startPoint, centerPoint, normalizedTime),
                    Vector3.Lerp(centerPoint, endPoint, normalizedTime),
                    normalizedTime
                );

            // alter the rotation using quarternion interpolation
            transform.rotation = Quaternion.Slerp(startRot, endRot, normalizedTime);

            yield return null;

            // perform this movement until duration is met
        } while (timeElapsed < moveDuration);

        //movement is complete
        moving = false;


    }

}

