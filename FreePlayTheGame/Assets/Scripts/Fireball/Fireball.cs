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
    bool original = true;
    float originalSize = 1f;
    float instantiateTime;

    // Start is called before the first frame update
    void Start()
    {
        SetRigidbody();
        instantiateTime = Time.time;
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

    void FixedUpdate(){
        if(original && hasSplit){
            Shrink();
            transform.GetComponent<Rigidbody>().velocity = Vector3.ClampMagnitude(transform.GetComponent<Rigidbody>().velocity, Mathf.Max(speedForward, speedUp) / 1.5f);
        }

        if(Time.time > instantiateTime + 2f){
            Destroy(gameObject);
        }
    }

    void Shrink(){
        transform.localScale *= 0.98f;
    }

    void Explode(){
        
        if(hasSplit == false){
            if(chainReact == false){
                GameObject aSmallFireball = (GameObject)Instantiate(smallFireball, transform.position, Quaternion.identity);
                aSmallFireball.transform.localScale = transform.localScale * 0.9f;
                aSmallFireball.GetComponent<Fireball>().SetOriginal(false);
                aSmallFireball.GetComponent<Fireball>().SetChainReact(true);
            }
            hasSplit = true;
            Destroy(gameObject, 1f);
        } 
    }
    
    // Only called for the original Fireball
    public void SetInitial(float sfor, float sup, Transform spoint, Vector3 mySize){
        float fatness = Mathf.Log(1f + mySize.x);
        speedForward = sfor * fatness;
        speedUp = sup * fatness;
        spawnPoint = spoint;
        transform.localScale = mySize;
        sizeThold *= mySize.x;
        originalSize = mySize.x;
    }

    void ChainReaction(){
        if(transform.localScale.x >= sizeThold){
            SpawnNewFireball();
        }
        
        // add many more balls when small
        // the smaller the more likely to switch has Split
        if(Random.Range(originalSize*-0.9f,originalSize*0.75f) > transform.localScale.x){
            ChainReaction();
        }
    }
    public void SetOriginal(bool value){
        original = value;
    }

    public void SetChainReact(bool value){
        chainReact = value;
    }
    void SetRigidbody(){
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.mass = transform.localScale.x;
        rb.drag = transform.localScale.x * 0.05f;
        rb.angularDrag = transform.localScale.x * 0.1f;
    }

    void SpawnNewFireball(){
            float offsetSize = 2f;
            Vector3 offset =  Random.insideUnitSphere * offsetSize + Random.onUnitSphere * 0.1f;
            offset.y *= 0.33f;
            GameObject aSmallFireball = (GameObject)Instantiate(smallFireball, transform.position + offset, Quaternion.identity);
            aSmallFireball.transform.localScale = transform.localScale * 0.91f;
            aSmallFireball.GetComponent<Fireball>().SetChainReact(true);
    
            float rV = Random.Range(-offsetSize, offsetSize);
            aSmallFireball.GetComponent<Rigidbody>().AddForce( new Vector3(rV,rV,rV) );

            
    }
}
