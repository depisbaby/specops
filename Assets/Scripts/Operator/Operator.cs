using NUnit.Framework;
using UnityEngine;
using System.Collections;

public class Operator : MonoBehaviour, I_Interactable
{
    public bool controllable;
    public bool enemy;
    [HideInInspector] public bool visible;

    bool alwaysVisible;

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public string GetInterfaceTag()
    {
        return "operator";
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        OnMissionStart();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMissionStart()
    {
        if (!enemy && controllable)
        {
            alwaysVisible = true;
            visible = true;
        }
    }
}
