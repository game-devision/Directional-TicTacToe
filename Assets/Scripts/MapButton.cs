using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MapButton : MonoBehaviour
{

    public int Index;
    [SerializeField] TMP_Text Title;
    public void Select()
    {
        MapLoader.Instance.SelectMap(Index);
    }
    public void SetName(string Name)
    {
        Title.text = Name;
    }
}
