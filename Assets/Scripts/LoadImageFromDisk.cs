using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class LoadImageFromDisk : MonoBehaviour
{
    string path = "";
    public Text pathText;
    public SpriteRenderer customImage;
  




    public void LoadImage()

    {
        path = pathText.text;
        string pathNew = path.Replace(@"\", @"/");
        Debug.Log(path);
        customImage.sprite = LoadImageAsSprite(pathNew);
    }

    public static Texture2D LoadImage(string path)
    {
        if (File.Exists(path))
        {
            byte[] bytes = File.ReadAllBytes(path);
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(bytes);
            Debug.Log("Test");
            return tex;
        }
        else
        {
            return null;
        }
    }
    public static Sprite LoadImageAsSprite(string pathNew)
    {
        Texture2D image = LoadImage(pathNew);
        if (image != null)
        {       
            Sprite sprite = Sprite.Create(LoadImage(pathNew), new Rect(0.0f, 0.0f, image.width,
            image.height), new Vector2(0.5f, 0.5f), 100.0f);
            return sprite;
        }
        return null;
    }

}
