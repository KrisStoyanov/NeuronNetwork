using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Movement : MonoBehaviour
{
    public Vector3 movingPrefabsStartingPosition = new Vector3(0, 29.6f, 90);

    public static event Action<Transform> SendCanvasContainer;
    public static event Action<GameObject> newElementSpawned;
    public static event Action<float> sendSpeed;

    private float speed = 6f;
    private int   NextElementNumber = 0;

    Dictionary<int, List<int>> allSegments;
    void Start()
    {
        GenerateAI.getCanvasContainer += GenerateAI_getCanvasContainer;
        GenerateAI.restartMap += GenerateAI_restartMap;
        allSegments = new Dictionary<int, List<int>>();
        SetupSegments();
    }
    private void OnDestroy()
    {
        GenerateAI.getCanvasContainer -= GenerateAI_getCanvasContainer;
        GenerateAI.restartMap -= GenerateAI_restartMap;
    }

    private void GenerateAI_getCanvasContainer()
    {
        SendCanvasContainer?.Invoke(this.transform.Find("GameObjectContainer"));
    }

    private void GenerateAI_restartMap()
    {
        this.transform.position = movingPrefabsStartingPosition;
        foreach (Transform child in this.transform.Find("GameObjectContainer").transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        GameObject newelementf = (GameObject)Instantiate(Resources.Load("Prefabs/Menus/InGameCanvases/Canvas 3"), this.transform.GetChild(0));
        GameObject newelements = (GameObject)Instantiate(Resources.Load("Prefabs/Menus/InGameCanvases/Canvas 2"), this.transform.GetChild(0));
        GameObject newelementt = (GameObject)Instantiate(Resources.Load("Prefabs/Menus/InGameCanvases/Canvas 1"), this.transform.GetChild(0));

    }

    private void SetupSegments()
    {
        allSegments.Add(1, new List<int> { 2, 3 });
        allSegments.Add(2, new List<int> { 1, 3 });
        allSegments.Add(3, new List<int> { 1, 2 });
    }
    private void DestroyNeededElement()
    {
        int childIndexes = this.transform.GetChild(0).childCount - 1;
        Destroy(this.transform.GetChild(0).GetChild(childIndexes).gameObject);
    }
    private void ChooseNextElement()
    {
        string lastElementName = this.transform.GetChild(0).GetChild(0).name;
        int lastElementNumber = int.Parse(lastElementName.Split(' ')[1].Split('(')[0]);
        if (!allSegments.TryGetValue(lastElementNumber, out var newSegmentSuggetions))
        {
            Debug.LogError("can not get next element");
        }
        int randomNextElementIndex = UnityEngine.Random.Range(0, newSegmentSuggetions.Count);
        NextElementNumber = newSegmentSuggetions[randomNextElementIndex];
    }
    private void SpawnNextElement()
    {
        GameObject newelement = (GameObject)Instantiate(Resources.Load("Prefabs/Menus/InGameCanvases/Canvas " + NextElementNumber), this.transform.GetChild(0));
        newelement.transform.SetAsFirstSibling();
        newElementSpawned?.Invoke(this.gameObject);

    }
    void Update()
    {
        if (this.transform.position.y <= 9.37f)
        {
            DestroyNeededElement();
            this.transform.position = new Vector3(this.transform.position.x, 9.37f + this.GetComponent<RectTransform>().rect.height, this.transform.position.z);
            ChooseNextElement();
            SpawnNextElement();
        }
        this.transform.position -= new Vector3(0, speed * Time.deltaTime, 0);
    }
}
