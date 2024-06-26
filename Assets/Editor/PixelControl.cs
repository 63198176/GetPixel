using PlasticPipe.Client;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;


public class PixelWindow : EditorWindow
{
    string path = "";
    int PexelWidth = 20;
    static EditorWindow Window = null;

    [MenuItem("PixelControl/OpenPixelWindow")]
    public static void OpenPixelWindow()
    {
        Window = GetWindow<PixelWindow>();
        Window.Show();
    }


    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("选择图片"))
        {
            path = EditorUtility.OpenFilePanel("选择图片", "", "jpg");
        }
        EditorGUILayout.LabelField(path);
        PexelWidth = EditorGUILayout.IntField(PexelWidth);
        if (GUILayout.Button("处理图片"))
        {
            var texture = LoadImage(path);

            RecalPixelImage(PexelWidth,texture);
        }
        EditorGUILayout.EndHorizontal();
    }

    static List<Color> TempCalColorMap = new List<Color>();
    public static void RecalPixelImage(int PexelWidth, Texture2D _tex)
    {
        Color[,] colorMap = new Color[_tex.width, _tex.height];
        for (int y = 0; y < _tex.height; y++)
        {
            for (int x = 0; x < _tex.width; x++)
            {
                colorMap[x, y] = _tex.GetPixel(x, y);
            }
        }

        for (int y = 0; y < _tex.height / PexelWidth; y++)
        {
            for (int x = 0; x < _tex.width / PexelWidth; x++)
            {
                TempCalColorMap.Clear();
                for (int i = 0; i < PexelWidth; i++)
                {
                    for (int j = 0; j < PexelWidth; j++)
                    {
                        TempCalColorMap.Add(colorMap[x * PexelWidth + j, y * PexelWidth + i]);
                    }
                }
                var color = CalResultColor(TempCalColorMap);

                for (int i = 0; i < PexelWidth; i++)
                {
                    for (int j = 0; j < PexelWidth; j++)
                    {
                        colorMap[x * PexelWidth + j, y * PexelWidth + i] = color;
                    }
                }
            }
        }

        CreateImage(_tex.width, _tex.height, colorMap);
    }

    public static void CreateImage(int width,int height, Color[,] _colorMa)
    { 
        Texture2D tex = new Texture2D(width, height);
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                tex.SetPixel(x, y, _colorMa[x, y]);
            }
        }
        tex.Apply();
        tex.EncodeToPNG();

        File.WriteAllBytes(Application.dataPath + "/test.png", tex.EncodeToPNG());
        Window.Close();
        AssetDatabase.Refresh();
    }

    public static Color CalResultColor(List<Color> _colors)
    {
        float r = 0;
        float g = 0;
        float b = 0;

        for (int i = 0; i < _colors.Count; i++)
        {
            r += _colors[i].r;
            g += _colors[i].g;
            b += _colors[i].b;
        }

        r /= _colors.Count;
        g /= _colors.Count;
        b /= _colors.Count;
        return new Color(r, g, b);
    }

    public static Texture2D LoadImage(string filePath)
    {
        byte[] fileData = File.ReadAllBytes(filePath);
        Texture2D texture = new Texture2D(10,10);
        texture.LoadImage(fileData);
        texture.Apply();

        return texture;
    }
}
