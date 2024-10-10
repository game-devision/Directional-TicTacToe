using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MapCreator : MonoBehaviour
{
    [SerializeField] TMP_InputField Name;
    [SerializeField] TMP_InputField Width;
    [SerializeField] TMP_InputField Height;
    public void NewMap()
    {
        var NewMap = new MapData(int.Parse(Width.text), int.Parse(Height.text));
        NewMap.Title = Name.text;
        GameController.Instance.LoadMap(NewMap);
        GameController.Instance.ExportMap();
        MapLoader.Instance.UpdateText();
        MapLoader.Instance.RefreshMaps();
    }
}
