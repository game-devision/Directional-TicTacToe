using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class MapLoader : MonoBehaviour
{
    public List<MapData> AllMaps = new List<MapData>();
    [SerializeField] TMP_Text MapName;
    [SerializeField] TMP_Text MapDescription;
    [SerializeField] TMP_InputField MapNameInput;
    [SerializeField] TMP_InputField MapDescriptionInput;
    [SerializeField] TMP_InputField Scale;
    public static MapLoader Instance;
    [SerializeField] List<MapButton> MapButtons = new List<MapButton>();
    [SerializeField] Transform Container;
    [SerializeField] GameObject ButtonPrefab;
    int SelectedIndex;
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
        if(MapButtons.Count > 0)
        {
            foreach(var  Button in MapButtons)
            {
                Destroy(Button.gameObject);
            }
            MapButtons.Clear();
        }
        foreach (var Map in AllMaps)
        {
            MapButtons.Add(Instantiate(ButtonPrefab, Container).GetComponent<MapButton>());
            MapButtons[MapButtons.Count - 1].Index = MapButtons.Count - 1;
            MapButtons[MapButtons.Count - 1].SetName(AllMaps[MapButtons.Count - 1].Title);
        }
    }

    public void SelectMap(int Index)
    {
        SelectedIndex = Index;
        GameController.Instance.LoadMap(AllMaps[Index]);
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
            MapName.text = AllMaps[SelectedIndex].Title;
            MapDescription.text = AllMaps[SelectedIndex].Description;

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
