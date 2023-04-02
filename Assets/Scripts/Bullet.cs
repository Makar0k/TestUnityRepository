using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Bullet : NetworkBehaviour
{
     void OnCollisionEnter(Collision collision)
    {
        if(!IsOwner) return;
        if(collision.gameObject.GetComponent<PlayerController>() != null)
        {
            collision.gameObject.GetComponent<PlayerController>().health.Value -= 10;
            Debug.Log(collision.gameObject.GetComponent<PlayerController>().health.Value);
        }
        Destroy(this.gameObject);
        GetComponent<NetworkObject>().Despawn(true);
    }
}
