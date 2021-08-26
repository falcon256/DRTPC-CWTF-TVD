using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScanningSystem : MonoBehaviour
{
    public SigCube sigCube = null;
    public float scanVertAngle = 45.0f;
    public float scanHorizAngle = 45.0f;
    private float curVertAngle = 0;
    private float curHorizAngle = 0;
    private float curVertDirection = 1.0f;
    private float curHorizDirection = 1.0f;
    public float angleDelta = 0.5f;
    public float maxScanningDurationInms = 50.0f;
    public int maxInterationsPerFrame = 1000;
    public float RaycastNoise = 0.01f;

   void FixedUpdate()
    {
        if (!sigCube)
        {
            Debug.LogError("Set the sigcube in the SAR");
            return;
        }
        float starttime = Time.realtimeSinceStartup;

        int iterations = 0;
        //start our scan, keep going while we have calculation time.
        while(Time.realtimeSinceStartup-starttime<maxScanningDurationInms*0.001f&&iterations<maxInterationsPerFrame)
        {
            iterations++;
            //keep our scanning within bounds
            if(curHorizAngle > scanHorizAngle)
            {
                curHorizDirection = -1;
                curVertAngle += curVertDirection * angleDelta;
            }
            if (curHorizAngle < -scanHorizAngle)
            {
                curHorizDirection = 1;
                curVertAngle += curVertDirection * angleDelta;
            }
            if (curVertAngle > scanVertAngle)
                curVertDirection = -1.0f;
            if (curVertAngle < -scanVertAngle)
                curVertDirection = 1.0f;

            //set up our raytracing
            this.transform.rotation = Quaternion.Euler(curVertAngle, curHorizAngle+180.0f, 0);
            RaycastHit[] hits = Physics.RaycastAll(this.transform.position, this.transform.forward+(Random.insideUnitSphere*RaycastNoise), 100.0f);
            for(int i = 0; i < hits.Length; i++)
            {

                if((hits!=null)&&(hits[i].rigidbody!=null))//if it is a valid hit and has a valid rigidbody
                {
                    //Debug.Log(hits[i].rigidbody.tag);//debug

                    if(hits[i].rigidbody.gameObject.tag == "SARTarget")//it is one of our SAR targets, thus valid;
                        logDetection(hits[i].point, 1.0f);
                }
            }
            curHorizAngle += curHorizDirection * angleDelta;
        }
    }

    private void logDetection(Vector3 pos, float intensity)
    {
        //Debug.Log("Hit" + pos);//debug

        //first check to make sure our detection is in the simulation space, hardcoded for time limits.
        if (pos.x > 5.0f ||
            pos.y > 5.0f ||
            pos.z > 5.0f ||
            pos.x < -5.0f ||
            pos.y < -5.0f ||
            pos.z < -5.0f)
            return;
        
        //log the detection in the volume data
        sigCube.markDetection(
            (int)(((pos.x + 5.0f) / 10.0f) * ((float)sigCube.volumeResolution)),
            (int)(((pos.y + 5.0f) / 10.0f) * ((float)sigCube.volumeResolution)),
            (int)(((pos.z + 5.0f) / 10.0f) * ((float)sigCube.volumeResolution)),
            (int)(intensity));
    }
}
