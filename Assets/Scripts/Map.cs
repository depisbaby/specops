using NUnit.Framework.Constraints;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public GameObject patrolWaypointsMarkers;
    [SerializeField] private GameObject roomsMarkers;
    
    [HideInInspector] public List<Room> rooms = new List<Room>();
    [HideInInspector] public List<PatrolWaypoint> patrolWaypoints = new List<PatrolWaypoint>();

    public class Room
    {
        public Vector3 center;
        public List<Transform> doorWays = new List<Transform>();
    }

    public class PatrolWaypoint 
    {
        public bool occupied;
        public Transform transform;

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.Instance.currentMap = this;

        //add patrol waypoints
        for (int i = 0; i < patrolWaypointsMarkers.transform.childCount; i++)
        {
            PatrolWaypoint pw = new PatrolWaypoint();
            pw.transform = patrolWaypointsMarkers.transform.GetChild(i).transform;
            patrolWaypoints.Add(pw);
        }

        //add rooms
        for (int i = 0; i < roomsMarkers.transform.childCount; i++)
        {
            GameObject go = roomsMarkers.transform.GetChild(i).gameObject;
            Room newRoom = new Room();
            newRoom.center = go.gameObject.transform.position;
            for (int j = 0; j < go.transform.childCount; j++)
            {
                newRoom.doorWays.Add(go.transform.GetChild(j).transform);
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
