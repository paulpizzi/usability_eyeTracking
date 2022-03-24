using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using UnityEngine.UI;
using System.Collections;

/* USAGE: 
    - drag & drop image into scene (parent object: "Images")
    - disable gameobject in scene
    - name gameobject in scene accordingly since this name is used in the dropdown menu
    - add gameobject in scene to "images" list
*/
public class UIHandler : MonoBehaviour
{
    public List<GameObject> images = new List<GameObject>();

    public TMP_Dropdown imageDropdown;
    public GameObject saveImgBtn;
    public Camera screenshotCam;

    public Canvas UICanvas;

    public EyeTracker tracker;

    public GameObject loadCustomObject;
    public TMP_Text savedTextPath;

    private int curImgIndex = 0;

    void Start()
    {
        var optionsList = new List<TMP_Dropdown.OptionData>();
        foreach (var img in images)
        {
            optionsList.Add(new TMP_Dropdown.OptionData(img.name));
        }

        imageDropdown.AddOptions(optionsList);

        images[curImgIndex].SetActive(true);
    }




    public void OnDropdownValueChanged(int option)
    {
        images[curImgIndex].SetActive(false);
        images[option].SetActive(true);
        curImgIndex = option;
      
        if(option == (imageDropdown.options.Count - 1))
        {
            loadCustomObject.SetActive(true);
        }else
        {
            loadCustomObject.SetActive(false);
        }
    }


    private IEnumerator SavePNG()
    {
        // We should only read the screen after all rendering is complete
        yield return new WaitForEndOfFrame();
      

    }
        public void OnSaveImgBtn()
    {
          SavePNG();
          Debug.Log("Saving image ...");

          RenderTexture currentRT = RenderTexture.active;
          RenderTexture.active = screenshotCam.targetTexture;

          screenshotCam.Render();

          Texture2D Image = new Texture2D(screenshotCam.targetTexture.width, screenshotCam.targetTexture.height);
          Image.ReadPixels(new Rect(0, 0, screenshotCam.targetTexture.width, screenshotCam.targetTexture.height), 0, 0);
          Image.Apply();
          RenderTexture.active = currentRT;

          var bytes = Image.EncodeToJPG();
          var path = Application.dataPath+"\\Results\\";
          var fileName =  images[curImgIndex].name + DateTime.Now.ToString("_yyyy-MM-dd_hh-mm-ss") + ".png";


              if (!Directory.Exists(path))
              {
              Debug.Log("No Ppath");
                  Directory.CreateDirectory(path);
              }
              path += fileName;          
              File.WriteAllBytes(path, bytes);
        
         savedTextPath.text = "Image saved to: " /*+ Application.dataPath.Replace(@"\", @"/")*/  + path.Replace(@"\", @"/");
         Debug.Log("Image saved to " + name);
    }

    public void OnStartTrackingBtn()
    {
        Debug.Log("Start Tracking");

        // disable dropdown because changing image should not be possible while tracking
        imageDropdown.interactable = false;
        UICanvas.enabled = false;
        tracker.InitTracking();
    }

    public void OnStopTrackingBtn()
    {
        if (tracker.isTracking)
        {
            savedTextPath.text = "";
            Debug.Log("Stop Tracking");
            tracker.FinalizeTracking();
            UICanvas.enabled = true;
            // re-enable dropdown after tracking is finished
            imageDropdown.interactable = true;

            saveImgBtn.SetActive(true);
        }
        else
        {
            Debug.Log("Tracking not started");
        }
    }
}
