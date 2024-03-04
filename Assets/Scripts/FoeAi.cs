using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum foeState {patrol, follow, search, engaging, retreat}

public class FoeManager : MonoBehaviour
{
    // material things
    [SerializeField]Color color;
    [SerializeField]Material material;

    // Navmesh things
    [SerializeField]UnityEngine.AI.NavMeshAgent agent;
    [SerializeField]foeState state;
    [SerializeField]Vector3[] waypoints;
    GameObject player;

    // Foe stat things
    float atkRadius;
    int hp = 10, damage = 10;

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

    void CheckState(){
        


    }

    Vector3 GetDestination(){
        return new Vector3(0,0,0);
    }
}
