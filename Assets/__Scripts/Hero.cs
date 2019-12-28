using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    static public Hero S;
    [Header("Set in Inspector")]
    // Поля, управляющие движением корабля
    public float speed = 30;
    public float rollMult = -45;
    public float pitchMult = 30;
    public float gameRestartDelay = 2f;
    public GameObject projectilePrefab;
    public float projectileSpeed = 40;
    public Weapon[] weapons;
    [Header("Set Dynamically")]
    [SerializeField]
    private int _missileCount = 0;
    private float _shieldLevel = 1;
    public int fired = 0;
    private float lastShotMissileTime; // Время последнего выстрела ракетой
    private float missileDelay; // Задержка выстрелов ракетой
    private GameObject lastTriggerGo = null; //Последний столкнувшийся игровой объект
    public delegate void WeaponFireDelegate();
    public WeaponFireDelegate fireDelegate;
    private void Start()
    {
        if (S == null) S = this;
        else Debug.LogError("Hero.Awake() - Attempted to assign second Hero.S!");
        //fireDelegate += TempFire;
        ClearWeapons();
        // Очистить массив Weapons и начать игру с 1 бластером
        weapons[0].SetType(WeaponType.blaster);
        weapons[1].SetType(WeaponType.none);
        weapons[2].SetType(WeaponType.none);
        weapons[3].SetType(WeaponType.none);
        weapons[4].SetType(WeaponType.none);
        this.missileCount = 10;
        lastShotMissileTime = 0;
        missileDelay = Main.GetWeaponDefinition(WeaponType.missile).delayBetweenShots;
    }
    // Update is called once per frame
    void Update()
    {
        // Извлечь информацию из класса Input
        float xAxis = Input.GetAxis("Horizontal");
        float yAxis = Input.GetAxis("Vertical");
        // Изменить transform.position, опираясь на информацию по осям
        Vector3 pos = transform.position;
        pos.x += xAxis * speed * Time.deltaTime;
        pos.y += yAxis * speed * Time.deltaTime;
        transform.position = pos;
        // Повернуть корабль, чтобы придать ощущение динамизма
        transform.rotation = Quaternion.Euler(yAxis * pitchMult, xAxis * rollMult, 0);
        // Позволить кораблю выстрелить
        // if (Input.GetKeyDown(KeyCode.Space)) TempFire();
        // Произвезти выстрел из всех видов оружия
        if (Input.GetAxis("Jump") == 1 && fireDelegate != null)
        {
            fired ++;
            fireDelegate();
        }
        else
        {
            fired = 0;
        }
        if (Input.GetAxis("Fire") == 1)
        {
            TempFire();
        }
    }
    void TempFire()
    {
        if (missileCount == 0) return;
        if (Time.time - lastShotMissileTime < missileDelay) return;
        missileCount--;
        GameObject projGO = Instantiate<GameObject>(projectilePrefab);
        projGO.transform.position = transform.position;
        Rigidbody rigidB = projGO.GetComponent<Rigidbody>();
        lastShotMissileTime = Time.time;
        //rigidB.velocity = Vector3.up * projectileSpeed;
        Projectile proj = projGO.GetComponent<Projectile>();
        proj.type = WeaponType.missile;
    }
    private void OnTriggerEnter(Collider other)
    {
        Transform rootT = other.gameObject.transform.root;
        GameObject go = rootT.gameObject;
        //print("Triggered: " + go.name);
        // Гарантировать невозможность повторного столкновения с тем же объектом
        if (go == lastTriggerGo) return;
        lastTriggerGo = go;
        if (go.tag == "Enemy") // Если защитное поле столкнулось с вражеским кораблём, уменьшить уровень защиты на1 и уничтожить врага
        {
            shieldLevel--;
            Destroy(go);
        }
        else if (go.tag == "PowerUp")
        {
            AbsorbPowerUp(go);
        }
        else print("Triggered by non-Enemy: " + go.name);
    }
    public void AbsorbPowerUp(GameObject go)
    {
        PowerUp pu = go.GetComponent<PowerUp>();
        switch (pu.type)
        {
            case WeaponType.shield:
                shieldLevel++;
                break;
            case WeaponType.missile:
                missileCount++;
                break;
            default:
                if (pu.type == weapons[0].type)
                {
                    Weapon w = GetEmptyWeaponSlot();
                    if (w != null) w.SetType(pu.type);
                }
                else
                {
                    ClearWeapons();
                    weapons[0].SetType(pu.type);
                }
                break;
        }
        pu.AbsorbedBy(this.gameObject);
    }
    public float shieldLevel
    {
        get { return (_shieldLevel); }
        set
        {
            _shieldLevel = Mathf.Min(value, 4);
            if (value < 0)
            {
                Destroy(this.gameObject);
                // Сообщить объекту Main.S о необходимости перезапустить игру
                Main.S.DelayedRestart(gameRestartDelay);
            }
        }
    }

    private int missileCount
    {
        get { return (_missileCount); }
        set
        {
            _missileCount = Mathf.Max(value, 0);
        }
    }
    Weapon GetEmptyWeaponSlot()
    {
        for (int i = 0; i < weapons.Length; i++) if (weapons[i].type == WeaponType.none) return (weapons[i]);
        return (null);
    }
    void ClearWeapons()
    {
        foreach (Weapon w in weapons) w.SetType(WeaponType.none);
    }
}
