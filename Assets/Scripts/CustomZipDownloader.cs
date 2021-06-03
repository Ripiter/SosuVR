using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using ICSharpCode;
using UnityEngine.Android;
using System;
using System.Diagnostics;
using System.Threading;
using UnityEngine.SceneManagement;

public class CustomZipDownloader : MonoBehaviour
{
    // Debug/user messages
    public TextMeshProUGUI progressText;
    string ProgressMessage = "";

    // Used in download
    public string coockieUrl = "";
    public string downloadUrl = "";
    string coockie = "";

    // Used in unzipping
    string persistantDataPath = "";
    bool isDone = false;
    string vidSavePathZip = "";
    bool loadingNewScene = false;

    void Start()
    {
        if (Application.targetFrameRate != 90)
            Application.targetFrameRate = 90;

        persistantDataPath = Application.persistentDataPath;


        string vidSavePath = Path.Combine(Application.persistentDataPath, "Videos");
        string vidSavePathUnZip = Path.Combine(vidSavePath, "VideoEdited");
        vidSavePathZip = Path.Combine(vidSavePath, "SosuVideos.zip");

        // If the directory exists the video was downloaded and unzipped
        if (Directory.Exists(vidSavePathUnZip))
        {
            StartTheGame();
            return;
        }


        // If there is a zip file, but its not unzipped
        if (File.Exists(vidSavePathZip) && !Directory.Exists(vidSavePathUnZip))
        {
            Thread t = new Thread(Unzip);
            t.Start();
            return;
        }

        try
        {
            GetCookie();
        }
        catch (Exception e)
        {
            // if getting cookie failed try 1 more time
            if (coockie == "")
                GetCookie();
        }
        finally
        {
            if(coockie == "")
                ProgressMessage = "There was a problem connecting with sharepoint";
            else
                StartCoroutine(LargeDownload());
        }
    }

    private void Update()
    {
        if (progressText.text != ProgressMessage)
            progressText.text = ProgressMessage;


        if (isDone)
            StartTheGame();
    }

    /// <summary>
    /// Gets cookie from sharepoint, to be able access the zip file
    /// </summary>
    void GetCookie()
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(coockieUrl);
        request.CookieContainer = new CookieContainer();

        // check if nessary to set user agent
        request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/89.0.4389.114 Safari/537.36";
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();

        coockie = response.GetResponseHeader("Set-Cookie");
    }

    /// <summary>
    /// Downloads the zip file with all the videos
    /// </summary>
    /// <returns></returns>
    IEnumerator LargeDownload()
    {
        //Create Directory if it does not exist
        if (!Directory.Exists(Path.GetDirectoryName(vidSavePathZip)))
            Directory.CreateDirectory(Path.GetDirectoryName(vidSavePathZip));
        

        UnityWebRequest uwr = new UnityWebRequest(downloadUrl);
        uwr.method = UnityWebRequest.kHttpVerbGET;
        
        uwr.SetRequestHeader("cookie", coockie);

        DownloadHandlerFile dh = new DownloadHandlerFile(vidSavePathZip);
        dh.removeFileOnAbort = true;
        uwr.downloadHandler = dh;
        uwr.disposeDownloadHandlerOnDispose = true;

        ProgressMessage = "Started downloading videos";

        yield return uwr.SendWebRequest();

        if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
        {
            ProgressMessage = "An error has occured while downloading videos \nError: " + uwr.error;
            yield return new WaitForSeconds(30);
        }
        else
        {
            try
            {
                Thread t = new Thread(Unzip);
                t.Start();
            }
            catch (Exception e)
            {
                ProgressMessage = "Error while unziping " + e.Message;
            }
            finally
            {
                uwr.Dispose();
            }
        }
    }

    void Unzip()
    {
        try
        {
            ProgressMessage = "Unziping the files";


            isDone = ZipFile.UnZip(vidSavePathZip, Path.Combine(persistantDataPath, "Videos"), ref ProgressMessage);
        }
        catch (Exception e)
        {

        }
        finally
        {
            if (isDone == false)
            {
                ProgressMessage = "Error while unzipping";
                ZipFile.UnZip(vidSavePathZip, Path.Combine(persistantDataPath, "Videos"), ref ProgressMessage);
            }
        }
    }

    void StartTheGame()
    {
        // Make sure it doesnt load the scene twice
        if (!loadingNewScene)
        {
            DeleteZipFile();
            SceneManager.LoadScene(1);
        }
        loadingNewScene = true;
    }
    void DeleteZipFile()
    {
        try
        {
            if (File.Exists(vidSavePathZip))
                File.Delete(vidSavePathZip);
        }
        catch (Exception e)
        {
            //errorUnzipMessage = "Couldn't delete the zip file";
        }
    }
}
