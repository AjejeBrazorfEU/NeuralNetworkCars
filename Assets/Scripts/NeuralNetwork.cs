using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;

public class NeuralNetwork : IComparable
{
    private int[] layers;  // {4,3,2} -> 3 layers with 4,3 and 2 neurons  
    private float[][] neurons;    
    private float[][] biases;    
    private float[][][] weights;    
    //private int[] activations;
    public float fitness = 0;

    public NeuralNetwork(int[] layers)
    {        
        this.layers = new int[layers.Length];        
        for (int i = 0; i < layers.Length; i++)        
        {            
            this.layers[i] = layers[i];        
        }        
        InitNeurons();        
        InitBiases();        
        InitWeights();    
    }
    // Create empty storage array for the neurons in the network.
    private void InitNeurons(){
        List<float[]> neuronsList = new List<float[]>();        
        for (int i = 0; i < layers.Length; i++)        
        {            
            neuronsList.Add(new float[layers[i]]);        
        }        
        neurons = neuronsList.ToArray();  
    }

    // Initializes and populates array for the biases being held within the network.
    private void InitBiases(){
        List<float[]> biasList = new List<float[]>();        
        for (int i = 0; i < layers.Length; i++)        
        {            
            float[] bias = new float[layers[i]];            
            for (int j = 0; j < layers[i]; j++)            
            {              
                // Random bias with median equals to 0  
                bias[j] = UnityEngine.Random.Range(-0.5f, 0.5f);            
            }            
            biasList.Add(bias);        
        }        
        biases = biasList.ToArray();  
    }

    private void InitWeights(){
        List<float[][]> weightsList = new List<float[][]>();        
        for (int i = 1; i < layers.Length; i++)        
        {            
            List<float[]> layerWeightsList = new List<float[]>();   
            int neuronsInPreviousLayer = layers[i - 1];            
            for (int j = 0; j < neurons[i].Length; j++)            
            {                 
                float[] neuronWeights = new float[neuronsInPreviousLayer];
                for (int k = 0; k < neuronsInPreviousLayer; k++)  
                {                               
                    // Random weight with median 0       
                    neuronWeights[k] = UnityEngine.Random.Range(-0.5f, 0.5f); 
                }               
                layerWeightsList.Add(neuronWeights);            
            }            
            weightsList.Add(layerWeightsList.ToArray());        
        }        
        weights = weightsList.ToArray();
    }

    public float activate(float value)    
    {        
        return (float)System.Math.Tanh(value);    
    }

    public float[] FeedForward(float[] inputs)    
    {        
        for (int i = 0; i < inputs.Length; i++)        
        {            
            neurons[0][i] = inputs[i];        
        }        
        for (int i = 1; i < layers.Length; i++)        
        {            
            int layer = i - 1;            
            for (int j = 0; j < neurons[i].Length; j++)            
            {                
                float value = 0f;               
                for (int k = 0; k < neurons[i - 1].Length; k++)  
                {                        
                    value += weights[i - 1][j][k] * neurons[i - 1][k];      
                }                
                neurons[i][j] = activate(value + biases[i][j]);            
            }        
        }        
        return neurons[neurons.Length - 1];    
    }

    //Comparing For NeuralNetworks performance.
    public int CompareTo(System.Object other)    
    {      
        NeuralNetwork ot = (NeuralNetwork) other; 
        if (other == null) 
            return 1;    
        if (fitness > ot.fitness)            
            return 1;        
        else if (fitness < ot.fitness)            
            return -1;        
        else            
            return 0;    
    }

    public void Save(string path)//this is used for saving the biases and weights within the network to a file.
    {
        File.Create(path).Close();
        StreamWriter writer = new StreamWriter(path, true);

        for (int i = 0; i < biases.Length; i++)
        {
            for (int j = 0; j < biases[i].Length; j++)
            {
                writer.WriteLine(biases[i][j]);
            }
        }

        for (int i = 0; i < weights.Length; i++)
        {
            for (int j = 0; j < weights[i].Length; j++)
            {
                for (int k = 0; k < weights[i][j].Length; k++)
                {
                    writer.WriteLine(weights[i][j][k]);
                }
            }
        }
        writer.Close();
    }

    public void Load(string path)//this loads the biases and weights from within a file into the neural network.
    {
        TextReader tr = new StreamReader(path);
        int NumberOfLines = (int)new FileInfo(path).Length;
        string[] ListLines = new string[NumberOfLines];
        int index = 1;
        for (int i = 1; i < NumberOfLines; i++)
        {
            ListLines[i] = tr.ReadLine();
        }
        tr.Close();
        if (new FileInfo(path).Length > 0)
        {
            for (int i = 0; i < biases.Length; i++)
            {
                for (int j = 0; j < biases[i].Length; j++)
                {
                    biases[i][j] = float.Parse(ListLines[index]);
                    index++;
                }
            }

            for (int i = 0; i < weights.Length; i++)
            {
                for (int j = 0; j < weights[i].Length; j++)
                {
                    for (int k = 0; k < weights[i][j].Length; k++)
                    {
                        weights[i][j][k] = float.Parse(ListLines[index]); ;
                        index++;
                    }
                }
            }
        }
    }


    public void Mutate(float chance, float val)//used as a simple mutation function for any genetic implementations.
    {
        for (int i = 0; i < biases.Length; i++)
        {
            for (int j = 0; j < biases[i].Length; j++)
            {
                if(UnityEngine.Random.value <= chance){
                    // Mutation occurred
                    biases[i][j] += UnityEngine.Random.Range(-val, val);
                }
            }
        }

        for (int i = 0; i < weights.Length; i++)
        {
            for (int j = 0; j < weights[i].Length; j++)
            {
                for (int k = 0; k < weights[i][j].Length; k++)
                {
                    if(UnityEngine.Random.value <= chance){
                        // Mutation occurred
                        weights[i][j][k] += UnityEngine.Random.Range(-val,val);
                    }
                }
            }
        }
    }

    // Returns a new NeuralNework, copied from the original and mutated
    public NeuralNetwork MutatedVersion(float chance,float val){
        NeuralNetwork nn = this.Copy();
        nn.Mutate(chance,val);
        return nn;
    }

    public void SetFitness(float fitness){
        this.fitness = fitness;
    }

    public NeuralNetwork Copy(){
        NeuralNetwork newNN = new NeuralNetwork(cloneArray(layers));

        newNN.biases = cloneMatrix(this.biases);
        newNN.weights = cloneMatrix(this.weights);
        newNN.neurons = cloneMatrix(this.neurons);

        return newNN;
    }
    /*
         
     */
    public int[] cloneArray(int[] source){
        int[] result = new int[source.Length];
        for(int i=0;i<source.Length;i++){
            result[i] = source[i];
        }
        return result;
    }
    public float[][][] getWeights(){
        return this.weights;
    }

    private float[] cloneMatrix(float[] source){
        float[] result = new float[source.Length];
        for(int i=0;i<source.Length;i++){
            result[i] = source[i];
        }
        return result;
    }
    private float[][] cloneMatrix(float[][] source){
        float[][] result = new float[source.Length][];
        for(int i=0;i<source.Length;i++){
            float[] row = cloneMatrix(source[i]);
            result[i] = row;
        }
        return result;
    }

    private float[][][] cloneMatrix(float[][][] source){
        float[][][] result = new float[source.Length][][];
        for(int i=0;i<source.Length;i++){
            float[][] matrix = cloneMatrix(source[i]);
            result[i] = matrix;
        }
        return result;
    }
}
