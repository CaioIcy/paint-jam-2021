using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[Serializable]
public class ItemInterest
{
    public string ItemID;
    public int Interest;
}

[Serializable]
public class Alien
{
    public string ID;
    public string Name;
    public Sprite Sprite;
    public string Behaviour;


    public List<string> Messages;
    public List<ItemInterest> Interests;

    //public string Message;
    //public string InterestItemID;
    //public int InterestCost;


}

public class AlienBv
{
    public const string BvDefault = "default";
    public const string BvAll = "accepts_all";
    public const string BvNone = "none";
}

public class AlienData : ScriptableObject
{
    public TextAsset Csv;
    public ItemData ItemData;
    public List<Alien> Aliens;

    public Alien GetAlienByName(string name)
    {
        return Aliens.SingleOrDefault(al => al.Name == name);
    }
    public Alien GetAlienByID(string id)
    {
        return Aliens.SingleOrDefault(al => al.ID == id);
    }
}



#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(AlienData))]
public class AlienDataEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var t = (AlienData)target;
        if (GUILayout.Button("Bake"))
        {
            t.Aliens = new List<Alien>();
            var found = GetAtPath<Sprite>("Art/Alphafyed/Aliens/");
            var ret = ProcessCsv(t.Csv);
            Debug.Log($"found {found?.Length}/{ret?.Count} aliens");

            if (found.Length != ret.Count)
            {
                throw new Exception("diff count aliens");
            }

            foreach (var alienSpr in found)
            {
                var it = t.Aliens.SingleOrDefault(it => it.ID == alienSpr.name);
                if (it == null)
                {
                    it = new Alien();
                    t.Aliens.Add(it);
                }
                SetAlienInfo(t, it, alienSpr, ret);
            }
            UnityEditor.EditorUtility.SetDirty(t);
        }
    }

    void SetAlienInfo(AlienData t, Alien alien, Sprite spr, Dictionary<string, (string, string, string)> dic)
    {
        alien.Sprite = spr;
        alien.ID = spr.name;
        alien.Name = spr.name;
        alien.Name = alien.Name.Replace("alien_", "");
        alien.Name = alien.Name.Replace("_alpha", "");
        alien.Name = alien.Name.Trim();

        if (!dic.ContainsKey(alien.Name))
        {
            Debug.LogError($"bad al [{alien.Name}]");
            return;
        }

        alien.Messages = new List<string> { dic[alien.Name].Item1 };
        //alien.Message = dic[alien.Name].Item1;
        alien.Behaviour = dic[alien.Name].Item2;

        if(alien.Behaviour != AlienBv.BvAll && alien.Behaviour != AlienBv.BvDefault && alien.Behaviour != AlienBv.BvNone)
        {
            throw new Exception($"bad behaviour in [{alien.Name}] = [{alien.Behaviour}]");
        }

        alien.Interests = new List<ItemInterest>();
        if (alien.Behaviour != AlienBv.BvDefault)
        {
            return;
        }

        var itemsStr = dic[alien.Name].Item3;
        var ss = itemsStr.Split(',');
        foreach(var itr in ss)
        {
            var s = itr.Trim().Split(':');
            if(s.Length != 2)
            {
                throw new Exception($"bad itr split in [{alien.Name}] => [{s}]");
            }
            var itemID = s[0].Replace("'", "_").Replace("&", "_").Trim();
            itemID = $"item_{itemID}_alpha";
            if (!t.ItemData.IsValidID(itemID))
            {
                //throw new Exception($"bad it in [{alien.Name}] => {itemID}");
                Debug.LogError($"bad it in [{alien.Name}] => {itemID}");
            }

            var cost = int.Parse(s[1].Trim());
            alien.Interests.Add(new ItemInterest
            {
                ItemID = itemID,
                Interest = cost,
            });
            //alien.InterestItemID = itemID;
            //alien.InterestCost = cost;
        }
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

        for (var i = 1; i < lenRows; ++i)
        {
            var nome = data[0, i].Replace("'", "_").Replace("&", "_").Trim();
            var diag = data[1, i].Trim();
            var bhv = data[2, i].Trim();
            var items = data[3, i].Trim();
            Debug.Log($"{nome} | {diag} | {bhv} | {items}");
            if (string.IsNullOrWhiteSpace(nome))
            {
                continue;
            }
            ret[nome] = (diag, bhv, items);
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

