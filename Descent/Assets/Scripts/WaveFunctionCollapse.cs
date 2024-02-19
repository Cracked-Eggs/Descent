using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveFunctionCollapse : MonoBehaviour
{
    public int mapHeight;
    public int mapWidth;
    public float mapSpacing;
    public Transform mapParent;

    public List<WFCPiece> mapPieces;

    private List<WFCCell> cells;
    private List<GameObject> instantiatedModels;

    private void Start()
    {
        CreateMap();
    }

    [ContextMenu("Regenerate Map")]
    void CreateMap()
    {
        if(instantiatedModels != null)
        {
            for (int i = 0; i < instantiatedModels.Count; i++)
            {
                Destroy(instantiatedModels[i]);
            }
        }

        instantiatedModels = new List<GameObject>();

        cells = new List<WFCCell>();
        for (int i = 0; i < mapHeight * mapWidth; i++)
        {
            WFCCell cell = new WFCCell();
            cell.possiblePieces = new List<WFCPiece>();

            for (int v = 0; v < mapPieces.Count; v++)
            {
                cell.possiblePieces.Add(mapPieces[v]);
            }

            cells.Add(cell);
        }

        //Will fill out grid row by row
        for (int i = 0; i < mapHeight; i++)
        {
            for (int v = 0; v < mapWidth; v++) 
            {
                int index = i * mapHeight + v;
                int bottomIndex = (i + 1) * mapHeight + v;
                int rightIndex = i * mapHeight + (v + 1);

                //Collapse current cell
                WFCCell currentCell = cells[index];

                int randomIndex = Random.Range(0, currentCell.possiblePieces.Count);

                GameObject model = Instantiate(currentCell.possiblePieces[randomIndex].prefab, mapParent);
                model.transform.position = new Vector3(i - mapHeight / 2, 0, v - mapWidth / 2) * mapSpacing;
                instantiatedModels.Add(model);

                WFCPiece piece = currentCell.possiblePieces[randomIndex];
                currentCell.possiblePieces = new List<WFCPiece> { piece };
                cells[index] = currentCell;

                //Propogate to neighbours
                if (i + 1 < mapHeight)
                {
                    WFCCell bottomCell = cells[bottomIndex];

                    for (int k = 0; k < bottomCell.possiblePieces.Count; k++)
                    {
                        bool foundPiece = false;
                        WFCPiece notFoundPiece = new WFCPiece();

                        for (int l = 0; l < piece.possibleAdjacencies.Count; l++)
                        {
                            if (bottomCell.possiblePieces[k].prefab.name == piece.possibleAdjacencies[l].name)
                            {
                                foundPiece = true;
                            }
                            else
                            {
                                notFoundPiece = bottomCell.possiblePieces[k];
                            }
                        }

                        if (!foundPiece)
                        {
                            bottomCell.possiblePieces.Remove(notFoundPiece);
                        }
                    }

                    cells[bottomIndex] = bottomCell;
                }

                if (v + 1 < mapWidth)
                {
                    WFCCell rightCell = cells[rightIndex];

                    for (int k = 0; k < rightCell.possiblePieces.Count; k++)
                    {
                        bool foundPiece = false;
                        WFCPiece notFoundPiece = new WFCPiece();

                        for (int l = 0; l < piece.possibleAdjacencies.Count; l++)
                        {
                            if (rightCell.possiblePieces[k].prefab.name == piece.possibleAdjacencies[l].name)
                            {
                                foundPiece = true;
                            }
                            else
                            {
                                notFoundPiece = rightCell.possiblePieces[k];
                            }
                        }

                        if (!foundPiece)
                        {
                            rightCell.possiblePieces.Remove(notFoundPiece);
                        }

                        cells[rightIndex] = rightCell;

                    }
                }
            }
        }
    }
}

[System.Serializable]
public class WFCPiece
{
    public GameObject prefab;
    public List<GameObject> possibleAdjacencies;
}

[System.Serializable]
public struct WFCCell
{
    public List<WFCPiece> possiblePieces;
}

