using UnityEngine;
using UnityEngine.UI;

public class TankShooting : MonoBehaviour
{
    public int m_PlayerNumber = 1;
    public int m_TeamNumber = 1;
    public Color m_PlayerColor = Color.white;

    public Rigidbody m_Shell;            
    public Transform m_FireTransform;              
    public AudioSource m_ShootingAudio;      
    public AudioClip m_FireClip;            
    

    //Shooting direction
    [HideInInspector]public GameObject m_Turret;

    private GameManager m_Manager;
    public Vector3 m_ClosestTankPosition;
    public float m_ClosestTankDistance;

    //Parabolic motion shooting
    public int m_RateOfFire = 1; //per second
    public bool m_ShootDelay = true;
    private float m_ShootingTimer;

    public float m_gravity = Physics.gravity.magnitude;

    public float m_InitialVelocity;
    private float m_InitialHeight;
    public float m_CurrentVelocity;
    public float m_CurrentHeight;

    public float m_ShootingAngle;
    public int m_ShootingAngleOffset = 30;
    
    private float m_MaxShootingRangeAngle = 45f;
    public float m_MaxShootingRange;
    

    private void OnEnable()
    {
        //m_CurrentLaunchForce = m_MinLaunchForce;
        //m_AimSlider.value = m_MinLaunchForce;
    }


    private void Start()
    {
        m_Manager = GameObject.Find("GameManager").GetComponent<GameManager>();

        m_Turret = gameObject.transform.Find("TankRenderers/TankTurret").gameObject;

        if (m_Turret != null)
        {
            Debug.Log("Turret Child found successfully!");
        }
        else
        {
            Debug.Log("Turret Child not found!");
        }

        m_ClosestTankPosition = GetClosestTankAlivePosition();

        m_InitialHeight = m_FireTransform.position.y;

        m_MaxShootingRange = CalculateShootingRange(m_MaxShootingRangeAngle, m_InitialVelocity, m_InitialHeight);

        m_ShootingAngle = CalculateShootingAngle(m_ClosestTankDistance, 0, m_InitialVelocity, m_InitialHeight);
    }
    

    private void Update()
    {
        //Turret pointing at closest tank
        m_ClosestTankPosition = GetClosestTankAlivePosition();
        m_ClosestTankPosition.y = 1.2f;

        m_Turret.transform.LookAt(m_ClosestTankPosition);

        //Fire when enemy is in range
        m_ClosestTankDistance = Vector3.Distance(m_ClosestTankPosition, m_FireTransform.position);

        m_MaxShootingRange = CalculateShootingRange(m_MaxShootingRangeAngle, m_InitialVelocity, m_InitialHeight);

        if (m_ClosestTankDistance < m_MaxShootingRange)
        {
            m_ShootingAngle = CalculateShootingAngle(m_ClosestTankDistance, 0, m_InitialVelocity, m_InitialHeight);

            if (!float.IsNaN(m_ShootingAngle))
            {
                m_Turret.transform.Rotate(-m_ShootingAngle, 0, 0);
            }

            //Fire Delay
            if (m_ShootingTimer < Time.time && !float.IsNaN(m_ShootingAngle))
            {
                //Fire shell
                Fire();
                m_ShootDelay = true;
                m_ShootingTimer = Time.time + m_RateOfFire;
            }

            if (!float.IsNaN(m_ShootingAngle))
            {
                m_Turret.transform.Rotate(m_ShootingAngleOffset, 0, 0);
            }
        }
        else
        {
            m_Turret.transform.LookAt(m_ClosestTankPosition);
        }
        

    }

    void OnDrawGizmos()
    {
        //Draw Radius circle
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(m_FireTransform.position, m_MaxShootingRange);
    }

    private void Fire()
    {
        // Instantiate and launch the shell.

        Rigidbody shellInstance = Instantiate(m_Shell, m_FireTransform.position, m_Turret.transform.rotation) as Rigidbody;

        shellInstance.velocity = m_InitialVelocity * m_Turret.transform.forward;

        shellInstance.useGravity = true;

        shellInstance.tag = "Projectile";

        m_ShootingAudio.clip = m_FireClip;
        m_ShootingAudio.Play();
    }

    private float CalculateShootingRange(float a, float v0, float h0)
    {
        float arad = Mathf.Deg2Rad * a;

        //Parabolic Horizontal Distance function
        return ((v0 * Mathf.Cos(arad)) / m_gravity) * ((v0 * Mathf.Sin(arad)) + (Mathf.Sqrt(((v0 * Mathf.Sin(arad)) * (v0 * Mathf.Sin(arad))) + (2 * m_gravity * h0))));
    }

    private float CalculateShootingAngle(float x, float y, float v, float h0)
    {
        float sqrt = (v * v * v * v) - (m_gravity * (m_gravity * (x * x) + 2 * y * (v * v)));

        sqrt = Mathf.Sqrt(sqrt);
        float anglePos = Mathf.Rad2Deg * Mathf.Atan(((v * v) + sqrt) / (m_gravity * x));
        float angleNeg = Mathf.Rad2Deg * Mathf.Atan(((v * v) - sqrt) / (m_gravity * x));

        if (m_PlayerNumber % 4 == 0) //If player number 4 (out of 4) will have a high parabola
        {
            return anglePos;
        }
        else if (m_PlayerNumber % 4 == 1) //If player number 1 (out of 4) will have a high parabola
        {
            return anglePos;
        }
        else if (m_PlayerNumber % 4 == 2) //If player number 2 (out of 4) will have a low parabola
        {
            return angleNeg;
        }
        else if (m_PlayerNumber % 4 == 3) //If player number 3 (out of 4) will have a low parabola
        {
            return angleNeg;
        }
        else
        {
            return angleNeg;
        }
    }

    private Vector3 GetClosestTankAlivePosition()
    {
        float dist = -1;
        float mindist = 0;

        Vector3 closest = Vector3.zero;
        
        //Iterating Teams
        for (int i = 0; i < m_Manager.m_Teams.Length; i++)
        {
            //Making sure its not drom its own team
            if (i != m_TeamNumber - 1)
            {
                //Iterating Tanks inside Team
                for (int j = 0; j < m_Manager.m_Teams[i].m_Tanks.Length; j++)
                {
                    //Making sure that the closest tank is not themselves (redundant now)
                    if (i != m_PlayerNumber - 1)
                    {
                        //Making sure that the closest tank is not dead
                        if (!m_Manager.m_TanksDead[i])
                        {
                            //First iteration
                            if (dist == -1)
                            {
                                mindist = dist = Vector3.Distance(m_Manager.m_TanksPosition[i], gameObject.transform.position);
                                closest = m_Manager.m_TanksPosition[i];
                            }
                            else
                            {
                                dist = Vector3.Distance(m_Manager.m_TanksPosition[i], gameObject.transform.position);

                                if (dist < mindist)
                                {
                                    mindist = dist;
                                    closest = m_Manager.m_TanksPosition[i];
                                }
                            }
                        }
                    }
                }
            }
        }

        return closest;
    }
}