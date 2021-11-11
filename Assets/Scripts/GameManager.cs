﻿using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public static bool isConstructMode = false;
    public static bool isColoringGrid = false;
    public static bool canPlace = false;

    public ConstructElemController[] constructElem;
    public ConstructElemController[] constructElems = new ConstructElemController[1];
    public ConstructElemUIController[] __constructElemsUI;
    Button but;

    public static ConstructElemController currentElemSelected;
    public static GridElemController currentGridElemSelected;
    public static GridElemController gridElemCollide;

    public GameObject mainPanel;
    public GameObject constructPanel;
    public GameObject placingItemPanel;
    public GameObject placeButton;

    public TMPro.TextMeshProUGUI scoreCounterText;
    public static float scoreCounter;
    public float startScore;
    public static float income;
    public float basicIncome;
    public static float scoreMultypler;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
            instance = this;
        else if (instance == this)
            Destroy(gameObject);

        constructElem = Resources.LoadAll<ConstructElemController>("ConstructElemsPrefabs/");
        for(int i = 0; i < constructElem.Length; i++)
        {
            Debug.Log(i + constructElem[i].name);

        }

        //__constructElemsUI = new ConstructElemUIController[constructElems.Length];

        scoreCounter = startScore;
        income = basicIncome;
        scoreMultypler = 1.0f;
        scoreCounterText.text = "$ " + scoreCounter;
    }

    // Update is called once per frame
    void Update()
    {
        scoreCounterText.text = "$ " + string.Format("{0:0.0}", scoreCounter);
        scoreCounter += income * scoreMultypler * Time.deltaTime;

        for(int i = 0; i < __constructElemsUI.Length; i++)
        {
            if(scoreCounter < constructElems[i].price)
            {
                __constructElemsUI[i].GetComponent<Button>().interactable = false;
            }
            else
            {
                __constructElemsUI[i].GetComponent<Button>().interactable = true;
            }
        }

        if(currentElemSelected != null && currentGridElemSelected != null)
        {
            if (!currentElemSelected.Equals(currentGridElemSelected.installedObject))
            {
                placeButton.GetComponent<Button>().interactable = false;
                Debug.Log("false");
            }
            else
            {
                placeButton.GetComponent<Button>().interactable = true;
                Debug.Log("true");
            }
        }
        
    }

    public void SwitchContructionMode()
    {
        isColoringGrid = true;

        if (isConstructMode)
        {
            isConstructMode = false;
            mainPanel.SetActive(true);
            constructPanel.SetActive(false);
        }

        else
        {
            isConstructMode = true;
            mainPanel.SetActive(false);
            constructPanel.SetActive(true);
        }
    }

    public void SwitchPlacingMode()
    {
        if (isConstructMode && placingItemPanel.activeInHierarchy)
        {
            constructPanel.SetActive(true);
            placingItemPanel.SetActive(false);

        }

        else
        {
            constructPanel.SetActive(false);
            placingItemPanel.SetActive(true);
        }
    }

    public void CancelPlacing()
    {
        if (currentElemSelected.Equals(currentGridElemSelected.installedObject))
            currentGridElemSelected.ChangeColor(currentGridElemSelected.buildingOKColor);
        Destroy(currentElemSelected.gameObject);
        currentGridElemSelected = null;
        SwitchPlacingMode();
    }

    public void FinishPlacingItemOnGrid()
    {
        income += currentElemSelected.incomeAscending;
        scoreCounter -= currentElemSelected.price;
        currentElemSelected = null;
        currentGridElemSelected = null;
        SwitchPlacingMode();

    }

    public void CreateConstructElem()
    {
        SwitchPlacingMode();

        constructElem[0].transform.localScale = new Vector3(GrideController.firstElem.transform.localScale.x * 5, GrideController.firstElem.transform.localScale.y * 5, GrideController.firstElem.transform.localScale.z * 5);
        currentGridElemSelected = GrideController.firstElem;
        currentElemSelected = Instantiate(constructElem[0], new Vector3(currentGridElemSelected.transform.position.x, constructElems[0].gameObject.transform.localScale.y / 2, currentGridElemSelected.transform.position.z), currentGridElemSelected.transform.rotation);
        currentGridElemSelected.TryPlaceItem(false);
    }

    public void CreateConstructElem(int elemIndex)
    {
        SwitchPlacingMode();

        constructElem[elemIndex].transform.localScale = new Vector3(GrideController.firstElem.transform.localScale.x * 5, GrideController.firstElem.transform.localScale.y * 5, GrideController.firstElem.transform.localScale.z * 5);
        currentGridElemSelected = GrideController.firstElem;
        currentElemSelected = Instantiate(constructElem[elemIndex], new Vector3(currentGridElemSelected.transform.position.x, constructElems[0].gameObject.transform.localScale.y / 2, currentGridElemSelected.transform.position.z), currentGridElemSelected.transform.rotation);
        currentGridElemSelected.TryPlaceItem(false);
    }
}
