using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ConstructElemSaveCompress
{
    public string elemName;

    public int yOnGrid;
    public int xOnGrid;

    public int level;

    public ConstructElemSaveCompress()
    {

    }

    public ConstructElemSaveCompress(string elemName, int yOnGrid, int xOnGrid, int level)
    {
        this.elemName = elemName;
        this.yOnGrid = yOnGrid;
        this.xOnGrid = xOnGrid;
        this.level = level;
    }
}
