using Newtonsoft.Json;
using System;
using System.IO;
using UnityEngine;

public class ReadButtonJson : MonoBehaviour
{
    public TextAsset textJSON; // Drag your JSON file here in the Inspector

    [System.Serializable]
    public class Button
    {
        public string id;
        public string action;
        public int value;
    }

    [System.Serializable]
    public class ButtonList
    {
        public Button[] button;
    }

    public ButtonList victorButtonList = new ButtonList();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        victorButtonList = JsonUtility.FromJson<ButtonList>(textJSON.text);
        

        //JsonTextReader reader = new JsonTextReader(new StringReader(jsonFile));

        /*
        if (jsonFile != null)
        {
            // Access the raw text content of the JSON file
            string jsonText = jsonFile.text;
            Debug.Log("JSON Content: " + jsonText);

            // Deserialize the JSON into your custom class (if applicable)
            // MyData data = JsonUtility.FromJson<MyData>(jsonText);
            // Debug.Log("Deserialized Name: " + data.name);
        }
        else
        {
            Debug.LogError("JSON file not assigned to the 'jsonFile' variable.");
        }
        */
        /*
        while (reader.Read())
        {
            if (reader.Value != null)
            {
                Console.WriteLine("Token: {0}, Value: {1}", reader.TokenType, reader.Value);
            }
            else
            {
                Console.WriteLine("Token: {0}", reader.TokenType);
            }
        }
        */
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
