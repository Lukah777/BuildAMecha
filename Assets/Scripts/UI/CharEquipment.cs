using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharEquipment : MonoBehaviour
{
    [SerializeField] private CharController m_char;
    [SerializeField] private EquipmentSlot m_equipmentSlot;
    [SerializeField] private List<Equipment> m_equipments;
    [SerializeField] private GameObject m_optionsPanel;
    [SerializeField] private GameObject m_prefab;
    [SerializeField] private Equipment m_chestEquipment;
    [SerializeField] private GameObject m_emptyEquipment;
    [SerializeField] private CharEquipment m_otherHand;

    [SerializeField] private AudioSource m_audioSource;

    private void Update()
    {
        List<Equipment> equipments = m_char.GetEquipment();
        foreach (Equipment equipment in equipments)
        {
            List<EquipmentSlot> slots = equipment.GetEquipmentSlot();
            foreach (EquipmentSlot slot in slots)
            {
                if (slot == m_equipmentSlot && !m_equipments.Contains(equipment))
                {
                    if (m_equipments.Contains(m_chestEquipment) && equipment.GetShield() > 0)
                        continue;

                    Instantiate(m_prefab, m_optionsPanel.transform);
                    Transform buttonTransform = m_optionsPanel.transform.GetChild(m_optionsPanel.transform.childCount - 1);
                    buttonTransform.GetChild(0).GetComponent<TMP_Text>().text = equipment.GetName();

                    if (equipment.GetShield() > 0 && m_equipmentSlot == EquipmentSlot.chest)
                        m_equipments.Add(m_chestEquipment);
                    else
                        m_equipments.Add(equipment);

                    int count = m_equipments.Count;
                    buttonTransform.GetComponent<Button>().onClick.AddListener(() => Equip(count - 1));

                    
                }
            }
        }
    }

    public void DisplayOptions()
    {
        m_audioSource.Play();
        m_optionsPanel.SetActive(true);
    }

    public void Equip(int slot = -1)
    {
        m_audioSource.Play();
        Equipment equipment = m_emptyEquipment.GetComponent<Equipment>();
        if (slot != -1)
        {
            equipment = m_equipments[slot];
            GetComponent<Image>().sprite = equipment.GetComponent<SpriteRenderer>().sprite;
        }
        else
        {
            GetComponent<Image>().sprite = m_emptyEquipment.GetComponent<SpriteRenderer>().sprite;
        }

        if (m_equipmentSlot == EquipmentSlot.head)
        {
            m_char.SetHead(equipment);
        }
        else if (m_equipmentSlot == EquipmentSlot.chest)
        {
            m_char.SetChest(equipment);
        }
        else if (m_equipmentSlot == EquipmentSlot.sholder1)
        {
            m_char.SetSholder1(equipment);
        }
        else if (m_equipmentSlot == EquipmentSlot.sholder2)
        {
            m_char.SetSholder2(equipment);
        }
        else if (m_equipmentSlot == EquipmentSlot.core)
        {
            m_char.SetCore(equipment);
        }
        else if (m_equipmentSlot == EquipmentSlot.weapon1)
        {
            if (equipment.GetTwoHanded() || m_char.GetWeapon2().GetTwoHanded())
                m_otherHand.Equip();

            m_char.SetWeapon1(equipment);
        }
        else if (m_equipmentSlot == EquipmentSlot.weapon2)
        {
            if (equipment.GetTwoHanded() || m_char.GetWeapon1().GetTwoHanded())
                m_otherHand.Equip();
            m_char.SetWeapon2(equipment);
        }
        else if (m_equipmentSlot == EquipmentSlot.legs)
        {
            m_char.SetLegs(equipment);
        }
        
        m_optionsPanel.SetActive(false);
        m_char.UpdateStats();
    }
}
