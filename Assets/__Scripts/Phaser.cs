using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phaser : Projectile
{
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
        // Установить начальную координату X объекта Phaser
        x0 = this.transform.position.x;
        birthTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
        Move();
    }
    public override void Move()
    {
        Vector3 tempPos = pos;
        float age = Time.time - birthTime;
        float theta = Mathf.PI * 2 * age / waveFrequency;
        float sin = Mathf.Sin(theta);
        tempPos.x = x0 + waveWidth * sin * direction;
        pos = tempPos;
    }
}
