using System.ComponentModel;
using UnityEngine;

enum foeState
{
    patrol,
    follow,
    search,
    attack,
    returning,
    recover // functionally same as return, but heals before doing anything else.
}

public class FoeManager : MonoBehaviour
{
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
    [SerializeField] float atkRadius = 2, sightRadius = 7, maxPatience = 3;
    [SerializeField] int hp = 10, maxHP = 10, damage = 10;

    // Private backend
    float timer = 0;
    int waypointIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        state = foeState.patrol;
        player = GameObject.FindWithTag("Player");
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckState();
        agent.SetDestination(GetDestination());
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
        }
        InjuryCheck();

        void CheckForPlayer() // Call from patrol, follow, return and search, sets state to follow if player found
        {
            // Get positions
            Vector3 ourPos = transform.position; Vector3 targetPos = player.transform.position;
            if (Vector3.Distance(targetPos, ourPos) < sightRadius) { state = foeState.follow; }
            else if (state == foeState.follow) { state = foeState.search; }
        }

        void AtkCheck() // Call from follow and attack
        {
            // Get positions
            Vector3 ourPos = transform.position; Vector3 targetPos = player.transform.position;
            if (Vector3.Distance(targetPos, ourPos) < atkRadius) { state = foeState.attack; agent.SetDestination(transform.position); }
            else if (state == foeState.attack) { state = foeState.follow; }
        }

        void InjuryCheck()
        {
            if (hp < 5) { state = foeState.recover; }
            if (state == foeState.recover && timer > maxHP - hp) { state = foeState.returning; }
        }

        void BoredomCheck() // Call from search. The guard is easily bored.
        {
            if (timer < maxPatience) { state = foeState.returning; }
        }

        void ReturnedCheck() // Call from return.
        {
            if (Vector3.Distance(transform.position, waypoints[waypointIndex]) < 0.25) { state = foeState.patrol; }
        }
    }

    Vector3 GetDestination()
    {
        return new Vector3(0, 0, 0);
    }
}

[RequireComponent(typeof(SphereCollider))]
public class FoeImplosion : MonoBehaviour
{
    // This is used for the foe attack.
}
