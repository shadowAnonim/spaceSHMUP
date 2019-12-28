﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    [Header("Set in Inspector")]
    public GameObject poi; // Корабль игрока
    public GameObject[] panels; // Прокручиваемые панели переднего плана
    public float scrollSpeed = -30f;
    public float motionMult = 0.25f; // Степень реакции панелей на перемещение корабля игрока
    private float panelHt; // Высота каждой панели
    private float depth; // Глубина панелей
    // Start is called before the first frame update
    void Start()
    {
        panelHt = panels[0].transform.localScale.y;
        depth = panels[0].transform.position.z;
        // Установить панели в начальные позиции
        panels[0].transform.position = new Vector3(0, 0, depth);
        panels[1].transform.position = new Vector3(0, panelHt, depth);
    }

    // Update is called once per frame
    void Update()
    {
        float tY, tX = 0;
        tY = Time.time * scrollSpeed % panelHt + (panelHt * 0.5f);
        if (poi != null) tX = -poi.transform.position.x * motionMult;
        // Сместить панель panels[0]
        panels[0].transform.position = new Vector3(tX, tY, depth);
        // Сместить панель panels[1]
        if (tY >= 0) panels[1].transform.position = new Vector3(tX, tY - panelHt, depth);
        else panels[1].transform.position = new Vector3(tX, tY + panelHt, depth);
    }
}
