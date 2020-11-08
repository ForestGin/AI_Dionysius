using UnityEngine;
using System.Collections;
using Unity.UNetWeaver;
using System.Dynamic;
using UnityEngine.UI;
using UnityEngine.AI;

public class TankMovement : MonoBehaviour
{
    public int m_PlayerNumber = 1;         
    public float m_Speed = 7f;            
    public float m_TurnSpeed = 180f;       
    public AudioSource m_MovementAudio;    
    public AudioClip m_EngineIdling;       
    public AudioClip m_EngineDriving;      
    public float m_PitchRange = 0.2f;

    
    private string m_MovementAxisName;     
    private string m_TurnAxisName;         
    private Rigidbody m_Rigidbody;         
    private float m_MovementInputValue;    
    private float m_TurnInputValue;        
    private float m_OriginalPitch;

    //------------------------------------------------------------
    private GameManager m_ManagerScript;
    //public Vector3 m_MyTankPosition;
    public Vector3 m_ClosestTankPosition;

    private GameObject m_Turret;
    private Transform m_TankTransform;
    private Transform m_TurretTransform;

    //wander
    Vector3 wayPoint = new Vector3(0.0f, 0.0f, 0.0f);
    private float Range = 20f;
    private NavMeshAgent Tank;
    private NavMeshPath path;
    private bool walkable = true;
    //frontiers
    private Transform TopFrontier;
    private Transform BotFrontier;
    private Transform LeftFrontier;
    private Transform RightFrontier;

    //patrol
    private GameObject[] pointChildren;
    private GameObject Points;  
    public int destPoint = -1;
    Vector3 wayPoint2 = new Vector3(0.0f, 0.0f, 0.0f);
    private float breakforce = 0.25f;
    private float speed = 3.5f;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }


    private void OnEnable ()
    {
        m_Rigidbody.isKinematic = false;
        m_MovementInputValue = 0f;
        m_TurnInputValue = 0f;
    }


    private void OnDisable ()
    {
        m_Rigidbody.isKinematic = true;
    }


    private void Start()
    {
        //wander
        TopFrontier = GameObject.Find("TopFrontier").GetComponent<Transform>();
        BotFrontier = GameObject.Find("BotFrontier").GetComponent<Transform>();
        LeftFrontier = GameObject.Find("LeftFrontier").GetComponent<Transform>();
        RightFrontier = GameObject.Find("RightFrontier").GetComponent<Transform>();

        path = new NavMeshPath();
        Tank = GetComponent<NavMeshAgent>();
        if (m_PlayerNumber == 2)
            Wander();
        //

        //patrol
        Points = GameObject.Find("Points");
        pointChildren = new GameObject[Points.transform.childCount];
        for (int i = 0; i < Points.transform.childCount; i++)
        {
            pointChildren[i] = Points.transform.GetChild(i).gameObject;
        }
        

        if (m_PlayerNumber == 1)
            Patrol();

        //

        m_ManagerScript = GameObject.Find("GameManager").GetComponent<GameManager>();

        //m_Turret = gameObject.transform.Find("TankRenderers/TankTurret").gameObject;
        
        //if (m_Turret != null)
        //{
        //    Debug.Log("Turret Child found successfully!");
        //}
        //else
        //{
        //    Debug.Log("Turret Child not found!");
        //}

        //m_ClosestTankPosition = GetClosestTankPosition();

        m_MovementAxisName = "Vertical" + m_PlayerNumber;
        m_TurnAxisName = "Horizontal" + m_PlayerNumber;

        m_OriginalPitch = m_MovementAudio.pitch;
    }

    private void Update()
    {
        //Debug path line
        for (int i = 0; i < path.corners.Length - 1; i++)
        {
            Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.red);
        }

        //wander logic and acceleration
        if (m_PlayerNumber == 2)
        {
           
            if ((transform.position - Tank.destination).magnitude <= 3f && Tank.speed > 1f)
            {
                Tank.speed -= breakforce;
            }

            if ((transform.position - Tank.destination).magnitude <= Tank.stoppingDistance)
            {
                Tank.speed = speed;
                Wander();
            }
        }

        //patrol
        if (m_PlayerNumber == 1)
        {
            if (!Tank.pathPending && Tank.remainingDistance <= 3f)
                Tank.speed -= breakforce;

            if (!Tank.pathPending && Tank.remainingDistance <= 1f)
            {
                Patrol();
                Tank.speed = speed;
            }
        }

            // Store the player's input and make sure the audio for the engine is playing.

            //m_MovementInputValue = Input.GetAxis (m_MovementAxisName);
            //m_TurnInputValue = Input.GetAxis (m_TurnAxisName);

        //m_ClosestTankPosition = GetClosestTankPosition();
        //m_ClosestTankPosition.y = 5;
        //m_Turret.transform.LookAt(m_ClosestTankPosition);

        EngineAudio ();
    }

    //Vector3 GetClosestTankPosition()
    //{
    //    float dist = -1;
    //    float mindist = 0;

    //    Vector3 closest = Vector3.zero;

    //    for (int i = 0; i < m_ManagerScript.m_Tanks.Length; i++)
    //    {
    //        //Making sure that the closest tank is not themselves
    //        if (i != m_PlayerNumber - 1)
    //        {
    //            //First iteration
    //            if (dist == -1)
    //            {
    //                mindist = dist = Vector3.Distance(m_ManagerScript.m_TanksPosition[i], gameObject.transform.position);
    //                closest = m_ManagerScript.m_TanksPosition[i];
    //            }
    //            else
    //            {
    //                dist = Vector3.Distance(m_ManagerScript.m_TanksPosition[i], gameObject.transform.position);

    //                if (dist < mindist)
    //                {
    //                    mindist = dist;
    //                    closest = m_ManagerScript.m_TanksPosition[i];
    //                }
    //            }
    //        }  
    //    }

    //    return closest;
    //}

    private void EngineAudio()
    {
        // Play the correct audio clip based on whether or not the tank is moving and what audio is currently playing.

        if (Mathf.Abs (m_MovementInputValue) < 0.1f && Mathf.Abs (m_TurnInputValue) < 0.1f)
        {
            if (m_MovementAudio.clip == m_EngineDriving)
            {
                m_MovementAudio.clip = m_EngineIdling;
                m_MovementAudio.pitch = Random.Range (m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                m_MovementAudio.Play ();
            }
        }
        else
        {
            if (m_MovementAudio.clip == m_EngineIdling)
            {
                m_MovementAudio.clip = m_EngineDriving;
                m_MovementAudio.pitch = Random.Range (m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                m_MovementAudio.Play ();
            }
        }
    }


    private void FixedUpdate()
    {
        // Move and turn the tank.
        Move ();
        Turn ();
    }


    private void Move()
    {
        // Adjust the position of the tank based on the player's input.
        Vector3 movement = transform.forward * m_MovementInputValue * m_Speed * Time.deltaTime;
    
        m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
    }


    private void Turn()
    {
        // Adjust the rotation of the tank based on the player's input.
        float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime;

        Quaternion turnRotation =  Quaternion.Euler (0f, turn, 0f);

        m_Rigidbody.MoveRotation (m_Rigidbody.rotation * turnRotation);
    }

    private void Wander()
    {
        Tank.angularSpeed = 200f;
        wayPoint.x = Random.Range(transform.position.x - Range, transform.position.x + Range);
        wayPoint.z = Random.Range(transform.position.z - Range, transform.position.z + Range);
        //transform.LookAt(wayPoint);

        if (wayPoint.x > LeftFrontier.position.x && wayPoint.x < RightFrontier.position.x && wayPoint.z < TopFrontier.position.z && wayPoint.z > BotFrontier.position.z)
        {
            Tank.destination = wayPoint;
            walkable = Tank.CalculatePath(Tank.destination, path);//returns true if path is find
        }
        else if(wayPoint.x < LeftFrontier.position.x && wayPoint.x > RightFrontier.position.x && wayPoint.z > TopFrontier.position.z && wayPoint.z < BotFrontier.position.z)
        {
            wayPoint.x = Random.Range(transform.position.x - Range, transform.position.x + Range);
            wayPoint.z = Random.Range(transform.position.z - Range, transform.position.z + Range);
            //transform.LookAt(wayPoint);

            Tank.destination = wayPoint;
            walkable = Tank.CalculatePath(Tank.destination, path);//repath the next waypoint
        }

        Debug.Log(walkable);
        Debug.Log(path.status);
        Debug.Log(wayPoint);
        
    }

    private void Patrol()
    {
        
        if (pointChildren.Length == 0)
            return;

        if (destPoint == -1)
        {
            Tank.destination = ClosestPatrolPoint();
        }
        else
        {
            Tank.destination = pointChildren[destPoint].transform.position;
            destPoint = (destPoint + 1) % pointChildren.Length;
        }

        Tank.angularSpeed = 200f;
        //transform.LookAt(Tank.destination);

        Debug.Log(pointChildren[2].transform.position.y);
        Debug.Log("MEMBERS:" + pointChildren.Length);

    }


    private Vector3 ClosestPatrolPoint()
    {
        float dist = -1;
        float mindist = 0;

        Vector3 closest = Vector3.zero;

        for (int i = 0; i < pointChildren.Length; i++)
        {
                        
            //First iteration
            if (dist == -1)
            {                
                mindist = dist = Vector3.Distance(pointChildren[i].transform.position, gameObject.transform.position);
                closest = pointChildren[i].transform.position;
                destPoint = i;
            }
            else
            {
                dist = Vector3.Distance(pointChildren[i].transform.position, gameObject.transform.position);

                if (dist < mindist)
                {
                    mindist = dist;
                    closest = pointChildren[i].transform.position;
                    destPoint = i;
                }
            }
            
        }

        return closest;
    }
}