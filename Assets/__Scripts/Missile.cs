using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : Projectile
{
    private GameObject target;
    // Start is called before the first frame update
    void Start()
    {
        this.standartVel = new Vector3(10, 10, 10);
        float minDistance = 1000;
        GameObject nearestEnemy = null;
        foreach (GameObject e in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            if (Vector3.Distance(this.transform.position, e.transform.position) < minDistance)
            {
                minDistance = Vector3.Distance(this.transform.position, e.transform.position);
                nearestEnemy = e;
            }
        }
        target = nearestEnemy;
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
        this.gameObject.transform.LookAt(target.transform, Vector3.forward);
        if (target == null) Destroy(this.gameObject);
        Vector3 tempVel = this.rigid.velocity;
        if(target.transform.position.x > this.transform.position.x)
        {
            tempVel.x = this.standartVel.y;
        }
        else if (target.transform.position.x < this.transform.position.x)
        {
            tempVel.x = -this.standartVel.y;
        }
        if (target.transform.position.y > this.transform.position.y)
        {
            tempVel.y = this.standartVel.y;
        }
        else if (target.transform.position.y < this.transform.position.y)
        {
            tempVel.y = -this.standartVel.y;
        }
        this.rigid.velocity = tempVel;
    }
}
