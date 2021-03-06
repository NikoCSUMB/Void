using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eye : Enemy
{
    public GameObject bullet;

    public override void Fire()
    {
        GameObject shot = Instantiate(bullet, transform.position, transform.rotation);
        shot.GetComponent<Bullet>().TargetPlayer(player.transform);
        Destroy(shot, 10);
    }
}
