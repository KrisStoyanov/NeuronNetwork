using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackPropNetworkComponent : MonoBehaviour
{
    BackPropNetwork propNetwork;

    public int[] layers;
    public string[] activation;

    void Start()
    {

    }

    public void SetNeuralNetwork(int layer, int length)
    {
        layers = new int[layer+2];
        layers[0] = 3;
        for (int i = 1; i < layers.Length-1; i++)
        {
            layers[i] = length;
        }
        layers[layers.Length - 1] = 1;

        activation = new string[layer + 1];
        for (int i = 0; i < activation.Length; i++)
        {
            activation[i] = "tanh";
        }


        if (propNetwork == null)
        {
            this.propNetwork = new BackPropNetwork(layers, activation);
        }
    }
    
    public void SetNeuralNetwork(BackPropNetwork other)
    {
        if (propNetwork == null)
        {
            propNetwork = new BackPropNetwork(other.layers, other.activation);
        }    
        propNetwork.Copy(other);
    }

    public BackPropNetwork GetNetwork()
    {
        return propNetwork;
    }

    public float ReturnOutput(float[] inputValues, float expectedOutput)
    {
        propNetwork.inputValues = inputValues;
        propNetwork.expected[0] = expectedOutput;
        return propNetwork.FeedForward(inputValues)[0];
    }
}
