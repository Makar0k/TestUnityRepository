using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerController : NetworkBehaviour
{
    public NetworkVariable<int> goldCount = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<int> health = new NetworkVariable<int>(100, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<bool> isColorChangedOnServer = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<float> color_r = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<float> color_g = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<float> color_b = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public int damage;
    [SerializeField]
    private float speed = 3f;
    private ScreenController controllerInputs;
    private float shootTime = 1f;
    private float shootTimer = 1f;
    private Quaternion latestRot;
    private bool isColorUpdated = false;
    [SerializeField]
    private GameObject bullet;
    void Start()
    {
        if(IsOwner)
        {
            color_r.Value = Random.Range(0f, 1f);
            color_b.Value = Random.Range(0f, 1f);
            color_g.Value = Random.Range(0f, 1f);
            isColorChangedOnServer.Value = true;
        }
        controllerInputs = GameObject.Find("Canvas").GetComponent<ScreenController>();
        transform.position = GameObject.Find("SpawnSystem").GetComponent<SpawnSystem>().GetRandomSpawnPoint();
        var gameRules = GameObject.Find("GameRules").GetComponent<GameRules>();
        gameRules.AddPlayer(this);
        GetComponent<Renderer>().material.SetColor("_Color", new Color(color_r.Value, color_g.Value, color_b.Value));
        if(!IsOwner) return;
        controllerInputs.colorElement.color = new Color(color_r.Value, color_g.Value, color_b.Value);

    }
    void Update()
    {
        if(!IsOwner && !isColorUpdated && isColorChangedOnServer.Value)
        {
            GetComponent<Renderer>().material.SetColor("_Color", new Color(color_r.Value, color_g.Value, color_b.Value));
            isColorUpdated = true;
        }
        if(!IsOwner) return;
        if(health.Value <= 0) return;
        var rb = GetComponent<Rigidbody>();
        controllerInputs.UpdateHud(health.Value, goldCount.Value);
        rb.velocity = new Vector3(controllerInputs.padDirectionX * speed, 0, controllerInputs.padDirectionY * speed);
        if(controllerInputs.padDirectionX != 0 || controllerInputs.padDirectionY != 0)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(rb.velocity, Vector3.up), Time.deltaTime * 300f);
            latestRot = transform.rotation;
        }
        else
        {
            transform.rotation = latestRot;
        }
        if(shootTimer > 0)
        {
            shootTimer -= Time.deltaTime;
        }
        else
        {
            ShootServerRpc(transform.position + transform.forward);
            shootTimer = shootTime;
        }
    }
    [ServerRpc]
    public void ShootServerRpc(Vector3 bulletSpawnPosition)
    {
        var gobj = Instantiate(bullet, bulletSpawnPosition, transform.rotation);
        gobj.GetComponent<Rigidbody>().velocity = transform.forward * 6;
        gobj.GetComponent<NetworkObject>().Spawn();
        
    }
}
