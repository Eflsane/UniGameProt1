using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConstructElemController : MonoBehaviour
{
    public string elemName;
    public Image elemImage;
    public float price;
    public float incomeAscending;

    public bool isMoving = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
        if (GameManager.isConstructMode && this == GameManager.currentElemSelected)
        {
            isMoving = true;
            GameManager.gridElemCollide = GameManager.currentGridElemSelected;
        }
            
    }

    private void OnTriggerEnter(Collider other)
    {
        if(isMoving)
        {
            if(GameManager.gridElemCollide.installedObject != null)
                GameManager.gridElemCollide.ChangeColor(GameManager.gridElemCollide.buildingBusyColor);
            else GameManager.gridElemCollide.ChangeColor(GameManager.gridElemCollide.buildingOKColor);

            GridElemController ge = other.gameObject.GetComponent<GridElemController>();
            GameManager.gridElemCollide = ge;
            ge.ChangeColor(Color.gray);
            /*if (Input.GetMouseButtonUp(0) && GameManager.currentElemSelected.isMoving)
            {
                
                ge.TryPlaceItemWithDrag();
                GameManager.currentElemSelected.isMoving = false;
            }*/
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
    }

}
