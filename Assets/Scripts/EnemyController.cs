using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : CharController
{
    [SerializeField] private List<Equipment> m_weapons = new List<Equipment>();

    public override bool Turn()
    {
        if (m_actions > 0)
        {
            int rand = UnityEngine.Random.Range(0, m_weapons.Count);
            Equipment currentWeapon = m_weapons[rand];

            if (currentWeapon != null)
            {
                if (InRange(currentWeapon))
                {
                    StartCoroutine(Attack(currentWeapon, m_char.gameObject));  
                }
                else
                {
                    if (m_moves > 0)
                    {
                        Move();
                        return true;
                    }
                    else
                    {
                        m_audioSource.Stop();
                        return false;
                    }
                }
            }
        }
        if (m_actions == 0)
        {
            m_audioSource.Stop();
            return false;
        }

        return true;
    }

    private IEnumerator Attack(Equipment currentEquipment, GameObject target)
    {
        if (currentEquipment.GetTwoHanded() == true)
        {
            m_audioSource.clip = m_rail;
            m_effects.SetInteger("WeaponType", 4);
        }
        else if (currentEquipment.GetName() == "Missle Launcher")
        {
            m_audioSource.clip = m_missle;
            m_effects.SetInteger("WeaponType", 3);
        }
        else if (currentEquipment.GetName() == "Sword")
        {
            m_audioSource.clip = m_melee;
            m_effects.SetInteger("WeaponType", 0);
        }
        else if (currentEquipment.GetPhisicalDmg() > 0)
        {
            m_audioSource.clip = m_rifle;
            m_effects.SetInteger("WeaponType", 2);
        }
        else if (currentEquipment.GetEnergyDmg() > 0)
        {
            m_audioSource.clip = m_lazer;
            m_effects.SetInteger("WeaponType", 1);
        }
        m_audioSource.Play();


        if (currentEquipment.GetTwoHanded() == true)
            m_animator.SetTrigger("2HAttack");
        else if (currentEquipment == m_weapon1)
            m_animator.SetTrigger("W1Attack");
        else if (currentEquipment == m_weapon2)
            m_animator.SetTrigger("W2Attack");
        else if (currentEquipment == m_sholder1)
            m_animator.SetTrigger("S1Attack");
        else if (currentEquipment == m_sholder2)
            m_animator.SetTrigger("S2Attack");

        m_actions--;
        currentEquipment.Activate(gameObject, target);

        yield return new WaitForSeconds(1f);
    }

    private void Move()
    {
        if (m_moves > 0 && m_moveLocation == new Vector3(-1000, -1000, -1000))
        {
            Vector3 dir = new Vector3(0,0,0);
            if (m_char.transform.position.x > transform.position.x)
            {
                dir.x = 1;
            }
            else if (m_char.transform.position.x < transform.position.x)
            {
                dir.x = -1;
            }
            if (m_char.transform.position.y > transform.position.y)
            {
                dir.y = 1;
            }
            else if (m_char.transform.position.y < transform.position.y)
            {
                dir.y = -1;
            }

            bool XYMove = true;
            bool XMove = true;
            bool YMove = true;
            foreach (GameObject wall in m_wallsList)
            {
                if (wall.transform.position == transform.position + dir)
                {
                    XYMove = false;
                }
                if (wall.transform.position == new Vector3( transform.position.x + dir.x, transform.position.y, transform.position.z))
                {
                    XMove= false;
                }
                if (wall.transform.position == new Vector3(transform.position.x, transform.position.y + dir.y, transform.position.z))
                {
                    YMove = false;
                }
            }

            if (XYMove)
            {
                m_moveLocation = transform.position + dir;
            }
            else if (XMove)
            {
                m_moveLocation = new Vector3(transform.position.x + dir.x, transform.position.y, transform.position.z);
            }
            else if (YMove)
            {
                m_moveLocation = new Vector3(transform.position.x, transform.position.y + dir.y, transform.position.z);
            }
            else
            {

            }
            m_audioSource.clip = m_walk;
            m_audioSource.Play();
            m_moves--;
        }
    }

    private bool InRange(Equipment equipment)
    {
        if (Mathf.Abs((m_char.transform.position.x - transform.position.x) + (m_char.transform.position.y - transform.position.y)) <= equipment.GetRange())
        {
            return true;
        }
        return false;
    }
}
