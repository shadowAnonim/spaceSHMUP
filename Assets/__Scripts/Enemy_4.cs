using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Part
{
    public string name; // Имя этой части
    public float health; // Степень стойкости этой части
    public string[] protectedBy; // Другие части, защищающие эту
    [HideInInspector]
    public GameObject go; // Игровой объект этой части
    [HideInInspector]
    public Material mat; // Материал для отображения повреждений
}
/// <summary>
/// Enemy_4 создается за верхней границей, выбирает случайную точку на экране и перемещается к ней. Добравшись до места выбирает другую случайную точку и продолжает двигаться, пока игрок не уничтожит его
/// </summary>
public class Enemy_4 : Enemy
{
    [Header("Set in Inspector: Enemy_4")]
    public Part[] parts; // Массив частей составляющих корабль
    private Vector3 p0, p1; // Две точки для интерполяции
    private float timeStart; // Время создания этого корабля
    private float duration = 4; // Продолжительность перемещения
    // Start is called before the first frame update
    void Start()
    {
        // Записать начальную позицию из Main.SpawnEnemy() в p0 и p1
        p0 = p1 = pos;
        InitMovement();
        // Записать игровой объект и материал каждой части в parts
        Transform t;
        foreach (Part prt in parts)
        {
            t = transform.Find(prt.name);
            if (t != null)
            {
                prt.go = t.gameObject;
                prt.mat = prt.go.GetComponent<Renderer>().material;
            }
        }
    }
    void InitMovement()
    {
        p0 = p1; // Переписать p1 в p0
        // Выбрать новую точку p1 на экране
        float widMinRad = bndCheck.camWidth - bndCheck.radius;
        float hgtMinRad = bndCheck.camHeight - bndCheck.radius;
        p1.x = Random.Range(-widMinRad, widMinRad);
        p1.y = Random.Range(-hgtMinRad, hgtMinRad);
        // Сбросить время
        timeStart = Time.time;
    }
    /// <summary>
    /// Этот метод переопределяет Enemy.Move() и реализует линейную интерполяцию
    /// </summary>
    public override void Move()
    {
        float u = (Time.time - timeStart) / duration;
        if (u>=1)
        {
            InitMovement();
            u = 0;
        }
        u = 1 - Mathf.Pow(1 - u, 2); // Применить плавное замедление
        pos = (1 - u) * p0 + u * p1; // Простая линейная интерполяция
    }
    /// <summary>
    /// Выполняет поиск части в массиве Parts по имени
    /// </summary>
    /// <param name="n"></param>
    /// <returns></returns>
    Part FindPart(string n)
    {
        foreach (Part prt in parts) if (prt.name == n) return (prt);
        return (null);
    }
    /// <summary>
    /// Выполняет поиск части в массиве Parts по ссылке на игровой объект
    /// </summary>
    /// <param name="go"></param>
    /// <returns></returns>
    Part FindPart (GameObject go)
    {
        foreach (Part prt in parts) if (prt.go == go) return (prt);
        return (null);
    }
    /// <summary>
    /// Возвращает true, если данная часть уничтожена
    /// </summary>
    /// <param name="go"></param>
    /// <returns></returns>
    bool Destroyed(GameObject go)
    {
        return (Destroyed(FindPart(go)));
    }
    /// <summary>
    /// Возвращает true, если данная часть уничтожена
    /// </summary>
    /// <param name="n"></param>
    /// <returns></returns>
    bool Destroyed(string n)
    {
        return (Destroyed(FindPart(n)));
    }
    /// <summary>
    /// Возвращает true, если данная часть уничтожена
    /// </summary>
    /// <param name="prt"></param>
    /// <returns></returns>
    bool Destroyed(Part prt)
    {
        if (prt == null) return (true);
        return (prt.health <= 0);
    }
    /// <summary>
    /// Окрашивает в красный часть корабля
    /// </summary>
    /// <param name="m"></param>
    void ShowLocalizedDamage(Material m)
    {
        m.color = Color.red;
        damageDoneTime = Time.time + showDamageDuration;
        showingDamage = true;
    }
    private void OnCollisionEnter(Collision coll)
    {
        GameObject other = coll.gameObject;
        switch (other.tag)
        {
            case "ProjectileHero":
                Projectile p = other.GetComponent<Projectile>();
                // Если корабль за границами экрана, не повреждать его
                if(!bndCheck.isOnScreen)
                {
                    Destroy(other);
                    break;
                }
                // Поразить корабль
                GameObject goHit = coll.contacts[0].thisCollider.gameObject;
                Part prtHit = FindPart(goHit);
                if (prtHit == null)
                {
                    goHit = coll.contacts[0].otherCollider.gameObject;
                    prtHit = FindPart(goHit);
                }
                // Проверить, защищена ли эта часть корабля
                if (prtHit.protectedBy != null)
                {
                    foreach (string s in prtHit.protectedBy)
                    {
                        if(!Destroyed(s))
                        {
                            Destroy(other); // Уничтожить снаряд
                            return;
                        }
                    }
                }
                // Получить разрушающую силу из Projectile.type и Main.WEAP_DICT
                prtHit.health -= Main.GetWeaponDefinition(p.type).damageOnHit + hero.fired * Main.GetWeaponDefinition(p.type).continuosDamage;
                // Показать эффект попадания в часть
                ShowLocalizedDamage(prtHit.mat);
                if (prtHit.health <= 0)
                {
                    hero.fired = 0;
                    prtHit.go.SetActive(false);
                }

                // Проверить был ли корабль полностью разрушен
                bool allDestroyed = true;
                foreach (Part prt in parts)
                {
                    if (!Destroyed(prt))
                    {
                        allDestroyed = false;
                        break;
                    }
                }
                if (allDestroyed)
                {
                    // Уведомить Main, что корабль разрушен
                    Main.S.ShipDestroyed(this);
                    // Уничтожить этот объект
                    Destroy(this.gameObject);
                }
                Destroy(other); // Уничтожить снаряд
                break;
        }
    }
}
