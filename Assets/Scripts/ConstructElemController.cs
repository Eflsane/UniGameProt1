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
        if(GameManager.isConstructMode)
        {
            Vector3 mousePos = Input.mousePosition;
            Ray castPoint = Camera.main.ScreenPointToRay(mousePos);
            RaycastHit hit;
            if(Physics.Raycast(castPoint, out hit, Mathf.Infinity) && this == GameManager.currentElemSelected) 
                transform.position = new Vector3(hit.point.x, transform.position.y, hit.point.z);
        }
    }

    private void OnMouseDown()
    {
        isMoving = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(GameManager.isConstructMode)
        {
            GridElemController ge = other.gameObject.GetComponent<GridElemController>();
            GameManager.prevGridElemSelected = ge;
            if (Input.GetMouseButtonUp(0) && GameManager.currentElemSelected.isMoving)
            {
                
                ge.TryPlaceItemWithDrag();
                GameManager.currentElemSelected.isMoving = false;
            }
        }
    }

    private void OnMouseUp()
    {
        GameManager.prevGridElemSelected.TryPlaceItemWithDrag();
        GameManager.currentElemSelected.isMoving = false;
        GameManager.prevGridElemSelected = null;
    }

}
