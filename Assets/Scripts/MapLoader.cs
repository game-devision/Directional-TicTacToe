using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class MapLoader : MonoBehaviour
{
    public List<MapData> AllMaps = new List<MapData>();
    [SerializeField]TMP_Dropdown Dropdown;
    [SerializeField] TMP_Text MapName;
    [SerializeField] TMP_Text MapDescription;

    private void Start()
    {
        RefreshMaps();
    }
    private void OnApplicationFocus(bool focus)
    {
        RefreshMaps();
    }
    public void RefreshMaps()
    {
        var Files = Directory.GetFiles($"{Application.streamingAssetsPath}/Maps", "*.json");
        AllMaps.Clear();
        foreach(var Path in Files) 
        {
            AllMaps.Add(JsonUtility.FromJson<MapData>(File.ReadAllText(Path)));
        }

        Dropdown.ClearOptions();
        foreach (var Map in AllMaps)
        {
            Dropdown.options.Add(new TMP_Dropdown.OptionData(Map.Title));
        }
    }

    public void SelectMap()
    {
        GameController.Instance.LoadMap(AllMaps[Dropdown.value]);
        MapName.text = AllMaps[Dropdown.value].Title;
        MapDescription.text = AllMaps[Dropdown.value].Description;
    }
}
