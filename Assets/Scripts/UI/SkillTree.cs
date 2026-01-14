using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillTree : MonoBehaviour
{
    [SerializeField] private CharController m_charController;
    [SerializeField] private List<Equipment> m_equipmentList = new List<Equipment>(); 

    [SerializeField] private List<GameObject> m_buttons = new List<GameObject>();

    [SerializeField] private AudioSource m_audioSource;

    private bool HavePoints()
    {
        if (m_charController.GetUpgradePoints() >= 1.0f)
            return true;
        else
            return false;
    }

    private void Update()
    {
        if (HavePoints())
        {
            for (int i = 0; i < m_buttons.Count; i++)
            {
                Button button = m_buttons[i].GetComponent<Button>();
                Image image = m_buttons[i].GetComponent<Image>();

                if (image.color != Color.red)
                {
                    button.interactable = true;
                    image.color = Color.white;
                }
            }
        }
        else
        {
            for (int i = 0; i < m_buttons.Count; i++)
            {
                Button button = m_buttons[i].GetComponent<Button>();
                Image image = m_buttons[i].GetComponent<Image>();

                if (image.color != Color.red)
                {
                    button.interactable = false;
                    image.color = Color.gray;
                }
            }
        }
    }

    private void UnlockEquipment(int i)
    {
        m_audioSource.Play();
        m_charController.UseUpgradePoint();
        m_charController.AddEquipment(m_equipmentList[i]);
        m_buttons[i].GetComponent<Button>().interactable = false;
        m_buttons[i].GetComponent<Image>().color = Color.red;
    }

    public void UnlockDirectedShield()
    {
        UnlockEquipment(0);
    }

    public void UnlockBodyArmor()
    {
        UnlockEquipment(1);
    }
    public void UnlockReflexDodge()
    {
        UnlockEquipment(2);
    }
    public void UnlockLazerRifle()
    {
        UnlockEquipment(3);
    }
    public void UnlockRifle()
    {
        UnlockEquipment(4);
    }
    public void UnlockMissleLauncher()
    {
        UnlockEquipment(5);
    }
    public void UnlockSword()
    {
        UnlockEquipment(6);
    }
    public void UnlockRailRifle()
    {
        UnlockEquipment(7);
    }
    public void UnlockAutoTargeting()
    {
        UnlockEquipment(8);
    }
    public void UnlockMobilityFrame()
    {
        UnlockEquipment(9);
    }
    public void UnlockSuperCooling()
    {
        UnlockEquipment(10);
    }

    public List<Equipment> GetEquipmentList() { return m_equipmentList; }
}
