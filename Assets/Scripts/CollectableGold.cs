using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CollectableGold : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter(Collider other)
    {
        if(!IsOwner) return;
        other.gameObject.GetComponent<PlayerController>().goldCount.Value++;
        GetComponent<NetworkObject>().Despawn(true);
        Destroy(this.gameObject);
    }
}
