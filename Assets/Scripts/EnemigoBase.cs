using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class EnemigoBase : MonoBehaviour
{
    [Tooltip("Límite de velocidad al cual puede ir este agente")]
    [SerializeField] private float maximaVelocidad = 5.0f; 

    /* Límite de fuerza que se puede aplicar a este agente para cambiar su velocidad
    // (realmente es una aceleración, pero normalmente se le llama fuerza en la literatura).*/
    [SerializeField] private float maximaFuerza = 5.0f;
    
    [Tooltip("Velocidad actual que tiene este agente. Está limitada por maxSpeed")]
    private Vector3 _velocidadActual = Vector3.zero;

    [SerializeField] private float masa = 1.0f; // Masa del objeto, afecta qué tanto cambian su aceleración las fuerzas. 

    // // variable1 hace tal
    // private int variable1;
    //
    // // variable2 hace X
    // private int variable2;
    //
    // // variable3 hace Y
    // private int variable3;
    //
    // private int variable4;      // Variable4 hace cosas chidas
    
    
    private SentidoDeVision _sentidoDeVision;
    private GameObject _objetivo;


    private bool _estaUsandoSeek = true;
    [SerializeField] private float tiempoParaCambiarEntreSeekYFlee = 2.0f;
    private float _tiempoTranscurrido = 0.0f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        _sentidoDeVision = GetComponent<SentidoDeVision>();
        if (_sentidoDeVision == null)
        {
            Debug.LogError("No hay componente SentidoDeVision asignado", gameObject);
        }
        
    }

    // Update is called once per frame
    void Update()
    {

        // Le pedimos al sentido de visión los objetos que conoce, y de esos nosotros vamos a perseguir a uno, si es que hay
        List<GameObject> objetosConocidos = _sentidoDeVision.GetObjetosConocidos();
        // si la lista no está vacía, es que hay al menos un conocido, y entonces yo lo puedo poner como mi objetivo.
        if (objetosConocidos.Count > 0)
        {
            // el tiempo solo transcurre si sí está detectando un objetivo.
            _tiempoTranscurrido += Time.deltaTime;
            if (_tiempoTranscurrido >= tiempoParaCambiarEntreSeekYFlee)
            {
                _estaUsandoSeek = !_estaUsandoSeek; // invertimos el valor
                _tiempoTranscurrido = 0.0f; // reseteamos este para esperar otros X segundos.
            }

            
            _objetivo = objetosConocidos[0];
            
            // Documento original de los steering behaviors: https://www.red3d.com/cwr/papers/1999/gdc99steer.pdf
            
            // punta menos cola desde mí hacia el objetivo
            Vector3 puntaMenosCola;
            if(_estaUsandoSeek)
                puntaMenosCola = _objetivo.transform.position - transform.position; // Seek
            else
                puntaMenosCola = transform.position - _objetivo.transform.position; // Flee
        
            // Para obtener la pura dirección de una flecha, usamos la Normalización
            // También se le llama Steering Direction.
            Vector3 direccionDeseada = puntaMenosCola.normalized;
            
            // Esta flecha me dice: voy hacia mi objetivo lo más rápido que puedo.
            Vector3 velocidadDeseada = direccionDeseada * maximaVelocidad;
        
            // Steering = velocidad deseada – velocidad actual
            Vector3 steeringVelocity = velocidadDeseada - _velocidadActual;
            
            // SteeringForce es truncate(direccionDeseada, maximaFuerza)
            Vector3 steeringForce = Vector3.ClampMagnitude(steeringVelocity, maximaFuerza);

            Vector3 aceleracion = steeringForce / masa;

            _velocidadActual = Vector3.ClampMagnitude(_velocidadActual + aceleracion * Time.deltaTime,
                maximaVelocidad);

            transform.position += _velocidadActual * Time.deltaTime;
            
            // Solo para demostrar que este y lo de abajo son prácticamente lo mismo.
            // tiempoTranscurrido += Time.deltaTime;
            // Vector3 posicionFinal = posicionInicial + puntMenosColaNormalizado * (tiempoTranscurrido * maxSpeed);
            // Debug.Log("La posicionInicial + la velocidad por todo el tiempo transcurrido es: " + posicionFinal); 
        
            // transform.position += direccionDeseada * (Time.deltaTime * maximaVelocidad);
            // Debug.Log("El transform.position que va aumentando cada cuadro es: " + transform.position); 
        }

        // AplicarGravedad();
    }

    void AplicarGravedad()
    {
        // Con esta función demostramos que esta forma de aplicar aceleración, velocidad y cambio de posición
        // es correcta matemáticamente.
        
        // vas a acelerar con la gravedad
        Vector3 direccionDeGravedad = new Vector3(0.0f, -1, 0.0f);
        float magnitudDeGravedad = 9.81f;
        
        // La gravedad es una aceleración, y la aceleración es "el cambio de la velocidad respecto al tiempo"
        _velocidadActual += direccionDeGravedad * (Time.deltaTime * magnitudDeGravedad);
        
        // Ahora que la aceleración ya cambió nuestra velocidad, ahora la velocidad va a cambiar nuestra posición
        transform.position += _velocidadActual * Time.deltaTime;
        
        Debug.Log($"Aceleración es: {direccionDeGravedad*magnitudDeGravedad}, " +
                  $"velocidad es: {_velocidadActual}, posición es: {transform.position}" );

    }
}
