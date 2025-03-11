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


                if (hit.transform.gameObject.layer != 6)
                {
                    OperatorManager.Instance.PlayerUnselectOperator();
                    return;

                }

                ClickedInteractable(hit.transform.parent.gameObject.GetComponent<I_Interactable>());

            }
            else
            {
                OperatorManager.Instance.PlayerUnselectOperator();
            }
        }

        if (Input.GetButtonDown("Fire2"))
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity, interactingLayer))
            {
                I_Interactable interactable = hit.transform.parent.gameObject.GetComponent<I_Interactable>();

                if(interactable != null)
                {
                    ControlPanel.Instance.SetWorldCursorData(interactable,hit.point);
                }
                else
                {
                    ControlPanel.Instance.SetWorldCursorData(null, hit.point);
                }

            }
            else
            {
                ControlPanel.Instance.SetWorldCursorData(null,Vector3.zero);

            }
        }


    }

    void ClickedInteractable(I_Interactable interactable)
    {
        //Debug.Log("hep");
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

}
