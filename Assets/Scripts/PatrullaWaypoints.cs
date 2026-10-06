using System.Collections.Generic;
using UnityEngine;

// esto forza a que cuando este Script esté asignado a un gameObject, se le añada automáticamente un rigidbody si 
// no lo tiene, y que no se le pueda quitar.
[RequireComponent(typeof(Rigidbody))]
public class PatrullaWaypoints : MonoBehaviour
{
    // Este es necesario porque el Pursuit y evade requieren saber la velocidad actual de su objetivo, y pues
    // el rigidbody tiene dicha información.
    private Rigidbody _rb;
    [SerializeField] private float maximaFuerza = 5.0f;
    
    [SerializeField] private float maximaVelocidad = 5.0f;

    [SerializeField] private float radioDeToleranciaAWaypoint = 5.0f;
    
    

    // los puntos que va a visitar y en qué orden los va a visitar. Cuando visite al último, se ciclará hacia el primero.
    [SerializeField] private List<Transform> waypoints = new List<Transform>();
    private Transform _waypointActual;
    private int _indiceWaypointActual = 0;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        if (_rb == null)
        {
            Debug.LogError("No tiene el rigidbody en este gameObject", gameObject);
        }
        
        // Cuando inicias, pones como Waypoint actual al primero de la lista
        _indiceWaypointActual = 0;
        _waypointActual = waypoints[_indiceWaypointActual];
    }

    // Update is called once per frame
    void Update()
    {
        // ¿cómo sabemos cuál es el waypoint actualmente activo?
        // Pues inicias por el primero... :D
        // ya que llegas al primero, te vas luego al segundo... :D 
        // ¿cómo sabes que ya llegaste al primero?
        // decimos que "llegamos" cuando estamos lo suficientemente cerca de nuestra posición objetivo
        // y ese "cerca" se mide con distancia entre mi posición y la posición del objetivo.
        float distanciaAlWaypoint = Vector3.Distance(transform.position, _waypointActual.position);
        
        // si dicha distancia es Menor que lo que yo considero Cerca
        if (distanciaAlWaypoint < radioDeToleranciaAWaypoint)
        {
            // entonces ya llegué, y ahora voy a ir hacia el siguiente waypoint
            // es decir, mi waypoint Objetivo ahora es el siguiente del actual.
            _indiceWaypointActual += 1;
            // si el índice es mayor o igual que el total de waypoints, es decir, ya no hay un waypoint siguiente al cual ir,
            if (_indiceWaypointActual >= waypoints.Count)
            {
                // entonces lo regresamos al primero
                _indiceWaypointActual = 0;
            }

            _waypointActual = waypoints[_indiceWaypointActual];
        }
        // y si no, pues nada más me sigo yendo al que ya estaba yendo.


        // Este agente hace Seek hacia la posición del Waypoint actualmente activo.
        Vector3 steeringForce = SteeringBehaviors.Seek(_waypointActual.position, transform.position,
            maximaVelocidad, _rb.linearVelocity, maximaFuerza);
        
        // usamos Acceleration para no tomar en cuenta la Masa del rigidbody (al menos por ahora).
        _rb.AddForce(steeringForce, ForceMode.Acceleration);
    }
}
