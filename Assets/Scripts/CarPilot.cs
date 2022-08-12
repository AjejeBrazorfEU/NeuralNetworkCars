using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarPilot : MonoBehaviour
{
    private float moveInput;
    private float turnInput;

    public float forwardSpeed;
    public float reverseSpeed;
    public float turnSpeed;

    public Transform RaycastOrigin;
    public float RaycastAngle;

    public Rigidbody sphereRB;

    private bool dead = false;

    private int Checkpoints = 0;

    private List<Transform> alreadyHitCheckpoint;

    void Start(){
        sphereRB.transform.parent = null;
        alreadyHitCheckpoint = new List<Transform>();
    }

    void Update(){
        if(!dead){
            /*moveInput = Input.GetAxis("Vertical");
            turnInput = Input.GetAxis("Horizontal");*/

            moveInput *= moveInput > 0 ? forwardSpeed : reverseSpeed;

            transform.position = new Vector3(sphereRB.transform.position.x,transform.position.y,sphereRB.transform.position.z);

            float newRotation = turnInput * turnSpeed * Time.deltaTime;
            transform.Rotate(0,newRotation,0,Space.World);
        }
    }

    public void setInput(float moveInput, float turnInput){
        this.turnInput = turnInput;
        this.moveInput = moveInput;
    }

    private void FixedUpdate(){
        if(!dead){
            sphereRB.AddForce(transform.forward * moveInput,ForceMode.Acceleration);
            GetDistances();
        }
    }

    public float[] GetDistances(){
        int numOfRays = 5;
        float[] distances = new float[numOfRays];
        float[] angles = {-2*RaycastAngle,-RaycastAngle,0,RaycastAngle,2*RaycastAngle};
        int layer_mask = LayerMask.GetMask("Walls");
        for(int i=0;i<numOfRays;i++){
            RaycastHit hit;
            Vector3 direction = transform.TransformDirection(new Vector3(Mathf.Sin(angles[i] * Mathf.Deg2Rad),0,Mathf.Cos(angles[i]* Mathf.Deg2Rad)));
            if(Physics.Raycast(RaycastOrigin.position,direction,out hit,Mathf.Infinity,layer_mask)){
                Debug.DrawRay(RaycastOrigin.position,direction * hit.distance,Color.yellow);
                distances[i] = hit.distance;
            }
        }
        return distances;
    }

    private void OnTriggerEnter(Collider other){
        if(other.transform.tag == "Walls"){
            //Hit a wall, dies
            dead = true;
        }else if(other.transform.tag == "Checkpoint"){
            /*if(!alreadyHitCheckpoint.Contains(other.transform)){
                Checkpoints++;
                //Debug.Log("Hit checkpoint: total points = " + Checkpoints);
                alreadyHitCheckpoint.Add(other.transform);
            }else{
                // Penalize if returns back
                Checkpoints--; 
            }*/
            string supposedCheckpointName = "Cube (" + (Checkpoints % 46) + ")";
            if(other.transform.name.Trim().Equals(supposedCheckpointName)){
                Checkpoints++;
                //Debug.Log("Positive Points");
                //Debug.Log("Hit checkpoint: total points = " + Checkpoints);
            }else{
                //Debug.Log("Penalization");
                //Debug.Log("Expected : " + supposedCheckpointName + " but got " + other.transform.name);
                // Penalize if returns back
                //Checkpoints--; 
            }
        }else if(other.transform.tag == "Finish"){
            alreadyHitCheckpoint.Clear();
        }
    }

    public bool isAlive(){
        return !dead;
    }

    public float getSpeed(){
        return sphereRB.velocity.magnitude;
    }

    public float GetFitness(){
        return Checkpoints;
    }

    public void reset(Vector3 newPosition){
        moveInput = 0;
        turnInput = 0;
        dead = false;
        Checkpoints = 0;
        alreadyHitCheckpoint.Clear();
        transform.position = newPosition;
        transform.rotation = Quaternion.Euler(0,0,0);
        sphereRB.transform.position = new Vector3(newPosition.x,sphereRB.transform.position.y,newPosition.z);
        sphereRB.velocity = Vector3.zero;
        sphereRB.angularVelocity = Vector3.zero;
        sphereRB.ResetInertiaTensor();
    }
}
