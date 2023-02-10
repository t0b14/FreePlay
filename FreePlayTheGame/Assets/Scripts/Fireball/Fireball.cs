using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    float speedUp;
    float speedForward;
    Transform spawnPoint;
    [SerializeField] GameObject smallFireball;
    float sizeThold = 0.25f;
    bool hasSplit = false;

    // Start is called before the first frame update
    void Start()
    {
        if(spawnPoint){
            transform.GetComponent<Rigidbody>().velocity = (spawnPoint.forward* speedForward + spawnPoint.up* speedUp);
            transform.position = spawnPoint.position;
        }

        
    }
    void OnCollisionEnter(Collision collision){
        Explode(collision);
    }

    void Explode(Collision collision){
        
        if(transform.localScale.x >= sizeThold && !hasSplit){
            float offsetSize = 0.5f;
            Vector3 offset = new Vector3(Random.Range(-offsetSize, offsetSize), 0f, Random.Range(-offsetSize, offsetSize));
            GameObject aSmallFireball = (GameObject)Instantiate(smallFireball, transform.position + offset, Quaternion.identity);
            aSmallFireball.transform.localScale = transform.localScale * 0.9f;
            //aSmallFireball.GetComponent<Rigidbody>().velocity = collision.transform.up * 2f*speedUp;
            hasSplit = true;
            // the smaller the more likely to switch has Split
            if(Random.Range(-0.75f,1f) > transform.localScale.x){
                hasSplit = false;
            }
        }
        float lifetime = 1f;
        if(transform.localScale.x == 1) Destroy(gameObject); 
        else{
            Destroy(gameObject, Random.Range( 0f, lifetime*(1f - transform.localScale.x)) );
        }
        
    }
    
    public void SetInitial(float sfor, float sup, Transform spoint){
        speedForward = sfor;
        speedUp = sup;
        spawnPoint = spoint;
    }
}
