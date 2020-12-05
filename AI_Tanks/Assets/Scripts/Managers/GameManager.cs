using UnityEngine;
using System.Collections;
//using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int m_NumRoundsToWin = 5;        
    public float m_StartDelay = 3f;         
    public float m_EndDelay = 3f;           
    public CameraControl m_CameraControl; 
    
    public Text m_MessageText;              

    private GameObject[] m_Shells;

    public GameObject m_BasePrefab;
    public GameObject m_TankPrefab;

    public TeamManager[] m_Teams;

    public Vector3[] m_TanksPosition;
    public bool[] m_TanksDead;
    public int m_TotalTanks = 0;

    public int m_InitialTankShellsMagazine;
    public int m_TotalTankShellsMagazine;
    public int m_TankShellMagazineRechargeRate;

    private int m_RoundNumber;              
    private WaitForSeconds m_StartWait;     
    private WaitForSeconds m_EndWait;    
    
    private TeamManager m_RoundWinner;
    private TeamManager m_GameWinner;  


    private void Start()
    {
        m_StartWait = new WaitForSeconds(m_StartDelay);
        m_EndWait = new WaitForSeconds(m_EndDelay);

        //Initializing the position array with the number of tanks
        m_TotalTanks = CountAllTanks();

        m_TanksPosition = new Vector3[m_TotalTanks];
        m_TanksDead = new bool[m_TotalTanks];

        SpawnAllTeams();
        GetTanksPosition();
        SetCameraTargets();

        StartCoroutine(GameLoop());
    }

    private void Update()
    {
        GetTanksPosition();
        //GetTanksDead(); //Set from Tank health
    }

    private int CountAllTanks()
    {
        int TotalTanks = 0;

        for (int i = 0; i < m_Teams.Length; i++)
        {
            TotalTanks += m_Teams[i].m_Tanks.Length;
        }
        return TotalTanks;
    }

    private void SpawnAllTeams()
    {
        int iter = 0;
        //int ambulanceiter = 0;

        for (int i = 0; i < m_Teams.Length; i++)
        {
            m_Teams[i].m_Instance = Instantiate(m_BasePrefab, m_Teams[i].m_BasePosition.position, m_Teams[i].m_BasePosition.rotation) as GameObject;
            m_Teams[i].m_TeamNumber = i + 1;

            m_Teams[i].Setup();

            for (int j = 0; j < m_Teams[i].m_Tanks.Length; j++)
            {
                m_Teams[i].m_Tanks[j].m_Instance = Instantiate(m_TankPrefab, m_Teams[i].m_Tanks[j].m_SpawnPoint.position, m_Teams[i].m_Tanks[j].m_SpawnPoint.rotation) as GameObject;
                m_Teams[i].m_Tanks[j].m_PlayerNumber = iter + 1;
                m_Teams[i].m_Tanks[j].m_TeamNumber = i + 1;
                m_Teams[i].m_Tanks[j].m_PlayerColor = m_Teams[i].m_TeamColor;

                m_Teams[i].m_Tanks[j].Setup();

                iter++;
            }

            //Another "for" ambulances
        }
    }

    private void GetTanksPosition()
    {
        int iter = 0;

        //Fill array of vec3 positions for every tank on every team (m_PlayerNumber - i)
        for (int i = 0; i < m_Teams.Length; i++)
        {
            for (int j = 0; j < m_Teams[i].m_Tanks.Length; j++)
            {
                m_TanksPosition[iter] = m_Teams[i].m_Tanks[j].m_Instance.transform.position;
                iter++;
            }             
        }
    }

    private void SetCameraTargets()
    {
        Transform[] targets = new Transform[m_TotalTanks];
        int iter = 0;

        //Fill array of transforms for every tank on every team
        for (int i = 0; i < m_Teams.Length; i++)
        {
            for (int j = 0; j < m_Teams[i].m_Tanks.Length; j++)
            {
                targets[iter] = m_Teams[i].m_Tanks[j].m_Instance.transform;
                iter++;
            }  
        }

        m_CameraControl.m_Targets = targets;
    }


    private IEnumerator GameLoop()
    {
        yield return StartCoroutine(RoundStarting());
        yield return StartCoroutine(RoundPlaying());
        yield return StartCoroutine(RoundEnding());

       if (m_GameWinner != null)
        {
            Application.LoadLevel(Application.loadedLevel);
        }
        else
        {
            StartCoroutine(GameLoop());
        }
    }


    private IEnumerator RoundStarting()
    {
        ResetAllTeams();
        DisableTankControl();

        m_CameraControl.SetStartPositionAndSize();

        m_RoundNumber++;
        m_MessageText.text = "ROUND " + m_RoundNumber;

        yield return m_StartWait;
    }


    private IEnumerator RoundPlaying()
    {
        EnableTankControl();

        m_MessageText.text = string.Empty;

        while(!OneTeamLeft())
        {
            yield return null;
        }
    }


    private IEnumerator RoundEnding()
    {
        DisableTankControl();

        m_RoundWinner = null;
        m_RoundWinner = GetRoundWinner();

        if (m_RoundWinner != null)
        {
            m_RoundWinner.m_Wins++;
        }

        m_GameWinner = GetGameWinner();

        string message = EndMessage();
        m_MessageText.text = message;

        yield return m_EndWait;
    }


    private bool OneTeamLeft()
    {
        int numTeamsLeft = 0;

        for (int i = 0; i < m_Teams.Length; i++)
        {
            if (m_Teams[i].m_Instance.activeSelf)
                numTeamsLeft++;
        }

        return numTeamsLeft <= 1;
    }

    private TeamManager GetRoundWinner()
    {
        for (int i = 0; i < m_Teams.Length; i++)
        {
            if (m_Teams[i].m_Instance.activeSelf)
                return m_Teams[i];
        }

        return null;
    }


    private TeamManager GetGameWinner()
    {
        for (int i = 0; i < m_Teams.Length; i++)
        {
            if (m_Teams[i].m_Wins == m_NumRoundsToWin)
                return m_Teams[i];
        }

        return null;
    }


    private string EndMessage()
    {
        string message = "DRAW!";

        if (m_RoundWinner != null)
            message = m_RoundWinner.m_ColoredTeamText + " WINS THE ROUND!";

        message += "\n\n\n\n";

        for (int i = 0; i < m_Teams.Length; i++)
        {
            message += m_Teams[i].m_ColoredTeamText + ": " + m_Teams[i].m_Wins + " WINS\n";
        }

        if (m_GameWinner != null)
            message = m_GameWinner.m_ColoredTeamText + " WINS THE GAME!";

        return message;
    }

    private void ResetAllTeams()
    {
        //Reset
        for (int i = 0; i < m_Teams.Length; i++)
        {
            m_Teams[i].Reset();
        }

        int iter = 0;
        //Reset Death array bools
        for (int i = 0; i < m_Teams.Length; i++)
        {
            for (int j = 0; j < m_Teams[i].m_Tanks.Length; j++)
            {
                m_TanksDead[iter] = false;
                iter++;
            }
        }

        //Destroy all shells left
        m_Shells = GameObject.FindGameObjectsWithTag("Projectile");

        foreach (GameObject Projectile in m_Shells)
        {
            Destroy(Projectile);
        }
    }

    private void EnableTankControl()
    {
        for (int i = 0; i < m_Teams.Length; i++)
        {
            m_Teams[i].EnableControl();
        }
    }

    private void DisableTankControl()
    {
        for (int i = 0; i < m_Teams.Length; i++)
        {
            m_Teams[i].DisableControl();
        }
    }
}