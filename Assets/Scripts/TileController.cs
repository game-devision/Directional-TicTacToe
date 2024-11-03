using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpaceState
{
    None,
    X,
    O
}
public enum EdgeType
{
    None,
    U,
    D,
    L,
    R,
    CornerUL,
    CornerUR,
    CornerDL,
    CornerDR
}
[System.Serializable]
public class TileData
{
    public bool[] Arrows = new bool[8] {false, false, false, false, false, false, false, false };
}
public class TileController : MonoBehaviour
{
    public TileData Data;
    [SerializeField] GameObject[] ArrowObjects;
    [SerializeField] SpriteRenderer[] ArrowSprites;
    public SpaceState State = SpaceState.None;
    public EdgeType EdgeType;
    [SerializeField] GameObject ShapeX;
    [SerializeField] GameObject ShapeO;
    public Vector2 GridPosition;
    public List<TileController> Connections;
    public void CheckEdgeType()
    {
        if (GridPosition.x == 0)
        {
            EdgeType = EdgeType.L;
            if (GridPosition.y == 0)
            {
                EdgeType = EdgeType.CornerDL;
            }
            if (GridPosition.y == GameController.Instance.CurrentMap.Height - 1)
            {
                EdgeType = EdgeType.CornerUL;
            }
        }
        if (GridPosition.x == GameController.Instance.CurrentMap.Width - 1)
        {
            EdgeType = EdgeType.R; 
            if (GridPosition.y == 0)
            {
                EdgeType = EdgeType.CornerDR;
            }
            if (GridPosition.y == GameController.Instance.CurrentMap.Height - 1)
            {
                EdgeType = EdgeType.CornerUR;
            }
        }
        if (GridPosition.y == 0)
        {
            EdgeType = EdgeType.D; 
            if (GridPosition.x == GameController.Instance.CurrentMap.Width - 1)
            {
                EdgeType = EdgeType.CornerDR;
            }
            if (GridPosition.x == 0)
            {
                EdgeType = EdgeType.CornerDL;
            }
        }
        if (GridPosition.y == GameController.Instance.CurrentMap.Height - 1)
        {
            EdgeType = EdgeType.U;
            if (GridPosition.x == GameController.Instance.CurrentMap.Width - 1)
            {
                EdgeType = EdgeType.CornerUR;
            }
            if (GridPosition.x == 0)
            {
                EdgeType = EdgeType.CornerUL;
            }
        }


    }
    public void FindConnections()
    {
        for(int i = 0; i < Data.Arrows.Length; i++)
        {
            if (Data.Arrows[i])
            {
                switch (i)
                {
                    case 0:
                        Connections.Add(GameController.Instance.Tiles[GridPosition + new Vector2(0, 1)]);
                        break;
                    case 1:
                        Connections.Add(GameController.Instance.Tiles[GridPosition + new Vector2(1, 1)]);
                        break;
                    case 2:
                        Connections.Add(GameController.Instance.Tiles[GridPosition + new Vector2(1, 0)]);
                        break;
                    case 3:
                        Connections.Add(GameController.Instance.Tiles[GridPosition + new Vector2(1, -1)]);
                        break;
                    case 4:
                        Connections.Add(GameController.Instance.Tiles[GridPosition + new Vector2(0, -1)]);
                        break;
                    case 5:
                        Connections.Add(GameController.Instance.Tiles[GridPosition + new Vector2(-1, -1)]);
                        break;
                    case 6:
                        Connections.Add(GameController.Instance.Tiles[GridPosition + new Vector2(-1, 0)]);
                        break;
                    case 7:
                        Connections.Add(GameController.Instance.Tiles[GridPosition + new Vector2(-1, 1)]);
                        break;

                    default:
                        break;
                }
            }
        }
    }
    public void AssignArrows(TileData Data)
    {
        this.Data = Data;
        CheckEdgeType();
        switch (EdgeType)
        {
            case EdgeType.None:
                break;
            case EdgeType.U:
                Data.Arrows[0] = false;
                Data.Arrows[1] = false;
                Data.Arrows[7] = false;
                break;
            case EdgeType.D:
                Data.Arrows[3] = false;
                Data.Arrows[4] = false;
                Data.Arrows[5] = false;
                break;
            case EdgeType.L:
                Data.Arrows[5] = false;
                Data.Arrows[6] = false;
                Data.Arrows[7] = false;
                break;
            case EdgeType.R:
                Data.Arrows[1] = false;
                Data.Arrows[2] = false;
                Data.Arrows[3] = false;
                break;
            case EdgeType.CornerDL:
                Data.Arrows[3] = false;
                Data.Arrows[4] = false;
                Data.Arrows[5] = false;
                Data.Arrows[6] = false;
                Data.Arrows[7] = false;
                break;
            case EdgeType.CornerDR:
                Data.Arrows[1] = false;
                Data.Arrows[2] = false;
                Data.Arrows[3] = false;
                Data.Arrows[4] = false;
                Data.Arrows[5] = false;
                break;
            case EdgeType.CornerUL:
                Data.Arrows[5] = false;
                Data.Arrows[6] = false;
                Data.Arrows[7] = false;
                Data.Arrows[0] = false;
                Data.Arrows[1] = false;
                break;
            case EdgeType.CornerUR:
                Data.Arrows[7] = false;
                Data.Arrows[0] = false;
                Data.Arrows[1] = false;
                Data.Arrows[2] = false;
                Data.Arrows[3] = false;
                break;
        }
        for (int i = 0; i < 8; i++)
        {
            ArrowObjects[i].SetActive(Data.Arrows[i]);
        }
        foreach (var Arrow in ArrowSprites)
        {
            if (Arrow.gameObject.activeSelf)
            {
                Arrow.color = Color.grey;
            }
        }
    }
    private void OnMouseEnter()
    {
        if (State == SpaceState.None)
        {
            foreach (var Arrow in ArrowSprites)
            {
                if (Arrow.gameObject.activeSelf)
                {
                    Arrow.color = Color.white;
                }
            }
        }
    }
    private void OnMouseExit()
    {
        if(State == SpaceState.None)
        {
            foreach (var Arrow in ArrowSprites)
            {
                if (Arrow.gameObject.activeSelf)
                {
                    Arrow.color = Color.grey;
                }
            }
        }
    }
    private void OnMouseDown()
    {
        if(GameController.Instance.EditMode)
        {
            TileEditor.Instance.SetCurrentTile(this);
        }
        else
        {
            if (State == SpaceState.None && GameController.Instance.GameStarted)
            {
                if(BotAI2.botEnabled == false || BotAI2.botsTurn == false)
                {
                    if (GameController.Instance.TurnCount % 2 == 0)
                    {
                        SelectThisTile(SpaceState.O);
                    }
                    else
                    {
                        SelectThisTile(SpaceState.X);
                    }
                }
                
                
            }

        }

    }
    public void SelectThisTile(SpaceState type)
    {
        State = type;
        if(type == SpaceState.O)
        {
            ShapeO.SetActive(true);
            foreach (var Arrow in ArrowSprites)
            {
                if (Arrow.gameObject.activeSelf)
                {
                    Arrow.color = Color.green;
                }
            }
        }
        else if(type == SpaceState.X)
        {
            ShapeX.SetActive(true);
            foreach (var Arrow in ArrowSprites)
            {
                if (Arrow.gameObject.activeSelf)
                {
                    Arrow.color = Color.red;
                }
            }
        }
        GameController.Instance.NextMove();
    }
}
