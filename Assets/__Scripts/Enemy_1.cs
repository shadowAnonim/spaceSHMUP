using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_1 : Enemy
{
    [Header("Set in Inspector: Enemy_1")]
    // Число секунд полного цикла синусоиды
    public float waveFrequency = 2;
    // Ширина синусоиды
    public float waveWidth = 4;
    public float waveRotY = 45;
    private float x0;// Начальное значение координаты X
    private float birthTime;
    // Start is called before the first frame update
    void Start()
    {
        // Установить начальную координату X объекта Enemy_1
        x0 = pos.x;
        birthTime = Time.time;
    }
    public override void Move()
    {
        Vector3 tempPos = pos;
        float age = Time.time - birthTime;
        float theta = Mathf.PI * 2 * age / waveFrequency;
        float sin = Mathf.Sin(theta);
        tempPos.x = x0 + waveWidth * sin;
        pos = tempPos;
        // Повернуть относительно оси Y
        Vector3 rot = new Vector3(0, sin * waveRotY, 0);
        this.transform.rotation = Quaternion.Euler(rot);
        base.Move();
        //print(bndCheck.isOnScreen);
    }
}
