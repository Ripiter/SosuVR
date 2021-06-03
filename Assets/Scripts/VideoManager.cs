using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using System;
using System.IO;

public class VideoManager : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    public ClipHolder[] clips;
    // Buttons that user can click on
    public Button[] btns;

    // Text on buttons
    public GameObject infomationTextBtn;

    public TextMeshProUGUI firstText;
    public TextMeshProUGUI secondText;
    public TextMeshProUGUI thirdText;

    // Notification that pops out when the wrong or correct answear was clicked
    public TextMeshProUGUI notificationText;
    public GameObject canvas;
    public GameObject environment;

    private ClipHolder clipVideo;

    // Object above the items that u can pick up
    public GameObject[] PickUpPointers;

    private void Start()
    {
        if (Application.targetFrameRate != 90)
            Application.targetFrameRate = 90;

        ShowNotficationText(false);
        ShowCanvas(false);

        // Only for debug purpose
        // PickedUp("Dummy2");
    }

    string GetPath(string videoName)
    {
        string path = "";
        try
        {
            string rootPath = Application.persistentDataPath;
            path = Path.Combine(rootPath + "\\Videos\\VideoEdited", videoName);

            if(File.Exists(path) == false)
                path = path.Replace('æ', '�');
        }
        catch (Exception e)
        {

        }
        return path;
    }


    void WaitForVideosBreak()
    {
        ShowCanvas(true);
        ShowButtons(true);
        ShowMesh(false);
        SetText();
    }

    public void StartPlayingFirstVideo()
    {
        ShowCanvas(false);
        ShowMesh(false);
        infomationTextBtn.SetActive(false);
        ShowNotficationText(false);

        StartCoroutine(PlayFirstVideo());
    }

    IEnumerator PlayFirstVideo()
    {
        if (clipVideo.startClip != "")
        {
            videoPlayer.url = GetPath(clipVideo.startClip);
            ShowSphere(true);
            videoPlayer.Play();
            yield return new WaitForSeconds(clipVideo.startClipTime);
            videoPlayer.Stop();
            ShowSphere(false);
        }

        WaitForVideosBreak();
    }

    public void BeforeVideoStarts()
    {
        ShowCanvas(true);
        ShowButtons(false);
        ShowNotficationText(false);
        infomationTextBtn.SetActive(true);
    }

    void ShowCanvas(bool _value)
    {
        canvas.SetActive(_value);

        if(canvas.GetComponentInParent<CanvasMove>() != null)
            canvas.GetComponentInParent<CanvasMove>().MoveCamera();
    }

    void ShowButtons(bool _value)
    {
        for (int i = 0; i < btns.Length; i++)
        {
            btns[i].gameObject.SetActive(_value);
        }
    }

    void ShowMesh(bool _value)
    {
        MeshRenderer[] renders = environment.GetComponentsInChildren<MeshRenderer>();
        SkinnedMeshRenderer[] skinnedMeshes = environment.GetComponentsInChildren<SkinnedMeshRenderer>();
        for (int i = 0; i < renders.Length; i++)
        {
            renders[i].enabled = _value;
        }
        for (int i = 0; i < skinnedMeshes.Length; i++)
        {
            skinnedMeshes[i].enabled = _value;
        }

        ShowPointers(_value);
    }

    void ShowPointers(bool _value)
    {
        for (int i = 0; i < PickUpPointers.Length; i++)
        {
            PickUpPointers[i].SetActive(_value);
        }
    }

    void ShowSphere(bool _value)
    {
        videoPlayer.gameObject.SetActive(_value);
    }

    // Set's button text according the the clip text, set in playing clip 
    public void SetText()
    {
        if (clipVideo != null)
        {
            infomationTextBtn.GetComponentInChildren<TextMeshProUGUI>().text = clipVideo.infomationText;
            firstText.text = clipVideo.firstText;
            secondText.text = clipVideo.secondText;
            thirdText.text = clipVideo.thirdText;
        }
    }

    public void FirstButtonPressed()
    {
        StartCoroutine(SetAfterButtonPressed(clipVideo.videoClips[0]));
    }

    public void SecondButtonPressed()
    {
        StartCoroutine(SetAfterButtonPressed(clipVideo.videoClips[1]));
    }

    public void ThirdButtonPressed()
    {
        StartCoroutine(SetAfterButtonPressed(clipVideo.videoClips[2]));
    }

    /// <summary>
    /// After button was pressed we take clip corresponding the the button and
    /// set default values accordingly
    /// </summary>
    /// <param name="_clip"></param>
    IEnumerator SetAfterButtonPressed(Clip _clip)
    {
        videoPlayer.Stop();
        videoPlayer.url = GetPath(_clip.clip);

        // not sure if u should play video here ???
        ShowSphere(true);
        videoPlayer.Play();
        ShowCanvas(false);
        ShowMesh(false);
        //yield return new WaitForSeconds((float)time);
        yield return new WaitForSeconds(_clip.clipTime);
        videoPlayer.Stop();
        ShowSphere(false);
        ShowAnswearAfterTime(_clip);
    }

    void ShowAnswearAfterTime(Clip clip)
    {
        ShowCanvas(true);
        SetNotificationText(clip.DisplayText);

        if (clip.isCorrectAnswear)
        {
            // Change color of the text for green
            StartCoroutine(HideAnswearAndCanvas());
        }
        else
        {
            StartCoroutine(ShowCanvasAfterTime(2));
        }
        ShowNotficationText(true);
    }

    IEnumerator ShowCanvasAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        ShowCanvas(true);
        ShowButtons(true);
        ShowNotficationText(false);
    }

    void SetNotificationText(string _text)
    {
        ShowCanvas(true);
        ShowButtons(false);

        notificationText.text = _text;
    }

    void ShowNotficationText(bool _value)
    {
        notificationText.gameObject.SetActive(_value);
    }

    IEnumerator HideAnswearAndCanvas()
    {
        yield return new WaitForSeconds(3f);

        if (clipVideo.nextClipName != "")
        {
            SetNextClipHolder(clipVideo.nextClipName);

            SetText();
            BeforeVideoStarts();
        }
        else
        {
            ShowCanvas(false);
            ShowSphere(false);
            ShowNotficationText(false);
            ShowMesh(true);
        }
    }

    public void PickedUp(string _pickedItemName)
    {
        SetNextClipHolder(_pickedItemName);
        SetText();
        BeforeVideoStarts();
        ShowMesh(false);
        ShowNotficationText(true);
    }

    public void SetNextClipHolder(string _name)
    {
        for (int i = 0; i < clips.Length; i++)
        {
            if (clips[i].ClipName == _name)
            {
                clipVideo = clips[i];
            }
        }
    }
}
[System.Serializable]
public class ClipHolder
{
    public string ClipName;
    public string startClip;
    public string infomationText;
    public string firstText;
    public string secondText;
    public string thirdText;

    // Play video for this amount of seconds
    public float startClipTime;

    public string nextClipName;
    public Clip[] videoClips;
}
[System.Serializable]
public class Clip
{
    public string clip;
    public string DisplayText;
    public bool isCorrectAnswear;
    public float clipTime;
}