using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class DropDownSort : MonoBehaviour
{
    private GameObject tempObject;
    private GameObject tempObject2;
    private GameObject tempObject3;
    private GameObject tempObject4;
    private GameObject tempObject5;
    private ScrollRect scrollRect;
    private Image image;
    private OnlineStorage onlineStorage;
    private SetCategories setCategories;
    private SortingAlgorithm sort;

    private string currentCategory = "";
    private string previousCategory = "";

    private bool categoryDown = false;

    private Dictionary<string, string> urls = new Dictionary<string, string>();

    // Use this for initialization
    void Start ()
    {
        tempObject = GameObject.FindGameObjectWithTag("DropDown");
        setCategories = (SetCategories)tempObject.GetComponent(typeof(SetCategories));

        urls.Add("Hummus", "http://cdn.shephertz.com/repository/files/28a2a9d00f5f028c8490a4678f4cf2ca2b67b4318cd91884d8ec7089f7fe7fb9/2700eb4071506fd681e1af0f8f27a2a16fc2b7ff/Hummus.txt");
        urls.Add("Breads", "http://cdn.shephertz.com/repository/files/28a2a9d00f5f028c8490a4678f4cf2ca2b67b4318cd91884d8ec7089f7fe7fb9/7356714e22a0d76bd13ab84f1447540a73c2c7a3/Breads.txt");
        urls.Add("Pasta", "http://cdn.shephertz.com/repository/files/28a2a9d00f5f028c8490a4678f4cf2ca2b67b4318cd91884d8ec7089f7fe7fb9/83736a729d162a42d59f5e60eeb252fe221a901d/Pasta.txt");
        urls.Add("Dessert", "http://cdn.shephertz.com/repository/files/28a2a9d00f5f028c8490a4678f4cf2ca2b67b4318cd91884d8ec7089f7fe7fb9/079c15ca7c89debb521cb9676a71ce41e31a1901/Dessert.txt");
        urls.Add("Beverage", "http://cdn.shephertz.com/repository/files/28a2a9d00f5f028c8490a4678f4cf2ca2b67b4318cd91884d8ec7089f7fe7fb9/b892b95c71342aad27a4ab8babd88180c1bc8cac/Beverage.txt");
        urls.Add("Soup", "http://cdn.shephertz.com/repository/files/28a2a9d00f5f028c8490a4678f4cf2ca2b67b4318cd91884d8ec7089f7fe7fb9/7e734b50eaf00875b174e674e3b4d871508cdb84/Soup.txt");
        urls.Add("Sandwich", "http://cdn.shephertz.com/repository/files/28a2a9d00f5f028c8490a4678f4cf2ca2b67b4318cd91884d8ec7089f7fe7fb9/1fd225b346ffbbc7a1e5b1075b0e8d9cdec82b4e/Sandwich.txt");
        urls.Add("Beef", "http://cdn.shephertz.com/repository/files/28a2a9d00f5f028c8490a4678f4cf2ca2b67b4318cd91884d8ec7089f7fe7fb9/92be297271150b1221aa4814e3fd64feaeda1796/Beef.txt");
        urls.Add("Lamb", "http://cdn.shephertz.com/repository/files/28a2a9d00f5f028c8490a4678f4cf2ca2b67b4318cd91884d8ec7089f7fe7fb9/9d0bea3617ffb5b3a8b25b7cff934c83283eab3f/Lamb.txt");
        urls.Add("Chicken", "http://cdn.shephertz.com/repository/files/28a2a9d00f5f028c8490a4678f4cf2ca2b67b4318cd91884d8ec7089f7fe7fb9/bb0f72c23b24949198cf9b8af2b038ec11db06aa/Chicken.txt");
        urls.Add("Salad", "http://cdn.shephertz.com/repository/files/28a2a9d00f5f028c8490a4678f4cf2ca2b67b4318cd91884d8ec7089f7fe7fb9/a0d7a109248853cfbf8cb3d4ad0274f98c4c339f/Salad.txt");
	}
	
	// Update is called once per frame
	void Update ()
    {
        previousCategory = currentCategory;
        currentCategory = setCategories.currentCategory;

        tempObject3 = GameObject.FindGameObjectWithTag("OnlineStorage");
        onlineStorage = (OnlineStorage)tempObject3.GetComponent(typeof(OnlineStorage));

        if (previousCategory != currentCategory && currentCategory != "")
        {
            ////////////////////////////////////////////////
            //onlineStorage.GetFile(currentCategory + ".txt");
            //////////////////////////////////////////////
            string url = urls[currentCategory];
            onlineStorage.DownloadBackup(url);

            categoryDown = true;
        }

        if (categoryDown && onlineStorage.downloaded)
        {
            tempObject4 = GameObject.FindGameObjectWithTag("ScrollView");

            for (int a = 1; a < tempObject4.transform.childCount; a++)
            {
                GameObject.DestroyImmediate(tempObject4.transform.GetChild(a).gameObject);
                a--;
            }

            categoryDown = false;
            onlineStorage.downloaded = false;

            List<MenuItem> menuItems = new List<MenuItem>();
            string contents = onlineStorage.currentString;
            string[] items = contents.Split('\n');
            foreach (string item in items)
            {
                if (item == "")
                {
                    continue;
                }
                string[] parts = item.Split(':');
                menuItems.Add(new MenuItem(parts[1], float.Parse(parts[2]), parts[0], currentCategory));
            }

            MenuItem[] sorted = menuItems.ToArray();

            for (int i = sorted.Length; i > 0; i--)
            {
                for (int j = 0; j < sorted.Length - 1; j++)
                {
                    if (sorted[j].Price > sorted[j + 1].Price)
                    {
                        MenuItem temp = sorted[j];
                        sorted[j] = sorted[j + 1];
                        sorted[j + 1] = temp;
                    }
                }
            }

            //tempObject2 = GameObject.FindGameObjectWithTag("Sort");
            //sort = (SortingAlgorithm)tempObject2.GetComponent(typeof(SortingAlgorithm));

            //MenuItem[] sorted = sort.SortByPrice(menuItems.ToArray(), 0, menuItems.Count - 1);

            tempObject4 = GameObject.FindGameObjectWithTag("ScrollView");
            //image = (Image)tempObject4.GetComponent(typeof(Image));
            image = tempObject4.transform.GetChild(0).gameObject.GetComponent<Image>();

            Image tempImage;

            for (int i = 0; i < sorted.Length; i++)
            {
                tempImage = Instantiate(image);
                tempImage.transform.SetParent(tempObject4.transform);

                //Debug.Log(tempImage.transform.position.y);
                
                //tempImage.transform.position.Set(tempImage.transform.position.x, tempImage.transform.position.y - 370, tempImage.transform.position.z);
                //image.transform.position.Set(image.transform.position.x, image.transform.position.y - 370, image.transform.position.z);

                Text[] texts = new Text[3];
                Text[] texts2 = new Text[3];
                //texts = tempImage.GetComponents<Text>();
                //texts2 = image.GetComponents<Text>();
                texts[0] = tempImage.transform.GetChild(0).gameObject.GetComponent<Text>();
                texts[1] = tempImage.transform.GetChild(1).gameObject.GetComponent<Text>();
                texts[2] = tempImage.transform.GetChild(2).gameObject.GetComponent<Text>();

                texts2[0] = image.transform.GetChild(0).gameObject.GetComponent<Text>();
                texts2[1] = image.transform.GetChild(1).gameObject.GetComponent<Text>();
                texts2[2] = image.transform.GetChild(2).gameObject.GetComponent<Text>();

                //Debug.Log(tempImage.transform.position.y);

                //for (int j = 0; j < texts.Length; j++)
                //{
                //    texts[i].transform.position.Set(texts[i].transform.position.x, texts[i].transform.position.y - 370, texts[i].transform.position.z);
                //    texts2[i].transform.position.Set(texts2[i].transform.position.x, texts2[i].transform.position.y - 370, texts2[i].transform.position.z);
                //}

                texts[0].text = sorted[i].Name;
                texts[1].text = Convert.ToString(sorted[i].Price);
                texts[2].text = sorted[i].Franchise;

                //tempImage.enabled = true;
                //image.GetComponent<Renderer>().enabled = false;
                //tempImage.GetComponent<Renderer>().enabled = true;

                //Debug.Log(tempImage.transform.position.y);
            }

            //Debug.Log(Convert.ToString(tempObject4.transform.childCount));

            //int y = Convert.ToInt32(tempObject4.transform.GetChild(0).localPosition.y) - 370;

            //for (int i = 1; i < tempObject4.transform.childCount; i++)
            //{
            //    tempObject4.transform.GetChild(i).position.Set(tempObject4.transform.GetChild(0).localPosition.x, y, tempObject4.transform.GetChild(0).localPosition.z);
            //    y -= 370;
            //}

            // Debug.Log(tempObject4.transform.GetChild(0).transform.localPosition.y);
            //Debug.Log(tempObject4.transform.GetChild(0).GetComponent<RectTransform>().position.y);
            //Debug.Log(tempObject4.transform.GetChild(0).transform.localScale.y);

            //Debug.Log(tempObject4.transform.GetChild(0).GetComponent<RectTransform>().position);

            //tempObject4.transform.GetChild(0).GetComponent<RectTransform>().position = new Vector3(0, -370, 0);

            //Debug.Log(tempObject4.transform.GetChild(0).GetComponent<RectTransform>().position);

            //Debug.Log(tempObject4.transform.GetChild(1).GetComponent<RectTransform>().position);
            //Debug.Log(tempObject4.transform.GetChild(2).GetComponent<RectTransform>().position);
            //Debug.Log(tempObject4.transform.GetChild(3).GetComponent<RectTransform>().position);
            
            //for (int i = 0; i < tempObject4.transform.childCount; i++)
            //{
            //    tempObject4.transform.GetChild(i).GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
            //}

            //GameObject.Destroy(tempObject4.transform.GetChild(0).gameObject);

            Vector2 tempVector = new Vector2(0, 0);

            for (int i = 1; i < tempObject4.transform.childCount; i++)
            {
                tempObject4.transform.GetChild(i).GetComponent<RectTransform>().anchoredPosition = tempVector;
                tempVector.y -= 370;
            }

            tempObject5 = GameObject.FindGameObjectWithTag("ScrollWheel");
            scrollRect = (ScrollRect)tempObject5.GetComponent(typeof(ScrollRect));
            
        }
	}

    
}
