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


    public GameObject prefab;
    public static bool operator ==(WFCPieceObject o1, WFCPieceObject o2) => o1.name == o2.name;
    public static bool operator !=(WFCPieceObject o1, WFCPieceObject o2) => !(o1.name == o2.name);
}
