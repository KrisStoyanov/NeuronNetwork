using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct Pair
{
    public float output;
    public float expectedOutput;
}


public class NeuralNetworkBackPropagation
{
    private List<Pair> Data;
    private const int inputLength = 3;
    private const int outputLength = 1;
    private float[,] input;
    private float[,] output;

    private float[] inputValues;

    private float[][,] hiddenLayers;

    private float[][,] hiddenWeightMatrices;
    private float[][] hiddenWeightErrors;

    private float[,] inputToHiddenWeights;

    private float[,] hiddenToOutputWeights;
    private float outputError;

    public int HiddenCount = 2;
    public int HiddenLength = 4;

    public float[,] Biases;
    public NeuralNetworkBackPropagation()
    {
        DeclareAllStructures();
        RandomizeMatrix(ref inputToHiddenWeights);
        for (int k = 0; k < HiddenCount - 1; k++)
        {
            RandomizeMatrix(ref hiddenWeightMatrices[k]);
            ErrorMatrixFiller(ref hiddenWeightErrors[k]);
        }
        RandomizeMatrix(ref hiddenToOutputWeights);
        outputError = 0f;
    }

    public NeuralNetworkBackPropagation(NeuralNetworkBackPropagation other)
    {
        DeclareAllStructures();
        inputValues = other.inputValues;
        inputToHiddenWeights = other.inputToHiddenWeights;
        hiddenWeightMatrices = other.hiddenWeightMatrices;
        hiddenWeightErrors = other.hiddenWeightErrors;
        hiddenToOutputWeights = other.hiddenToOutputWeights;
    }

    public float[] GetInput()
    {
        return inputValues;
    }

    private void DeclareAllStructures()
    {
        input = new float[1, inputLength];
        inputValues = new float[inputLength];
        inputToHiddenWeights = new float[inputLength, HiddenLength];
        //inputToHiddenErrors = new float[inputLength];
        hiddenLayers = new float[HiddenCount][,];
        for (int i = 0; i < HiddenCount; i++)
        {
            hiddenLayers[i] = new float[1, HiddenLength];
        }
        hiddenToOutputWeights = new float[HiddenLength, outputLength];
        hiddenWeightMatrices = new float[HiddenCount - 1][,];
        hiddenWeightErrors = new float[HiddenCount][];
        for (int i = 0; i < HiddenCount; i++)
        {
            hiddenWeightErrors[i] = new float[HiddenLength];
        }
        for (int i = 0; i < HiddenCount - 1; i++)
        {
            hiddenWeightMatrices[i] = new float[HiddenLength, HiddenLength];
        }
        Biases = new float[1, HiddenLength];
        output = new float[1, outputLength];
    }

    private void RandomizeMatrix(ref float[,] matrix)
    {
        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                matrix[i, j] = UnityEngine.Random.Range(-1.0f, 1.0f);
            }
        }
    }

    private void ErrorMatrixFiller(ref float[] matrix)
    {
        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            matrix[i] = 0f;
        }
    }

    private void SetInput(float[] values)
    {
        inputValues = values;
        if (values.Length != input.GetLength(1))
        {
            Debug.LogError("NeuralNetwork.SetInput: parameter length does NOT match input layer length.");
        }
        for (int i = 0; i < values.Length; i++)
        {
            input[0, i] = values[i];
        }
    }
    public float[] GetInputValues()
    {
        return inputValues;
    }

    /// <summary>
    /// THe main function of the neural network that accepts an input and computes an output.
    /// </summary>
    /// <param name="inputValues">The values that form the input layer of the neural network.</param>
    /// <returns>A float value between -1 and 1.</returns>
    public float ProcessInput(float[] inputValues, float expectedResult)
    {
        SetInput(inputValues);

        hiddenLayers[0] = NormalizeMatrix(MultiplyMatrices(input, inputToHiddenWeights));
        for (int i = 1; i < HiddenCount; i++)
        {
            hiddenLayers[i] = NormalizeMatrix(SumMatrices(MultiplyMatrices(hiddenLayers[i - 1], hiddenWeightMatrices[i - 1]), Biases));
        }
        output = NormalizeMatrix(MultiplyMatrices(hiddenLayers[HiddenCount - 1], hiddenToOutputWeights));
        Pair newPair = new Pair();
        float returnableOutput = (Sigmoid(output[0, 0]) * 2) - 1;
        newPair.expectedOutput = expectedResult;
        newPair.output = returnableOutput;
        Data.Add(newPair);
        return returnableOutput;
    }

    private float[,] MultiplyMatrices(float[,] matrix1, float[,] matrix2)
    {
        if (matrix1.GetLength(1) != matrix2.GetLength(0))
        {
            Debug.LogError("NeuralNetwork: attempted to multiply matrices with incorrect dimensions.");
        }

        float[,] result = new float[matrix1.GetLength(0), matrix2.GetLength(1)];

        for (int i = 0; i < result.GetLength(0); i++)
        {
            for (int j = 0; j < result.GetLength(1); j++)
            {
                result[i, j] = 0;

                for (int k = 0; k < matrix1.GetLength(1); k++)
                {
                    result[i, j] += matrix1[i, k] * matrix2[k, j];
                }
            }
        }

        return result;
    }

    private float[,] SumMatrices(float[,] matrix1, float[,] matrix2)
    {
        if (matrix1.GetLength(0) != matrix2.GetLength(0) || matrix1.GetLength(1) != matrix2.GetLength(1))
        {
            Debug.LogError("NeuralNetwork: attempted to sum matrices with incorrect dimensions.");
        }

        float[,] result = new float[matrix1.GetLength(0), matrix1.GetLength(1)];

        for (int i = 0; i < matrix1.GetLength(0); i++)
        {
            for (int j = 0; j < matrix2.GetLength(1); j++)
            {
                result[i, j] = matrix1[i, j] + matrix2[i, j];
            }
        }

        return result;
    }

    private float Sigmoid(float x)
    {
        return (float)(1.0f / (1.0f + Math.Exp(-x)));
    }

    private float[,] NormalizeMatrix(float[,] matrix)
    {
        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                matrix[i, j] = 2 * Sigmoid(matrix[i, j]) - 1;
            }
        }

        return matrix;
    }

    

    private void AdjustWeights(float[,] weightsMatrix, float error, float[,] hiddenLayer, int layer)
    {
        for (int i = 0; i < hiddenLayer.Length; i++)
        {
            weightsMatrix[i, layer] = Sigmoid(weightsMatrix[i, layer] + error * hiddenLayer[layer, i]);
        }
    }

    public void BackPropagate()
    {
        for (int i = 0; i < Data.Count; i++)
        {
            outputError += Sigmoid(Data[i].expectedOutput - Data[i].output);
        }
        outputError = (float)(outputError / (float)Data.Count);
        AdjustWeights(hiddenToOutputWeights, outputError, hiddenLayers[HiddenCount - 1], 0);
        //for (int i = 0; i < length; i++)
        //{

        //}
    }
    //public void ReCalculate()
    //{
    //    for (int i = 0; i < Data.Count; i++)
    //    {
    //        outputNeuron.error = Sigmoid.Output(Data[i].expectedOutput - Data[i].output);
    //        outputNeuron.adjustWeights();
    //        for (int j = 0; j < hiddenLength; j++)
    //        {
    //            hiddenNeurons[hiddenNeuronsNumber-1][j].error =Sigmoid.Output(outputNeuron.output)*outputNeuron.error*outputNeuron.weights[j];
    //            hiddenNeurons[hiddenNeuronsNumber-1][j].adjustWeights();
    //        }
    //        for (int k = hiddenNeuronsNumber-2; k >=0; k--)
    //        {
    //            List<Neuron> previoustHiddenLayer = new List<Neuron>();
    //            previoustHiddenLayer = hiddenNeurons[k + 1];
    //            //Debug.Log(k-1);
    //            for (int j = 0; j < hiddenLength; j++)
    //            {
    //                float multiplyValue =1f;
    //                for (int p = 0; p < hiddenLength; p++)
    //                {
    //                    multiplyValue *=previoustHiddenLayer[p].error*previoustHiddenLayer[p].weights[j];
    //                }
    //                hiddenNeurons[k][j].error *=Sigmoid.Output(outputNeuron.output)*multiplyValue;
    //                hiddenNeurons[k][j].adjustWeights();
    //            }
    //        }
    //    }
    //}

}
