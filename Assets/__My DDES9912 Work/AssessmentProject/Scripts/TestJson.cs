using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TestJson : MonoBehaviour
{

    [Serializable]
    public class MyItem // Represents a single item in the list
    {
        public string id;
        public string action;
        public int value;
    }

    [Serializable]
    public class MyItemList // Holds the list of items
    {
        public List<MyItem> buttons; // The name of this field must match the JSON array key
    }

    public TextAsset jsonFile; // Assign your JSON file (as TextAsset) in the Inspector

    void Start()
    {
        if (jsonFile != null)
        {
            string jsonString = jsonFile.text;
            MyItemList data = JsonUtility.FromJson<MyItemList>(jsonString);

            // Accessing items from the list
            if (data != null && data.buttons != null && data.buttons.Count > 0)
            {
                // Select the first item
                MyItem firstItem = data.buttons[0];
                Debug.Log($"First item: ID={firstItem.id}, Value={firstItem.value}");

                // Select an item by index (e.g., the third item)
                if (data.buttons.Count > 2)
                {
                    MyItem thirdItem = data.buttons[2];
                    Debug.Log($"Third item: ID={thirdItem.id}, Value={thirdItem.value}");
                }

                // Iterate through the list to find a specific item
                foreach (MyItem item in data.buttons)
                {
                    if (item.id == "ButtonBlack_Col2_Row1")
                    {
                        Debug.Log($"Found specific item: ID={item.id}, Value={item.value}");
                        break; // Exit the loop once found
                    }
                }
            }
        }
        else
        {
            Debug.LogError("JSON file not assigned!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
