using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pillar : MonoBehaviour
{
    [SerializeField] GameObject pillars;
    [SerializeField] float maxPillarRange = 10f;
    [SerializeField] Camera PlayerCam;
    [SerializeField] GameObject signal;
    [SerializeField] LayerMask lMask;
    [SerializeField] float riseTime;
    [SerializeField] Vector2 clusterDimensions;
    [SerializeField] float lifeDuration;
    Vector3 nowhere = new Vector3(1e6f,1e6f,1e6f);
    Vector3 groundPos;

    void Update(){
        if (Input.GetKey("2"))
        {
            Ray ray = PlayerCam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, maxPillarRange,lMask))
            {
                    signal.transform.position = hit.point;
                    groundPos = hit.point;
            }

        }
        if (Input.GetKeyUp("2"))
        {
            signal.transform.position = nowhere;
            GameObject pillarCluster = (GameObject)Instantiate(pillars, groundPos, Quaternion.identity, transform);
            pillarCluster.GetComponent<PillarCluster>().SetInitial(groundPos, riseTime, clusterDimensions.x, clusterDimensions.y, lifeDuration);
        }
    }
}
