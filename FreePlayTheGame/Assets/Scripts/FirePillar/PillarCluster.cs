using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillarCluster : MonoBehaviour
{
    [SerializeField] GameObject pillar;
    [SerializeField] Vector3 spawnPoint;
    [SerializeField] float travelTime;
    [SerializeField] float xSize;
    [SerializeField] float zSize;
    [SerializeField] GameObject parent;
    [SerializeField] float deathTime;
    bool fall = false;

    private void Start() {
        CastPillars();
        DieGracefully();
        transform.SetParent(parent.transform);
    }

    public void CastPillars(){
        spawnPoint -= new Vector3(xSize/2f,0f,zSize/2f);
        for(float i=0; i < xSize; ++i){
            for(float j=0; j < zSize; ++j){
                CastaPillar(i,j);
            }
        }
    }

    void CastaPillar(float i, float j){
        Vector3 displacement = new Vector3(i,0f,j);
        GameObject aPillar= (GameObject)Instantiate(pillar, new Vector3(0f,0f,0f), Quaternion.identity, transform);
        if(!parent){ 
            parent = FindObjectOfType<Holder>().gameObject;
        }
        
        aPillar.GetComponent<RisePillar>().SetInit(spawnPoint + displacement, spawnPoint + new Vector3(0f,Random.Range(1.5f,2.5f),0f) + displacement, travelTime);
    }

    public void SetInitial(Vector3 sPoint, float tTime, float newXSize, float newZSize, float dTime){
        spawnPoint = sPoint - new Vector3(0f,2.5f,0f);
        travelTime = tTime;
        xSize = newXSize;
        zSize = newZSize;
        deathTime = dTime;
    }

    void DieGracefully(){
        Debug.Log("i want to die");
        Destroy(gameObject,deathTime);
        StartCoroutine(Fall());
    }

    IEnumerator Fall(){
        yield return new WaitForSeconds(0.8f * deathTime);
        fall = true;
    }

    void FixedUpdate(){
        if(fall){
            transform.position -= new Vector3(0f,2f * Time.deltaTime,0f);
        }
    }

}
