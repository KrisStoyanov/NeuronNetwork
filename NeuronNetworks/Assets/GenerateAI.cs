using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class GenerateAI : MonoBehaviour
{
    private class Pair
    {
        public NeuralNetwork neuralNetwork;
        public float fitness;
    }
    public static event Action restartMap;

    public static event Action getCanvasContainer;
    private int GenerationChildren = 50;
    private int childrenDied = 0;
    NeuralNetwork bestNeuralNetwork = null;
    private float bestNNFitness = 0;
    List<NeuralNetwork> allchildrenBrains = null;
    List<NeuralNetwork> deadChildren = null;
    List<float> allchildrenFitness = null;
    private float maxX = 2.8f;
    private float mixX = -2.8f;
    Transform highscore = null;
    Transform generation = null;
    Transform aliveChildren = null;
    Transform canvasContainer = null;
    private float score = 0;
    private int gen = 0;
    private int alive = 0;
    private float playerSpeed = 10f;
    List<BackPropNetwork> allchildrenBrainsBackProp = null;
    private bool isItBackProp = false;

    private bool inputRecieved = false;
    private int hiddenLayers = 0;
    private int neuronsInLayers = 0;

    void Start()
    {
        MainScreen.StartGame += MainScreen_StartGame;
    }

    private void MainScreen_StartGame(bool arg1, int arg2, int arg3, int arg4)
    {
        inputRecieved      = true;
        isItBackProp       = arg1;
        GenerationChildren = arg2;
        hiddenLayers       = arg3;
        neuronsInLayers    = arg4;

        if (!isItBackProp)
        {
            deadChildren = new List<NeuralNetwork>();
            allchildrenBrains = new List<NeuralNetwork>();
            allchildrenFitness = new List<float>();
        }
        else
        {
            allchildrenBrainsBackProp = new List<BackPropNetwork>();
        }
        UpdateHUD();
        PlayerCollider.OnPlayerDeadNN += PlayerCollider_OnPlayerDeadNN;
        PlayerCollider.OnPlayerDeadNNBP += PlayerCollider_OnPlayerDeadNNBP;
        Movement.SendCanvasContainer += Movement_SendCanvasContainer;
        getCanvasContainer?.Invoke();
        SpawnFirstChildren();
    }

    private void Movement_SendCanvasContainer(Transform obj)
    {
        canvasContainer = obj;
    }

    private void SpawnFirstChildren()
    {
        gen++;
        childrenDied = 0;
        GameObject AILocation = this.gameObject.transform.parent.transform.Find("AI").gameObject;
        for (int i = 0; i < GenerationChildren; i++)
        {
            GameObject Balloon = Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Enemies/BalloonAI"), AILocation.transform);
            if (!isItBackProp)
            {
                Balloon.AddComponent<NeuralNetworkComponent>();
                Balloon.GetComponent<NeuralNetworkComponent>().SetNeuralNetwork(hiddenLayers, neuronsInLayers);
            }
            else
            {
                Balloon.AddComponent<BackPropNetworkComponent>();
                Balloon.GetComponent<BackPropNetworkComponent>().SetNeuralNetwork(hiddenLayers, neuronsInLayers);
            }
            Balloon.GetComponent<PlayerCollider>().setNN(isItBackProp);
        }
    }

    private void SpawnChildren()
    {
        gen++;
        childrenDied = 0;
        GameObject AILocation = this.gameObject.transform.parent.transform.Find("AI").gameObject;
        foreach (Transform child in AILocation.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        for (int i = 0; i < GenerationChildren; i++)
        {
            GameObject Balloon = Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Enemies/BalloonAI"), AILocation.transform);
            if (!isItBackProp)
            {
                Balloon.AddComponent<NeuralNetworkComponent>();
                Balloon.GetComponent<NeuralNetworkComponent>().SetNeuralNetwork(allchildrenBrains[i]);
            }
            else
            {
                Balloon.AddComponent<BackPropNetworkComponent>();
                Balloon.GetComponent<BackPropNetworkComponent>().SetNeuralNetwork(allchildrenBrainsBackProp[i]);
               
            }
            Balloon.GetComponent<PlayerCollider>().setNN(isItBackProp);
        }
        if (!isItBackProp)
        {
            allchildrenBrains.Clear();
        }
        else
        {
            allchildrenBrainsBackProp.Clear();
        }
    }

    private void UpdateHUD()
    {
        alive = GenerationChildren - childrenDied;
        GameObject HUD = this.transform.parent.Find("HUD").gameObject;
        highscore = HUD.transform.Find("MainHolder/HighScore/Score");
        highscore.GetComponent<TextMeshProUGUI>().text = ((int)score).ToString();
        generation = HUD.transform.Find("MainHolder/Gen/Score");
        generation.GetComponent<TextMeshProUGUI>().text = gen.ToString();
        aliveChildren = HUD.transform.Find("MainHolder/Alive/Score");
        aliveChildren.GetComponent<TextMeshProUGUI>().text = alive.ToString();
    }

    private void PlayerCollider_OnPlayerDeadNN(NeuralNetwork deadnetwork)
    {
        deadChildren.Add(deadnetwork);
        allchildrenFitness.Add(CalculateFitness(deadnetwork, score));
        if (this.transform.parent.transform.Find("AI").childCount == 1)
        {
            //Debug.Log("last man standing!");
            bestNeuralNetwork = this.transform.parent.transform.Find("AI").GetChild(0).transform.GetComponent<NeuralNetworkComponent>().GetNetwork();
            bestNNFitness = score;
        }
        childrenDied++;
        UpdateHUD();
        if (childrenDied == GenerationChildren)
        {
            Pair pair = MaxFitnessNetwork();
            NeuralNetwork neuralNetwork = pair.neuralNetwork;
            if (bestNeuralNetwork == null)
            {

                bestNeuralNetwork = neuralNetwork;
                bestNNFitness = pair.fitness;
            }
            else
            {
                //Debug.Log("bestNNFitness: " + bestNNFitness + "< maxFitness: " + pair.fitness);
                if (bestNNFitness < pair.fitness)
                {
                    bestNeuralNetwork = neuralNetwork;
                    bestNNFitness = pair.fitness;
                }
            }
            deadChildren.Clear();
            allchildrenFitness.Clear();
            ALLAIDied();
        }
    }

    private void PlayerCollider_OnPlayerDeadNNBP(BackPropNetwork deadnetwork)
    {
        allchildrenBrainsBackProp.Add(deadnetwork);
        childrenDied++;
        UpdateHUD();
        if (childrenDied == GenerationChildren)
        {
            ALLAIDied();
        }
    }

    private Pair MaxFitnessNetwork()
    {
        float maxFitness = 0;
        int index = 0;
        for (int i = 0; i < allchildrenFitness.Count; i++)
        {
            if (allchildrenFitness[i] >= maxFitness)
            {
                index = i;
                maxFitness = allchildrenFitness[i];
            }
        }
        //Debug.Log("BEST FITNESS: " + maxFitness);
        Pair pair = new Pair();
        pair.neuralNetwork = deadChildren[index];
        pair.fitness = maxFitness;
        return pair;
    }
    private float CalculateFitness(NeuralNetwork deadnetwork, float score)
    {
        float[] deadnetworkInput = deadnetwork.GetInput();
        float fitness = score;
        if (deadnetworkInput[0] >= 0 && deadnetworkInput[1] >= 0)
        {
            fitness -= 1 - Math.Abs(deadnetworkInput[0] - deadnetworkInput[1]);
        }
        else
        {
            if (deadnetworkInput[0] < 0)
            {
                fitness -= Math.Abs(deadnetworkInput[0]);
            }
            else
            {
                fitness -= Math.Abs(deadnetworkInput[1]);
            }
        }

        return fitness;
    }

    public void RestartMap()
    {
        restartMap?.Invoke();
        score = 0;
    }

    public void Restart()
    {
        Mutate();
        StartCoroutine(Coroutine());

    }

    public IEnumerator Coroutine()
    {
        yield return new WaitForSeconds(0.5f);
        RestartMap();
        SpawnChildren();
    }

    public void Mutate()
    {
        if (!isItBackProp)
        {
            for (int i = 0; i < GenerationChildren; i++)
            {
                allchildrenBrains.Add(new NeuralNetwork(bestNeuralNetwork));
            }

            System.Random rand = new System.Random();

            for (int i = 1; i < allchildrenBrains.Count; i++)
            {
                allchildrenBrains[i] = allchildrenBrains[i].CrossoverWith(allchildrenBrains[rand.Next(1, allchildrenBrains.Count)]);
                allchildrenBrains[i].Mutate();
            }
        }
        else
        {
            for (int i = 0; i < allchildrenBrainsBackProp.Count; i++)
            {
                allchildrenBrainsBackProp[i].BackPropagate();
            }
        }
    }

    public void ALLAIDied()
    {
        if (score > 2000)
        {
            Debug.Log("Result 2000 at generation " + gen + " with " + alive + " survivors.");
            EditorApplication.Beep();
        }
        
        Restart();
    }

    public void GetNeuronNetworkInputData()
    {
        float positionToRightBranch = 0;
        float positionToLeftBranch = 0;
        float distanceToBranch = 0;
        float positionOfLeftBranchPoint = 0;
        float positionOfRightBranchPoint = 0;
        float distanceOfBranch = 0;
        Vector2 playerPosition;
        foreach (Transform PlayerObject in this.transform.parent.transform.Find("AI").transform)
        {
            GameObject player = PlayerObject.gameObject;
            playerPosition = new Vector2(player.GetComponent<RectTransform>().position.x, player.GetComponent<RectTransform>().position.y);
            List<Transform> lastTwoCanvases = new List<Transform>();
            List<Transform> nearestBranchesAbovePlayer = new List<Transform>();
            List<Transform> allBranches = new List<Transform>();
            int canvasCount = canvasContainer.transform.childCount;
            lastTwoCanvases.Add(canvasContainer.transform.GetChild(canvasCount - 1));
            lastTwoCanvases.Add(canvasContainer.transform.GetChild(canvasCount - 2));

            foreach (var item in lastTwoCanvases)
            {
                foreach (Transform column in item.GetChild(0).transform)
                {
                    if (column.transform.childCount != 0)
                    {
                        foreach (Transform currentBranch in column)
                        {
                            if (currentBranch.transform.position.y > playerPosition.y)
                            {
                                allBranches.Add(currentBranch);
                            }
                        }
                    }
                }
            }

            allBranches.Sort(delegate (Transform x, Transform y)
            {
                return x.transform.position.y.CompareTo(y.transform.position.y);
            });

            for (int i = 0; i < allBranches.Count; i++)
            {
                Transform item = allBranches[i];
                if (nearestBranchesAbovePlayer.Count != 0)
                {
                    if (item.transform.position.y < nearestBranchesAbovePlayer[0].transform.position.y)
                    {
                        nearestBranchesAbovePlayer.Clear();

                        nearestBranchesAbovePlayer.Add(item);
                    }
                    else if (item.transform.position.y == nearestBranchesAbovePlayer[0].transform.position.y)
                    {
                        nearestBranchesAbovePlayer.Add(item);
                    }
                }
                else
                {
                    nearestBranchesAbovePlayer.Add(item);
                }
            }
            if (nearestBranchesAbovePlayer.Count == 2)
            {
                foreach (var item in nearestBranchesAbovePlayer)
                {
                    if (item.transform.position.x > 0)//right
                    {
                        positionOfRightBranchPoint = item.transform.position.x - (item.transform.GetComponent<BoxCollider2D>().size.x * item.transform.localScale.x) / 2;
                    }
                    else//left
                    {
                        positionOfLeftBranchPoint = item.transform.position.x + (item.transform.GetComponent<BoxCollider2D>().size.x * item.transform.localScale.x) / 2;
                    }

                }
                Transform innerbranch = nearestBranchesAbovePlayer[0];
                distanceToBranch = innerbranch.transform.position.y - (innerbranch.transform.GetComponent<BoxCollider2D>().size.y * innerbranch.transform.localScale.y) / 2;
            }
            else
            {
                Transform innerbranch = nearestBranchesAbovePlayer[0];
                if (innerbranch.transform.position.x > 0)//right
                {
                    positionOfRightBranchPoint = innerbranch.transform.position.x - (innerbranch.transform.GetComponent<BoxCollider2D>().size.x * innerbranch.transform.localScale.x) / 2;
                    positionOfLeftBranchPoint = mixX;
                }
                else//left
                {
                    positionOfLeftBranchPoint = innerbranch.transform.position.x + (innerbranch.transform.GetComponent<BoxCollider2D>().size.x * innerbranch.transform.localScale.x) / 2;
                    positionOfRightBranchPoint = maxX;
                }
            }
            Transform autherbranch = nearestBranchesAbovePlayer[0];
            distanceOfBranch = autherbranch.transform.position.y - (autherbranch.transform.GetComponent<BoxCollider2D>().size.y * autherbranch.transform.localScale.y) / 2;
            positionToLeftBranch = playerPosition.x - positionOfLeftBranchPoint;
            positionToRightBranch = positionOfRightBranchPoint - playerPosition.x;
            distanceToBranch = distanceOfBranch - playerPosition.y;
            float[] neuralNetworkInput = new float[3] { positionToRightBranch, positionToLeftBranch, distanceToBranch };
            if (!isItBackProp)
            {
                float neuralNetworkOutput = player.transform.GetComponent<NeuralNetworkComponent>().ReturnOutput(neuralNetworkInput);
                player.transform.position += new Vector3(neuralNetworkOutput * playerSpeed * Time.deltaTime, 0, 0);
            }
            else
            {
                float expectedResult = CalculateExpectedResult(positionOfRightBranchPoint, positionOfLeftBranchPoint, distanceOfBranch, playerPosition);
                float neuralNetworkOutput = player.transform.GetComponent<BackPropNetworkComponent>().ReturnOutput(neuralNetworkInput, expectedResult);
                //Debug.Log(neuralNetworkOutput);
                if (float.IsNaN(neuralNetworkOutput))
                {
                    Debug.Log("Float is NaN.");
                }
                player.transform.position += new Vector3(neuralNetworkOutput * playerSpeed * Time.deltaTime, 0, 0);
                //Debug.Log("output: " + neuralNetworkOutput + " | expected output: " + expectedResult);
            }
        }

    }

    float CalculateExpectedResult(float positionOfRightBranchPoint, float positionOfLeftBranchPoint, float distanceOfBranch, Vector2 playerposition)
    {
        float betweenBranchesX = (positionOfRightBranchPoint - positionOfLeftBranchPoint) / 2;
        float returnableValue = 0f;
        if (distanceOfBranch <= 7f)
        {
            if (distanceOfBranch <= 1f)
            {
                distanceOfBranch = 1f;
            }
            if (betweenBranchesX > playerposition.x)
            {
                //Debug.Log("move right!");
                returnableValue = 1 / distanceOfBranch;
            }
            else if (betweenBranchesX < playerposition.x)
            {
                //Debug.Log("move left!");
                returnableValue = -1 / distanceOfBranch;
            }
        }
        return returnableValue;
    }

    void Update()
    {
        if(inputRecieved)
        {
            score += 0.1f;
            GetNeuronNetworkInputData();
            UpdateHUD();
        }
    }

    private void OnDestroy()
    {
        PlayerCollider.OnPlayerDeadNN -= PlayerCollider_OnPlayerDeadNN;
        PlayerCollider.OnPlayerDeadNNBP -= PlayerCollider_OnPlayerDeadNNBP;
        Movement.SendCanvasContainer -= Movement_SendCanvasContainer;
        MainScreen.StartGame -= MainScreen_StartGame;
    }
}
