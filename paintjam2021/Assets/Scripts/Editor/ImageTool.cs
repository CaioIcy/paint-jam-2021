using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Collections.Generic;

public class ImageTool : EditorWindow
{
    Texture2D img;
    Texture2D newImg;
    Color colorToRemove = Color.magenta;
    public static ImageTool win;

    [MenuItem("Window/Tools/Alpha-fy Images")]
    static void Init()
    {
        win = ScriptableObject.CreateInstance(typeof(ImageTool)) as ImageTool;
        win.ShowUtility();
    }

    void OnGUI()
    {
        GUILayout.BeginHorizontal();

        /** Toolbar **/
        GUILayout.BeginVertical();
        img = (Texture2D)EditorGUILayout.ObjectField(img, typeof(Texture2D), false, GUILayout.MinWidth(128), GUILayout.MinHeight(128), GUILayout.MaxWidth(128), GUILayout.MaxHeight(128));

        colorToRemove = EditorGUILayout.ColorField(colorToRemove, GUILayout.MaxWidth(128));

        if (GUILayout.Button("Preview", GUILayout.MinWidth(128), GUILayout.MinHeight(32), GUILayout.MaxWidth(128), GUILayout.MaxHeight(128)))
            newImg = RemoveColor(colorToRemove, img);

        if (GUILayout.Button("Alpha-fy All", GUILayout.MinWidth(128), GUILayout.MinHeight(32), GUILayout.MaxWidth(128), GUILayout.MaxHeight(128)))
            RemoveColor(colorToRemove, Selection.GetFiltered<Texture2D>(SelectionMode.Assets));

        GUILayout.EndVertical();

        GUILayout.BeginVertical();
        GUILayout.Label("Selected Files", EditorStyles.boldLabel);
        foreach (Texture2D selected in Selection.GetFiltered<Texture2D>(SelectionMode.Assets))
        {
            GUILayout.Label(selected.name);
        }
        GUILayout.EndVertical();

        /** Image Display **/
        GUILayout.BeginVertical();
        GUILayout.Label("Preview", EditorStyles.boldLabel);
        if (newImg)
        {
            GUILayout.Label(newImg);
        }
        GUILayout.EndVertical();

        GUILayout.EndHorizontal();

    }

    public static void RemoveColor(Color c, UnityEngine.Object[] imgs)
    {
        if (!Directory.Exists("Assets/Art/Alphafyed/"))
        {
            Directory.CreateDirectory("Assets/Art/Alphafyed/");
        }
        float inc = 0f;
        foreach (Texture2D i in imgs)
        {
            inc++;
            if (EditorUtility.DisplayCancelableProgressBar(
                "Playin' With Pixels",
                "Seaching for Color Matches",
                ((float)inc / (float)imgs.Length)))
            {
                break;
            }

            //ssEditorTools.MaxImportSettings(i);
            Color[] pixels = i.GetPixels(0, 0, i.width, i.height, 0);
            for (int p = 0; p < pixels.Length; p++)
            {
                if (pixels[p] == c)
                    pixels[p] = new Color(0, 0, 0, 0);
            }

            Texture2D n = new Texture2D(i.width, i.height);
            n.SetPixels(0, 0, i.width, i.height, pixels, 0);
            n.Apply();

            byte[] bytes = n.EncodeToPNG();
            File.WriteAllBytes("Assets/Art/Alphafyed/" + i.name + "_alpha.png", bytes);
        }

        EditorUtility.ClearProgressBar();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    public static Texture2D RemoveColor(Color c, Texture2D i)
    {
        //ssEditorTools.MaxImportSettings(i);

        Color[] pixels = i.GetPixels(0, 0, i.width, i.height, 0);

        for (int p = 0; p < pixels.Length; p++)
        {
            if (EditorUtility.DisplayCancelableProgressBar(
                "Playin' With Pixels",
                "Seaching for Color Matches",
                ((float)p / pixels.Length)))
            {
                break;
            }

            if (pixels[p] == c)
                pixels[p] = new Color(0, 0, 0, 0);
        }

        Texture2D n = new Texture2D(i.width, i.height);
        n.SetPixels(0, 0, i.width, i.height, pixels, 0);
        n.Apply();
        EditorUtility.ClearProgressBar();
        return (n);
    }
}