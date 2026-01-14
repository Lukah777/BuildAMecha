using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public enum EquipmentSlot {head, chest, sholder1, sholder2, core, weapon1, weapon2, legs}
public enum Dir { N, S, E, W }

public class Equipment : MonoBehaviour
{
    [SerializeField] private string m_name;
    [SerializeField] private string m_description;
    [SerializeField] private List<EquipmentSlot> m_equipmentSlot = new List<EquipmentSlot>();
    [SerializeField] private int m_shield = 0;
    [SerializeField] private int m_armor = 0;
    [SerializeField] private int m_dodge = 0;
    [SerializeField] private int m_energyDmg = 0;
    [SerializeField] private int m_phisicalDmg = 0;
    [SerializeField] private int m_range = 0;
    [SerializeField] private int m_moves = 0;
    [SerializeField] private int m_maxCooldown = 0;
    [SerializeField] private int m_cooldown = 0;
    [SerializeField] private int m_accuracy = 0;
    [SerializeField] private bool m_twoHanded = false;

    private Equipment(string name, string description, List<EquipmentSlot> equipmentSlot, int shield, int armor, int dodge, int energyDmg, int phisicalDmg, int range, int moves, int maxCooldown, int cooldown, int accuracy, bool twoHanded)
    {
        m_name = name;
        m_description = description;
        m_equipmentSlot = equipmentSlot;
        m_shield = shield;
        m_armor = armor;
        m_dodge = dodge;
        m_energyDmg = energyDmg;
        m_phisicalDmg = phisicalDmg;
        m_range = range;
        m_moves = moves;
        m_maxCooldown = maxCooldown;
        m_cooldown = cooldown;
        m_accuracy = accuracy;
        m_twoHanded = twoHanded;
    }

    private void Start()
    {
        m_cooldown = 0;
    }

    public virtual bool Activate(GameObject origin, GameObject target) // acts as a weapon
    {
        int rand = UnityEngine.Random.Range(0, 100);
        TMP_Text message = origin.GetComponent<CharController>().GetMessage();
        CharController targetCharController = target.GetComponent<CharController>();
        if (!targetCharController)
        {
            return false;
        }

        if (rand <= m_accuracy + origin.GetComponent<CharController>().GetAccuracy())
        {
            if (origin.name == "You")
            {
                message.text = origin.name + " attack " + target.name;
            }
           else
                message.text = origin.name + " attacks " + target.name;


            Vector2 direction = (target.transform.position - origin.transform.position).normalized;
            RaycastHit2D[] hits = Physics2D.RaycastAll(origin.transform.position, direction, Vector3.Distance(target.transform.position, origin.transform.position));


            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider != null)
                {
                    if (hit.collider.CompareTag("Wall"))
                    {
                        message.text = origin.name + " can not hit " + target.name;
                        return false;
                    }
                    else if (hit.collider.CompareTag("Cover"))
                    {
                        Cover cover = hit.collider.GetComponent<Cover>();
                        int rand2 = UnityEngine.Random.Range(0, 100);
                        if (rand2 !<= m_accuracy + origin.GetComponent<CharController>().GetAccuracy() - cover.GetCoverPercent() && cover.transform.position != origin.transform.position)
                        {
                            message.text = origin.name + " hit the " + cover.GetName();
                            cover.UpdateHealth(m_energyDmg + m_phisicalDmg, targetCharController);
                            return false;
                        }
                    }
                }
            }

            SetCooldown();

            double yDist = Mathf.Abs(origin.transform.position.y - target.transform.position.y);
            double xDist = Mathf.Abs(origin.transform.position.x - target.transform.position.x);

            

            if (yDist > xDist)
            {
                if (origin.transform.position.y > target.transform.position.y)
                {
                    targetCharController.Damage(Dir.N, m_energyDmg, m_phisicalDmg);
                }
                else if (origin.transform.position.y < target.transform.position.y)
                {
                    targetCharController.Damage(Dir.S, m_energyDmg, m_phisicalDmg);
                }
            }
            else
            {
                if (origin.transform.position.x > target.transform.position.x)
                {
                    targetCharController.Damage(Dir.E, m_energyDmg, m_phisicalDmg);
                }
                else if (origin.transform.position.x < target.transform.position.x)
                {
                    targetCharController.Damage(Dir.W, m_energyDmg, m_phisicalDmg);
                }
            }

            if (origin.transform.position.x > target.transform.position.x)
            {
                origin.transform.rotation = new Quaternion(0, 0, 0, 0);
            }
            else if (origin.transform.position.x < target.transform.position.x)
            {
                origin.transform.rotation = new Quaternion(0, 180, 0, 0);
            }
        }
        else
            message.text = origin.name + " missed.";

        return true;
    }

    public Equipment Clone()
    {
        Equipment clone = Instantiate(gameObject, new Vector3(-10f, -10f, -10f),transform.rotation).GetComponent<Equipment>();
        return clone;
    }

    public void TickCooldown() { m_cooldown--; }

    // Getters
    public string GetName() { return m_name; }
    public string GetDisc() { return m_description; }
    public List<EquipmentSlot> GetEquipmentSlot() { return m_equipmentSlot; }
    public int GetShield() { return m_shield; }
    public int GetArmor() { return m_armor; }
    public int GetDodge() { return m_dodge; }
    public int GetEnergyDmg() { return m_energyDmg; }
    public int GetPhisicalDmg() { return m_phisicalDmg; }
    public int GetRange() { return m_range; }
    public int GetMoves() { return m_moves; }
    public int GetCooldown() { return m_cooldown; }
    public int GetAccuracy() { return m_accuracy; }
    public bool GetTwoHanded() { return m_twoHanded; }

    // Setters
    public void SetCooldown(int cooldown = -1) 
    {
        if (cooldown != -1)
            m_cooldown = cooldown;
        else
            m_cooldown = m_maxCooldown;
    }
}
