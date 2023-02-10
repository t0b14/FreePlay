using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootFireball : MonoBehaviour
{
    [SerializeField] GameObject fireball;
    [SerializeField] Transform spawnPoint;
    [SerializeField] float speedUpFireball;
    [SerializeField] float speedForwardFireball;

    public void ShootFireballs(){
        GameObject aFireball= (GameObject)Instantiate(fireball, spawnPoint.position, Quaternion.identity);
        aFireball.GetComponent<Fireball>().SetInitial(speedForwardFireball, speedUpFireball, spawnPoint);
    }
}
