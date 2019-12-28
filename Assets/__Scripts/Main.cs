using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    static public Main S;
    static Dictionary<WeaponType, WeaponDefinition> WEAP_DICT;
    [Header("Set in Inspector")]
    public GameObject[] prefabEnemies; // Массив шаблонов Enemy
    public float enemySpawnPerSecond = 0.5f; // Вражеских кораблей в секунду
    public float enemyDefaultPadding = 1.5f; // Отступ для позиционирования
    //[HideInInspector]
    public WeaponDefinition[] weaponDefinitions;
    public GameObject prefabPowerUp;
    public WeaponType[] PowerUPFrequency = new WeaponType[] { WeaponType.blaster, WeaponType.blaster, WeaponType.spread, WeaponType.shield };
    private BoundsCheck bndCheck;
    public void ShipDestroyed(Enemy e)
    {
        // Сгенерировать бонус с заданной вероятностью
        if (Random.value <= e.powerUpDropChance)
        {
            // Выбрать тип бонуса - один из элементов PowerUpFrequency
            int ndx = Random.Range(0, PowerUPFrequency.Length);
            WeaponType puType = PowerUPFrequency[ndx];
            // Создать экземпляр PowerUp
            GameObject go = Instantiate(prefabPowerUp) as GameObject;
            PowerUp pu = go.GetComponent<PowerUp>();
            // Установить соответствующий тип WeaponType
            pu.SetType(puType);
            // Поместить в место, где находился разрушенный корабль
            pu.transform.position = e.transform.position;
        }
    }
    private void Awake()
    {
        S = this;
        // Записать в bndCheck ссылку на компонент BoundsCheck этого игрового объекта
        bndCheck = GetComponent<BoundsCheck>();
        // Вызвать SpawnEnemy()
        Invoke("SpawnEnemy", 1f / enemySpawnPerSecond);
        // Словарь с ключами типа WeaponType
        WEAP_DICT = new Dictionary<WeaponType, WeaponDefinition>();
        foreach (WeaponDefinition def in weaponDefinitions) WEAP_DICT[def.type] = def;
    }

    public void SpawnEnemy()
    {
        // Выбрать случайный шаблон Enemy для создания
        int ndx = Random.Range(0, prefabEnemies.Length);
        GameObject go = Instantiate<GameObject>(prefabEnemies[ndx]);
        // Разместить вражеский корабль над экраном в случайной позиции x
        float enemyPadding = enemyDefaultPadding;
        if (go.GetComponent<BoundsCheck>() != null) enemyPadding = Mathf.Abs(go.GetComponent<BoundsCheck>().radius);
        // Установить начальные координаты созданного вражеского корабля
        Vector3 pos = Vector3.zero;
        float xMin = -bndCheck.camWidth + enemyPadding;
        float xMax = bndCheck.camWidth - enemyPadding;
        pos.x = Random.Range(xMin, xMax);
        pos.y = bndCheck.camHeight + enemyPadding;
        go.transform.position = pos;
        // Снова вызвать SpawnEnemy()
        Invoke("SpawnEnemy", 1f / enemySpawnPerSecond);
    }

    public void DelayedRestart(float delay)
    {
        // Вызвать метод Restart() через delay секунд
        Invoke("Restart", delay);
    }

    public void Restart()
    {
        // Перезагрузить _Scene_0, чтобы перезапустить игру
        SceneManager.LoadScene("_Scene_0");
    }
    /// <summary>
    /// Статическая функция, возвращающая WeaponDefinition из защищенного поля WEAP_DICT класса Main
    /// </summary>
    /// <param name="wt">Тип оружия WeaponType, для которого требуется получить WeaponDefinition</param>
    /// <returns>Экземпляр WeaponDefinition или новый экземпляр WeaonDefinition с типом none, если нет такого определения для указанного WeaponType</returns>
    static public WeaponDefinition GetWeaponDefinition(WeaponType wt)
    {
        // Проверить наличие указанного ключа в словаре
        if (WEAP_DICT.ContainsKey(wt)) return (WEAP_DICT[wt]);
        // Возвращает новый экземпляр WeaponDefinition
        return (new WeaponDefinition());
    }
}
