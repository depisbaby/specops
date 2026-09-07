using NUnit.Framework;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine.UIElements;
using NUnit.Framework.Constraints;
using System.Threading.Tasks;

public class Operator : MonoBehaviour, I_Interactable
{
    [Header("Skills")]
    public AnimationCurve movementAccuracyModifier;

    [Header("Hitboxes")]
    public Collider[] hitBoxes;
    public Collider interactionHitbox;

    [Header("Line of sight checking")]
    public GameObject eyes;
    public GameObject body;
    public GameObject[] lineOfSightCheckPoints;

    [Header("AI Settings")]
    public bool controllable;
    public bool enemy;

    [Header("Other")]
    public Gun gun;
    public Material friendlyMaterial;
    public Material enemyMaterial;
    public MeshRenderer meshRenderer;
    public GameObject pivotBase;
    public LayerMask visionBlocking;
    public GameObject muzzle;
    public GameObject repositionLinePrefab;

    [HideInInspector] public bool visible;
    [HideInInspector] public int health;

    bool alwaysVisible;
    private Vector3 targetPosition;
    private Queue<Vector3> waypoints = new Queue<Vector3>();
    ushort tick;
    ushort tickOffset;
    ushort tickCheckFrequency;
    NavMeshAgent nav;
    private Vector3 aimAtPoint;
    private List<Operator> enemiesInFieldOfView = new List<Operator>();
    private List<Operator> enemiesVisible = new List<Operator>();
    private Operator targetedEnemy;
    private Vector3 lastPosition;
    private Vector3 targetPivot;
    private float visibilityValue;
    private float reactionTime;
    private float offTarget;
    private AITask task;
    private bool alerted;
    private Map.PatrolWaypoint currentPatrolWaypoint;
    
    public enum AITask
    {
        None = 1,
        Patrol = 2,
        Idle = 3,
        Hide = 5,
        Hunt = 6,
    }


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

    void OnDestroy()
    {
    }

    private void FixedUpdate()
    {
        UpdateVisibility();

        if (health > 0)
        {
            Pivot();
            ReactionTime();

            tick++;
            if (tick % tickCheckFrequency != 0)
                return;

            StartMoving();
            CheckPath();

            //vision
            GetEnemyOperatorsInFieldOfView();
            GetEnemyOperatorsVisible();
            GetTargetOperator();

            //combat
            EngageTarget();

            if (!controllable)
            {
                ChooseTask();
            }
        }

        lastPosition = transform.position;
    }

    void OnMissionStart()
    {

        OperatorManager.Instance.AddOperatorToGame(this);

        nav = GetComponent<NavMeshAgent>();
        health = 100;
        tickCheckFrequency = 10;
        tickOffset = (ushort)Random.Range(0, 60 * 5);
        task = AITask.None;

        if (!enemy && controllable)
        {
            alwaysVisible = true;
            visible = true;
            meshRenderer.material = new Material(friendlyMaterial);
        }

        if (enemy)
        {
            alwaysVisible = false;
            visible = false;
            meshRenderer.material = new Material(enemyMaterial);
            meshRenderer.material.SetFloat("_Visibility", 0);
        }

        if (GameManager.Instance.debugMode)
        {
            alwaysVisible = true;
        }
    }

    void UpdateVisibility()
    {
        if (controllable)
            return;
        
        if (alwaysVisible)
        {
            visibilityValue = Mathf.Lerp(visibilityValue, 5.0f, Time.deltaTime * 5);
            meshRenderer.material.SetFloat("_Visibility", visibilityValue);
            return;
        }
        
        if (visible && visibilityValue < 4.9f)
        {
            visibilityValue = Mathf.Lerp(visibilityValue, 5.0f, Time.deltaTime * 5);
            meshRenderer.material.SetFloat("_Visibility", visibilityValue);
        }

        if(!visible && visibilityValue > 0.1)
        {
            visibilityValue = Mathf.Lerp(visibilityValue, 0.0f, Time.deltaTime * 5);
            meshRenderer.material.SetFloat("_Visibility", visibilityValue);
        }

    }

    public void ProjectileHit(int damage)
    {
        if (health <= 0)
            return;

        health -= damage;

        if (health <= 0)
        {
            if (OperatorManager.Instance.selectedOperator == this)
            {
                OperatorManager.Instance.PlayerUnselectOperator();
            }

            foreach (var item in enemiesVisible)
            {
                item.visible = false;
            }

            pivotBase.transform.rotation = Quaternion.Euler(90, 0, 0); //TEMP
            pivotBase.transform.position += new Vector3(0,0.1f,0);
            nav.enabled = false;
            interactionHitbox.gameObject.SetActive(false);

            
        }
        

    }

    

    #region AI

    void StartMoving()
    {

        if (targetPosition == Vector3.zero)
        {
            //Debug.Log("1");
            return;
        }

        if (nav.hasPath)
        {
            //Debug.Log("2");
            return;
        }

        if (nav.pathPending)
        {
            //Debug.Log("3");
            return;
        }

        nav.SetDestination(targetPosition);

    }

    void CheckPath()
    {
        if (nav.hasPath)
        {
            if(nav.path.status == NavMeshPathStatus.PathPartial || nav.path.status == NavMeshPathStatus.PathInvalid)
            {
                targetPosition = Vector3.zero;
                nav.ResetPath();
                return;
            }

            if (nav.remainingDistance < 0.1f)
            {
                if(task == AITask.Patrol)
                {
                    Idle(Random.Range(20, 30));
                    Vector3 lookPosition = transform.position + currentPatrolWaypoint.transform.forward + new Vector3(0, 1, 0);
                    AimAtPoint(lookPosition);
                }

                targetPosition = Vector3.zero;
                nav.ResetPath();
            }
        }
    }

    void Pivot()
    {
        //aim at enemy
        if(targetedEnemy != null)
        {
            Vector3 a = new Vector3(targetedEnemy.transform.position.x, pivotBase.transform.position.y, targetedEnemy.transform.position.z);
            targetPivot = a - pivotBase.transform.position;
            
        }
        else if (aimAtPoint != Vector3.zero) // aim at point
        {
            Vector3 a = new Vector3(aimAtPoint.x, pivotBase.transform.position.y, aimAtPoint.z);
            targetPivot = a - pivotBase.transform.position;

        }
        else
        {
            targetPivot = (transform.position - lastPosition).normalized;
           
        }


        Vector3 pivot = Vector3.Lerp(pivotBase.transform.forward, targetPivot, Time.deltaTime*2);
        offTarget = Vector3.Dot(pivotBase.transform.forward, targetPivot.normalized);
        //Debug.Log(offTarget);

        pivotBase.transform.rotation = Quaternion.LookRotation(pivot, Vector3.up);
    }

    void GetEnemyOperatorsInFieldOfView()
    {
        enemiesInFieldOfView.Clear();

        if (enemy)
        {
            foreach (Operator _operator in OperatorManager.Instance.friendlyOperators)
            {
                Vector3 vectorToOperator = _operator.body.transform.position - eyes.transform.position;
                float dot = Vector3.Dot(vectorToOperator, eyes.transform.forward);
                if (dot > 1.2f)
                {
                    enemiesInFieldOfView.Add(_operator);
                }
            }
        }
        else
        {
            foreach (Operator _operator in OperatorManager.Instance.enemyOperators)
            {
                Vector3 vectorToOperator = _operator.body.transform.position - eyes.transform.position;
                float dot = Vector3.Dot(vectorToOperator, eyes.transform.forward);
                //Debug.Log(dot);
                if (dot > 1.2f)
                {
                    enemiesInFieldOfView.Add(_operator);
                }
            }
        }
    }

    void GetEnemyOperatorsVisible()
    {
        foreach(Operator _operator in enemiesVisible)
        {
            _operator.visible = false;
        }

        bool enemyVisible = false;
        enemiesVisible.Clear();
        foreach (var _operator in enemiesInFieldOfView)
        {
            foreach (var lineOfSightCheckPoint in _operator.lineOfSightCheckPoints)
            {
                if (Physics.Linecast(eyes.transform.position, lineOfSightCheckPoint.transform.position, visionBlocking))
                {
                    continue;
                }

                if (_operator.health <= 0)
                    continue;

                enemiesVisible.Add(_operator);
                _operator.visible = true;
                enemyVisible = true;
                break;
            }
        }

        if(enemyVisible)
        {
            tickCheckFrequency = 3;
        }
        else
        {
            tickCheckFrequency = 10;
        }
    }

    void GetTargetOperator()
    {
        //keep current target
        if(enemiesVisible.Contains(targetedEnemy))
        {
            return;
        }
        
        //TODO BETTER TARGETING!!!

        targetedEnemy = null;

        for (int i = 0; i < enemiesVisible.Count; i++)
        {
            if (enemiesVisible[i].health <= 0)
                continue;

            targetedEnemy = enemiesVisible[0];
        }
    }

    void EngageTarget()
    {
        if (targetedEnemy == null)
            return;

        if (gun.cycling)
            return;

        if (reactionTime < 1.0)
            return;

        if(offTarget < 0.9f || offTarget > 1.1f)
            return;
        


        if (gun.ammo == 0)
        {
            //auto order switch to sidearm
            return;
        }

        float inaccuracy = movementAccuracyModifier.Evaluate(nav.velocity.magnitude) * 0.03f; 
        Vector3 direction = (targetedEnemy.body.transform.position - muzzle.transform.position).normalized;
        Vector3 modifiedDirection = direction + new Vector3(Random.Range(-inaccuracy, inaccuracy), Random.Range(-inaccuracy, inaccuracy), Random.Range(-inaccuracy, inaccuracy));
        gun.Fire(muzzle.transform.position, modifiedDirection);
    }

    void ReactionTime()
    {
        if (targetedEnemy != null)
        {
            reactionTime = Mathf.Clamp(reactionTime + Time.deltaTime*2, 0.0f, 1.0f);
        }
        else
        {
            reactionTime = Mathf.Clamp(reactionTime - Time.deltaTime*2, 0.0f, 1.0f);
        }
    }

    void ChooseTask()
    {
        if (task != AITask.None)
            return;

        if (!alerted && enemy)
        {
            Patrol();
        }

    }

    void Patrol()
    {
        task = AITask.Patrol;


        Map.PatrolWaypoint pw = null;
        foreach (var item in GameManager.Instance.currentMap.patrolWaypoints)
        {
            if (!item.occupied)
            {
                pw = item;
                item.occupied = true;
                break;
            }
        }

        if (pw == null)
            return;

        if(currentPatrolWaypoint != null)
        {
            currentPatrolWaypoint.occupied = false;
            currentPatrolWaypoint = null;
        }

        currentPatrolWaypoint = pw;
        Vector3 position = currentPatrolWaypoint.transform.position;
        Reposition(position);
        ClearAim();
    }

    #endregion

    #region Orders

    public void ClearAim()
    {
        aimAtPoint = Vector3.zero;
    }

    public void Reposition(Vector3 position)
    {
        if (nav.hasPath)
        {
            targetPosition = Vector3.zero;
            nav.ResetPath();
        }
        targetPosition = position;
    }

    public void HoldPosition()
    {
        if (nav.hasPath)
        {
            nav.ResetPath();
        }

        if (waypoints.Count > 0)
        {
            waypoints.Clear();
        }

        targetPosition = Vector3.zero;
    }

    public void AimAtPoint(Vector3 position)
    {
        aimAtPoint = position;
    }

    public async void Idle(int seconds)
    {
        task = AITask.Idle;

        await Task.Delay  (1000*seconds);

        if(this != null)
        {
            task = AITask.None;
        }
    }

    #endregion
}
