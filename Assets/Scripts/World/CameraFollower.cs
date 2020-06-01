using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    [SerializeField] GameObject player;

    void Update () 
    {
        transform.position = player.transform.position;
    }
}
