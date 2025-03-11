using UnityEngine;
using UnityEngine.UIElements;

public class OperatorManager : MonoBehaviour
{
    #region Singleton
    public static OperatorManager Instance;
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

    public Operator selectedOperator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayerSelectOperator(Operator _operator)
    {
        if (!_operator.visible)
            return;

        selectedOperator = _operator;
    }

    public void PlayerUnselectOperator()
    {
        selectedOperator = null;
    }


    #region Orders
    public void ClearAim(Operator _operator)
    {

    }

    public void HoldPosition(Operator _operator)
    {

    }

    public void RepositionOperator(Operator _operator, Vector3 position)
    {

    }

    #endregion

}
