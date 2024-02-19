using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveFunctionCollapse : MonoBehaviour
{
    public int mapSize;
    public float mapSpacing;
    public Transform mapParent;

    public List<WFCPieceObject> mapPieces;

    private List<WFCCell> cells;
    private List<GameObject> instantiatedModels;
    private int nullIndex = -1;

    private void Start()
    {
        CreateMap();
    }

    private void Propogate(int index, List<WFCPieceObject> adjacencies)
    {
        if (index == nullIndex)
            return;

        WFCCell originalCell = cells[index];
        WFCCell newCell = new WFCCell(cells[index]);

        for (int i = 0; i < originalCell.possiblePieces.Count; i++)
        {
            WFCPieceObject currentPiece = originalCell.possiblePieces[i];

            if (!adjacencies.Contains(currentPiece))
            {
                Debug.Log("Removed possible piece " + currentPiece.name);
                newCell.possiblePieces.Remove(currentPiece);
            }
        }

        cells[index] = newCell;
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
        for (int i = 0; i < mapSize * mapSize; i++)
        {
            WFCCell cell = new WFCCell();
            cell.possiblePieces = new List<WFCPieceObject>(mapPieces);

            cells.Add(cell);
        }

        for (int i = 0; i < mapSize; i++)
        {
            for (int v = 0; v < mapSize; v++) 
            {
                int currentIndex = i * mapSize + v;

                //God help me when I make this 3D
                List<int> indicies = new List<int>()
                {
                    //Top/Bottom
                    (i - 1) >= 0 ? (i - 1) * mapSize + v : nullIndex,
                    (i + 1) < mapSize ? (i + 1) * mapSize + v : nullIndex,

                    //Left/Right
                    (v - 1) >= 0 ? i * mapSize + (v - 1) : nullIndex,
                    (v + 1) < mapSize ? i * mapSize + (v + 1) : nullIndex,

                    //TopLeft/TopRight
                    (v - 1) >= 0 && (i - 1) >= 0 ? (i - 1) * mapSize + (v - 1) : nullIndex,
                    (v + 1) < mapSize && (i - 1) >= 0 ? (i - 1) * mapSize + (v + 1) : nullIndex,
                                        
                    //BottomLeft/BottomRight
                    (v - 1) >= 0 && (i + 1) < mapSize ? (i + 1) * mapSize + (v - 1) : nullIndex,
                    (v + 1) < mapSize && (i + 1) < mapSize ? (i + 1) * mapSize + (v + 1) : nullIndex,
                };

                WFCCell currentCell = cells[currentIndex];

                int index = 0;
                float randomValue = Random.value;

                for (int j = 0; j < currentCell.possiblePieces.Count; j++)
                {
                    float weight = currentCell.possiblePieces[j].weight;

                    if (randomValue <= weight)
                    {
                        index = j;
                        break;
                    }

                    randomValue -= weight;
                }

                GameObject model = Instantiate(currentCell.possiblePieces[index].prefab, mapParent);
                model.transform.position = new Vector3(i - mapSize / 2, 0, v - mapSize / 2) * mapSpacing;
                instantiatedModels.Add(model);

                WFCPieceObject piece = currentCell.possiblePieces[index];
                currentCell.possiblePieces = new List<WFCPieceObject> { piece };

                cells[currentIndex] = currentCell;
                Debug.Log(i + " " + v + " " + piece.name);

                for (int j = 0; j < indicies.Count; j++)
                {
                    List<WFCPieceObject> adjacencies = new List<WFCPieceObject>();
                    switch (j)
                    {
                        case 0:
                            adjacencies = piece.topAdjacencies; 
                            break;
                        case 1:
                            adjacencies = piece.bottomAdjacencies;
                            break;
                        case 2:
                            adjacencies = piece.leftAdjacencies;
                            break;
                        case 3:
                            adjacencies = piece.rightAdjacencies;
                            break;
                        case 4:
                            adjacencies = piece.topLeftAdjacencies;
                            break;
                        case 5:
                            adjacencies = piece.topRightAdjacencies;
                            break;
                        case 6:
                            adjacencies = piece.bottomLeftAdjacencies;
                            break;
                        case 7:
                            adjacencies = piece.bottomRightAdjacencies;
                            break;
                    }

                    Propogate(indicies[j], adjacencies);
                }
            }
        }
    }
}

[System.Serializable]
public struct WFCCell
{
    public WFCCell(WFCCell other) => possiblePieces = new List<WFCPieceObject> (other.possiblePieces);
    public List<WFCPieceObject> possiblePieces;
}

