using System.Collections.Generic;
using UnityEngine;

public class TestTuples : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 1️ Declare a list of tuples — each holds (int id, string name)
        List<(int id, string name)> myTuples = new List<(int, string)>();

        // 2️ Populate the list
        myTuples.Add((1, "Start"));
        myTuples.Add((2, "Jump"));
        myTuples.Add((3, "Reload"));
        myTuples.Add((4, "Crouch"));

        // 3️ Loop through and print them
        foreach (var t in myTuples)
        {
            Debug.Log($"ID: {t.id}, Name: {t.name}");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}