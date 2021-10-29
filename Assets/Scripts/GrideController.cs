using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrideController : MonoBehaviour
{
    public int gridElemsCount = 0;
    private int oldGridElemsCount = 0;

    public float gridElemsSize = 0.1f;

    private GridElemController[,] __gridElems;
    public static GridElemController firstElem;
    public GridElemController ge;

    // Start is called before the first frame update
    void Start()
    {
        CreateGrid();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateGrid();
        ColorizeAllGrid();
    }

    private void CreateGrid()
    {
        oldGridElemsCount = gridElemsCount;
        __gridElems = new GridElemController[gridElemsCount, gridElemsCount];
        ge.transform.localScale = new Vector3(gridElemsSize, gridElemsSize, gridElemsSize);
        Vector3 gridStartPos = (new Vector3(-0.5f * gridElemsSize * 10 * gridElemsCount / 2, 0, 0.5f * gridElemsSize * 10 * gridElemsCount / 2));
        Vector3 gridPos = new Vector3(gridStartPos.x, gridStartPos.y, gridStartPos.z);
        for (int i = 0; i < gridElemsCount; i++)
        {
            for (int j = 0; j < gridElemsCount; j++)
            {
                GridElemController getedElem = Instantiate(ge, gridPos, new Quaternion(0, 0, 0, 0));
                __gridElems[i, j] = getedElem;

                gridPos = new Vector3(gridPos.x + gridElemsSize * 10, gridPos.y, gridPos.z);
            }

            gridPos = new Vector3(gridStartPos.x, gridPos.y, gridPos.z + gridElemsSize * 10);
        }

        firstElem = __gridElems[0, 0];
    }

    private void UpdateGrid()
    {
        ge.transform.localScale = new Vector3(gridElemsSize, gridElemsSize, gridElemsSize);
        Vector3 gridStartPos = (new Vector3(-0.5f * gridElemsSize * 10 * gridElemsCount / 2, 0, 0.5f * gridElemsSize * 10 * gridElemsCount / 2));
        Vector3 gridPos = new Vector3(gridStartPos.x, gridStartPos.y, gridStartPos.z);

        if (oldGridElemsCount != gridElemsCount)
        {
            for (int i = 0; i < oldGridElemsCount; i++)
            {
                for (int j = 0; j < oldGridElemsCount; j++)
                {
                    Destroy(__gridElems[i, j].gameObject);
                }
            }


            CreateGrid();
        }
        else
        {
            for (int i = 0; i < gridElemsCount; i++)
            {
                for (int j = 0; j < gridElemsCount; j++)
                {
                    __gridElems[i, j].transform.localScale = new Vector3(gridElemsSize, gridElemsSize, gridElemsSize);
                    __gridElems[i, j].transform.position = new Vector3(gridPos.x, gridPos.y, gridPos.z);

                    gridPos = new Vector3(gridPos.x + gridElemsSize * 10, gridPos.y, gridPos.z);


                }

                gridPos = new Vector3(gridStartPos.x, gridPos.y, gridPos.z + gridElemsSize * 10);
            }
        }
    }

    private void ColorizeAllGrid()
    {
        if (GameManager.isConstructMode && GameManager.isColoringGrid)
        {
            for(int i = 0; i < __gridElems.GetLength(0); i++)
            {
                for(int j = 0; j < __gridElems.GetLength(1); j++)
                {
                    if (__gridElems[i, j].installedObject == null)
                    {
                        __gridElems[i, j].ChangeColor(__gridElems[i, j].buildingOKColor);
                    }
                    else
                        __gridElems[i, j].ChangeColor(__gridElems[i, j].buildingBusyColor);

                }
            }
            GameManager.isColoringGrid = false;
        }
        else if (!GameManager.isConstructMode && GameManager.isColoringGrid)
        {
            for (int i = 0; i < __gridElems.GetLength(0); i++)
            {
                for (int j = 0; j < __gridElems.GetLength(1); j++)
                {
                    __gridElems[i, j].ChangeColor(__gridElems[i, j].baseColor);
                }       
            }
            GameManager.isColoringGrid = false;
        }
    }
}
