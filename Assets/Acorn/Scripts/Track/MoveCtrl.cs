using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCtrl : MonoBehaviour
{
    private float speedRate = 4f;

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 speed = new Vector3(horizontal, vertical, 0) * speedRate;
        transform.position += speed * Time.deltaTime;
    }
}
