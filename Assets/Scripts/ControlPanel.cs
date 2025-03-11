using UnityEngine;
using System.Threading.Tasks;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ControlPanel : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    #region Singleton
    public static ControlPanel Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.Log("Deleted PlayerInteract dublicate");
            Destroy(gameObject);
        }
    }
    #endregion


    public GameObject root;
    public Image[] wheelButtons;
    public Action[] definedActions;

    [HideInInspector] public int activeWheelButton;

    private I_Interactable selectedInteractable;
    private Vector3 worldCursorPoint;
    private bool worldCursorDataSent;
    private List<Action> currentActions = new List<Action>();

    [System.Serializable]
    public struct Action
    {
        public string name;
        public Sprite sprite;
    }

    void Start()
    {
        root.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire2"))
        {
            OpenWheel(); //async
            return;

        }

        if (Input.GetButtonUp("Fire2"))
        {
            if(activeWheelButton != -1)
            {
                OrderClicked(activeWheelButton);
            }

            CloseWheel();
        }

    }

    public void OrderClicked(int id)
    {
        if (id >= currentActions.Count)
            return;

        Action action = currentActions[id];
        activeWheelButton = -1;

        switch (action.name)
        {
            case "Clear aim":
                
                break;

            case "Hold position":
                Debug.Log("Hold position");
                break;

            case "Reposition":
                if (worldCursorPoint == Vector3.zero)
                    break;
                OperatorManager.Instance.RepositionOperator(OperatorManager.Instance.selectedOperator, worldCursorPoint);
                break;

            case "Aim on point":
                Debug.Log("Aim on point");
                break;

            case "Aim at direction":
                Debug.Log("Aim at direction");
                break;

            default:
                break;
        }

        selectedInteractable = null;

    }

    public void SetWorldCursorData(I_Interactable interactable, Vector3 _worldCursorPoint)
    {
        worldCursorPoint = _worldCursorPoint;
        selectedInteractable = interactable;
        worldCursorDataSent = true;
    }


    //wheel
    async void OpenWheel()
    {
        activeWheelButton = -1;
        while (worldCursorDataSent == false)
        {
            await Task.Yield();
        }
        worldCursorDataSent = false;

        UpdateWheel();

        root.SetActive(true);

    }

    void CloseWheel()
    {
        root.SetActive(false);
    }

    void UpdateWheel()
    {
        currentActions.Clear();

        if(OperatorManager.Instance.selectedOperator != null)
        {
            currentActions.Add(definedActions[0]);
            currentActions.Add(definedActions[1]);
            currentActions.Add(definedActions[2]);
            currentActions.Add(definedActions[3]);
            currentActions.Add(definedActions[4]);
        }

        
        for (int i = 0; i < wheelButtons.Length; i++)
        {
            wheelButtons[i].enabled = false;

            if(i < currentActions.Count)
            {
                wheelButtons[i].sprite = currentActions[i].sprite;
                wheelButtons[i].enabled = true;
            }
        }
        

    }

    //ui event triggers
    public void WheelButtonEnter(int buttonIndex)
    {
        activeWheelButton = buttonIndex;
    }

    public void WheelButtonExit()
    {
        activeWheelButton = -1;
    }


}
