using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/*
 * Field of View Script
 * Adds any GameObject.Transforms from the GameObjects with the layer "player" to a list
 * Check sees if player is within view of angle
 * 
 * References:
 * https://www.youtube.com/watch?v=rQG9aUWarwE&ab_channel=SebastianLague
 */
public class FieldOfView : MonoBehaviour {

	// Radius the AI can see
	public float viewRadius;

	// Angle the bee can see, range prevents angle larger than possible
	[Range(0, 360)]
	public float viewAngle;

	public LayerMask targetMask;
	public LayerMask obstacleMask;

	// List of Player Objects visible
	public List<Transform> visibleTargets = new List<Transform>();

	void Start() {
		StartCoroutine("FindTargetsWithDelay", .2f);
	}


	IEnumerator FindTargetsWithDelay(float delay) {
		while (true) {
			yield return new WaitForSeconds(delay);
			FindVisibleTargets();
		}
	}

	// Locate all targets within angle
	void FindVisibleTargets() {
		visibleTargets.Clear();

		// get array of all colliders within viewRadius, filtered to only return objects using targetMask (Player layer in this case) 
		Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

		//iterate through all in array, should typically only expect one
		for (int i = 0; i < targetsInViewRadius.Length; i++) {
			// set transform from collider
			Transform target = targetsInViewRadius[i].transform;
			// create direction vector3, normalise to 1
			Vector3 dirToTarget = (target.position - transform.position).normalized;
			// Check if the transform is within view angle
			if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2) {
				// log distance from ai
				float dstToTarget = Vector3.Distance(transform.position, target.position);

				// if there are no obstacles in the way of view, add to List of visible targets
				// this stops ai from seeing player through walls
				if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask)) {
					visibleTargets.Add(target);
				}
			}
		}
	}

	// used in FOVEditor to generate angle and display it on screen
	public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal) {
		if (!angleIsGlobal) {
			angleInDegrees += transform.eulerAngles.y;
		}
		return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
	}
}