using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TileEditor : MonoBehaviour
{
    [SerializeField] Toggle[] Toggles;
    TileController CurrentTile;
    [SerializeField] TMP_Text TilePos;
    public static TileEditor Instance;
    bool TileLoading;
    public void Start()
    {
        Instance = this;
    }
    public void SetCurrentTile(TileController Tile)
    {
        TileLoading = false;
        CurrentTile = Tile;
        TilePos.text = Tile.GridPosition.ToString();
        for(int i = 0; i < Toggles.Length; i++)
        {
            Toggles[i].isOn = CurrentTile.Data.Arrows[i];
        }
        TileLoading = true;
    }
    public void UpdateTile()
    {
        if (!TileLoading)
            return;

        for (int i = 0; i < Toggles.Length; i++)
        {
            CurrentTile.Data.Arrows[i] = Toggles[i].isOn;
        }
        if (GameController.Instance.EditMode)
        {
            CurrentTile.AssignArrows(CurrentTile.Data);
        }
    }
}
