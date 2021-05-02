using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class PaintAssetProcessor : UnityEditor.AssetPostprocessor
{
    static readonly Color FilterColor = Color.magenta;

    void OnPreprocessTexture()
    {
        if(!assetPath.Contains("Assets/Art/Source"))
        {
            return;
        }
        TextureImporter textureImporter = (TextureImporter)assetImporter;
        //textureImporter.alphaSource = TextureImporterAlphaSource.None;
        textureImporter.filterMode = FilterMode.Point;
        textureImporter.isReadable = true;
        textureImporter.textureCompression = TextureImporterCompression.Uncompressed;

        ////var ctx = (Texture2D)context.mainObject;
        ////ImageTool.RemoveColor(FilterColor, ctx);
        //Debug.Log(context);
        //Debug.Log(context.mainObject);
        //var objs = new List<Object>();
        //Debug.Log(objs[0]);
    }
}
