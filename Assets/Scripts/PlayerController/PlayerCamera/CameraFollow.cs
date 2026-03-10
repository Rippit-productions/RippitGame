using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // Public variable to link to the player's transform

    private Vector3 offset; // Store the initial offset from the camera to the player

    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position - player.position;
    }

    // LateUpdate is called once per frame after Update, 
    // ensuring that the player's move for that frame has been completed
    void LateUpdate()
    {
        transform.position = new Vector3(player.position.x + offset.x - 3, player.position.y + offset.y, transform.position.z);
    }
}
