using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using UnityEditor.MemoryProfiler;


public class BotAI2 : MonoBehaviour
{
    public SpaceState type = SpaceState.O;
    public bool changesTeams = false;
    public static bool botEnabled = false;
    public static bool botsTurn = false;
    public float Offensiveness = 1; //between 0 and 2
    public float Defensiveness = 1; //between 0 and 2
    public float Randomness = 0.1f; //should probably remain a very low number (less than 0.2ish). Lower numbers will make the bot smarter but also more predictable
    public List<TileController> OpenTiles = new List<TileController>();
    public List<float> TileWeights = new List<float>();

    public void PlayTurn()
    {
        SpaceState currentTurnType;
        if(GameController.Instance.TurnCount % 2 == 0)
        {
            currentTurnType = SpaceState.O;
        }
        else
        {
            currentTurnType = SpaceState.X;
        }
        if(currentTurnType == type)
        {
            botsTurn = true;
            StartCoroutine(delayTurn());

        }
        else
        {
            botsTurn = false;
        }
    }
    public void RunThroughTiles()
    {
        if(changesTeams == true)
        {
            UpdateTurn();
        }
        if (GameController.Instance.GameStarted == true)
        {
            GetAllOpenTiles();
            for (int i = 0; i < OpenTiles.Count; i++)
            {
                EvaluateTile(i);
            }
            float bestTileWeight = 0;
            for (int l = 0; l < OpenTiles.Count; l++)
            {
                float currentWeight = TileWeights[l];
                if (currentWeight > bestTileWeight)
                {
                    bestTileWeight = currentWeight;
                }
            }
            List<TileController> bestTiles = new List<TileController>();
            for (int x = 0; x < OpenTiles.Count; x++) {
                if(TileWeights[x] >= bestTileWeight + Random.Range(-Randomness, 0))
                {
                    bestTiles.Add(OpenTiles[x]);
                }
            }
            TileController randomBestTile = bestTiles[Random.Range(0, bestTiles.Count)];
            randomBestTile.SelectThisTile(type);
        }
       
    }
    public IEnumerator delayTurn()
    {
        yield return new WaitForSeconds(0.4f);
        RunThroughTiles();
    }
    void UpdateTurn()
    {
        if (GameController.Instance.TurnCount % 2 == 0)
        {
            type = SpaceState.O;
        }
        else
        {
            type = SpaceState.X;
        }
    }
    void GetAllOpenTiles()
    {
        OpenTiles.Clear();
        TileWeights.Clear();
        //get a list of all available tiles
        foreach (var tile in GameController.Instance.Tiles)
        {
            if (tile.Value.State == SpaceState.None)
            {
                //empty tile
                OpenTiles.Add(tile.Value);
                TileWeights.Add(0);
            }
        }
    }
    void EvaluateTile(int tileId) {
        List<TileController> TilesToCheck = new List<TileController>() { OpenTiles[tileId] };
        List<TileController> NextTiles = new List<TileController>();
        List<TileController> AlreadyCheckedTiles = new List<TileController>();
        EdgeType[] TargetEdges;
        switch (OpenTiles[tileId].EdgeType)
        {
            case EdgeType.U:
                TargetEdges = new EdgeType[] { EdgeType.D, EdgeType.CornerDL, EdgeType.CornerDR };
                break;
            case EdgeType.D:
                TargetEdges = new EdgeType[] { EdgeType.U, EdgeType.CornerUL, EdgeType.CornerUR };
                break;
            case EdgeType.R:
                TargetEdges = new EdgeType[] { EdgeType.L, EdgeType.CornerUL, EdgeType.CornerDL };
                break;
            case EdgeType.L:
                TargetEdges = new EdgeType[] { EdgeType.R, EdgeType.CornerUR, EdgeType.CornerDR };
                break;
            case EdgeType.CornerUL:
                TargetEdges = new EdgeType[] { EdgeType.D, EdgeType.R, EdgeType.CornerDR, EdgeType.CornerDL, EdgeType.CornerUR };
                break;
            case EdgeType.CornerUR:
                TargetEdges = new EdgeType[] { EdgeType.D, EdgeType.L, EdgeType.CornerDL, EdgeType.CornerDR, EdgeType.CornerUL };
                break;
            case EdgeType.CornerDL:
                TargetEdges = new EdgeType[] { EdgeType.U, EdgeType.R, EdgeType.CornerUR, EdgeType.CornerDR, EdgeType.CornerUL };
                break;
            case EdgeType.CornerDR:
                TargetEdges = new EdgeType[] { EdgeType.U, EdgeType.L, EdgeType.CornerUL, EdgeType.CornerUR, EdgeType.CornerDL };
                break;
            default:
                return;
        }
        Debug.Log($"Starting Trace for {OpenTiles[tileId].EdgeType}. Target Edges {TargetEdges[0]}, {TargetEdges[1]}, {TargetEdges[2]}");
        while (true)
        {
            foreach (var CurrentSpace in TilesToCheck)
            {
                AlreadyCheckedTiles.Add(CurrentSpace);
                Debug.Log($"Tracing Path for {OpenTiles[tileId].EdgeType}: {CurrentSpace.GridPosition}, type {CurrentSpace.EdgeType}");
                foreach (var Connection in CurrentSpace.Connections)
                {
                    if (Connection.State == type)
                    {
                        TileWeights[tileId] += Offensiveness;
                        if (TargetEdges.Contains(Connection.EdgeType) && Connection != OpenTiles[tileId])
                        {
                            return;
                        }

                    }
                    else if(Connection.State != type && Connection.State != SpaceState.None)
                    {
                        TileWeights[tileId] += Defensiveness;

                    }
                    if (!NextTiles.Contains(Connection) && Connection != OpenTiles[tileId] && !AlreadyCheckedTiles.Contains(Connection))
                        NextTiles.Add(Connection);
                   
                }
            }
            if (NextTiles.Count == 0)
            {
                Debug.Log($"Tracing Complete for {OpenTiles[tileId].EdgeType}, result Failure");
                return;
            }
            TilesToCheck.Clear();
            foreach (var Tile in NextTiles)
            {
                TilesToCheck.Add(Tile);
            }
            NextTiles.Clear();
        }
    }
}
