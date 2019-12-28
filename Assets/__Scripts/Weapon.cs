using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Перечисление всех возможных типов оружия
/// </summary>
public enum WeaponType
{
    none, // По умолчанию / нет оружия
    blaster, // Простой бластер
    spread, // Веерная пушка, стреляющая несколькими снарядами
    phaser, // Волновой лазер
    missile, // Самонаводящиеся ракеты
    laser, // Наносит повреждения при долговременном воздействии
    shield // Увеличивает shieldLevel
}
[System.Serializable]
public class WeaponDefinition
{
    public WeaponType type = WeaponType.none;
    public string letter; // Буква на кубике, изображающем бонус
    public Color color = Color.white; // Цвет ствола оружия и кубика бонуса
    public GameObject projectilePrefab; //Шаблон снарядов
    public Color projectileColor = Color.white;
    public float damageOnHit = 0; // Разрушительная мощность
    public float continuosDamage = 0; // Степень разрушеня в секунду
    public float delayBetweenShots = 0;
    public float velocity = 20;
}
public class Weapon : MonoBehaviour
{
    static public Transform PROJECTILE_ANCHOR;
    [Header("Set Dynamically")]
    [SerializeField]
    private WeaponType _type = WeaponType.none;
    public WeaponDefinition def;
    public GameObject collar;
    public float lastShotTime; // Время последнего выстрела
    private Renderer collarRend;
    // Start is called before the first frame update
    void Start()
    {
        collar = transform.Find("Collar").gameObject;
        collarRend = collar.GetComponent<Renderer>();
        // Вызвать SetType(), чтобы заменить тип оружия по умолчанию
        SetType(_type);
        // Создать точку привазки для всех снарядов
        if (PROJECTILE_ANCHOR == null)
        {
            GameObject go = new GameObject("_ProjectileAnchor");
            PROJECTILE_ANCHOR = go.transform;
        }
        // Найти fireDelegate в корневом игровом объекте
        GameObject rootGo = transform.root.gameObject;
        if (rootGo.GetComponent<Hero>() != null) rootGo.GetComponent<Hero>().fireDelegate += Fire;
    }
    public WeaponType type { get { return (_type); } set { SetType(value); } }
    public void SetType(WeaponType wt)
    {
        _type = wt;
        if (type == WeaponType.none)
        {
            this.gameObject.SetActive(false);
            return;
        }
        else this.gameObject.SetActive(true);
        def = Main.GetWeaponDefinition(_type);
        collarRend.material.color = def.color;
        lastShotTime = 0; // Позволяет сразу же выстрелить
    }
    public void Fire()
    {
        // Если this.gameObject неактивен, выйти
        if (!gameObject.activeInHierarchy) return;
        // Если между выстрелами прошло недостаточно много времени, выйти
        if (Time.time - lastShotTime < def.delayBetweenShots) return;
        Projectile p;
        Vector3 vel = Vector3.up * def.velocity;
        if (transform.up.y < 0) vel.y = -vel.y;
        switch(type)
        {
            case WeaponType.blaster:
                p = MakeProjectile();
                p.rigid.velocity = vel;
                break;
            case WeaponType.spread:
                p = MakeProjectile();// Снаряд, летящий прямо
                p.rigid.velocity = vel;
                p = MakeProjectile(); // Снаряд, летящий вправо
                p.transform.rotation = Quaternion.AngleAxis(10, Vector3.back);
                p.rigid.velocity = p.transform.rotation * vel;
                p = MakeProjectile(); // Снаряд, летящий влево
                p.transform.rotation = Quaternion.AngleAxis(-10, Vector3.back);
                p.rigid.velocity = p.transform.rotation * vel;
                p = MakeProjectile(); // 2 снаряд летящий вправо
                p.transform.rotation = Quaternion.AngleAxis(20, Vector3.back);
                p.rigid.velocity = p.transform.rotation * vel;
                p = MakeProjectile(); // 2 снаряд летящий влево
                p.transform.rotation = Quaternion.AngleAxis(-20, Vector3.back);
                p.rigid.velocity = p.transform.rotation * vel;
                break;
            case WeaponType.phaser:
                p = MakeProjectile(); // Первый снаряд
                p.rigid.velocity = vel;
                p.direction = 1;
                p = MakeProjectile(); // Второй снаряд
                p.rigid.velocity = vel;
                p.direction = -1;
                break;
            case WeaponType.laser:
                p = MakeProjectile();
                p.standartVel = vel;
                p.rigid.velocity = vel;
                break;
            case WeaponType.missile:
                if (GameObject.FindGameObjectsWithTag("Enemy").Length != 0)
                {
                    p = MakeProjectile();
                    p.standartVel = vel;
                    p.rigid.velocity = vel;
                }
                break;
        }
    }
    public Projectile MakeProjectile()
    {
        GameObject go = Instantiate<GameObject>(def.projectilePrefab);
        if (transform.parent.gameObject.tag == "Hero")
        {
            go.tag = "ProjectileHero";
            go.layer = LayerMask.NameToLayer("ProjectileHero");
        }
        else
        {
            go.tag = "ProjectileEnemy";
            go.layer = LayerMask.NameToLayer("ProjectileEnemy");
        }
        go.transform.position = collar.transform.position;
        go.transform.SetParent(PROJECTILE_ANCHOR, true);
        Projectile p = go.GetComponent<Projectile>();
        p.type = type;
        p.collar = collar;
        lastShotTime = Time.time;
        return (p);
    }
}
