using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    float speedUp;
    float speedForward;
    Transform spawnPoint;
    [SerializeField] GameObject smallFireball;
    [SerializeField] bool destroyable = true;
    float sizeThold = 0.25f; 
    bool chainReact = false;
    bool hasSplit = false;
    bool splitAgain = false;
    bool original = true;
    float originalSize;
    float instantiateTime;
    Vector3 connectPoint;
    float spawnInterval = 0.25f;
    float connectionTime;
    float nextEmitTime = 0f;
    float bigShrinkFactor = 0.8f;
    float continousShrink = 0.98f;

    void Start()
    {
        SetRigidbody();
        instantiateTime = Time.time;
        if(spawnPoint && original){
            transform.GetComponent<Rigidbody>().velocity = 
                        (spawnPoint.forward * speedForward + spawnPoint.up* speedUp);
            transform.position = spawnPoint.position;
        }
        if(chainReact == true){
            ChainReaction();
        }
    }

    void OnCollisionEnter(Collision collision){
        SetOriginalConnectPoint();
        Explode();
    }

    void SetOriginalConnectPoint(){
        if(original && !hasSplit){
            connectionTime = Time.time;
            connectPoint = transform.position;
            nextEmitTime = connectionTime + spawnInterval;

        }
    }

    void SetConnectPoint(Vector3 newPoint){
        connectPoint = newPoint;
    }

    Vector3 GetConnectPoint(){
        return connectPoint;
    }

    void FixedUpdate(){
        if(original && hasSplit){
            Shrink();

        }

        if(original && nextEmitTime > 0f){
            EmitFire();
        }

        if(Time.time > instantiateTime + transform.localScale.x + 2f && destroyable){
            Destroy(gameObject);
        }
    }

    void EmitFire(){
        if(nextEmitTime < Time.time){
            nextEmitTime = Time.time + spawnInterval;
            splitAgain = true;
            connectPoint = transform.position;
            
            Explode();
            splitAgain = false;
            continousShrink -= 0.01f;
        }
    }
    void Shrink(){
        transform.localScale *= continousShrink;
    }

    void BigShrink(){
        transform.localScale *= bigShrinkFactor;
    }

    void Explode(){

        if(!hasSplit || splitAgain){
            if(!hasSplit){
                BigShrink();
            }
            if(!chainReact){
                SpawnNewFireballFromOriginal();
            }
            hasSplit = true;
        } 
    }

    void SpawnNewFireballFromOriginal(){
        GameObject aSmallFireball = (GameObject)Instantiate(smallFireball, transform.position, Quaternion.identity);
        Fireball fb = aSmallFireball.GetComponent<Fireball>();
        
        aSmallFireball.transform.localScale = transform.localScale * 0.9f;
        fb.SetOriginal(false);
        fb.SetChainReact(true);

        if(connectPoint != null){  
            fb.SetConnectPoint(connectPoint);
            fb.SetOriginalSize(originalSize);
        }
    }

    public void CastHellfireWithFireball(float newSize){
        connectPoint = transform.position;
        SetInitial(0f,0f, transform, new Vector3(newSize,newSize,newSize));
        SpawnNewFireballFromOriginal();
    }    

    void SetOriginalSize(float value){
        originalSize = value;
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
        if(Random.Range(originalSize*-1f,originalSize*0.75f) > transform.localScale.x){
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
        if(GetComponent<Rigidbody>()){
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.mass = transform.localScale.x;
            rb.drag = transform.localScale.x * 0.05f;
            rb.angularDrag = transform.localScale.x * 0.1f;
        }
    }

    void SpawnNewFireball(){
        float offsetSize = originalSize;
        Vector3 offset =  Random.insideUnitSphere * offsetSize;
        offset.y *= 0.33f;
        GameObject aSmallFireball = (GameObject)Instantiate(smallFireball, connectPoint + offset, Quaternion.identity);
        Fireball fb = aSmallFireball.GetComponent<Fireball>();
        
        aSmallFireball.transform.localScale = transform.localScale * 0.89f;
        fb.SetChainReact(true);
        if(connectPoint != null){
            fb.SetConnectPoint(connectPoint);
            fb.SetOriginalSize(originalSize);
        }
    }

}
