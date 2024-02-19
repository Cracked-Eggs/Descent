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

                List<int> indicies = new List<int>()
                {
                    //Top/Bottom
                    (i - 1) >= 0 ? (i - 1) * mapSize + v : nullIndex,
                    (i + 1) < mapSize ? (i + 1) * mapSize + v : nullIndex,

                    //Left/Right
                    (v - 1) >= 0 ? i * mapSize + (v - 1) : nullIndex,
                    (v + 1) < mapSize ? i * mapSize + (v + 1) : nullIndex,
                };

                WFCCell currentCell = cells[currentIndex];

                int randomIndex = Random.Range(0, currentCell.possiblePieces.Count);

                GameObject model = Instantiate(currentCell.possiblePieces[randomIndex].prefab, mapParent);
                model.transform.position = new Vector3(i - mapSize / 2, 0, v - mapSize / 2) * mapSpacing;
                instantiatedModels.Add(model);

                WFCPieceObject piece = currentCell.possiblePieces[randomIndex];
                currentCell.possiblePieces = new List<WFCPieceObject> { piece };

                cells[currentIndex] = currentCell;
                Debug.Log(i + " " + v + " " + piece.name);

                for (int j = 0; j < indicies.Count; j++)
                {
                    List<WFCPieceObject> adjacencies = new List<WFCPieceObject>();
                    switch (j)
                    {
                        case 0:
                            Debug.Log("Removing top pieces");
                            adjacencies = piece.topAdjacencies; 
                            break;
                        case 1:
                            Debug.Log("Removing bottom pieces");
                            adjacencies = piece.bottomAdjacencies;
                            break;
                        case 2:
                            Debug.Log("Removing left pieces");
                            adjacencies = piece.leftAdjacencies;
                            break;
                        case 3:
                            Debug.Log("Removing right pieces");
                            adjacencies = piece.rightAdjacencies;
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

