using System;
using System.ComponentModel;
using UnityEngine;

public enum foeState
{
    patrol,
    follow,
    search,
    attack,
    returning,
    recover, // functionally same as return, but heals before doing anything else.
    death
}

public class FoeController : MonoBehaviour
{
    [SerializeField] UIManager uIManager;

    [Category("Material things")]
    // material things
    [SerializeField] Color color;
    [SerializeField] Material material;

    [Category("Navmesh things")]
    // Navmesh things
    [SerializeField] UnityEngine.AI.NavMeshAgent agent;
    [SerializeField] foeState state;
    [SerializeField] Vector3[] waypoints;
    GameObject player;

    [Category("Stats")]
    // Foe stat things
    [SerializeField] float atkRadius = 2, hitLeniency = 0.25f, sightRadius = 7, maxPatience = 3, attackCD = 3;
    [SerializeField] int hp = 10, maxHP = 10, damage = 10;

    // Private variables
    float timer = 0;
    int waypointIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        state = foeState.patrol;
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();

        uIManager = GameObject.FindWithTag("UIMan").GetComponent<UIManager>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckState();
        uIManager.UpdateStateText(state);
        agent.SetDestination(GetDestination());
        UpdateColor();
    }

    private void UpdateColor()
    {
        MeshRenderer mr = GetComponent<MeshRenderer>();
        mr.material.color = ColorFetchur.GetColour(state);
    }

    void CheckState() // This should be used to set state
    {
        switch (state)
        {
            case foeState.patrol: CheckForPlayer(); break;
            case foeState.attack: AtkCheck(); break;
            case foeState.follow: CheckForPlayer(); AtkCheck(); break;
            case foeState.search: CheckForPlayer(); BoredomCheck(); break;
            case foeState.returning: ReturnedCheck(); break;
            // due to a lack of a playerside attack, recovery isn't needed. Throw a notimplementedexception.
            case foeState.recover: throw new NotImplementedException();
            case foeState.death: print("State was death, destroying gameObject"); Destroy(gameObject); break;
        }
        InjuryCheck();

        void CheckForPlayer() // Call from patrol, follow, return and search, sets state to follow if player found
        {
            // Create a local player ref that is disposed after, because we only need to check if the player is nearby.
            GameObject player = GameObject.FindWithTag("Player");

            // Get positions
            Vector3 ourPos = transform.position; Vector3 targetPos = player.transform.position;
            if (Vector3.Distance(targetPos, ourPos) < sightRadius) { state = foeState.follow; this.player = GameObject.FindWithTag("Player"); }
            else if (state == foeState.follow) { state = foeState.search; this.player = null; } // Dispose of player, we don't need to know they exist anymore.
        }

        void AtkCheck() // Call from follow and attack
        {
            // Get positions
            Vector3 ourPos = transform.position; Vector3 targetPos = player.transform.position;
            if (Vector3.Distance(targetPos, ourPos) < atkRadius) { state = foeState.attack; }
            else if (state == foeState.attack) { state = foeState.follow; timer = 0; }
        }

        void InjuryCheck()
        {
            if (hp < 5) { state = foeState.recover; if (hp < 0) { state = foeState.death; } }
            if (state == foeState.recover && timer > maxHP - hp) { state = foeState.returning; timer = 0; }
        }

        void BoredomCheck() // Call from search. The guard is easily bored.
        {
            if (timer > maxPatience) { state = foeState.returning; timer = 0; }
        }

        void ReturnedCheck() // Call from return.
        {
            if (IsNearCurrentWaypoint()) { state = foeState.patrol; }
        }
    }

    private bool IsNearCurrentWaypoint()
    {
        return Vector3.Distance(transform.position, waypoints[waypointIndex]) < 1.25f;
    }



    void TakeDamage(int value)
    {
        // This will reset healing and searching timers. 
        hp -= value; timer = 0;
    }

    Vector3 GetDestination()
    {
        Vector3 returnable = new();

        switch (state)
        {
            case foeState.patrol: return Patrol();
            case foeState.follow: return player.transform.position;
            case foeState.search: UpdateBoredom(); return agent.destination;
            case foeState.recover: Recover(); return waypoints[0];
            case foeState.returning: waypointIndex = 0; return waypoints[0];
            case foeState.attack: Attack(); return transform.position;
        }

        Vector3 Patrol()
        {
            if (IsNearCurrentWaypoint())
            {
                // reset index if out of range.
                waypointIndex++; if (waypointIndex == waypoints.Length) { waypointIndex = 0; }
            }
            return waypoints[waypointIndex];
        }

        void Recover()
        {
            timer += Time.deltaTime;
            if (timer > maxPatience) { hp = maxHP; }
        }

        void UpdateBoredom()
        {
            timer += Time.deltaTime;
        }

        void Attack()
        {
            timer += Time.deltaTime;
            if (timer > attackCD)
            {
                timer = 0;
                FoeImplosion attack = GameObject.CreatePrimitive(PrimitiveType.Sphere).AddComponent<FoeImplosion>();
                attack.transform.position = transform.position;
                object[] sendable = { atkRadius + hitLeniency, damage };
                attack.SendMessage("TransferData", sendable);
            }
        }

        return returnable;
    }
}

[RequireComponent(typeof(SphereCollider))]
public class FoeImplosion : MonoBehaviour
{
    // This is used for the foe attack.
    int timer = 15;
    float radius;
    int damage;

    protected virtual void FixedUpdate()
    {
        // Keep this alive for {let x = timer} fixed updates.
        timer--;

        if (timer < 0) { Destroy(gameObject); }
    }

    void TransferData(object[] data)
    {
        SphereCollider collider = GetComponent<SphereCollider>();
        radius = (float)data[0];
        damage = (int)data[1];

        transform.localScale = Vector3.one * radius * 2;
        collider.isTrigger = true;

        MeshRenderer meshRenderer = collider.GetComponent<MeshRenderer>();
        Color c = Color.HSVToRGB(0, 0.75f, 0.75f); c.a = 0.5f;
        meshRenderer.material.color = c;
    }

    private void OnTriggerEnter(Collider other)
    {
        other.gameObject.SendMessageUpwards("TakeDamage", damage);
    }
}