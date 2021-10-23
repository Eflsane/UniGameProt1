using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public static bool isConstructMode = false;
    public static bool isColoringGrid = false;
    public static bool canPlace = false;

    public ConstructElemController[] constructElems = new ConstructElemController[1];
    public ConstructElemUIController[] __constructElemsUI;

    public static ConstructElemController currentElemSelected;
    public static GridElemController currentGridElemSelected;
    public static GridElemController prevGridElemSelected;

    public GameObject mainPanel;
    public GameObject constructPanel;
    public GameObject placingItemPanel;
    public GameObject placeButton;

    public TMPro.TextMeshProUGUI scoreCounterText;
    public static float scoreCounter;
    public static float income;
    public static float scoreMultypler;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
            instance = this;
        else if (instance == this)
            Destroy(gameObject);

        //__constructElemsUI = new ConstructElemUIController[constructElems.Length];

        scoreCounter = 0f;
        income = 1.0f;
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

        constructElems[0].transform.localScale = new Vector3(GrideController.firstElem.transform.localScale.x * 5, GrideController.firstElem.transform.localScale.y * 5, GrideController.firstElem.transform.localScale.z * 5);
        currentGridElemSelected = GrideController.firstElem;
        currentElemSelected = Instantiate(constructElems[0], new Vector3(currentGridElemSelected.transform.position.x, constructElems[0].gameObject.transform.localScale.y / 2, currentGridElemSelected.transform.position.z), currentGridElemSelected.transform.rotation);
        currentGridElemSelected.TryPlaceItem(false);
    }
}
