using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    private int currentEpoch = 0;
    public int epoch;
    public float mutationChance;
    public float mutationValue = 0.5f;
    public int numberOfUnitsPerEpoch = 50;

    private NeuralNetwork[] nns;
    private CarPilot[] cars;

    public GameObject carGameObject;

    public Transform SpawnPosition;

    public int EpochSurvivor = 10; // Indicates how many cars survive to the next epoch

    public float EpochTimeLimit = 20f;
    
    private float startTime;

    void Start(){
        startTime = Time.time;
        nns = new NeuralNetwork[numberOfUnitsPerEpoch];
        cars = new CarPilot[numberOfUnitsPerEpoch];
        //currentEpoch = 0;
        for(int i=0;i<numberOfUnitsPerEpoch;i++){
            nns[i] = new NeuralNetwork(new []{6,3,2});
            cars[i] = Instantiate(carGameObject,SpawnPosition.position,Quaternion.Euler(0,0,0)).GetComponent<CarPilot>();
        }
    }

    void Update(){
        int remainingCars = 0;
        for(int i=0;i<numberOfUnitsPerEpoch;i++){
            if(cars[i].isAlive()){
                remainingCars++;
                // Get Input values from the car (5 for the distances and 1 for the speed)
                List<float> inputs = new List<float>(cars[i].GetDistances());
                inputs.Add(cars[i].getSpeed());

                // Get Output from the NN ("Vertical","Horizontal") axis
                float[] outputs = nns[i].FeedForward(inputs.ToArray());
                cars[i].setInput(outputs[0],outputs[1]);
            }
        }

        if(remainingCars == 0 || (Time.time - startTime) > EpochTimeLimit){
            NewEpoch();
        }
    }

    private void NewEpoch(){
        // One epoch of the simulation has ended, create new epoch
        currentEpoch++;
        Debug.Log("Creating Epoch " + currentEpoch);
        // Order the cars based on the fitness
        for(int i=0;i<numberOfUnitsPerEpoch;i++){
            nns[i].SetFitness(cars[i].GetFitness());
        }

        // Sort the nns in descending order based on the fitness
        Array.Sort(nns);
        Array.Reverse(nns);

        for(int i=0;i<numberOfUnitsPerEpoch;i++){
            Debug.Log("Fitness(" + i +") = " + nns[i].fitness);
        }
        // Get the better performing
        NeuralNetwork[] newnns = new NeuralNetwork[numberOfUnitsPerEpoch];

        // Survives the max EpochSurvivor cars
        for(int i=0;i<EpochSurvivor;i++){
            newnns[i] = nns[i].Copy();
        }
        // The remaining nns are created based on the top EpochSurvivor
        for(int i=EpochSurvivor;i<numberOfUnitsPerEpoch;i++){
            int index = (int)(UnityEngine.Random.value * Mathf.Max(EpochSurvivor-1,0));
            //Debug.Log("Creating nn["+i+"] , chance:" + mutationChance+ " value:" + mutationValue + " with fitness:" + nns[index].fitness);
            //PrintWeights(nns[index].getWeights());
            newnns[i] = nns[index].MutatedVersion(mutationChance,mutationValue);
            //PrintWeights(newnns[i].getWeights());
            
            //Debug.Log(newnns[i].getWeights() + " FROM " + nns[index].getWeights());
        }
        for(int i=0;i<numberOfUnitsPerEpoch;i++){
            Debug.Log("nns["+i+"].fitness=" + nns[i].fitness);
        }
        for(int i=0;i<EpochSurvivor;i++){
            newnns[i].SetFitness(0);
        }
        // Reset all the cars
        foreach(CarPilot c in cars){
            c.reset(SpawnPosition.position);
        }
        nns = newnns;
        
        startTime = Time.time;
    }

    private void PrintWeights(float[][][] weights){
        string res = "";
        for(int i=0;i<weights.Length;i++){
            for(int j=0;j<weights[i].Length;j++){
                for(int k=0;k<weights[i][j].Length;k++){
                    res += weights[i][j][k] + " ";
                }
            }
        }
        Debug.Log(res);
    }

    public int getCurrentEpoch(){
        return currentEpoch;
    }

    public float getElapsedTime(){
        return Time.time - startTime;
    }
}
