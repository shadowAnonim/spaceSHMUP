using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    protected BoundsCheck bndCheck;
    private Renderer rend;
    [Header("Set Dynamically")]
    public Rigidbody rigid;
    [HideInInspector]
    public short direction;
    [HideInInspector]
    public LineRenderer line;
    [HideInInspector]
    public Vector3 standartVel;
    //[HideInInspector]
    public GameObject collar;
    protected GameObject hero;
    [SerializeField]
    private WeaponType _type;
    public WeaponType type { get { return (_type); } set { SetType(value); } }
    private void Awake()
    {
        bndCheck = GetComponent<BoundsCheck>();
        rend = GetComponent<Renderer>();
        rigid = GetComponent<Rigidbody>();
        line = this.GetComponent<LineRenderer>();
        hero = GameObject.Find("_Hero");
    }
    private void Start()
    {
        //standartVel = this.rigid.velocity;
    }
    // Update is called once per frame
    protected void Update()
    {
        if (bndCheck.offUp)
        {
            hero.GetComponent<Hero>().fired = 0;
            Destroy(gameObject);
        }
    }
    public void SetType (WeaponType eType)
    {
        //Установить _type
        _type = eType;
        WeaponDefinition def = Main.GetWeaponDefinition(_type);
        rend.material.color = def.projectileColor;
    }
    public Vector3 pos { get { return (this.transform.position); } set { this.transform.position = value; } }
    public virtual void Move() { }
}
