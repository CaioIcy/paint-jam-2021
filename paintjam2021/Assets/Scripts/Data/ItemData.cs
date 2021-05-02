using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[Serializable]
public class Item
{
    public string ID;
    public string Name;
    public Sprite Sprite;

    public string Use;
    public string Discovery;
    public string Observations;
}

public class ItemData : ScriptableObject
{
    public TextAsset Csv;
    public List<Item> Items;

    public List<Item> GetInitialItems()
    {
        return Items.ToList();
    }

    public Item GetItem(string itemID)
    {
        return Items.SingleOrDefault(it => it.ID == itemID);
    }

    public bool IsValidID(string itemID)
    {
        return GetItem(itemID) != null;
    }
}


#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(ItemData))]
public class ItemDataEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var t = (ItemData) target;
        if(GUILayout.Button("Bake"))
        {
            var found = GetAtPath<Sprite>("Art/Alphafyed/Items/");
            var ret = ProcessCsv(t.Csv);
            Debug.Log($"found {found?.Length}/{ret?.Count} items");

            if(found.Length != ret.Count)
            {
                throw new Exception("diff count items");
            }

            foreach(var itemSpr in found)
            {
                var it = t.Items.SingleOrDefault(it => it.ID == itemSpr.name);
                if (it == null) {
                    it = new Item();
                    t.Items.Add(it);
                }
                SetItemInfo(it, itemSpr, ret);
            }
        }
    }

    void SetItemInfo(Item item, Sprite spr, Dictionary<string, (string, string, string)> dic)
    {
        item.Sprite = spr;
        item.ID = spr.name;
        item.Name = spr.name;
        item.Name = item.Name.Replace("item_", "");
        item.Name = item.Name.Replace("_alpha", "");

        if (!dic.ContainsKey(item.Name))
        {
            Debug.LogError($"bad it {item.Name}");
            return;
        }

        item.Use = dic[item.Name].Item1;
        item.Discovery = dic[item.Name].Item2;
        item.Observations = dic[item.Name].Item3;
    }

    public Dictionary<string, (string, string, string)> ProcessCsv(TextAsset Csv)
    {
        if (Csv == null)
        {
            throw new Exception("No CSV file detected");
        }

        var ret = new Dictionary<string, (string, string, string)>();

        var data = SplitCsvGrid(Csv.text);
        var lenCols = data.GetUpperBound(0);
        var lenRows = data.GetUpperBound(1);

        for(var i = 1; i < lenRows; ++i)
        {
            var nome = data[0, i].Replace("'", "_");
            var use = data[1, i];
            var disc = data[2, i];
            var obs = data[3, i];
            Debug.Log($"{nome} | {use} | {disc} | {obs}");
            if(string.IsNullOrWhiteSpace(nome)
                || string.IsNullOrWhiteSpace(use)
                || string.IsNullOrWhiteSpace(disc)
                || string.IsNullOrWhiteSpace(obs))
            {
                continue;
            }
            ret[nome] = (use, disc, obs);
        }

        return ret;
    }

    // From https://wiki.unity3d.com/index.php/CSVReader
    private static string[,] SplitCsvGrid(string csvText)
    {
        string[] SplitCsvLine(string line)
        {
            return (from System.Text.RegularExpressions.Match m in System.Text.RegularExpressions.Regex.Matches(line,
                    @"(((?<x>(?=[,\r\n]+))|""(?<x>([^""]|"""")+)""|(?<x>[^,\r\n]+)),?)",
                    System.Text.RegularExpressions.RegexOptions.ExplicitCapture)
                    select m.Groups[1].Value).ToArray();
        }

        string[] lines = csvText.Split("\n"[0]);

        // finds the max width of row
        int width = 0;
        for (int i = 0; i < lines.Length; i++)
        {
            string[] row = SplitCsvLine(lines[i]);
            width = Mathf.Max(width, row.Length);
        }

        // creates new 2D string grid to output to
        string[,] outputGrid = new string[width + 1, lines.Length + 1];
        for (int y = 0; y < lines.Length; y++)
        {
            string[] row = SplitCsvLine(lines[y]);
            for (int x = 0; x < row.Length; x++)
            {
                outputGrid[x, y] = row[x];

                // This line was to replace "" with " in my output. 
                // Include or edit it as you wish.
                outputGrid[x, y] = outputGrid[x, y].Replace("\"\"", "\"");
            }
        }

        return outputGrid;
    }

    public static T[] GetAtPath<T>(string path)
    {
        ArrayList al = new ArrayList();
        var npath = Application.dataPath + "/" + path;
        string[] fileEntries = Directory.GetFiles(npath);
        Debug.Log("find in path: " + npath);

        foreach (string fileName in fileEntries)
        {
            int assetPathIndex = fileName.IndexOf("Assets");
            string localPath = fileName.Substring(assetPathIndex);

            var t = UnityEditor.AssetDatabase.LoadAssetAtPath(localPath, typeof(T));

            if (t != null)
                al.Add(t);
        }
        T[] result = new T[al.Count];
        for (int i = 0; i < al.Count; i++)
            result[i] = (T)al[i];

        return result;
    }
}
#endif // UNITY EDITOR

