using System;
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

    // Se llama una vez por frame y determina el valor de Time.deltaTime.
    void Update()
    {

    }

    // Es lo "mismo" que el Update, PERO este a fuerzas se manda a llamar cada cierto tiempo FIJO, independientemente
    // del framerate.
    // Por defecto se ejecuta 50 veces por segundo (cada 0.02 milisegundos),
    // lo pueden cambiar en Project Settings->Time->Fixed Timestep, pero es rara la vez que necesiten cambiarlo,
    // aumentarlo la cantidad de veces es pesado porque se aumenta el cuántas veces se checan las físicas de TODA la escena.
    // Este FixedUpdate es el que siempre se recomienda para el manejo de movimiento de Rigidbodies (gameObjects que contengan rigidbodies)
    // y (creo) que character controllers, siempre y cuando haya elementos con simulación física o detección de colisión física.
    // este tiene Time.fixedDeltaTime, que es el que debe de usarse aquí en lugar del Time.deltaTime normalito.
    private void FixedUpdate()
    {
        // Por ejemplo, addForces, cambios a la velocity de un rigidbody, o a la rotación de un objeto que depende de rigidbody, 
        // entre otras. Como regla general, casi siempre que tenga que ver con actualizar un rigidbody de forma "constante",
        // va a ser en este fixedUpdate.
        
        
        // COMPORTAMIENTO DEL AGENTE:
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
