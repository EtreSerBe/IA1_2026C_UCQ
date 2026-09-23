using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;


[RequireComponent(typeof(SphereCollider), typeof(SentidoDeVision))]
public class EnemigoBase : MonoBehaviour
{
    [Tooltip("Límite de velocidad al cual puede ir este agente")]
    [SerializeField] protected float maximaVelocidad = 5.0f; 

    /* Límite de fuerza que se puede aplicar a este agente para cambiar su velocidad
    // (realmente es una aceleración, pero normalmente se le llama fuerza en la literatura).*/
    [SerializeField] protected float maximaFuerza = 5.0f;
    
    [Tooltip("Velocidad actual que tiene este agente. Está limitada por maxSpeed")]
    protected Vector3 VelocidadActual = Vector3.zero;

    [SerializeField] protected float masa = 1.0f; // Masa del objeto, afecta qué tanto cambian su aceleración las fuerzas. 


    protected int Vida = 5;
    
    // Si hacen (o reciben) daño por contacto, entonces deben ser capaces de detectar contacto.
    protected int DanioDeContacto = 1;
    // Es el collider que representa el "cuerpo" de este enemigo. Es decir, si este collider hace contacto 
    // con el player entonces le hace daño; si el arma del player entra en contacto contigo entonces te hace daño; etc. 
    protected Collider ColliderPropio;
    
    
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
    
    
    protected SentidoDeVision SentidoDeVision;
    protected GameObject Objetivo;


    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected void Start()
    {

        SentidoDeVision = GetComponent<SentidoDeVision>();
        if (SentidoDeVision == null)
        {
            Debug.LogError("No hay componente SentidoDeVision asignado", gameObject);
        }

        // Siempre siempre siempre checar que el componente obtenido tras un GetComponent no sea null.
        ColliderPropio = GetComponent<Collider>();
        if (ColliderPropio == null)
        {
            Debug.LogError("No hay componente Collider asignado a colliderPropio", gameObject);
        }
        
        // TryGetComponent()

    }



    // Update is called once per frame
    void Update()
    {

        // Le pedimos al sentido de visión los objetos que conoce, y de esos nosotros vamos a perseguir a uno, si es que hay
        List<GameObject> objetosConocidos = SentidoDeVision.GetObjetosConocidos();
        // si la lista no está vacía, es que hay al menos un conocido, y entonces yo lo puedo poner como mi objetivo.
        if (objetosConocidos.Count > 0)
        {
            Objetivo = objetosConocidos[0];
            
            // Documento original de los steering behaviors: https://www.red3d.com/cwr/papers/1999/gdc99steer.pdf

            Vector3 steeringForce = Seek();
            
            ActualizarAceleracionVelocidadYPosicion(steeringForce);
            
            // Solo para demostrar que este y lo de abajo son prácticamente lo mismo.
            // tiempoTranscurrido += Time.deltaTime;
            // Vector3 posicionFinal = posicionInicial + puntMenosColaNormalizado * (tiempoTranscurrido * maxSpeed);
            // Debug.Log("La posicionInicial + la velocidad por todo el tiempo transcurrido es: " + posicionFinal); 
        
            // transform.position += direccionDeseada * (Time.deltaTime * maximaVelocidad);
            // Debug.Log("El transform.position que va aumentando cada cuadro es: " + transform.position); 
        }


        // AplicarGravedad();
    }

    protected Vector3 Flee()
    {
        return SteeringBehaviors.Flee(Objetivo.transform.position, transform.position,
            maximaVelocidad, VelocidadActual, maximaFuerza);
    }
    
    protected Vector3 Seek()
    {
        return SteeringBehaviors.Seek(Objetivo.transform.position, transform.position,
            maximaVelocidad, VelocidadActual, maximaFuerza);
        
        // // punta menos cola desde mí hacia el objetivo
        // Vector3 puntaMenosCola = Objetivo.transform.position - transform.position; // Seek
        //
        // // Para obtener la pura dirección de una flecha, usamos la Normalización
        // // También se le llama Steering Direction.
        // Vector3 direccionDeseada = puntaMenosCola.normalized;
        //     
        // // Esta flecha me dice: voy hacia mi objetivo lo más rápido que puedo.
        // Vector3 velocidadDeseada = direccionDeseada * maximaVelocidad;
        //
        // // Steering = velocidad deseada – velocidad actual
        // Vector3 steeringVelocity = velocidadDeseada - VelocidadActual;
        //     
        // // SteeringForce es truncate(direccionDeseada, maximaFuerza)
        // Vector3 steeringForce = Vector3.ClampMagnitude(steeringVelocity, maximaFuerza);
        //
        // return steeringForce;
    }

    protected void ActualizarAceleracionVelocidadYPosicion(Vector3 steeringForce)
    {
        transform.position = SteeringBehaviors.ActualizarAceleracionVelocidadYPosicion(steeringForce, masa, maximaVelocidad,
            ref VelocidadActual, transform.position);
    }
    
    void AplicarGravedad()
    {
        // Con esta función demostramos que esta forma de aplicar aceleración, velocidad y cambio de posición
        // es correcta matemáticamente.
        
        // vas a acelerar con la gravedad
        Vector3 direccionDeGravedad = new Vector3(0.0f, -1, 0.0f);
        float magnitudDeGravedad = 9.81f;
        
        // La gravedad es una aceleración, y la aceleración es "el cambio de la velocidad respecto al tiempo"
        VelocidadActual += direccionDeGravedad * (Time.deltaTime * magnitudDeGravedad);
        
        // Ahora que la aceleración ya cambió nuestra velocidad, ahora la velocidad va a cambiar nuestra posición
        transform.position += VelocidadActual * Time.deltaTime;
        
        Debug.Log($"Aceleración es: {direccionDeGravedad*magnitudDeGravedad}, " +
                  $"velocidad es: {VelocidadActual}, posición es: {transform.position}" );

    }
}
