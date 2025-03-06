using UnityEngine;

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
    public Order currentOrder;

    public enum Order
    {
        None = 0,
        Reposition= 1,
        HoldAimAt = 2,
        AimAtDirection =3,
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire2"))
        {
            if (currentOrder != Order.None)
            {
                StopGivingOrder();
                return;
            }

            root.SetActive(!root.activeSelf);
        }
    }

    public void OrderClicked(int id)
    {
        StartGivingOrder(id);
        root.SetActive(false);
    }

    public void StartGivingOrder(int id)
    {
        currentOrder = (Order)id;
    }

    public void StopGivingOrder()
    {
        currentOrder = Order.None;
    }
}
