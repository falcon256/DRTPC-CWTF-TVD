using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowRotate : MonoBehaviour
{
    private void FixedUpdate()
    {
        this.transform.Rotate(0, 0.2f, 0);
    }
}
