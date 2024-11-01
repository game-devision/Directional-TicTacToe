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
    [SerializeField] TMP_InputField MapNameInput;
    [SerializeField] TMP_InputField MapDescriptionInput;
    [SerializeField] TMP_InputField Scale;
    public static MapLoader Instance;

    private void Start()
    {
        RefreshMaps();
        Instance = this;
    }
    private void OnApplicationFocus(bool focus)
    {
        RefreshMaps();
    }
    public void RefreshMaps()
    {
        var Files = Resources.LoadAll<TextAsset>("Maps");
        AllMaps.Clear();
        foreach(var Path in Files) 
        {
            AllMaps.Add(JsonUtility.FromJson<MapData>(Path.text));
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
        UpdateText();
    }
    public void UpdateText()
    {
        if (GameController.Instance.EditMode)
        {
            MapNameInput.text = GameController.Instance.CurrentMap.Title;
            MapDescriptionInput.text = GameController.Instance.CurrentMap.Description;
            Scale.text = GameController.Instance.CurrentMap.BoardScale.ToString();
        }
        else
        {
            MapName.text = AllMaps[Dropdown.value].Title;
            MapDescription.text = AllMaps[Dropdown.value].Description;

        }
    }
    public void UpdateMapName()
    {

        GameController.Instance.CurrentMap.Title = MapNameInput.text;
        GameController.Instance.CurrentMap.Description = MapDescriptionInput.text;
    }
    public void UpdateMapScale()
    {
        GameController.Instance.CurrentMap.BoardScale = float.Parse(Scale.text);
        GameController.Instance.LoadMap(GameController.Instance.CurrentMap);
    }
}
