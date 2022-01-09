using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConstructElemController : MonoBehaviour
{
    private string userParamsPref = "STroiKaGame_UserParams";

    public string elemName;
    public Sprite elemImage;
    public int count;

    public float price;
    public float totalPrice;

    public float multiply;

    public float incomeAscending;
    public float totalIncomeAscending;

    public float upgradePrice;
    public float totalUpgradePrice;
    

    public int level;

    public GridElemController gridElemPlaced;

    public bool isMoving = false;

    // Start is called before the first frame update
    void Start()
    {
        level = 1;
        totalPrice = price;

        totalIncomeAscending = incomeAscending;
        totalUpgradePrice = upgradePrice;

        /*if (!PlayerPrefs.HasKey(userParamsPref))
            count = 1;*/
    }

    // Update is called once per frame
    void Update()
    {
        //ClearConstructElemSelection();
    }

    private void OnMouseDrag()
    {
        if(isMoving)
        {
            Vector3 mousePos = Input.mousePosition;
            Ray castPoint = Camera.main.ScreenPointToRay(mousePos);
            RaycastHit hit;
            if(Physics.Raycast(castPoint, out hit, Mathf.Infinity)) 
                transform.position = new Vector3(hit.point.x, transform.position.y, hit.point.z);
        }
    }

    private void OnMouseDown()
    {
        if (GameManager.isPlacingMode)
        {
            if (this == GameManager.currentElemSelected)
            {
                isMoving = true;
                GameManager.gridElemCollide = GameManager.currentGridElemSelected;
            }
            return;
        }
        else if (GameManager.isConstructMode || GameManager.isConstructElemSelectionMode || GameManager.isSettingsMode ||GameManager.isShopMode)
            return;
        

        GameManager.currentElemSelected = this;
        GameManager.currentGridElemSelected = gridElemPlaced;
        GameManager.instance.SwitchElemSelectingMode(true);
        GameManager.instance.canClick = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(isMoving)
        {
            if(GameManager.gridElemCollide.installedObject != null)
                GameManager.gridElemCollide.ChangeColor(GrideController.instance.buildingBusyColor);
            else GameManager.gridElemCollide.ChangeColor(GrideController.instance.buildingOKColor);

            GridElemController ge = other.gameObject.GetComponent<GridElemController>();
            GameManager.gridElemCollide = ge;
            ge.ChangeColor(Color.gray);
        }
    }

    private void OnMouseUp()
    {
        if(isMoving)
        {
            GameManager.gridElemCollide.TryPlaceItemWithDrag();
            GameManager.currentElemSelected.isMoving = false;
            GameManager.gridElemCollide = null;
        }

        GameManager.instance.canClick = true;
    }

    //temporally off, maybe)
    private void ClearConstructElemSelection()
    {
        if(Input.GetMouseButtonDown(0) && GameManager.isConstructElemSelectionMode && GameManager.instance.canClick)
        {
            GameManager.currentElemSelected = null;
            GameManager.currentGridElemSelected = null;
            GameManager.instance.SwitchElemSelectingMode(false);
        }    
    }

    public void UpgradeChange()
    {
        level++;
        totalUpgradePrice = upgradePrice * Mathf.Pow(multiply * GameManager.instance.gridLevel, level - 1);
        totalIncomeAscending = incomeAscending * multiply * GameManager.instance.gridLevel;
    }

    public void UpgradeChange(int newLevel)
    {
        level = newLevel;
        totalUpgradePrice = upgradePrice * Mathf.Pow(multiply * GameManager.instance.gridLevel, level - 1);
        totalIncomeAscending = incomeAscending * multiply * GameManager.instance.gridLevel;
    }

    public void PriceUpgrade()
    {
        count++;
        float newPrice = price * Mathf.Pow(multiply, GameManager.instance.gridLevel - 1);
        totalPrice = newPrice * Mathf.Pow(multiply, count);
        //totalPrice = price * Mathf.Pow(multiply * GameManager.instance.gridLevel, count);
    }

    public void PriceUpgrade(int newCount)
    {
        count = newCount;
        float newPrice = price * Mathf.Pow(multiply, GameManager.instance.gridLevel - 1);
        totalPrice = newPrice * Mathf.Pow(multiply, count);
    }
}
