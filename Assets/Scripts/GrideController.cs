using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GrideController : MonoBehaviour
{
    //Part of udpate grid in real time
    /*public int gridElemsCount = 0;
    private int oldGridElemsCount = 0;*/

    public int startGridSize = 0;

    public int GridSize 
    { 
        get
        {
            return __gridElems.GetLength(0);
        }            
    }

    public float gridElemsSize = 0.1f;

    private GridElemController[,] __gridElems;
    public static GridElemController firstElem;
    public GridElemController ge;

    public Color baseColor;
    public Color buildingOKColor;
    public Color buildingBusyColor;
    public Color buildingCollisColor;

    public static GrideController instance;

    // Start is called before the first frame update
    void Start()
    {
        if(instance == null)
            instance = this;
        else if (instance == this)
            Destroy(gameObject);

        //CreateGrid();
        CreateGridWithParams(startGridSize);

        baseColor = Color.white;
        buildingOKColor = Color.green;
        buildingBusyColor = Color.red;
        buildingCollisColor = Color.yellow;

        ColorizeAllGrid(baseColor);
    }

    // Update is called once per frame
    void Update()
    {
        //UpdateGridInRealtime();
        //ColorizeAllGrid();
        
    }

    //First(Maybe will depricated as part of real time grid) creation of the grid
    /*private void CreateGrid()
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
    }*/

    //It's too heavy to pick up
    /*private void UpdateGridInRealtime()
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
    }*/

    public void CreateGridWithParams(int gridSize) 
    {
        __gridElems = new GridElemController[gridSize, gridSize];
        ge.transform.localScale = new Vector3(gridElemsSize, ge.transform.localScale.y, gridElemsSize);
        Vector3 gridStartPos = (new Vector3(-0.5f * gridElemsSize * 10 * gridSize / 2, 0, 0.5f * gridElemsSize * 10 * gridSize / 2));
        Vector3 gridPos = new Vector3(gridStartPos.x, gridStartPos.y, gridStartPos.z);
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                GridElemController getedElem = Instantiate(ge, gridPos, new Quaternion(0, 0, 0, 0));
                __gridElems[i, j] = getedElem;
                //getedElem.transform.parent = transform;

                gridPos = new Vector3(gridPos.x + gridElemsSize * 10, gridPos.y, gridPos.z);
            }

            gridPos = new Vector3(gridStartPos.x, gridPos.y, gridPos.z + gridElemsSize * 10);
        }
        firstElem = __gridElems[0, 0];
    }

    public void DestroyGrid()
    {
        for(int i = 0; i < __gridElems.GetLength(0); i++)
        {
            for(int j = 0; j < __gridElems.GetLength(1); j++)
            {
                Destroy(__gridElems[i, j].installedObject?.gameObject);
                Destroy(__gridElems[i, j].gameObject);
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
                        __gridElems[i, j].ChangeColor(buildingOKColor);
                    }
                    else
                        __gridElems[i, j].ChangeColor(buildingBusyColor);

                }
            }
        }
        else if (!GameManager.isConstructMode && GameManager.isColoringGrid)
        {
            for (int i = 0; i < __gridElems.GetLength(0); i++)
            {
                for (int j = 0; j < __gridElems.GetLength(1); j++)
                {
                    __gridElems[i, j].ChangeColor(baseColor);
                }       
            }
        }

        GameManager.isColoringGrid = false;
    }

    public void ColorizeAllGrid(Color color)
    {
        for (int i = 0; i < __gridElems.GetLength(0); i++)
        {
            for (int j = 0; j < __gridElems.GetLength(1); j++)
            {
                if (__gridElems[i, j].installedObject != null && (GameManager.isConstructMode || GameManager.isPlacingMode))
                {
                    __gridElems[i, j].ChangeColor(buildingBusyColor);
                }
                else
                    __gridElems[i, j].ChangeColor(color);

            }
        }
        
    }


    public void LoadGridFromPrefs(string loadingElemsString)
    {
        if (string.IsNullOrEmpty(loadingElemsString) || loadingElemsString == "{}")
            return;

        List<ConstructElemSaveCompress> loadingElems = new List<ConstructElemSaveCompress>();
        loadingElems.AddRange(JsonHelper.FromJson<ConstructElemSaveCompress>(loadingElemsString));
        for (int i = 0; i < loadingElems.Count; i++)
        {
            ConstructElemController prefabElem = GameManager.instance.constructElemsPrefabs.Where<ConstructElemController>(x => x.elemName == loadingElems[i].elemName).ToArray()[0];
            ConstructElemController constructElemParamsKeeper = GameManager.instance.constructElemsParamsKeepers.Where<ConstructElemController>(x => x.elemName == loadingElems[i].elemName).ToArray()[0];
            ConstructElemController  elem = Instantiate(prefabElem);
            elem.UpgradeChange(loadingElems[i].level);

            constructElemParamsKeeper.count++;
            constructElemParamsKeeper.PriceUpgrade(constructElemParamsKeeper.count);
            elem.transform.localScale = new Vector3(firstElem.transform.localScale.x * 5, elem.transform.localScale.y, firstElem.transform.localScale.z * 5);
            __gridElems[loadingElems[i].yOnGrid, loadingElems[i].xOnGrid].TryPlaceItem(elem);
                
        }
    }

    public string SaveGridToPrefs()
    {
        List<ConstructElemSaveCompress>  savingElems = new List<ConstructElemSaveCompress>();
        for (int i = 0; i < GridSize; i++)
        {
            for (int j = 0; j < GridSize; j++)
            {
                if (__gridElems[i, j].installedObject != null)
                {
                    savingElems.Add(new ConstructElemSaveCompress(__gridElems[i, j].installedObject.elemName, i, j, __gridElems[i, j].installedObject.level));
                }
            }
        }
        string s = JsonHelper.ToJson<ConstructElemSaveCompress>(savingElems.ToArray());
        return s;
    }
}
