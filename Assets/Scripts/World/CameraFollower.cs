using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    public void GetPlayerPos(Transform player)
    {
        transform.position = player.position;
    }
}
