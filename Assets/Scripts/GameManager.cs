using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.UI.Image;

public class GameManager : MonoBehaviour
{
    [SerializeField] private List<CharController> m_units;
    [SerializeField] private bool m_nextUnit = false;
    [SerializeField] private int m_unitIndex = 0;
    [SerializeField] private UIManager m_UIManager;
    [SerializeField] private float m_actionDelay = 0.5f;
    [SerializeField] private CharController m_player;

    [SerializeField] private GameObject[] m_reactors = new GameObject[4];

    private bool m_wait = false;

    private void Start()
    {
        CharController[] units = FindObjectsByType<CharController>(FindObjectsSortMode.None);
        foreach (CharController unit in units)
        {
            m_units.Add(unit);
        }
    }

    private void Update()
    {
        if (!m_wait)
        {
            if (m_nextUnit)
            {                
                m_unitIndex++;
                if (m_unitIndex == m_units.Count)
                {
                    m_UIManager.TickCooldowns();
                    m_unitIndex = 0;
                }
                m_units[m_unitIndex].Reset();
                m_nextUnit = false;
            }

            if (m_units[m_unitIndex] != null)
            {
                double yDist = Mathf.Abs(m_units[m_unitIndex].transform.position.y - m_player.transform.position.y);
                double xDist = Mathf.Abs(m_units[m_unitIndex].transform.position.x - m_player.transform.position.x);

                if (yDist <= 16 && xDist <= 16)
                {
                    if (!m_units[m_unitIndex].Turn())
                    { 
                        m_nextUnit = true; 
                        return;
                    }
                }
                else
                {
                    m_nextUnit = true;
                    return;
                }

                if (m_units[m_unitIndex].name != "You")
                    StartCoroutine(DelayedAction());
            }
            else
                m_nextUnit = true;
        }

        int count = 0;
        foreach (GameObject reactor in m_reactors)
        {
            if (reactor == null)
                count++;
        }

        if (count == 4)
        {
            SceneManager.LoadScene("TheEnd", LoadSceneMode.Single);
        }
    }

    IEnumerator DelayedAction()
    {
        m_wait = true;
        yield return new WaitForSeconds(m_actionDelay);
        m_wait = false;
    }
}
