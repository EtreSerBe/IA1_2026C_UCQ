using UnityEngine;

public class EnemigoPursuit : EnemigoBase
{
    [SerializeField] private float tiempoAFuturo = 3.0f;


    [Header("Gizmos")] 
    [SerializeField] private float tamanoPosicionFutura = 0.5f;

    [SerializeField] private Color colorEsferaPosicionFutura = Color.red;
    
    void Update()
    {
        ActualizarObjetivo();

        if(Objetivo != null)
        {
            // Necesitamos obtener el rigidbody del objetivo para poder saber su velocidad actual.
            Rigidbody rbDelObjetivo = Objetivo.GetComponent<Rigidbody>();
            if (rbDelObjetivo == null)
            {
                Debug.LogError($"El Objetivo: {Objetivo.name} NO tiene un rigidbody," +
                               $" por lo que no se puede hacer pursuit", gameObject);
                return; // nos salimos porque si no tronaría el Pursuit
            }
            
            // Pursuit con un tiempo a futuro "ingenuo" o puesto por diseñador
            // Vector3 steeringForce = Pursuit(rbDelObjetivo.linearVelocity, tiempoAFuturo);
            
            // pursuit con un tiempo a futuro estimado basado en las propiedades de nuestro agente.
            // Distancia actual entre él y yo
            float distancia = Vector3.Distance(transform.position, Objetivo.transform.position);
            // dividimos dicha distancia entre mi máxima velocidad, para obtener cuánto tiempo me tomaría a mí alcanzarlo
            // en este preciso momento si fuera a mi máxima velocidad
            float tiempoQueMeTomariaActualmente = distancia / maximaVelocidad;
            
            Vector3 steeringForce = Pursuit(rbDelObjetivo.linearVelocity, tiempoQueMeTomariaActualmente);

            
            ActualizarAceleracionVelocidadYPosicion(steeringForce);
        }
    }

    void OnDrawGizmos()
    {
        if(Objetivo != null)
        {
            Rigidbody rbDelObjetivo = Objetivo.GetComponent<Rigidbody>();
            if (rbDelObjetivo == null)
            {
                Debug.LogError($"El Objetivo: {Objetivo.name} NO tiene un rigidbody," +
                               $" por lo que no se puede hacer pursuit", gameObject);
                return; // nos salimos porque no vale la pena dibujar lo que sigue.
            }
            
            // queremos saber la posición a la que este mono está haciendo seek.
            // Obtenemos la posición que tendrá nuestro objetivo tras transcurrir X tiempo a Futuro.
            // Vector3 posicionFutura = Objetivo.transform.position + rbDelObjetivo.linearVelocity * tiempoAFuturo;
            
            // pursuit con un tiempo a futuro estimado basado en las propiedades de nuestro agente.
            // Distancia actual entre él y yo
            float distancia = Vector3.Distance(transform.position, Objetivo.transform.position);
            // dividimos dicha distancia entre mi máxima velocidad, para obtener cuánto tiempo me tomaría a mí alcanzarlo
            // en este preciso momento si fuera a mi máxima velocidad
            float tiempoQueMeTomariaActualmente = distancia / maximaVelocidad;
            
            // Obtenemos la posición que tendrá nuestro objetivo tras transcurrir dicho tiempo que me tomaría.
            Vector3 posicionFutura = Objetivo.transform.position + rbDelObjetivo.linearVelocity * tiempoQueMeTomariaActualmente;
            
            
            Gizmos.color = colorEsferaPosicionFutura;
            
            Gizmos.DrawSphere(posicionFutura, tamanoPosicionFutura);
            
            Gizmos.DrawLine(transform.position, posicionFutura);
        }
    }
}
