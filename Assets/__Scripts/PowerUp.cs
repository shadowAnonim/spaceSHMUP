using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [Header("Set in Inspector")]
    public Vector2 rotMinMax = new Vector2(15, 90);
    public Vector2 driftMinMax = new Vector2(.25f, 2);
    public float lifeTime = 6f; // Время в секундах существования PowerUp
    public float fadeTime = 4f; // Время в секундах растворения PowerUp
    [Header("SetDynamically")]
    public WeaponType type; // Тип бонуса
    public GameObject cube; // Ссылка на вложеный куб
    public TextMesh letter; // Ссылка на TextMesh
    public Vector3 rotPerSecond; // Скорость вращения
    public float birthTime;
    private Rigidbody rigid;
    private BoundsCheck bndCheck;
    private Renderer cubeRend;
    private void Awake()
    {
        // Получить ссылку на куб
        cube = transform.Find("Cube").gameObject;
        // Получить ссылки на TextMesh и другие компоненты
        letter = GetComponent<TextMesh>();
        rigid = GetComponent<Rigidbody>();
        bndCheck = GetComponent<BoundsCheck>();
        cubeRend = cube.GetComponent<Renderer>();
        // Выбрать случайную скорость
        Vector3 vel = Random.onUnitSphere; // Получить случайную скорость
        vel.z = 0; // Отобразить vel на площадь XY
        vel.Normalize();
        vel *= Random.Range(driftMinMax.x, driftMinMax.y);
        rigid.velocity = vel;
        transform.rotation = Quaternion.identity;
        // Выбрать случайную скорость вращения для вложенного куба
        rotPerSecond = new Vector3(Random.Range(rotMinMax.x, rotMinMax.y),
                                   Random.Range(rotMinMax.x, rotMinMax.y),
                                   Random.Range(rotMinMax.x, rotMinMax.y));
        birthTime = Time.time;
    }
    // Update is called once per frame
    void Update()
    {
        cube.transform.rotation = Quaternion.Euler(rotPerSecond * Time.time);
        float u = (Time.time - (birthTime + lifeTime)) / fadeTime;
        // Если u >= 1, уничтожить бонус
        if (u >= 1)
        {
            Destroy(this.gameObject);
            return;
        }
        // Использоать u для определения альфа-значения куба и буквы
        if(u>0)
        {
            Color c = cubeRend.material.color;
            c.a = 1f - u;
            cubeRend.material.color = c;
            // Растворить букву медленнее
            c = letter.color;
            c.a = 1f - (u * 0.5f);
            letter.color = c;
        }
        if (!bndCheck.isOnScreen) Destroy(gameObject); // Если бонус полностью вышел за границу экрана, уничтожить его
    }
    public void SetType(WeaponType wt)
    {
        // Получить WeaponDefinition из Main
        WeaponDefinition def = Main.GetWeaponDefinition(wt);
        // Установить цвет дочернего куба
        cubeRend.material.color = def.color;
        //letter.color = def.color; // Окрасить букву в тот же цвет
        letter.text = def.letter; //Установить отображаемую букву
        type = wt; // Установить тип
    }
    public void AbsorbedBy(GameObject target)
    {
        Destroy(this.gameObject);
    }
}
