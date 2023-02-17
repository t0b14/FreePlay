using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootFireball : MonoBehaviour
{
    [SerializeField] GameObject fireball;
    [SerializeField] Transform spawnPoint;
    [SerializeField] float speedUpFireball;
    [SerializeField] float speedForwardFireball;
    void Update(){
        if (Input.GetKeyDown("space"))
        {
            ShootFireballs();
        }
    }

    public void ShootFireballs(){
        Vector3 eyeDisplacement = new Vector3(0f,10f,0f);
        GameObject aFireball= (GameObject)Instantiate(fireball, spawnPoint.position + eyeDisplacement, Quaternion.identity, transform);
        aFireball.GetComponent<Fireball>().SetInitial(speedForwardFireball, speedUpFireball, spawnPoint);
    }
}
