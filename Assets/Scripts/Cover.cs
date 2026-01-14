using UnityEngine;

public class Cover : MonoBehaviour
{
    [SerializeField] private string m_name = "Cover";
    [SerializeField] private bool m_dstructable = false;
    [SerializeField] private bool m_hazardous = false;
    [SerializeField] private int m_health = 10;
    [SerializeField] private int m_coverPercent = 50;
    [SerializeField] private int m_eDmg = 2;
    [SerializeField] private int m_pDmg = 2;

    public string GetName() { return m_name; }
    public int GetCoverPercent() {  return m_coverPercent; }
    public void UpdateHealth(int dmg, CharController target) 
    {
        if (m_dstructable)
            m_health -= dmg;

        if (m_hazardous && m_health <= 0)
            target.Damage(Dir.N, m_eDmg, m_pDmg);
    }
}
