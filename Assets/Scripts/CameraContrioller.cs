using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float m_moveSpeed = 10f;
    [SerializeField] private Camera m_camera;
    [SerializeField] private GameObject m_player;

    private float m_x = 0, m_y = 0, m_OS = 5;

    // Detect keyboard input to move camera around the world
    void FixedUpdate()
    {
        Vector3 pos = m_player.transform.position;
        transform.position = new Vector3(pos.x + m_x, pos.y + m_y, transform.position.z);
        m_camera.orthographicSize = m_OS;

        if (Input.GetKey(KeyCode.W))
        {
            m_y += m_moveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            m_x -= m_moveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            m_y -= m_moveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
           m_x +=  m_moveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            m_OS += m_moveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.E))
        {
            m_OS -= m_moveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.R))
        {
            m_x = 0;
            m_y = 0;
            m_OS = 5;
        }

        if (m_camera.orthographicSize < 1)
        {
            m_camera.orthographicSize = 1;
        }
    }
}
