

using System.Collections;
using UnityEngine;

/*
 * Prefab Instantiate doesn't work with withinFOV because of the ':Conditional' class
 * Class creates prefab in scene and sets position = parent object (bee AI)
 */
public class NoticedPlayer : MonoBehaviour {
    public GameObject seenPlayerPrefab;

    public void ShowNoticedPlayer() {
        Instantiate(seenPlayerPrefab, transform.position, Quaternion.identity, transform);
    }

}