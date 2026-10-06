using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.LowLevelPhysics2D;
using Vector3 = UnityEngine.Vector3;


[RequireComponent(typeof(SphereCollider), typeof(SentidoDeVision))]
public class EnemigoBase : MonoBehaviour
{
    [Header("Steering Behaviour")]

    [Tooltip("Límite de velocidad al cual puede ir este agente")]
    [SerializeField] protected float maximaVelocidad = 5.0f; 

    /* Límite de fuerza que se puede aplicar a este agente para cambiar su velocidad
    // (realmente es una aceleración, pero normalmente se le llama fuerza en la literatura).*/
    [SerializeField] protected float maximaFuerza = 5.0f;
    
    [Tooltip("Velocidad actual que tiene este agente. Está limitada por maxSpeed")]
    protected Vector3 VelocidadActual = Vector3.zero;

    [SerializeField] protected float masa = 1.0f; // Masa del objeto, afecta qué tanto cambian su aceleración las fuerzas. 


    [Header("Propiedades de Combate")]
    
    [SerializeField] protected int Vida = 5;
    
    // Si hacen (o reciben) daño por contacto, entonces deben ser capaces de detectar contacto.
    [SerializeField] protected int DanioDeContacto = 1;
    
    // Es el collider que representa el "cuerpo" de este enemigo. Es decir, si este collider hace contacto 
    // con el player entonces le hace daño; si el arma del player entra en contacto contigo entonces te hace daño; etc. 
    [SerializeField] protected Collider ColliderPropio;
    
    // [SerializeField] protected Collider ColliderDeLaEspada;
    
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

    protected int myInt = 0;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected void Start()
    {

        SentidoDeVision = GetComponent<SentidoDeVision>();
        if (SentidoDeVision == null)
        {
            Debug.LogError("No hay componente SentidoDeVision asignado", gameObject);
        }

        // Siempre siempre siempre checar que el componente obtenido tras un GetComponent no sea null.
        // ColliderPropio = GetComponent<Collider>();
        // Este tiene que ser puesto manualmente en el editor, por seguridad (por ahora, para no hacernos bolas).
        if (ColliderPropio == null)
        {
            Debug.LogError("No hay componente Collider asignado a colliderPropio", gameObject);
        }
        
        // obtener el capsule collider de la espada
        // ColliderDeLaEspada = GetComponentInChildren<CapsuleCollider>();
        // if (ColliderDeLaEspada == null)
        // {
        //     Debug.LogError("No hay componente Collider asignado a ColliderDeLaEspada", gameObject);
        // }

        // TryGetComponent()

    }



    // Lo ideal será que este update de la clase padre quede vacío, porque las clases hijas tienen updates muy
    // distintos entre sí, entonces no vale la pena forzarlas a sobreescribirlo.
    void Update()
    {
        ActualizarObjetivo();

        if(Objetivo != null)
        {
            
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

    protected void ActualizarObjetivo()
    {
        // Le pedimos al sentido de visión los objetos que conoce, y de esos nosotros vamos a perseguir a uno, si es que hay
        List<GameObject> objetosConocidos = SentidoDeVision.GetObjetosConocidos();
        // si la lista no está vacía, es que hay al menos un conocido, y entonces yo lo puedo poner como mi objetivo.
        if (objetosConocidos.Count > 0)
        {
            Objetivo = objetosConocidos[0];
        }
        else // Nos faltaba el otro lado de la moneda: cuando ya no ves a nadie, entonces ya no tienes objetivo!
        {
            Objetivo = null;
        }
    }

    protected Vector3 Flee()
    {
        return SteeringBehaviors.Flee(Objetivo.transform.position, transform.position,
            maximaVelocidad, VelocidadActual, maximaFuerza);
    }
    
    protected Vector3 Flee(Vector3 position)
    {
        return SteeringBehaviors.Flee(position, transform.position,
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

    protected Vector3 Pursuit(Vector3 posicionDelObjetivo, Vector3 velocidadDelObjetivo, float tiempoAFuturo)
    {
        return SteeringBehaviors.Pursuit(posicionDelObjetivo, velocidadDelObjetivo, tiempoAFuturo,
            transform.position,
            maximaVelocidad, VelocidadActual, maximaFuerza);
    }

    // Esta usa la posición del Objetivo ya obtenido por el ActualizarObjetivo.
    protected Vector3 Pursuit(Vector3 velocidadDelObjetivo, float tiempoAFuturo)
    {
        return SteeringBehaviors.Pursuit(Objetivo.transform.position, velocidadDelObjetivo, tiempoAFuturo,
            transform.position,
            maximaVelocidad, VelocidadActual, maximaFuerza);
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


    // Esto ya hace que todas las clases que sean EnemigoBase o hereden de ella, pierdan 1 de vida cuando entren en 
    // contacto con algo en la capa de "AtaqueDeJugador".
    protected void OnCollisionEnter(Collision other)
    {
        // Debug.Log("OnCollisionEnter", gameObject);

        // Checar contra quién estamos colisionando:
        // Si chocamos contra algo que está en la capa de AtaqueDeJugador, entonces yo como enemigo recibo daño.
        if (other.gameObject.layer == LayerMask.NameToLayer("AtaqueDeJugador"))
        {
            // entonces yo como enemigo recibo daño. Cuando recibo daño me bajan vida, entonces bajo mi vida actual.
            Vida--; // por ahora, cualquier contacto con un ataque del jugador baja 1 de vida.
            Debug.Log("colisioné con un ataque de jugador, y ahora mi vida actual es: " + Vida, gameObject);
            
            // Si la vida llega a 0 o menos, entonces este enemigo muere
            if (Vida <= 0)
            {
                Debug.Log("Vida llegó a 0, este enemigo va a morir: " + gameObject.name, gameObject);
                
                // No pongo el destroy de una vez porque ustedes pueden poner una animación de morir, o un VFX, etc.
                // A veces no lo vas a querer destruir, solo "despawnearlo" y mandarlo a un Pool.
                // Destroy(gameObject);
            }
        }
    }


    // Siempre que se esté viendo la vista de Scene, se va a mandar a llamar esta función Y se tiene que actualizar
    // algo en dicha pestaña, por ejemplo, se tiene que redibujar porque moviste el mouse o la vista.
    // Se ejecuta aunque NO esté en modo play, y nos sirven mucho para acomodar cosas en escena, debuggear, etc.
    protected void OnDrawGizmos()
    {
        // Gizmos.DrawLine();
        // Objetivo
        // Debug.Log("se está viendo la pestaña de Scene");
        
        // Que solo me dibuje esa línea si sí hay un Objetivo.
        if(Objetivo != null)
            Gizmos.DrawLine(transform.position, Objetivo.transform.position);
        
        // myInt++;
        // Debug.Log("myInt= " + myInt);
        //
        // myInt -= 2;
        // Debug.Log("myInt= " + myInt);
        //
        // myInt = FuncionEjemplo(myInt); // función que le suma 3
        // Debug.Log("myInt= " + myInt);
    }

    int FuncionEjemplo(int miEntero)
    {
        return miEntero + 3;
    }

    // breakpoint, es decir, un punto de pausa. Es más bien un break de "tomarse un break".
    // Qué es lo que se va a pausar? pues la ejecución del programa TODA la ejecución del programa.
    // Para usarlos, solo hay que dar click izquierdo en el número de línea en el IDE cuando salga el circulito rojo.
    // Una vez puesto, hay que presionar: Attach to Unity Editor (en el bichito verde) y ahora sí, ir a Unity y
    // usar el proyecto/juego y hacer lo necesario para que se ejecute la línea de código donde está el breakpoint.
    // Cada vez que se ejecute dicha línea, el programa entero se detiene ahí, y podemos ver el estado y valores
    // de las variables disponibles en ese momento (en ese contexto).
    
    // Para continuar la ejecución después de un breakpoint, usa el botón de Continuar/Resume (botón verde de play en
    // el IDE, generalmente), o la tecla F10.
    // Para ir a la siguiente línea de código, usa F12.
    // Para entrar a la siguiente instrucción, por ejemplo, a la ejecución dentro de una función, usa F11,
    
    // Para desactivar un breakpoint, solo es necesario hacer click izquierdo sobre él otra vez, o click derecho y
    // deshabilitarlo, o presionar el botón rojo de Stop en el IDE (esta opción deshabilita todos los demás breakpoints
    // también, hasta que se vuelva a presionar el Attach to Unity Editor otra vez).
    
    // Es lo mismo que OnDrawGizmos, pero solo cuando el GameObject que tiene este script está seleccionado en el editor.
    protected void OnDrawGizmosSelected()
    {
        // Debug.Log("se está viendo la pestaña de Scene y este GameObject está seleccionado", gameObject);
    }
}
