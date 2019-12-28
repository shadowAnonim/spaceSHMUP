using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Set in Inspector: Enemy")]
    public float speed = 10f; // Скорость
    public float fireRate = 0.3f; // Время между выстрелами
    public float health = 10;
    public int score = 100; // Очки за уничтожение этого корабля
    public float showDamageDuration = 0.1f; // Длительность эффекта попадания в секундах
    public float powerUpDropChance = 1f; // Вероятность сбросить бонус
    [Header("SetDynamically: Enemy")]
    public Color[] originalColors;
    public Material[] materials; // Все материалы игрового объекта и его потомков
    public bool showingDamage = false;
    public float damageDoneTime; // Время прекращения отображения эффекта
    public bool notifiedOfDestruction = false;
    protected BoundsCheck bndCheck;
    protected Hero hero;
    private void Awake()
    {
        bndCheck = GetComponent<BoundsCheck>();
        hero = GameObject.Find("_Hero").GetComponent<Hero>();
        // Получить материалы и цвет этого игрового объекта и его потомков
        materials = Utils.GetAllMaterials(gameObject);
        originalColors = new Color[materials.Length];
        for (int i = 0; i < materials.Length; i++) originalColors[i] = materials[i].color;
    }
    public Vector3 pos { get { return (this.transform.position); } set { this.transform.position = value; } }
    // Update is called once per frame
    void Update()
    {
        Move();
        if (showingDamage && Time.time > damageDoneTime) UnShowDamage();
        // Корабль за нижней границей экрана, поэтому его нужно уничтожить
        if (bndCheck != null && bndCheck.offDown) Destroy(gameObject);
    }
    public virtual void Move()
    {
        Vector3 tempPos = pos;
        tempPos.y -= speed * Time.deltaTime;
        pos = tempPos;
    }
    private void OnCollisionEnter(Collision coll)
    {
        GameObject otherGO = coll.gameObject;
        switch (otherGO.tag)
        {
            case "ProjectileHero":
                Projectile p = otherGO.GetComponent<Projectile>();
                WeaponType type = p.type;
                // Если вражеский корабль за границами экрана, не наносить ему повреждений
                if (!bndCheck.isOnScreen)
                {
                    Destroy(otherGO);
                    break;
                }
                // Поразить вражеский корабль
                ShowDamage();
                // Получить разрущающую силу из WEAP_DICT в классе Main
                health -= Main.GetWeaponDefinition(p.type).damageOnHit + hero.fired * Main.GetWeaponDefinition(p.type).continuosDamage;
                if (health <= 0)
                {
                    hero.fired = 0;
                    // Сообщить Main об уничтожении
                    if (!notifiedOfDestruction) Main.S.ShipDestroyed(this);
                    notifiedOfDestruction = true;
                    // Уничтожить этот корабль
                    Destroy(this.gameObject);
                }
                Destroy(otherGO);
                break;
            default:
                print("Enemy hit by non-ProjectileHero: " + otherGO.name);
                break;
        }
    }
    void ShowDamage()
    {
        foreach (Material m in materials) m.color = Color.red;
        showingDamage = true;
        damageDoneTime = Time.time + showDamageDuration;
    }
    void UnShowDamage()
    {
        for (int i = 0; i < materials.Length; i++) materials[i].color = originalColors[i];
        showingDamage = false;
    }
}
