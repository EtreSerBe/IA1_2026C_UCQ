using UnityEngine;
using UnityEngine.AI;

public class NavMeshCharacter : MonoBehaviour
{
    public Transform posicionObjetivo;
    private NavMeshAgent _navMeshAgent;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!TryGetComponent<NavMeshAgent>(out _navMeshAgent))
        {
            Debug.LogError("Error, no hay navmesh Agent asignado");
        }


        if (posicionObjetivo != null)
            _navMeshAgent.destination = posicionObjetivo.position;

    }

    // Update is called once per frame
    void Update()
    {
        if (posicionObjetivo != null)
        {
            // solo si la posición objetivo cambió.
            if (posicionObjetivo.position != _navMeshAgent.destination)
            {
                _navMeshAgent.destination = posicionObjetivo.position;

            }
            
        }

            
    }
}
