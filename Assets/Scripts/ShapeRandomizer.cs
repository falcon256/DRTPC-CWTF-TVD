using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeRandomizer : MonoBehaviour
{
    private void FixedUpdate()
    {
        if(Input.GetKey(KeyCode.R))
        {
            this.GetComponent<Rigidbody>().AddForce(Random.insideUnitSphere*Random.Range(0,100.0f) * Random.Range(0, 100.0f));
        }
        if (this.transform.position.y < -20)
            this.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
    }
}
