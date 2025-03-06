using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraInteract : MonoBehaviour
{
    #region Singleton
    public static CameraInteract Instance;
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

    [SerializeField] private LayerMask interactingLayer;

    private void Start()
    {

    }
    //
    private void Update()
    {
        Inputs();
        
    }

    void Inputs()
    {
        if (ControlPanel.Instance.root.activeSelf)
            return;

        if (Input.GetButtonDown("Fire1"))
        {
            RaycastHit hit;
            if(Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity, interactingLayer))
            {

                if (ControlPanel.Instance.currentOrder != ControlPanel.Order.None)
                {
                    GiveOrder();
                }

                if (hit.transform.gameObject.layer != 6)
                    return;

                ClickedInteractable(hit.transform.parent.gameObject.GetComponent<I_Interactable>());


            }
            else
            {
                
            }
        }


    }

    void ClickedInteractable(I_Interactable interactable)
    {
        Debug.Log("hep");
        string interfaceTag = interactable.GetInterfaceTag();

        switch (interfaceTag)
        {
            case "operator":
                OperatorManager.Instance.PlayerSelectOperator(interactable.GetGameObject().GetComponent<Operator>());
                break;

            default:
                break;
        }
    }

    void GiveOrder()
    {
        switch (ControlPanel.Instance.currentOrder)
        {
            case ControlPanel.Order.None:
                break;
            case ControlPanel.Order.Reposition:
                break;
            case ControlPanel.Order.HoldAimAt:
                break;
            case ControlPanel.Order.AimAtDirection:
                break;
        }
    }



}
