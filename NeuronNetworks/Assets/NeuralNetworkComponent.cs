using UnityEngine;

public class NeuralNetworkComponent : MonoBehaviour
{
    NeuralNetwork neuralNetwork = null;

    public float[,] Biases
    {
        set { neuralNetwork.Biases = Biases; }
        get { return neuralNetwork.Biases; }
    }
    public int HiddenCount
    {
        set { neuralNetwork.HiddenCount = value; }
        get { return neuralNetwork.HiddenCount; }
    }
    public int HiddenLength
    {
        set { neuralNetwork.HiddenLength = value; }
        get { return neuralNetwork.HiddenLength; }
    }
    public float MutateInsertRate
    {
        set { neuralNetwork.MutateInsertRate = value; }
        get { return neuralNetwork.MutateInsertRate; }
    }
    public float MutateSwapRate
    {
        set { neuralNetwork.MutateSwapRate = value; }
        get { return neuralNetwork.MutateSwapRate; }
    }
    public float CrossoverRate
    {
        set { neuralNetwork.CrossoverRate = value; }
        get { return neuralNetwork.CrossoverRate; }
    }

    private void Start()
    {
        //SetNeuralNetwork();
    }

    public NeuralNetwork GetNetwork()
    {
        return neuralNetwork;
    }

    public void SetNeuralNetwork(int count, int length)
    {
        if (neuralNetwork == null)
        {
            neuralNetwork = new NeuralNetwork(count,length);
        }
    }

    public void SetNeuralNetwork(NeuralNetwork neuralNetwork)
    {
        this.neuralNetwork = new NeuralNetwork(neuralNetwork);
    }

    public float ReturnOutput(float[] inputValues)
    {
        return neuralNetwork.ProcessInput(inputValues);
    }
}