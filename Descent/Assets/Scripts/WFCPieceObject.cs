using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Piece", menuName = "WFC Pieces")]
public class WFCPieceObject : ScriptableObject
{
    public List<WFCPieceObject> topAdjacencies;
    public List<WFCPieceObject> bottomAdjacencies;

    public List<WFCPieceObject> leftAdjacencies;
    public List<WFCPieceObject> rightAdjacencies;

    public List<WFCPieceObject> topLeftAdjacencies;
    public List<WFCPieceObject> topRightAdjacencies;

    public List<WFCPieceObject> bottomLeftAdjacencies;
    public List<WFCPieceObject> bottomRightAdjacencies;


    public GameObject prefab;
    [Range(0.0f, 1.0f)] public float weight;
    public static bool operator ==(WFCPieceObject o1, WFCPieceObject o2) => o1.name == o2.name;
    public static bool operator !=(WFCPieceObject o1, WFCPieceObject o2) => !(o1.name == o2.name);
}
