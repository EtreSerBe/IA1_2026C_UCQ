using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemigoBaseFSM : MonoBehaviour
{
    [Header("Propiedades de Combate")]
    
    [SerializeField] protected int Vida = 5;
    
    // Si hacen (o reciben) daño por contacto, entonces deben ser capaces de detectar contacto.
    [SerializeField] protected int DanioDeContacto = 1;
    
    // Es el collider que representa el "cuerpo" de este enemigo. Es decir, si este collider hace contacto 
    // con el player entonces le hace daño; si el arma del player entra en contacto contigo entonces te hace daño; etc. 
    [SerializeField] protected Collider ColliderPropio;
    
        
    protected SentidoDeVision SentidoDeVision;
    protected GameObject _playerObjetivo;
    
    protected BaseFSM _fsm;
    
    protected NavMeshAgent _navMeshAgent;
    // es el que se usa para modificar las variables (parameters) del animation controller,
    // o parar o iniciar animaciones por código, entre otras.
    protected Animator animator;
    public Animator GetAnimator => animator;
    

    protected bool _haDetectadoAlPlayer = false;
    public bool HaDetectadoAlPlayer => _haDetectadoAlPlayer;
    protected bool _hayLineaDeVisionDirecta = false;
    public bool HayLineaDeVisionDirecta => _hayLineaDeVisionDirecta;
    public LayerMask capasQueObstruyenLineaDeVisionAlObjetivo;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
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
        
        if (!TryGetComponent<NavMeshAgent>(out _navMeshAgent))
        {
            Debug.LogError("Error, no hay navmesh Agent asignado", gameObject);
        }
        
        if (!TryGetComponent(out _fsm))
        {
            Debug.LogError("Error, no hay FSM asignada", gameObject);
        }
        
        if (!TryGetComponent(out animator))
        {
            Debug.LogError("Error, no hay animator asignado", gameObject);
        }
        
        capasQueObstruyenLineaDeVisionAlObjetivo |= (1 << LayerMask.NameToLayer("Jugador"));
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        ActualizarObjetivo();

    }
    
    protected void ActualizarObjetivo()
    {
        // Le pedimos al sentido de visión los objetos que conoce, y de esos nosotros vamos a perseguir a uno, si es que hay
        List<GameObject> objetosConocidos = SentidoDeVision.GetObjetosConocidos();
        // si la lista no está vacía, es que hay al menos un conocido, y entonces yo lo puedo poner como mi objetivo.
        if (objetosConocidos.Count > 0)
        {
            _playerObjetivo = objetosConocidos[0];
        }
        else // Nos faltaba el otro lado de la moneda: cuando ya no ves a nadie, entonces ya no tienes objetivo!
        {
            _playerObjetivo = null;
        }
    }
    
    
    // POR SI LAS DUDAS ME LA TRAJE.
    // Esto ya hace que todas las clases que sean EnemigoBase o hereden de ella, pierdan 1 de vida cuando entren en 
    // contacto con algo en la capa de "AtaqueDeJugador".
    protected virtual void OnCollisionEnter(Collision other)
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
    
     public void AsignarDestino(Vector3 posicionDestino)
    {
        _navMeshAgent.destination = posicionDestino;
    }

    public void AsignarIsStopped(bool estaDetenido)
    {
        _navMeshAgent.isStopped = estaDetenido;
    }

    public Vector3 ObtenerPosicionPlayerObjetivo()
    {
        return _playerObjetivo.transform.position;
    }
    
    public void DetectarJugador()
    {
        List<GameObject> objetosConocidos = SentidoDeVision.GetObjetosConocidos();
        foreach (var objeto in objetosConocidos)
        {
            if(objeto.layer == LayerMask.NameToLayer("Jugador"))
            {
                // si sí es igual, entonces este objeto es un Player, y ya lo habremos detectado
                _haDetectadoAlPlayer = true;
                _playerObjetivo = objeto;
                
                // OJO, al hacerlo genérico, y no solo del escapista, ya no puede hacer esta transición desde aquí,
                // la tiene qu hacer el estado donde se mande a llamar esta función o el context.
                // _fsm.CambiarEstado<EscapistaEstadoActivo>(); // hacemos que deje de estar tranquilo (idle) y pasa a Activo.
                
               break;
            }
        }
    }
    
    public void ActualizarLineaDeVision()
    {
        Vector3 direccionDeRayo = (_playerObjetivo.transform.position - transform.position).normalized;
        Ray ray = new Ray(transform.position, direccionDeRayo);
        RaycastHit rayHit;
        if (Physics.Raycast(ray, out rayHit, Vector3.Distance(transform.position, _playerObjetivo.transform.position),
                capasQueObstruyenLineaDeVisionAlObjetivo))
        {
            // si sí chocó contra algo el rayo, hay que checar contra qué fue. 
            // Si fue un objeto en la capa Jugador, entonces SÍ tenemos línea de visión directa, y si no, no.
            if (rayHit.collider.gameObject.layer == LayerMask.NameToLayer("Jugador"))
            {
                // Sí tenemos línea de visión directa.
                _hayLineaDeVisionDirecta = true;
            }
            else
            {
                _hayLineaDeVisionDirecta = false;
            }

            // Esta sería la versión del if-else de arriba, en una sola línea.
            // _hayLineaDeVisionDirecta = rayHit.collider.gameObject.layer == LayerMask.NameToLayer("Jugador");
        }
    }

    public float ObtenerRadioDeDeteccion()
    {
        return SentidoDeVision.GetRadioDeColliderDeDeteccion();
    }
}
