using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RisePillar : MonoBehaviour
{

    [SerializeField] Vector3 StartPosition;
    [SerializeField] Vector3 EndPosition;
    [SerializeField] float travelTime;
    float initialTime;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = StartPosition;
        initialTime = Time.time;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(Time.time < initialTime + travelTime){
            float timeFactor = (Time.time - initialTime) / travelTime;
            transform.position = Vector3.Lerp(StartPosition, EndPosition, timeFactor);
        }
    }

    public void SetInit(Vector3 newStart, Vector3 newEnd, float t){
        StartPosition = newStart;
        EndPosition = newEnd;
        travelTime = t;
    }
}
