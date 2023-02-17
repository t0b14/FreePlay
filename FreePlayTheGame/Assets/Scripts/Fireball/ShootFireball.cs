using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootFireball : MonoBehaviour
{
    float fireTime = 0f;
    Vector3 ballSize = new Vector3(1f,1f,1f);
    [SerializeField] GameObject fireball;
    [SerializeField] Transform spawnPoint;
    [SerializeField] float speedUpFireball;
    [SerializeField] float speedForwardFireball;
    void Update(){
        if (Input.GetKeyDown("space"))
        {
            fireTime = Time.time;
        }
        if (Input.GetKeyUp("space"))
        {
            if(fireTime > 0f){
                fireTime = Time.time - fireTime;
                float sizeMultiplier = Mathf.Log(fireTime+2f,2f);
                ShootFireballs(sizeMultiplier);
            }
        }
    }

    public void ShootFireballs(float sizeMultiplier){
        Vector3 eyeDisplacement = new Vector3(0f,10f,0f);
        GameObject aFireball= (GameObject)Instantiate(fireball, spawnPoint.position + eyeDisplacement, Quaternion.identity, transform);
        aFireball.GetComponent<Fireball>().SetInitial(speedForwardFireball, speedUpFireball, spawnPoint, sizeMultiplier*ballSize);
    }
}
