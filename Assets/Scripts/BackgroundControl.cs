using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundControl : MonoBehaviour
{
    public GameObject background, player;
    private Vector3 lastPosition;
    public float parallaxSpeed;

    private void Start()
    {
        lastPosition = player.transform.position;
    }
    private void Update()
    {
        Vector3 distance = player.transform.position - lastPosition;
        background.transform.position += new Vector3(parallaxSpeed*distance.x, distance.y, 0);
        lastPosition = player.transform.position;
    }
}
