using System;
using UnityEngine;

[Serializable]
public class TeamManager
{
    public Color m_TeamColor;

    public Transform m_BasePosition;
    public Transform m_TankSpawnPoint;
    public Transform m_AmbulanceSpawnPoint;

    [HideInInspector] public int m_TeamNumber;
    [HideInInspector] public string m_ColoredTeamText;
    [HideInInspector] public GameObject m_Instance;
    [HideInInspector] public int m_Wins;
    [HideInInspector] public bool m_IsBaseDestroyed = false;

    //public GameObject m_BasePrefab;
    //public GameObject m_TankPrefab;
    public TankManager[] m_Tanks;
    //public AmbulanceManager[] m_Ambulance;

    private TankManager m_TankManager;

    private GameObject m_CanvasGameObject;
    
    public void Setup()
    {
        //m_Movement = m_Instance.GetComponent<TankMovement>();
        //m_Shooting = m_Instance.GetComponent<TankShooting>();
        //m_Health = m_Instance.GetComponent<TankHealth>();
        //m_CanvasGameObject = m_Instance.GetComponentInChildren<Canvas>().gameObject;
        
        //m_TankManager.m_PlayerColor = m_TeamColor;
        //m_Movement.m_PlayerNumber = m_PlayerNumber;
        //m_Movement.m_PlayerColor = m_PlayerColor;

        //m_Shooting.m_PlayerNumber = m_PlayerNumber;
        //m_Shooting.m_PlayerColor = m_PlayerColor;

        //m_Health.m_PlayerNumber = m_PlayerNumber;
        //m_Health.m_PlayerColor = m_PlayerColor;

        m_ColoredTeamText = "<color=#" + ColorUtility.ToHtmlStringRGB(m_TeamColor) + ">TEAM " + m_TeamNumber + "</color>";

        MeshRenderer baserenderer = m_Instance.GetComponentInChildren<MeshRenderer>();

        //for (int i = 0; i < renderers.Length; i++)
        //{
            baserenderer/*[i]*/.material.color = m_TeamColor;
        //}
    }

    public void DisableControl()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].DisableControl();
        }
    }

    public void EnableControl()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].EnableControl();
        }
    }


    public void Reset()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].Reset();
        }
    }
}
