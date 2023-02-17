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
    bool chainReact = false;
    bool hasSplit = false;

    // Start is called before the first frame update
    void Start()
    {
        if(spawnPoint){
            transform.GetComponent<Rigidbody>().velocity = (spawnPoint.forward* speedForward + spawnPoint.up* speedUp);
            transform.position = spawnPoint.position;
        }
        if(chainReact == true){
            ChainReaction();
        }
    }

    void OnCollisionEnter(Collision collision){
        Explode();
    }

    void Explode(){
        
        if(hasSplit == false){
            if(chainReact == false){
                GameObject aSmallFireball = (GameObject)Instantiate(smallFireball, transform.position, Quaternion.identity);
                aSmallFireball.transform.localScale = transform.localScale * 0.9f;
                aSmallFireball.GetComponent<Fireball>().SetChainReact(true);
            }
            hasSplit = true;
            Destroy(gameObject, 1f);
        } 
    }
    
    public void SetInitial(float sfor, float sup, Transform spoint){
        speedForward = sfor;
        speedUp = sup;
        spawnPoint = spoint;
    }

    void ChainReaction(){
        if(transform.localScale.x >= sizeThold){
            SpawnNewFireball();
        }
        
        // add many more balls when small
        // the smaller the more likely to switch has Split
        if(Random.Range(-1f,0.75f) > transform.localScale.x){
            ChainReaction();
        }
    }
    public void SetChainReact(bool value){
        chainReact = value;
    }

    void SpawnNewFireball(){
            float offsetSize = 0.5f;
            Vector3 offset = new Vector3(Random.Range(-offsetSize, offsetSize), 0f, Random.Range(-offsetSize, offsetSize));
            GameObject aSmallFireball = (GameObject)Instantiate(smallFireball, transform.position + offset, Quaternion.identity);
            aSmallFireball.transform.localScale = transform.localScale * 0.9f;
            aSmallFireball.GetComponent<Fireball>().SetChainReact(true);
            float rV = Random.Range(-offsetSize, offsetSize);
            aSmallFireball.GetComponent<Rigidbody>().velocity = new Vector3(rV,rV,rV);

            Destroy(gameObject, Random.Range(0.5f, 2f));
    }
}