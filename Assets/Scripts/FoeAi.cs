using System.ComponentModel;
using UnityEngine;

enum foeState { patrol, follow, search, engaging, retreat, recover }

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
    [SerializeField] float atkRadius = 3, sightRadius = 7, maxPatience = 3;
    [SerializeField] int hp = 10, damage = 10;

    // Private backend
    float timer = 0;

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
            case foeState.patrol: PlayerDistanceCheck(); break;
        }

        void PlayerDistanceCheck() // Call from patrol and search, sets state to follow if player found
        {
            // Get positions
            Vector3 ourPos = transform.position; Vector3 targetPos = player.transform.position;
            if (Vector3.Distance(targetPos, ourPos) < sightRadius) { state = foeState.follow; }
        }

    }

    Vector3 GetDestination()
    {
        return new Vector3(0, 0, 0);
    }
}
