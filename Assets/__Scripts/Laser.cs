using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : Projectile
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
        Vector3 xPos = this.transform.position;
        xPos.x = hero.transform.position.x;
        this.transform.position = xPos;
        line.SetPosition(0, collar.transform.position);
        line.SetPosition(1, this.transform.position);
    }
}
