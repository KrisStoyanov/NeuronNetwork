using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuralNetworkBackPropagationComponent : MonoBehaviour
{
    NeuralNetworkBackPropagation neuralNetwork = null;

    private void Start()
    {
        SetNeuralNetwork();
    }

    public NeuralNetworkBackPropagation GetNetwork()
    {
        return neuralNetwork;
    }

    public void SetNeuralNetwork()
    {
        if (neuralNetwork == null)
        {
            this.neuralNetwork = new NeuralNetworkBackPropagation();
        }
    }

    public void SetNeuralNetwork(NeuralNetworkBackPropagation neuralNetwork)
    {
        if (neuralNetwork == null)
        {
            Debug.LogError("SetNeuralNetwork: parameter is null.");
        }
        this.neuralNetwork = new NeuralNetworkBackPropagation(neuralNetwork);
    }

    public float ReturnOutput(float[] inputValues, float expectedResult)
    {
        return neuralNetwork.ProcessInput(inputValues, expectedResult);
    }
}
