using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.UI;
using UnityEngine;

public class CellManager : MonoBehaviour
{
    public GameObject cellPrefab;
    public int cellYDensity;

    public float cellSpawnChance;
    public int cellBaseLife;
    public List<Material> cellLifeStage;

    public float cycleDurationSeconds;

    private Vector3 areaDimension;
    private Vector3 cellDimension;

    private int cellXDensity;

    //0,0 is bottom left
    private GameObject[,] cellContainer;

    private List<Cell> createCells = new List<Cell>();
    private List<Cell> damageCells = new List<Cell>();

    // Start is called before the first frame update
    void Start()
    {
        areaDimension = GetComponent<SpriteRenderer>().bounds.size;
        cellPrefab.transform.localScale = new Vector3(areaDimension.y / cellYDensity, areaDimension.y / cellYDensity, 1);
        cellDimension = cellPrefab.GetComponent<SpriteRenderer>().bounds.size;

        cellXDensity = (int)Mathf.Ceil(areaDimension.x / cellDimension.x);

        Debug.Log(cellXDensity);

        cellContainer = new GameObject[cellXDensity, cellYDensity];

        for (int j = 0; j < cellXDensity; j++)
        {
            for (int i = 0; i < cellYDensity; i++)
            {
                cellContainer[j,i] = Instantiate(cellPrefab, new Vector3((-areaDimension.x / 2) + (cellDimension.x / 2) + (j * cellDimension.x), (-areaDimension.y / 2) + (cellDimension.y / 2) + (i * cellDimension.y), 0), Quaternion.identity);

                Cell cell = cellContainer[j, i].GetComponent<Cell>();

                cell.xPos = j;
                cell.yPos = i;

                //Make Cell Alive
                if (Random.value < cellSpawnChance)
                {
                    cell.life = cellBaseLife;
                }
                else
                {
                    cell.life = 0;
                }

                cellContainer[j, i].GetComponent<SpriteRenderer>().material = cellLifeStage[cell.life];
            }
        }

        StartCoroutine(CellLifeCycle());
    }

    IEnumerator CellLifeCycle()
    {
        createCells = new List<Cell>();
        damageCells = new List<Cell>();

        yield return new WaitForSeconds(cycleDurationSeconds);

        for (int j = 0; j < cellXDensity; j++)
        {
            for (int i = 0; i < cellYDensity; i++)
            {
                Cell cell = cellContainer[j, i].GetComponent<Cell>();

                int adjAlive = GetAdjacentAliveCount(j, i);

                if (cell.life == 0 && adjAlive >= 2)
                {
                    createCells.Add(cell);
                }
                else if(cell.life > 0 && (adjAlive < 2 || adjAlive > 3))
                {
                    damageCells.Add(cell);
                }
            }
        }

        foreach (Cell cell in createCells)
        {
            GiveCellLife(cell);
        }
        foreach(Cell cell in damageCells)
        {
            ReduceCellLife(cell);
        }

        StartCoroutine(CellLifeCycle());
    }

    public void GiveCellLife(Cell cell)
    {
        cell.life = cellBaseLife;
        cell.GetComponent<SpriteRenderer>().material = cellLifeStage[cell.life];
    }

    public void ReduceCellLife(Cell cell)
    {
        cell.life -= 1;
        cell.GetComponent<SpriteRenderer>().material = cellLifeStage[cell.life];
    }

    public int GetAdjacentAliveCount(int x, int y)
    {
        int aliveCount = 0;

        //left side
        if (x > 0)
        {
            if (cellContainer[x-1,y].GetComponent<Cell>().life > 0)
            {
                aliveCount++;
            }

            //bottom left side
            if (y > 0)
            {
                if (cellContainer[x - 1, y - 1].GetComponent<Cell>().life > 0)
                {
                    aliveCount++;
                }
            }
            //top left side
            if (y + 1 < cellYDensity)
            {
                if (cellContainer[x - 1, y + 1].GetComponent<Cell>().life > 0)
                {
                    aliveCount++;
                }
            }
        }
        //right side
        if (x + 1 < cellXDensity)
        {
            if (cellContainer[x + 1, y].GetComponent<Cell>().life > 0)
            {
                aliveCount++;
            }

            //bottom right side
            if (y > 0)
            {
                if (cellContainer[x + 1, y - 1].GetComponent<Cell>().life > 0)
                {
                    aliveCount++;
                }
            }
            //top right side
            if (y + 1 < cellYDensity)
            {
                if (cellContainer[x + 1, y + 1].GetComponent<Cell>().life > 0)
                {
                    aliveCount++;
                }
            }
        }

        //bottom side
        if (y > 0)
        {
            if (cellContainer[x, y - 1].GetComponent<Cell>().life > 0)
            {
                aliveCount++;
            }
        }
        //top side
        if (y + 1 < cellYDensity)
        {
            if (cellContainer[x, y + 1].GetComponent<Cell>().life > 0)
            {
                aliveCount++;
            }
        }

        return aliveCount;
    }
}
