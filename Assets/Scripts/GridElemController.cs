using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridElemController : MonoBehaviour
{
    public ConstructElemController installedObject;

    public Renderer __renderer;


    public bool isBusy = false;
    // Start is called before the first frame update
    void Start()
    {
        __renderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseUpAsButton()
    {
        if(GameManager.isPlacingMode)
        {
            TryPlaceItem(false);
        }
        
    }

    public void TryPlaceItem(bool isFirst)
    {
        if(GameManager.currentElemSelected != null)
        {
            if(GameManager.currentGridElemSelected.installedObject != null)
            {
                if (GameManager.currentGridElemSelected.installedObject.Equals(GameManager.currentElemSelected))
                {
                    GameManager.currentGridElemSelected.installedObject = null;
                    GameManager.currentGridElemSelected.ChangeColor(GrideController.instance.buildingOKColor);
                }
                else
                {
                    GameManager.currentGridElemSelected.ChangeColor(GrideController.instance.buildingBusyColor);
                }
            }
            
                

            if (installedObject != null)
            {
                GameManager.currentGridElemSelected = this;
                __renderer.material.SetColor("_Color", GrideController.instance.buildingCollisColor);
                GameManager.currentElemSelected.transform.position = new Vector3(transform.position.x, GameManager.currentElemSelected.gameObject.transform.localScale.y / 2, transform.position.z);

                return;
            }

            installedObject = GameManager.currentElemSelected;
            GameManager.currentGridElemSelected = this;
            __renderer.material.SetColor("_Color", GrideController.instance.buildingBusyColor);
            installedObject.transform.position = new Vector3(transform.position.x, GameManager.currentElemSelected.gameObject.transform.localScale.y / 2, transform.position.z);
        }
        
    }
    public void TryPlaceItemWithDrag()
    {

        if (GameManager.currentElemSelected != null)
        {
            if (GameManager.currentGridElemSelected.installedObject != null)
            {
                if (GameManager.currentGridElemSelected.installedObject.Equals(GameManager.currentElemSelected))
                {
                    GameManager.currentGridElemSelected.installedObject = null;
                    GameManager.currentGridElemSelected.ChangeColor(GrideController.instance.buildingOKColor);
                }
                else
                {
                    GameManager.currentGridElemSelected.ChangeColor(GrideController.instance.buildingBusyColor);
                }
            }

            if (installedObject != null)
            {
                GameManager.currentGridElemSelected = this;
                __renderer.material.SetColor("_Color", GrideController.instance.buildingCollisColor);
                GameManager.currentElemSelected.transform.position = new Vector3(transform.position.x, GameManager.currentElemSelected.gameObject.transform.localScale.y / 2, transform.position.z);

                return;
            }

            installedObject = GameManager.currentElemSelected;
            GameManager.currentGridElemSelected = this;
            __renderer.material.SetColor("_Color", GrideController.instance.buildingBusyColor);
            installedObject.transform.position = new Vector3(transform.position.x, GameManager.currentElemSelected.gameObject.transform.localScale.y / 2, transform.position.z);
        }
    }

    public void TryPlaceItem(ConstructElemController elem)
    {       

        installedObject = elem;
        elem.gridElemPlaced = this;
        installedObject.transform.position = new Vector3(transform.position.x, elem.gameObject.transform.localScale.y / 2, transform.position.z);
        
    }

    public void ChangeColor(Color color)
    {
        __renderer.material.SetColor("_Color", color);
    }
}
