using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using UnityEngine.UI;
using TMPro;
using System.Linq;

[System.Serializable]
public class MapData
{
    public string Title;
    public string Description;
    public int Width;
    public int Height;
    public float BoardScale = 1f;
    public List<TileData> Tiles;
    public MapData(int Width = 4, int Height = 4)
    {
        this.Width = Width;
        this.Height = Height;
        Tiles = new List<TileData>();
        for(int i = 0; i < Width * Height; i++)
        {
            Tiles.Add(new TileData());
        }
    }
}
public class GameController : MonoBehaviour
{
    public static GameController Instance;
    public MapData CurrentMap;
    public Dictionary<Vector2, TileController> Tiles;
    [SerializeField] GameObject TilePrefab;
    public int TurnCount;
    [SerializeField] Image Xup;
    [SerializeField] Image Oup;
    public bool GameStarted;
    [SerializeField] TMP_Text Result;
    public List<TileController> EdgeTiles = new List<TileController>();
    [SerializeField] GameObject PlayButton;
    [SerializeField] Transform MapContainer;
    public BotAI2 bot;

    public bool EditMode;
    private void Start()
    {
        GameController.Instance = this;
        if(bot == null)
        {
            bot = GameObject.Find("Bot").GetComponent<BotAI2>();
        }
    }
    public void ExportMap()
    {
#if UNITY_EDITOR
        File.WriteAllText($"Assets/Resources/Maps/{CurrentMap.Title}.json", JsonConvert.SerializeObject(CurrentMap, Formatting.Indented));
#endif
    }
    public void StartGame()
    {
        LoadMap(CurrentMap);
        Result.text = string.Empty;
        GameStarted = true;
        PlayButton.SetActive(false);
        TurnCount = 0;
        NextMove();
    }
    public void NextMove()
    {
        TurnCount++;
        Oup.enabled = (TurnCount % 2 == 0);
        Xup.enabled = !(TurnCount % 2 == 0);
        if(BotAI2.botEnabled == true)
        {
            bot.PlayTurn();
        }
        CheckVictory();
    }
    public void CheckVictory()
    {
        foreach (var Tile in EdgeTiles)
        {
            if (Tile.State != SpaceState.None)
            {
                TraceConnections(Tile);
            }

        }
        if (TurnCount > 16 && GameStarted)
        {
            Result.text = "Out of moves";
            GameStarted = false;
            PlayButton.SetActive(true);
        }
    }
    public void LoadMap(MapData Map)
    {
        StartCoroutine(FancyLoadMap(Map));
    }
    IEnumerator FancyLoadMap(MapData Map)
    {
        MapContainer.localScale = new Vector3(Map.BoardScale, 1, Map.BoardScale);
        CurrentMap = Map;
        if (Tiles != null)
        {
            foreach (var Tile in Tiles)
            {
                if (Tile.Value != null)
                {
                    Destroy(Tile.Value.gameObject);
                }
            }
            Tiles.Clear();
        }
        Tiles = new Dictionary<Vector2, TileController>();
        int counter = 0;
        float OffsetX = (CurrentMap.Width / 2f) - 0.5f;
        float OffsetZ = (CurrentMap.Height / 2f) - 0.5f;
        EdgeTiles.Clear();
        for (int i = 0; i < CurrentMap.Width; i++)
        {
            for (int e = CurrentMap.Height - 1; e >= 0; e--)
            {
                var TileScript = Instantiate(TilePrefab, MapContainer).GetComponent<TileController>();
                TileScript.transform.localPosition = new Vector3(i - OffsetX, 0, e - OffsetZ);

                TileScript.GridPosition = new Vector2(i, e);
                if ((i == 0 || i == CurrentMap.Width - 1) || (e == 0 || e == CurrentMap.Height - 1))
                {

                    EdgeTiles.Add(TileScript);

                }
                yield return null;
                TileScript.AssignArrows(CurrentMap.Tiles[counter]);
                Tiles.Add(TileScript.GridPosition, TileScript);
                counter++;
            }
        }
        foreach (var Tile in Tiles)
        {
            Tile.Value.FindConnections();
        }
    }
    public void TraceConnections(TileController StartTile)
    {
        List<TileController> TilesToCheck = new List<TileController>() { StartTile};
        List<TileController> NextTiles = new List<TileController>();
        List<TileController> AlreadyCheckedTiles = new List<TileController>();
        EdgeType[] TargetEdges; 
        switch (StartTile.EdgeType)
        {
            case EdgeType.U:
                TargetEdges = new EdgeType[]{ EdgeType.D, EdgeType.CornerDL, EdgeType.CornerDR };
                break;
            case EdgeType.D:
                TargetEdges = new EdgeType[] { EdgeType.U, EdgeType.CornerUL, EdgeType.CornerUR };
                break;
            case EdgeType.R:
                TargetEdges = new EdgeType[] { EdgeType.L, EdgeType.CornerUL, EdgeType.CornerDL };
                break;
            case EdgeType.L:
                TargetEdges = new EdgeType[] { EdgeType.R, EdgeType.CornerUR, EdgeType.CornerDR};
                break;
            case EdgeType.CornerUL:
                TargetEdges = new EdgeType[] { EdgeType.D, EdgeType.R, EdgeType.CornerDR, EdgeType.CornerDL, EdgeType.CornerUR};
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
        Debug.Log($"Starting Trace for {StartTile.EdgeType}. Target Edges {TargetEdges[0]}, {TargetEdges[1]}, {TargetEdges[2]}");
        while (true)
        {
            foreach(var CurrentSpace in TilesToCheck)
            {
                AlreadyCheckedTiles.Add(CurrentSpace);
                Debug.Log($"Tracing Path for {StartTile.EdgeType}: {CurrentSpace.GridPosition}, type {CurrentSpace.EdgeType}");
                foreach (var Connection in CurrentSpace.Connections)
                {
                    if (Connection.State == CurrentSpace.State)
                    {
                        if (TargetEdges.Contains(Connection.EdgeType) && Connection != StartTile)
                        {
                            Debug.Log($"Tracing Complete for {StartTile.EdgeType}, result Success");
                            Result.text = $"{Connection.State} wins";
                            GameStarted = false;
                            PlayButton.SetActive(true);
                            return;
                        }
                        if(!NextTiles.Contains(Connection) && Connection != StartTile && !AlreadyCheckedTiles.Contains(Connection))
                            NextTiles.Add(Connection);
                    }
                }
            }
            if(NextTiles.Count == 0)
            {
                Debug.Log($"Tracing Complete for {StartTile.EdgeType}, result Failure");
                return;
            }
            TilesToCheck.Clear();
            foreach(var Tile in NextTiles)
            {
                TilesToCheck.Add(Tile);
            }
            NextTiles.Clear();
        }

    }
}
