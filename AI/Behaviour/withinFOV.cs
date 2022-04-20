using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WithinFOV : MonoBehaviour
{
    public float distance;
    public FieldOfView fov;
    public GameObject player;
    public GameObject seenPlayerPrefab;
    public float distanceToFlee = 50;
    private NoticedPlayer NoticedPlayer;

    public void Start() {
        fov = this.GetComponent<FieldOfView>();
        NoticedPlayer = GetComponent<NoticedPlayer>();

    }

    public void Update() {


        distance = Vector3.Distance(transform.position, player.transform.position);


        if (fov.visibleTargets.Count > 0 | distance < distanceToFlee) {
            if(seenPlayerPrefab) {
                Debug.Log(Vector3.Distance(transform.position, player.transform.position));
                NoticedPlayer.ShowNoticedPlayer();

            }
        } 
    }





}

