using UnityEngine;
using System.Collections;
using com.shephertz.app42.paas.sdk.csharp;
using com.shephertz.app42.paas.sdk.csharp.upload;
using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine.UI;

public class OnlineStorage : MonoBehaviour
{
    private const string KEY_API = "28a2a9d00f5f028c8490a4678f4cf2ca2b67b4318cd91884d8ec7089f7fe7fb9";
    private const string KEY_SECRET = "11b1c14582e5df3a0cab8fb290b8b8f1c79855bed83121cd7e1e7a6a8566d7d0";

    private ServiceAPI serviceAPI;
    private UploadService uploadService;

    public string currentString = "";
    public bool downloaded = false;

    // Use this for initialization
    public void Initialize ()
    {
        //App42API.Initialize(KEY_API, KEY_SECRET);

        serviceAPI = new ServiceAPI(KEY_API, KEY_SECRET);
        uploadService = serviceAPI.BuildUploadService();
    }
	
    public void GetFile(string fileName)
    {
        uploadService.GetFileByName(fileName, new UnityCallBack());
    }

    public void DownloadBackup(string url)
    {
        using (WebClient wc = new WebClient())
        {
            currentString = wc.DownloadString(url);
            downloaded = true;
        }
    }

    public class UnityCallBack : App42CallBack
    {
        public void OnSuccess(object response)
        {
            Upload upload = (Upload)response;
            IList<Upload.File> fileList = upload.GetFileList();

            GameObject gameObject = GameObject.FindGameObjectWithTag("OnlineStorage");
            OnlineStorage script = (OnlineStorage)gameObject.GetComponent(typeof(OnlineStorage));

            using (WebClient client = new WebClient())
            {
                script.currentString = client.DownloadString(fileList[0].GetUrl());
            }

            script.downloaded = true;
        }

        public void OnException(Exception e)
        {
            GameObject gameObject = GameObject.FindGameObjectWithTag("OnlineStorage");
            OnlineStorage script = (OnlineStorage)gameObject.GetComponent(typeof(OnlineStorage));

            script.currentString = "";
            Debug.Log(e);
        }
    }
}
