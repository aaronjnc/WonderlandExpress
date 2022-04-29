using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MapTownName : MonoBehaviour
{
    [Tooltip("Index of the town this one represents")]
    public int townIndex;
    [Tooltip("Town name")]
    public string townName;
    [Tooltip("Text box")]
    public TextMeshProUGUI textBox;
    // Start is called before the first frame update
    void Start()
    {
        textBox = gameObject.GetComponent<TextMeshProUGUI>();
        townName = TownDictionary.Instance.GetTownNameByIndex(townIndex);
        textBox.text = townName;
    }
}
