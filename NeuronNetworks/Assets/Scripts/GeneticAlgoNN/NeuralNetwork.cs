using System;
using UnityEngine;

public class NeuralNetwork
{
    private const int inputLength = 3;
    private const int outputLength = 1;
    private float[,] input;
    private float[,] output;

    private float[] inputValues;

    private float[][,] hiddenLayers;
    private float[][,] hiddenWeightMatrices;
    private float[,] inputToHiddenWeights;
    private float[,] hiddenToOutputWeights;

    public int HiddenCount = 0;
    public int HiddenLength = 0;

    public float[,] Biases;
    public float MutateInsertRate = 0.1f;
    public float MutateSwapRate = 0.2f;
    public float CrossoverRate = 0.7f;

    public NeuralNetwork(int hiddenCount, int hiddenLength)
    {
        HiddenCount = hiddenCount;
        HiddenLength = hiddenLength;
        DeclareAllStructures();
        RandomizeMatrix(ref inputToHiddenWeights);
        for (int k = 0; k < HiddenCount - 1; k++)
        {
            RandomizeMatrix(ref hiddenWeightMatrices[k]);
        }
        RandomizeMatrix(ref hiddenToOutputWeights);
    }

    public NeuralNetwork(NeuralNetwork other)
    {
        HiddenCount = other.HiddenCount;
        HiddenLength = other.HiddenLength;
        DeclareAllStructures();
        inputValues = other.inputValues;
        inputToHiddenWeights = other.inputToHiddenWeights;
        hiddenWeightMatrices = other.hiddenWeightMatrices;
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
        hiddenLayers = new float[HiddenCount][,];
        for (int i = 0; i < HiddenCount; i++)
        {
            hiddenLayers[i] = new float[1, HiddenLength];
        }
        hiddenToOutputWeights = new float[HiddenLength, outputLength];
        hiddenWeightMatrices = new float[HiddenCount - 1][,];
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
    /// The main function of the neural network that accepts an input and computes an output.
    /// </summary>
    /// <param name="inputValues">The values that form the input layer of the neural network.</param>
    /// <returns>A float value between -1 and 1.</returns>
    public float ProcessInput(float[] inputValues)
    {
        SetInput(inputValues);

        hiddenLayers[0] = NormalizeMatrix(MultiplyMatrices(input, inputToHiddenWeights));
        for (int i = 1; i < HiddenCount; i++)
        {
            hiddenLayers[i] = NormalizeMatrix(SumMatrices(MultiplyMatrices(hiddenLayers[i - 1], hiddenWeightMatrices[i - 1]), Biases));
        }
        output = NormalizeMatrix(MultiplyMatrices(hiddenLayers[HiddenCount - 1], hiddenToOutputWeights));

        return (Sigmoid(output[0, 0]) * 2) - 1;
    }

    #region CrossoverFunctions

    /// <summary>
    /// A one-point crossover genetic algorithm. This neural network + the neural network parameter = child's neural network.
    /// </summary>
    /// <param name="other">The neural network of the other parent.</param>
    /// <returns>The neural network of the child.</returns>
    public NeuralNetwork CrossoverWith(NeuralNetwork other)
    {
        if (hiddenWeightMatrices.Length != other.hiddenWeightMatrices.Length)
        {
            Debug.LogError("NeuralNetwork crossover failed: weights do not match.");
        }

        NeuralNetwork childNetwork = new NeuralNetwork(HiddenCount,HiddenLength);

        if (UnityEngine.Random.Range(0f, 1.0f) < CrossoverRate)
        {
            childNetwork.inputToHiddenWeights = GetCrossoverMatrix(inputToHiddenWeights, other.inputToHiddenWeights);
            for (int k = 0; k < HiddenCount - 1; k++)
            {
                childNetwork.hiddenWeightMatrices[k] = GetCrossoverMatrix(hiddenWeightMatrices[k], other.hiddenWeightMatrices[k]);
            }
            childNetwork.hiddenToOutputWeights = GetCrossoverMatrix(hiddenToOutputWeights, other.hiddenToOutputWeights);
        }

        return childNetwork;
    }

    private float[,] GetCrossoverMatrix(float[,] matrix1, float[,] matrix2)
    {
        float[,] result = new float[matrix1.GetLength(0), matrix1.GetLength(1)];

        int ithPos = UnityEngine.Random.Range(1, matrix1.GetLength(0));
        int jthPos = UnityEngine.Random.Range(1, matrix2.GetLength(1));

        for (int i = 0; i < matrix1.GetLength(0); i++)
        {
            for (int j = 0; j < matrix1.GetLength(1); j++)
            {
                if (i <= ithPos && j <= jthPos)
                {
                    result[i, j] = matrix1[i, j];
                }
                else
                {
                    result[i, j] = matrix2[i, j];
                }
            }
        }

        return result;
    }
    #endregion

    #region MutationFunctions

    /// <summary>
    /// An inner-mutation genetic algorithm that uses both MutateInsert() and MutateSwap() class methods.
    /// </summary>
    /// <returns>True if mutations have occured, false otherwise.</returns>
    public bool Mutate()
    {
        bool isInsertMutated = MutateInsert();
        bool isSwapMutated = MutateSwap();

        return (isInsertMutated || isSwapMutated);
    }

    /// <summary>
    /// An inner-mutation genetic algorithm that uses random insertion. Mutation occurs depending on the MutateInsertRate property.
    /// </summary>
    /// <returns>True if mutations have occured, false otherwise.</returns>
    public bool MutateInsert()
    {
        bool isMutated = false;

        if (UnityEngine.Random.Range(0f, 1.0f) < MutateInsertRate)
        {
            isMutated = MutateInsertMatrix(ref inputToHiddenWeights);
            for (int i = 0; i < HiddenCount - 1; i++)
            {
                MutateInsertMatrix(ref hiddenWeightMatrices[i]);
            }
            isMutated = MutateInsertMatrix(ref hiddenToOutputWeights);
        }

        return isMutated;
    }

    /// <summary>
    /// An inner-mutation genetic algorithm that uses random swap. Mutation occurs depending on the MutateSwapRate property.
    /// </summary>
    /// <returns>True if mutations have occured, false otherwise.</returns>
    public bool MutateSwap()
    {
        bool isMutated = false;

        if (UnityEngine.Random.Range(0f, 1.0f) < MutateSwapRate)
        {
            isMutated = MutateSwapMatrix(ref inputToHiddenWeights);
            for (int i = 0; i < HiddenCount - 1; i++)
            {
                isMutated = MutateSwapMatrix(ref hiddenWeightMatrices[i]);
            }
            isMutated = MutateSwapMatrix(ref hiddenToOutputWeights);
        }

        return isMutated;
    }

    // Helper function
    private bool MutateInsertMatrix(ref float[,] matrix)
    {
        bool isMutated = false;

        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                if (UnityEngine.Random.Range(0f, 1.0f) < MutateInsertRate)
                {
                    matrix[i, j] = UnityEngine.Random.Range(-1.0f, 1.0f);
                    isMutated = true;
                }
            }
        }

        return isMutated;
    }

    // Helper function
    private bool MutateSwapMatrix(ref float[,] matrix)
    {
        bool isMutated = false;

        if (UnityEngine.Random.Range(0f, 1.0f) < MutateSwapRate)
        {
            int x1 = UnityEngine.Random.Range(0, matrix.GetLength(0));
            int x2 = UnityEngine.Random.Range(0, matrix.GetLength(0));
            int y1 = UnityEngine.Random.Range(0, matrix.GetLength(1));
            int y2 = UnityEngine.Random.Range(0, matrix.GetLength(1));
            Swap(ref matrix[x1, y1], ref matrix[x2, y2]);

            isMutated = true;
        }

        return isMutated;
    }
    #endregion

    #region OtherHelperFunctions

    private void Swap(ref float left, ref float right)
    {
        float temp = left;
        left = right;
        right = temp;
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
                    result[i, j] = result[i, j] + matrix1[i, k] * matrix2[k, j];
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
    #endregion
}
