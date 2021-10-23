using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridElemController : MonoBehaviour
{
    public ConstructElemController installedObject;

    public Renderer __renderer;
    public Color baseColor;
    public Color buildingOKColor;
    public Color buildingBusyColor;
    public Color buildingCollisColor;


    public bool isBusy = false;
    // Start is called before the first frame update
    void Start()
    {
        __renderer = GetComponent<Renderer>();

        baseColor = Color.white;
        buildingOKColor = Color.green;
        buildingBusyColor = Color.red;
        buildingCollisColor = Color.yellow;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseUpAsButton()
    {
        if(GameManager.isConstructMode)
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
                    GameManager.currentGridElemSelected.ChangeColor(buildingOKColor);
                }
                else
                {
                    GameManager.currentGridElemSelected.ChangeColor(buildingBusyColor);
                }
            }
            
                

            if (installedObject != null)
            {
                GameManager.currentGridElemSelected = this;
                __renderer.material.SetColor("_Color", buildingCollisColor);
                GameManager.currentElemSelected.transform.position = new Vector3(transform.position.x, GameManager.currentElemSelected.gameObject.transform.localScale.y / 2, transform.position.z);

                return;
            }

            installedObject = GameManager.currentElemSelected;
            GameManager.currentGridElemSelected = this;
            __renderer.material.SetColor("_Color", buildingBusyColor);
            installedObject.transform.position = new Vector3(transform.position.x, GameManager.currentElemSelected.gameObject.transform.localScale.y / 2, transform.position.z);

        }
        
    }

    /*private void OnMouseOver()
    {
        if (GameManager.isConstructMode)
        {
            if(Input.GetMouseButtonUp(0))
                TryPlaceItemWithDrag();
        }
    }*/

    /*private void OnTriggerStay(Collider other)
    {
        if (GameManager.isConstructMode)
        {
            if (Input.GetMouseButtonUp(0) && GameManager.currentElemSelected.isMoving)
            {
                TryPlaceItemWithDrag();
                GameManager.currentElemSelected.isMoving = false;
            }
                

            Debug.Log("Construction Element in grid");
        }
    }*/

    public void TryPlaceItemWithDrag()
    {

        if (GameManager.currentElemSelected != null)
        {
            if (GameManager.currentGridElemSelected.installedObject != null)
            {
                if (GameManager.currentGridElemSelected.installedObject.Equals(GameManager.currentElemSelected))
                {
                    GameManager.currentGridElemSelected.installedObject = null;
                    GameManager.currentGridElemSelected.ChangeColor(buildingOKColor);
                }
                else
                {
                    GameManager.currentGridElemSelected.ChangeColor(buildingBusyColor);
                }
            }



            if (installedObject != null)
            {
                GameManager.currentGridElemSelected = this;
                __renderer.material.SetColor("_Color", buildingCollisColor);
                GameManager.currentElemSelected.transform.position = new Vector3(transform.position.x, GameManager.currentElemSelected.gameObject.transform.localScale.y / 2, transform.position.z);

                return;
            }

            installedObject = GameManager.currentElemSelected;
            GameManager.currentGridElemSelected = this;
            __renderer.material.SetColor("_Color", buildingBusyColor);
            installedObject.transform.position = new Vector3(transform.position.x, GameManager.currentElemSelected.gameObject.transform.localScale.y / 2, transform.position.z);
        }
    }

    public void ChangeColor(Color color)
    {
        __renderer.material.SetColor("_Color", color);
    }
}
