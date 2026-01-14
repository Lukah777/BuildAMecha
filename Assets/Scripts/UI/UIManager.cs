using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GraphicRaycaster m_graphicRaycaster;
    [SerializeField] private PointerEventData m_pointerEventData;
    [SerializeField] private EventSystem m_eventSystem;
    [SerializeField] private LayerMask m_layerMask;

    [SerializeField] private CharController m_char;
    [SerializeField] private CharController m_target;

    [SerializeField] private GameObject m_charInfo;
    [SerializeField] private TMP_Text m_charStats;
    [SerializeField] private TMP_Text m_weapon1Stats;
    [SerializeField] private TMP_Text m_weapon2Stats;
    [SerializeField] private TMP_Text m_sholder1Stats;
    [SerializeField] private TMP_Text m_sholder2Stats;
    [SerializeField] private TMP_Text m_coreStats;
    [SerializeField] private Image m_charIcon;
    [SerializeField] private RectTransform m_charHealthBar;
    [SerializeField] private Image m_targetIcon;
    [SerializeField] private RectTransform m_targetHealthBar;
    [SerializeField] private Button m_weapon1;
    [SerializeField] private Button m_weapon2;
    [SerializeField] private Button m_ability1;
    [SerializeField] private Button m_ability2;
    [SerializeField] private Button m_ult;

    [SerializeField] private GameObject m_hoverBox;
    [SerializeField] private List<Equipment> m_equipmentList = new List<Equipment>();

    [SerializeField] protected Equipment m_empty;

    [SerializeField] private AudioSource m_audioSource;

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D[] hits = Physics2D.RaycastAll(ray.origin, ray.direction, 100f, m_layerMask);

        m_target = null;
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null)
                if (hit.transform.parent.CompareTag("Enemy"))
                    m_target = hit.transform.parent.gameObject.GetComponent<CharController>();
        }

        m_pointerEventData = new PointerEventData(m_eventSystem);
        m_pointerEventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();

        m_graphicRaycaster.Raycast(m_pointerEventData, results);

        m_hoverBox.SetActive(false);
        foreach (RaycastResult result in results)
        {
            if (result.gameObject.CompareTag("Skill"))
            {
                m_hoverBox.SetActive(true);
                m_hoverBox.transform.position = new Vector3 (Input.mousePosition.x + 75, Input.mousePosition.y);
                foreach (Equipment equipment in m_equipmentList)
                {
                    if (equipment.GetName() == result.gameObject.name)
                    {
                        m_hoverBox.transform.GetChild(0).GetComponent<TMP_Text>().text = equipment.GetDisc();
                    }
                }
            }
        }

        //
        if (m_char)
        {
            if (m_char.GetMaxHealth() > 0)
                m_charHealthBar.sizeDelta = new Vector2(((float)m_char.GetHealth() / (float)m_char.GetMaxHealth()) * 400f, 25f);
            else
                m_charHealthBar.sizeDelta = new Vector2(0f, 25f);
        }
        if (m_target)
        {
            m_targetIcon.transform.parent.gameObject.SetActive(true);
            m_targetIcon.sprite = m_target.GetIcon();
            if (m_target.GetMaxHealth() > 0)
                m_targetHealthBar.sizeDelta = new Vector2(((float)m_target.GetHealth() / (float)m_target.GetMaxHealth()) * 400f, 25f);
            else
                m_targetHealthBar.sizeDelta = new Vector2(0f, 25f);
        }
        else if (!m_target)
        {
            m_targetIcon.transform.parent.gameObject.SetActive(false);
        }

        //
        m_charStats.text =
            "Health: " + m_char.GetHealth().ToString() + "/" + m_char.GetMaxHealth().ToString() + "\n" +
            "Armor: " + m_char.GetArmor().ToString() + "\n" +
            "Dodge Chance: " + m_char.GetDodge().ToString() + "/100 \n" +
            "Shield: " + m_char.GetCurrentShields().ToString() + "/" + m_char.GetMaxShield().ToString() + "\n \n" +

            "Upgrade Points: " + m_char.GetUpgradePoints().ToString() + "\n \n" +

            "Moves: " + m_char.GetCurrentMoves().ToString() + "/" + m_char.GetMaxMoves().ToString() + "\n" +
            "Actions: " + m_char.GetActions().ToString() + "/" + m_char.GetMaxActions().ToString() + "\n";

        m_weapon1Stats.text = "Weapon 1: \n" + EquipmentStats(m_char.GetWeapon1());
        m_weapon2Stats.text = "Weapon 2: \n" + EquipmentStats(m_char.GetWeapon2());
        m_sholder1Stats.text = "Sholder 1: \n" + EquipmentStats(m_char.GetSholder1());
        m_sholder2Stats.text = "Sholder 2: \n" + EquipmentStats(m_char.GetSholder2());
        m_coreStats.text = "Core: \n" + EquipmentStats(m_char.GetCore());

        //
        CheckCooldown(m_char.GetWeapon1(), m_weapon1);
        CheckCooldown(m_char.GetWeapon2(), m_weapon2);
        CheckCooldown(m_char.GetSholder1(), m_ability1);
        CheckCooldown(m_char.GetSholder2(), m_ability2); 
        CheckCooldown(m_char.GetCore(), m_ult);
    }

    private string EquipmentStats(Equipment equipment)
    {
        return
        equipment.GetName() + ": \n" +
        "Discription: " + equipment.GetDisc() + "\n" +
        "TwoHanded: " + equipment.GetTwoHanded().ToString() + "\n" +
        "Cooldown: " + (equipment.GetCooldown() + m_char.GetCooldown()).ToString() + "\n" +
        "Range: " + equipment.GetRange().ToString() + "\n" +
        "Accuracy: " + (equipment.GetAccuracy() + m_char.GetAccuracy()).ToString() + "/100 \n" +
        "Energy Damage: " + equipment.GetEnergyDmg().ToString() + "\n" +
        "Physical Damage: " + equipment.GetPhisicalDmg().ToString() + "\n \n";
    }

    private void CheckCooldown( Equipment equipment, Button button)
    {
        int cooldown = equipment.GetCooldown();
        if (cooldown <= 0)
        {
            button.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = equipment.GetName();
            button.interactable = true;
            equipment.SetCooldown(0);
        }
        else if (cooldown > 0)
        {
            button.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = cooldown.ToString();
            button.interactable = false;
        }
    }

    public void TickCooldowns()
    {
       m_char.GetWeapon1().TickCooldown();
       m_char.GetWeapon2().TickCooldown();
       m_char.GetSholder1().TickCooldown();
       m_char.GetSholder2().TickCooldown();
       m_char.GetCore().TickCooldown();
    }

    public void ToggleCharInfo()
    {
        m_audioSource.Play();
        if (m_charInfo.activeSelf)
            m_charInfo.SetActive(false);
        else
            m_charInfo.SetActive(true);
    }
}
