using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * LegPairControl manages the state of a set of 4 legs
 * While opposing legs are moving, prevent the others from doing so until complete.
 */
public class LegPairControl : MonoBehaviour {
    // Start is called before the first frame update
    public LegControl left;
    public LegControl right;
    public LegControl backLeft;
    public LegControl backRight;

    void Awake() {
        // always try to move set of legs
        StartCoroutine(MovePair());
    }

    // Update is called once per frame
    void Update() {

    }

    //https://www.youtube.com/watch?v=GtHzpX0FCFY&ab_channel=animan1999 Animation structure
    // https://weaverdev.io/blog/bonehead-procedural-animation code for development
    // Only diagonal leg pairs can tryMove() together
    public IEnumerator MovePair() {
        while (true) {
            //try moving one diagonal pair of legs
            do {
                // allow one of these legs to move at a time
                left.TryMove();
                backRight.TryMove();
                yield return null;

                // If backRight or front Left is moving, stay within the loop
            } while (backRight.moving || left.moving);

            //perform the same action for the opposing legs
            do {
                // allow one of these legs to move at a time
                right.TryMove();
                backLeft.TryMove();
                yield return null;
            } while (backLeft.moving || right.moving);
        } 
    }
}

