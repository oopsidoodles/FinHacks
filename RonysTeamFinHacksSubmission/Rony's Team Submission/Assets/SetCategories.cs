using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class SetCategories : MonoBehaviour {

    private GameObject tempGameObject;
    private GameObject tempGameObject2;
    private OnlineStorage onlineStorage;
    private Dropdown dropDown;
    private Text text;
    public string currentCategory = "";
    private bool categoryDown = false;

    // Use this for initialization
    void Start()
    {




        tempGameObject = GameObject.FindGameObjectWithTag("OnlineStorage");
        onlineStorage = (OnlineStorage)tempGameObject.GetComponent(typeof(OnlineStorage));

        onlineStorage.Initialize();

        /////////////////////////////////////
        //onlineStorage.GetFile("Categories.txt");
        /////////////////////////////////////
        string url = "http://cdn.shephertz.com/repository/files/28a2a9d00f5f028c8490a4678f4cf2ca2b67b4318cd91884d8ec7089f7fe7fb9/60faac041ae1195496e6f5b32ab525ef0ead858a/Categories.txt";
        onlineStorage.DownloadBackup(url);

        categoryDown = true;
	}
	
	// Update is called once per frame
	void Update ()
    {
        tempGameObject = GameObject.FindGameObjectWithTag("OnlineStorage");
        onlineStorage = (OnlineStorage)tempGameObject.GetComponent(typeof(OnlineStorage));

        if (onlineStorage.downloaded && categoryDown)
        {
            categoryDown = false;
            onlineStorage.downloaded = false;

            string[] tempArray = onlineStorage.currentString.Split('\n');
            List<string> tempList = new List<string>();
            tempList.Add("Choose Food Category");
            tempList.AddRange(tempArray);

            tempGameObject = GameObject.FindGameObjectWithTag("DropDown");
            dropDown = (Dropdown)tempGameObject.GetComponent(typeof(Dropdown));

            dropDown.ClearOptions();

            dropDown.AddOptions(tempList);
        }

        tempGameObject = GameObject.FindGameObjectWithTag("DropDown");
        dropDown = (Dropdown)tempGameObject.GetComponent(typeof(Dropdown));
        tempGameObject2 = GameObject.FindGameObjectWithTag("SearchText");
        text = (Text)tempGameObject2.GetComponent(typeof(Text));

        text.text = dropDown.options[dropDown.value].text;

        if (text.text != "Choose Food Category")
        {
            currentCategory = text.text;
        }
        else
        {
            currentCategory = "";
        }
    }
}
