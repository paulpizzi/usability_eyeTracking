using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

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

    public EyeTracker tracker;

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
    }

    public void OnSaveImgBtn()
    {
        Debug.Log("Saving image ...");

        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = screenshotCam.targetTexture;

        screenshotCam.Render();

        Texture2D Image = new Texture2D(screenshotCam.targetTexture.width, screenshotCam.targetTexture.height);
        Image.ReadPixels(new Rect(0, 0, screenshotCam.targetTexture.width, screenshotCam.targetTexture.height), 0, 0);
        Image.Apply();
        RenderTexture.active = currentRT;

        var bytes = Image.EncodeToJPG();
        var name = "Results\\" + images[curImgIndex].name + DateTime.Now.ToString("_yyyy-MM-dd_hh-mm-ss") + ".png";
        File.WriteAllBytes(name, bytes);

        Debug.Log("Image saved to " + name);
    }

    public void OnStartTrackingBtn()
    {
        Debug.Log("Start Tracking");

        // disable dropdown because changing image should not be possible while tracking
        imageDropdown.interactable = false;

        tracker.InitTracking();
    }

    public void OnStopTrackingBtn()
    {
        if (tracker.isTracking)
        {
            Debug.Log("Stop Tracking");
            tracker.FinalizeTracking();

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
