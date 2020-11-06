using UnityEngine;
using UnityEngine.UI;

public class TankShooting : MonoBehaviour
{
    public int m_PlayerNumber = 1;       
    public Rigidbody m_Shell;            
    public Transform m_FireTransform;    
    //public Slider m_AimSlider;           
    public AudioSource m_ShootingAudio;  
    //public AudioClip m_ChargingClip;     
    public AudioClip m_FireClip;         
    public GameManager m_manager;

    private string m_FireButton;         
    private float m_CurrentLaunchForce = 30f;          
    
    //Parabolic motion shooting
    public int m_RateOfFire = 1; //per second
    public bool m_ShootDelay;
    public float m_ShootingTimer;
    
    public float m_MaxRange;
    public float m_ShootingAngle;

    private void OnEnable()
    {
        //m_CurrentLaunchForce = m_MinLaunchForce;
        //m_AimSlider.value = m_MinLaunchForce;
    }


    private void Start()
    {
        m_FireButton = "Fire" + m_PlayerNumber;

        //m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime;
    }
    

    private void Update()
    {
        //Fire when enemy is in range

        //Fire Delay
        if (m_ShootingTimer < Time.time)
        {
            m_ShootDelay = false;
        }

        if (!m_ShootDelay)
        {
            //Fire();
            m_ShootDelay = true;
            m_ShootingTimer = Time.time + m_RateOfFire;
        }
    }


    private void Fire()
    {
        // Instantiate and launch the shell.
        Rigidbody shellInstance = Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;

        shellInstance.velocity = m_CurrentLaunchForce * m_FireTransform.forward;

        m_ShootingAudio.clip = m_FireClip;
        m_ShootingAudio.Play();
    }
}