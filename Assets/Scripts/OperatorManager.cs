using NUnit.Framework.Constraints;
using System.Collections.Generic;
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

    private Queue<Vector3> waypoints;

    public List<Operator> friendlyOperators;
    public List<Operator> enemyOperators;

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

        if (!_operator.controllable)
            return;

        if (selectedOperator != null)
        {
            PlayerUnselectOperator();
        }

        _operator.meshRenderer.material.SetFloat("_Selection", 10f);

        selectedOperator = _operator;
    }

    public void PlayerUnselectOperator()
    {
        if (selectedOperator == null)
            return;

        selectedOperator.meshRenderer.material.SetFloat("_Selection", 0f);

        selectedOperator = null;
    }

    public void AddOperatorToGame(Operator _operator)
    {
        if (_operator.enemy)
        {
            enemyOperators.Add(_operator);
        }
        else
        {
            friendlyOperators.Add(_operator);
        }
    }

    #region Orders
    public void ClearAim(Operator _operator)
    {
        _operator.ClearAim();
    }

    public void HoldPosition(Operator _operator)
    {
        _operator.HoldPosition();
    }

    public void RepositionOperator(Operator _operator, Vector3 position)
    {
        _operator.Reposition(position);
    }

    public void AimAtPoint(Operator _operator, Vector3 position)
    {
        _operator.AimAtPoint(position);
    } 

    #endregion

}
