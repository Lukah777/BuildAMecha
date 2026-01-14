using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class CharController : MonoBehaviour
{
    [SerializeField] protected float m_moveSpeed = 2f;
    protected Vector3 m_moveLocation;
    [SerializeField] private LayerMask m_layerMask;
    [SerializeField] protected Animator m_animator;
    [SerializeField] protected Animator m_effects;

    [SerializeField] private TMP_Text m_message;
    [SerializeField] private Sprite m_icon;

    [SerializeField] protected int m_health = 10;
    [SerializeField] protected int m_maxHealth = 10;
    [SerializeField] protected int m_maxShield = 0;
    [SerializeField] protected Dictionary<Dir, int> m_allShields = new Dictionary<Dir, int>();

    [SerializeField] protected int m_moves = 3;
    [SerializeField] protected int m_OGMoves = 3;
    [SerializeField] protected int m_maxMoves = 3;
    [SerializeField] protected int m_actions = 3;
    [SerializeField] protected int m_maxActions = 3;

    [SerializeField] protected float m_upgradePoints = 0;
    [SerializeField] protected List<Equipment> m_unlockedEquipment = new List<Equipment>();

    [SerializeField] protected GameObject m_headPos, m_chestPos, m_sholder1Pos, m_sholder2Pos, m_corePos, m_weapon1Pos, m_weapon2Pos, m_legsPos;
    [SerializeField] protected Equipment m_head, m_chest, m_sholder1, m_sholder2, m_core, m_weapon1, m_weapon2, m_legs;
    [SerializeField] protected GameObject m_shieldN, m_shieldS, m_shieldE, m_shieldW;
    [SerializeField] protected GameObject[] m_ranges = new GameObject[5];
    [SerializeField] protected List<SpriteRenderer> m_rangeRenderers = new List<SpriteRenderer>();

    [SerializeField] protected GameObject m_inventory;

    [SerializeField] protected Equipment m_empty;
    [SerializeField] protected CharController m_char;

    [SerializeField] protected Transform m_walls;

    [SerializeField] protected AudioSource m_audioSource;
    [SerializeField] protected AudioSource m_audioSource2;

    [SerializeField] protected AudioClip m_destroy;
    [SerializeField] protected AudioClip m_walk;
    [SerializeField] protected AudioClip m_melee;
    [SerializeField] protected AudioClip m_rifle;
    [SerializeField] protected AudioClip m_lazer;
    [SerializeField] protected AudioClip m_rail;
    [SerializeField] protected AudioClip m_missle;


    protected List<GameObject> m_wallsList = new List<GameObject>();

    private bool m_weapon1Aiming = false;
    private bool m_weapon2Aiming = false;
    private bool m_ability1Aiming = false;
    private bool m_ability2Aiming = false;
    private bool m_ultAiming = false;

    protected bool m_turn = true;

    private void Start()
    {
        m_rangeRenderers.Clear();
        if (m_ranges[0] != null)
        {
            foreach (GameObject range in m_ranges)
            {
                SpriteRenderer[] spriteRenderers = range.GetComponentsInChildren<SpriteRenderer>();
                foreach (SpriteRenderer spriteRenderer in spriteRenderers)
                {
                    if (spriteRenderer != null)
                        m_rangeRenderers.Add(spriteRenderer);
                }
            }
        }
       
        for (int i = 0; i < m_walls.childCount; i++)
        {
            m_wallsList.Add(m_walls.GetChild(i).gameObject);
        }
        m_char = GameObject.Find("You").GetComponent<CharController>();
        m_moveLocation = transform.position;
        UpdateStats();

        if (m_head != null && m_head != m_empty)
            Instantiate(m_head, m_inventory.transform);
        if (m_chest != null || m_chest != m_empty)
            Instantiate(m_chest, m_inventory.transform);
        if (m_sholder1 != null || m_sholder1 != m_empty)
            Instantiate(m_sholder1, m_inventory.transform);
        if (m_sholder2 != null || m_sholder2 != m_empty)
            Instantiate(m_sholder2, m_inventory.transform);
        if (m_core != null && m_core != m_empty)
            Instantiate(m_core, m_inventory.transform);
        if (m_weapon1 != null && m_weapon1 != m_empty)
            Instantiate(m_weapon1, m_inventory.transform);
        if (m_weapon2 != null && m_weapon2 != m_empty)
            Instantiate(m_weapon2, m_inventory.transform);
        if (m_legs != null && m_legs != m_empty)
            Instantiate(m_legs, m_inventory.transform);

    }

    public void UpdateStats()
    {
        // down allow equiping resets
        m_maxMoves = GetMoves();
        m_maxShield = GetShield();

        if (m_chest.GetShield() > 0 && !m_allShields.ContainsKey(Dir.N))
            m_allShields.Add(Dir.N, m_chest.GetShield());
        else if (m_chest.GetShield() == 0 && m_allShields.ContainsKey(Dir.N))
            m_allShields.Remove(Dir.N);

        if (m_core.GetShield() > 0 && !m_allShields.ContainsKey(Dir.S))
            m_allShields.Add(Dir.S, m_core.GetShield());
        else if (m_core.GetShield() == 0 && m_allShields.ContainsKey(Dir.S))
            m_allShields.Remove(Dir.S);

        if (m_sholder1.GetShield() > 0 && !m_allShields.ContainsKey(Dir.W))
            m_allShields.Add(Dir.W, m_sholder1.GetShield());
        else if (m_sholder1.GetShield() == 0 && m_allShields.ContainsKey(Dir.W))
            m_allShields.Remove(Dir.W);

        if (m_sholder2.GetShield() > 0 && !m_allShields.ContainsKey(Dir.E))
            m_allShields.Add(Dir.E, m_sholder2.GetShield());
        else if (m_sholder2.GetShield() == 0 && m_allShields.ContainsKey(Dir.E))
            m_allShields.Remove(Dir.E);


        m_headPos.GetComponent<SpriteRenderer>().sprite = m_head.GetComponent<SpriteRenderer>().sprite;
        m_chestPos.GetComponent<SpriteRenderer>().sprite = m_chest.GetComponent<SpriteRenderer>().sprite;
        m_sholder1Pos.GetComponent<SpriteRenderer>().sprite = m_sholder1.GetComponent<SpriteRenderer>().sprite;
        m_sholder2Pos.GetComponent<SpriteRenderer>().sprite = m_sholder2.GetComponent<SpriteRenderer>().sprite;
        m_corePos.GetComponent<SpriteRenderer>().sprite = m_core.GetComponent<SpriteRenderer>().sprite;
        m_weapon1Pos.GetComponent<SpriteRenderer>().sprite = m_weapon1.GetComponent<SpriteRenderer>().sprite;
        m_weapon2Pos.GetComponent<SpriteRenderer>().sprite = m_weapon2.GetComponent<SpriteRenderer>().sprite;
        m_legsPos.GetComponent<SpriteRenderer>().sprite = m_legs.GetComponent<SpriteRenderer>().sprite;
    }

    IEnumerator DestroyDelay(int time)
    {
        yield return new WaitForSeconds(time);
        m_audioSource.Play();
        if (gameObject.name == "You")
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        else
        {
            m_char.AddUpgradePoints(0.5f);
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (m_health <= 0)
        {
            m_audioSource.clip = m_destroy;
            StartCoroutine(DestroyDelay(1));
        }

        if (m_moveLocation != new Vector3(-1000,-1000, -1000))
        {
            m_animator.SetBool("Walking", true);
            float step = m_moveSpeed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, m_moveLocation, step);
            if (transform.position == m_moveLocation)
            {
                m_moveLocation = new Vector3(-1000, -1000, -1000);
                m_animator.SetBool("Walking", false);
                m_audioSource.Stop();
            }
        }

        if (m_allShields.ContainsKey(Dir.N))
        {
            if (m_allShields[Dir.N] > 0)
                m_shieldN.SetActive(true);
            else
                m_shieldN.SetActive(false);
        }
        else
            m_shieldN.SetActive(false);

        if (m_allShields.ContainsKey(Dir.S))
        {
            if (m_allShields[Dir.S] > 0)
                m_shieldS.SetActive(true);
            else
                m_shieldS.SetActive(false);
        }
        else
            m_shieldS.SetActive(false);

        if (m_allShields.ContainsKey(Dir.E))
        {
            if (m_allShields[Dir.E] > 0)
                m_shieldE.SetActive(true);
            else
                m_shieldE.SetActive(false);
        }
        else
            m_shieldE.SetActive(false);

        if (m_allShields.ContainsKey(Dir.W))
        {
            if (m_allShields[Dir.W] > 0)
                m_shieldW.SetActive(true);
            else
                m_shieldW.SetActive(false);
        }
        else
            m_shieldW.SetActive(false);
    }

    // Character Turn
    public virtual bool Turn()
    {
        foreach (GameObject range in m_ranges)
        {
            range.SetActive(false);
        }
        foreach (SpriteRenderer renderer in m_rangeRenderers)
        {
            renderer.color = new Color(0, 255, 0, .25f);
        }
        for (int i = 0; i < m_moves; i++)
        {
            m_ranges[i].SetActive(true);
        }

        if (m_moves > 0 && !m_weapon1Aiming && !m_weapon2Aiming && !m_ability1Aiming && !m_ability2Aiming && !m_ultAiming && m_moveLocation == new Vector3(-1000, -1000, -1000))
        {
            GameObject location = GetRange(ref m_moves);
            if (location != null)
            {
                m_moveLocation = location.transform.position;
                m_audioSource.clip = m_walk;
                m_audioSource.Play();
            }
        }

        if (m_actions > 0)
        {
            int range = 0;
            Equipment currentEquipment = null;
            if (m_weapon1Aiming)
            {
                range = m_weapon1.GetRange();
                currentEquipment = m_weapon1;
            }
            else if (m_weapon2Aiming)
            {
                range = m_weapon2.GetRange();
                currentEquipment = m_weapon2;
            }
            else if (m_ability1Aiming)
            {
                range = m_sholder1.GetRange();
                currentEquipment = m_sholder1;
            }
            else if (m_ability2Aiming)
            {
                range = m_sholder2.GetRange();
                currentEquipment = m_sholder2;
            }
            else if (m_ultAiming)
            {
                range = m_core.GetRange();
                currentEquipment = m_core;
            }
            else 
            {
                return m_turn;
            }

            if (currentEquipment.GetRange() == 0)
            {
                StartCoroutine(Attack(currentEquipment, gameObject));
            }
            else
            {
                foreach (GameObject _range in m_ranges)
                {
                    _range.SetActive(false);
                }
                foreach (SpriteRenderer renderer in m_rangeRenderers)
                {
                    renderer.color = new Color(255, 0, 0, .25f);
                }
                for (int i = 0; i < range; i++)
                {
                    m_ranges[i].SetActive(true);
                }
                GameObject target = GetRange(ref range, true);
                if (target != null)
                {
                    StartCoroutine(Attack(currentEquipment, target));
                }
            }
           
        }
        return m_turn;
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
        m_weapon1Aiming = false;
        m_weapon2Aiming = false;
        m_ability1Aiming = false;
        m_ability2Aiming = false;
        m_ultAiming = false;

        yield return new WaitForSeconds(1f);
        m_audioSource.Stop();
        
    }

    private GameObject GetRange(ref int tick, bool attacking = false)
    {
        if (Input.GetButtonDown("Fire1") && !EventSystem.current.IsPointerOverGameObject())
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D[] hits = Physics2D.RaycastAll(ray.origin, ray.direction, 100f, m_layerMask);

            bool inRange = false;
            bool targetHit = false;
            RaycastHit2D rangeHit = new RaycastHit2D();
            GameObject target = null;

            foreach (RaycastHit2D hit in hits)
            { 
                if (hit.collider.transform.parent.tag == "Range")
                {
                    inRange = true;
                    rangeHit = hit;
                }

                if (hit.collider.transform.tag == "Wall")
                {
                    return null;
                }

                if (hit.collider.transform.parent.tag == "Enemy")
                {
                    targetHit = true;
                    target = hit.transform.parent.gameObject;
                }
            }

            if (inRange)
            {
                if (attacking && target != null)
                {
                    Vector2 direction = (target.transform.position - transform.position).normalized;
                    RaycastHit2D[] hits2 = Physics2D.RaycastAll(transform.position, direction, Vector3.Distance(target.transform.position, transform.position));

                    foreach(RaycastHit2D hit in hits2)
                    {
                        if (hit.collider != null)
                        {
                            if (hit.collider.CompareTag("Wall"))
                            {
                                m_message.text = name + " can not hit the target";
                                return null;
                            }
                        }
                    }


                    return target;
                }
                else if (!targetHit)
                {
                    
                    Vector2 direction = (rangeHit.collider.transform.position - transform.position).normalized;
                    RaycastHit2D[] hits3 = Physics2D.RaycastAll(transform.position, direction, Vector3.Distance(rangeHit.collider.transform.position, transform.position));

                    foreach (RaycastHit2D hit in hits3)
                    {
                        if (hit.collider != null)
                        {
                            if (hit.collider.CompareTag("Wall") || hit.collider.CompareTag("Water"))
                            {
                                m_message.text = name + " can not move directly to there";
                                return null;
                            }
                        }
                    }
                    Transform parentTransform = rangeHit.collider.transform.parent;

                    if (parentTransform.tag == "Range")
                    {
                        if (parentTransform.name == "Range1")
                        {
                            tick -= 1;
                        }
                        else if (parentTransform.name == "Range2")
                        {
                            tick -= 2;
                        }
                        else if (parentTransform.name == "Range3")
                        {
                            tick -= 3;
                        }
                        else if (parentTransform.name == "Range4")
                        {
                            tick -= 4;
                        }
                        else if (parentTransform.name == "Range5")
                        {
                            tick -= 5;
                        }
                        return rangeHit.transform.gameObject;
                    }
                }
            }
        }
        return null;
    }

    public void Reset()
    {
        m_message.text = "";
        m_moves = m_maxMoves;
        m_actions = m_maxActions;
        foreach (var shield in m_allShields.ToList())
        {
            m_allShields[shield.Key] = 1;
        }
        m_turn = true;
    }

    // Getters
    public TMP_Text GetMessage() { return m_message; }
    public Sprite GetIcon() { return m_icon; }
    public int GetHealth() { return m_health; }
    public int GetMaxHealth() { return m_maxHealth; }
    public int GetMoves() { return (m_OGMoves + m_head.GetMoves() + m_chest.GetMoves() + m_core.GetMoves() + m_sholder1.GetMoves() + m_sholder2.GetMoves() + m_core.GetMoves() + m_weapon1.GetMoves() + m_weapon2.GetMoves() + m_legs.GetMoves()); }
    public int GetCurrentMoves() { return m_moves; }
    public int GetMaxMoves() { return  m_maxMoves; }
    public int GetActions() { return m_actions; }
    public int GetMaxActions() { return m_maxActions; }
    public int GetShield() { return (m_head.GetShield() + m_chest.GetShield() + m_core.GetShield() + m_sholder1.GetShield() + m_sholder2.GetShield() + m_core.GetShield() + m_weapon1.GetShield() + m_weapon2.GetShield() + m_legs.GetShield()); }
    public int GetCurrentShields()
    {
        int totalshield = 0;
        foreach (var shield in m_allShields)
        {
            totalshield += shield.Value;
        }
        return totalshield;
    }
    public int GetMaxShield() { return m_maxShield; }
    public int GetArmor() { return (m_head.GetArmor() + m_chest.GetArmor() + m_core.GetArmor() + m_sholder1.GetArmor() + m_sholder2.GetArmor() + m_core.GetArmor() + m_weapon1.GetArmor() + m_weapon2.GetArmor() + m_legs.GetArmor()); }
    public int GetDodge() { return (m_head.GetDodge() + m_chest.GetDodge() + m_core.GetDodge() + m_sholder1.GetDodge() + m_sholder2.GetDodge() + m_core.GetDodge() + m_weapon1.GetDodge() + m_weapon2.GetDodge() + m_legs.GetDodge()); }
    public int GetCooldown() { return (m_head.GetCooldown() + m_chest.GetCooldown() + m_core.GetCooldown() + m_legs.GetCooldown()); }
    public int GetAccuracy() 
    {
        int total = 0;
        if (m_head != m_empty)
            total += m_head.GetAccuracy();
        if (m_chest != m_empty)
            total += m_chest.GetAccuracy();
        if (m_core != m_empty)
            total += m_core.GetAccuracy();
        if (m_legs != m_empty)
            total += m_legs.GetAccuracy();
        return total; 
    }
    public Equipment GetWeapon1() { return m_weapon1; }
    public Equipment GetWeapon2() { return m_weapon2; }
    public Equipment GetSholder1() { return m_sholder1; }
    public Equipment GetSholder2 () { return m_sholder2; }
    public Equipment GetCore() { return m_core; }
    public float GetUpgradePoints() { return m_upgradePoints; }


    public void AddUpgradePoints(float upgradePoints) { m_upgradePoints += upgradePoints; }
    public void UseUpgradePoint() { m_upgradePoints -= 1f; }
    public void EndTurn() 
    {
        m_audioSource2.Play();
        m_turn = false; 
    } 

    // Setters
    public void SetTurn(bool turn) { m_turn = turn; }

    public List<Equipment> GetEquipment() { return m_unlockedEquipment; }
    public void AddEquipment(Equipment equipment) { m_unlockedEquipment.Add(equipment); }

    public void SetHead(Equipment equipment) { SetEquipment(ref m_head, equipment); }
    public void SetChest(Equipment equipment) { SetEquipment(ref m_chest, equipment); }
    public void SetSholder1(Equipment equipment) { SetEquipment(ref m_sholder1, equipment); }
    public void SetSholder2(Equipment equipment) { SetEquipment(ref m_sholder2, equipment);   }
    public void SetCore(Equipment equipment) { SetEquipment(ref m_core, equipment); }
    public void SetWeapon1(Equipment equipment) { SetEquipment(ref m_weapon1, equipment); }
    public void SetWeapon2(Equipment equipment) { SetEquipment(ref m_weapon2, equipment); }
    public void SetLegs(Equipment equipment) { SetEquipment(ref m_legs, equipment); }

    private void SetEquipment(ref Equipment equipmentSlot ,Equipment equipment)
    {
        if (equipment == m_empty || equipment == null)
        {
            equipmentSlot = m_empty;
        }
        else
        {
            bool haveEquipment =  false;
            GameObject equ;
            Equipment[] inv = m_inventory.GetComponentsInChildren<Equipment>();
            foreach (Equipment item in inv)
            { 
                if (item.GetName() == equipment.GetName())
                {
                    equipmentSlot = item;
                    haveEquipment = true;
                    break; 
                }
            }
            if (!haveEquipment)
            {
                equ = Instantiate(equipment.gameObject, m_inventory.transform);
                equipmentSlot = equ.GetComponent<Equipment>();
            }
        }
    }

    public void Heal()
    {
        m_audioSource2.Play();
        if (m_upgradePoints >= 1f)
        {
            m_upgradePoints -= 1f;
            m_health = m_maxHealth;
            m_message.text = "You have repaired yourself to full health.";
        }
        else
            m_message.text = "You do not have enough upgrade points to heal.";
    }

    public void Damage(Dir dir, int energyDmg = 0, int physicalDmg = 0)
    {
        int rand = UnityEngine.Random.Range(1, 101);
        if (rand <= GetDodge())
        {
            if (name == "You")
            {
                m_message.text += " and you dodged.";
            }
            else
                m_message.text += " and they dodged.";
            return;
        }

        int damge = 0;

        if (physicalDmg > 1)
            physicalDmg -= GetArmor();

        if (m_allShields.Count > 0)
        {
            if (m_allShields.ContainsKey(dir))
            {
                if (energyDmg > 0)
                {
                    m_allShields[dir] -= energyDmg * 2;
                }
                if ( physicalDmg > 0)
                {
                    m_allShields[dir] -= physicalDmg;
                }

                if (m_allShields[dir] < 0)
                {
                    damge = Mathf.Abs(m_allShields[dir]);
                    m_allShields[dir] = 0;
                }
            }
        }
        else
            damge = damge + energyDmg + physicalDmg;

         int totalDmg = (damge);
        if (totalDmg > 0)
            m_health -= totalDmg;

        if (name == "You")
        {
            m_message.text += " and you took " + totalDmg + " total damage.";
        }
        else
            m_message.text += " and they took " + totalDmg + " total damage.";
    }
    public void ToggleWeapon1Aiming()
    {
        m_audioSource2.Play();
        if (m_weapon1Aiming)
            m_weapon1Aiming = false;
        else
            m_weapon1Aiming = true;
        m_weapon2Aiming = false;
        m_ability1Aiming = false;
        m_ability2Aiming = false;
        m_ultAiming = false;
    }
    public void ToggleWeapon2Aiming()
    {
        m_audioSource2.Play();
        if (m_weapon2Aiming)
            m_weapon2Aiming = false;
        else
            m_weapon2Aiming = true;
        m_weapon1Aiming = false;
        m_ability1Aiming = false;
        m_ability2Aiming = false;
        m_ultAiming = false;
    }
    public void ToggleAbility1Aiming()
    {
        m_audioSource2.Play();
        if (m_ability1Aiming)
            m_ability1Aiming = false;
        else
            m_ability1Aiming = true;
        m_weapon1Aiming = false;
        m_weapon2Aiming = false;
        m_ability2Aiming = false;
        m_ultAiming = false;
    }
    public void ToggleAbility2Aiming()
    {
        m_audioSource2.Play();
        if (m_ability2Aiming)
            m_ability2Aiming = false;
        else
            m_ability2Aiming = true;
        m_weapon1Aiming = false;
        m_weapon2Aiming = false;
        m_ability1Aiming = false;
        m_ultAiming = false;
    }
    public void ToggleUltAiming()
    {
        m_audioSource2.Play();
        if (m_ultAiming)
            m_ultAiming = false;
        else
            m_ultAiming = true;
        m_weapon1Aiming = false;
        m_weapon2Aiming = false;
        m_ability1Aiming = false;
        m_ability2Aiming = false;
    }
}
