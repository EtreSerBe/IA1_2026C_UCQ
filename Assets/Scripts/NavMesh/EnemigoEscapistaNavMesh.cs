using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.AI;
using Vector3 = UnityEngine.Vector3;

public class EnemigoEscapistaNavMesh : EnemigoBase
{
    private enum EEstados
    {
        Idle, // Idle es cuando aún no ha detectado al player por primera vez. Por ejemplo, también podría ser que si el enemigo mata al player, podría volver a Idle
        Activo,
        Cansado
    }
    
    private NavMeshAgent _navMeshAgent;

    private GameObject _playerObjetivo;
    
    private bool _haDetectadoAlPlayer = false;
    private bool _hayLineaDeVisionDirecta = false;
    private bool _estaHuyendo = false;
    private EEstados _estado = EEstados.Idle;


    [SerializeField] private float distanciaDeEscape = 5.0f;
    [SerializeField] private float tiempoEntreDisparosActivo = 1.0f;
    [SerializeField] private float tiempoEntreDisparosCansado = 1.0f;
    [SerializeField] private float radioDeDeteccionDeHuida = 8.0f;
    [SerializeField] private float tiempoHastaElCansancio = 3.0f;
    [SerializeField] private float tiempoDeDescanso = 3.0f;

    
    private float _tiempoEntreDisparosActual;
    
    // Paredes, Obstáculos, Piso tal vez? Vehículos, etc. lo que sea que ustedes tengan en su juego que pudiera obstruir la línea visión
    public LayerMask capasQueObstruyenLineaDeVisionAlObjetivo;

    private Coroutine _corrutinaDisparo;
    private Coroutine _corrutinaTiempoDeCansancio;
    
    private Vector3 _posicionHuidaObjetivo; // Nos sirve para dibujar un gizmo en caso de que fuera una posición no-válida.
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        base.Start(); // Esto es mandar a llamar la función Start() de la clase padre, en este caso es EnemigoBase.
        // esto nos inicializa el Sentido de visión, y el collider propio.
        
        if (!TryGetComponent<NavMeshAgent>(out _navMeshAgent))
        {
            Debug.LogError("Error, no hay navmesh Agent asignado");
        }
        
        // Fuente: https://docs.unity.com/en-us/engine/6000.6/manual/working-with-gameobjects/layers/and-layermasks/layermask-add
        // Añadimos por código la capa de Jugador, ya que si no nuestro raycast no detectaría jugadores, y 
        // así evitamos que se le olvide a alguien poner dicha capa en el editor.
        capasQueObstruyenLineaDeVisionAlObjetivo |= (1 << LayerMask.NameToLayer("Jugador"));

        _tiempoEntreDisparosActual = tiempoEntreDisparosActivo; // por pura buena práctica.
    }

    // Update is called once per frame
    void Update()
    {
        // La detección del player es independiente de si este agente está cansado o no.
        // Específicamente, yo voy a hacer que este enemigo NO sepa la ubicación del player al comenzar, sino que
        // lo tenga que detectar al menos una primera vez mediante el SentidoDeVisión.
        // Por esto, una vez lo haya detectado, ya no tengo que actualizar el sentido de visión.

        // dependiendo del estado en el que esté, es lo que va a hacer
        switch (_estado)
        {
            case EEstados.Idle:
                if (!_haDetectadoAlPlayer) // NOTA este if ni siquiera sería necesario realmente como está ahorita
                {
                    DetectarJugador();
                }
                break;
            case EEstados.Activo:
                UpdateEstadoActivo();
                break;
            case EEstados.Cansado:
                UpdateEstadoCansado();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        
    }
    
    // El enemigo siempre va a estar disparando mientras tenga un objetivo, independientemente de si está cansado o no.
    private IEnumerator CorrutinaDisparo()
    {
        while(true)
        {
            yield return new WaitForSeconds(_tiempoEntreDisparosActual);
            
            ActualizarLineaDeVision();
            
            // Si sí hay línea de visión ya podemos disparar (porque el temporizador de disparo está impuesto en la corrutina)
            if (_hayLineaDeVisionDirecta)
            {
                // entonces, disparamos (por ahora es el puro debug)
                Debug.Log($"Disparando al Player: {_playerObjetivo.name}", gameObject);
            }
        }
    }
    
    private void UpdateEstadoActivo()
    {
        // Esta parte de que haya línea de visión solamente importa mientras el agente no está huyendo.
        if(!_estaHuyendo)
        {
            // checamos si la distancia entre la posición de este agente y la posición del jugador
            // es menor o igual que nuestro radio de detección para empezar a huir
            float distanciaAlPlayer = Vector3.Distance(transform.position, _playerObjetivo.transform.position);
            if (distanciaAlPlayer <= radioDeDeteccionDeHuida)
            {
                // entonces está dentro del radio, y nosotros comenzamos a huir
                _estaHuyendo = true;
                // comenzamos la corrutina de cansancio.
                // Medida de seguridad Y buena práctica. 
                if (_corrutinaTiempoDeCansancio != null) // Si no es null es porque está activa, y que detenerla antes de volverla a iniciar.
                {
                    StopCoroutine(_corrutinaTiempoDeCansancio);
                    _corrutinaTiempoDeCansancio = null;
                }
                _corrutinaTiempoDeCansancio = StartCoroutine(CorrutinaTiempoDeCansancio());
                
                // Obtenemos la posición objetivo
                CalcularPosicionObjetivoDeHuida();
            }
            else if (!_hayLineaDeVisionDirecta) // Si no hay línea de visión,
            {
                // entonces ponemos la posición del player como destino de nuestro navMeshAgent.
                _navMeshAgent.destination = _playerObjetivo.transform.position;
                _navMeshAgent.isStopped = false;
            }
            else // si sí hay línea de visión, deja de moverse y continúa disparando
            {
                // _navMeshAgent.destination = transform.position; // esta antes era una manera confiable de detenerlo.
                _navMeshAgent.isStopped = true;
            }
        }
        else // si sí está huyendo
        {
            // Lo único que nos importa es llegar (o movernos hacia) esa posición de huida que pusimos como objetivo.
            // Sobre el objetivo de huída:
            // Solo se pone una vez, cuando el player entra en el radio de huída Y no teníamos ya un punto de huída vigente
            // y no está cansado el agente.
            // ese objetivo de huída se borra en cuanto el agente se cansa.
        }
    }

    private void UpdateEstadoCansado()
    {
        // No se mueve
        
        // podemos voltear hacia donde está el player, como para no quedarnos de espaldas a él, pero ahorita no lo voy a poner.
    }

    private IEnumerator CorrutinaTiempoDeCansancio()
    {
        yield return new WaitForSeconds(tiempoHastaElCansancio);

        // ponemos que su está es cansado
        _estado = EEstados.Cansado;
        // podemos hacer que el tiempo de disparo sea más lento cuando está cansado
        _tiempoEntreDisparosActual = tiempoEntreDisparosCansado;
        // _navMeshAgent.destination = transform.position;
        _navMeshAgent.isStopped = true;
        _estaHuyendo = false;

        yield return new WaitForSeconds(tiempoDeDescanso);
        
        // Lo regresamos al valor de activo, pues ya descansó.
        _estado = EEstados.Activo;
        _tiempoEntreDisparosActual = tiempoEntreDisparosActivo;
        _navMeshAgent.destination = transform.position; // seguridad extra para que ya no se mueva hasta que se le diga.
        _navMeshAgent.isStopped = false; // quiere decir que me puedo mover de nuevo cuando me lo pidan

        // la ponemos explícitamente en null por seguridad. (Según yo no es necesario, pero por si las dudas).
        _corrutinaTiempoDeCansancio = null;
    }
    
    private void CalcularPosicionObjetivoDeHuida()
    {
        // Obtenemos la dirección que va desde el player hacia este agente (que sería la dirección opuesta de la que viene el player)
        Vector3 direccionDePlayerAAgente = (transform.position - _playerObjetivo.transform.position).normalized;

        Vector3 posicionHuidaObjetivo = transform.position + distanciaDeEscape * direccionDePlayerAAgente;
        
        NavMeshHit navHit;
        // Si esa posición es válida dentro del navMesh, 
        if (NavMesh.SamplePosition(posicionHuidaObjetivo, out navHit, Mathf.Infinity, NavMesh.AllAreas))
        {
            // Entonces ponemos dicha posición como el objetivo del navmeshAgent
            _navMeshAgent.destination = posicionHuidaObjetivo;
        }
        // si no fuera válida, podemos hacer varias cosas, pero por ahora simplemente voy a imprimir que no fue una posición válida
        else
        {
            Debug.LogWarning($"La posición: {posicionHuidaObjetivo} No fue una posición válida dentro del navMesh");
        }
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;

        switch (_estado)
        {
            case EEstados.Idle: // dibujamos el rango de detección
            {
                Gizmos.DrawWireSphere(transform.position, SentidoDeVision.GetRadioDeColliderDeDeteccion());
                Gizmos.color = Color.lightGreen;
                Vector3 posicionEncimaDelPersonaje = transform.position + 2.0f * transform.up;
                Gizmos.DrawCube(posicionEncimaDelPersonaje, Vector3.one);
                break;
            }
            case EEstados.Activo:
            {
                // Radio de huída
                Gizmos.color = Color.orange;
                Gizmos.DrawWireSphere(transform.position, radioDeDeteccionDeHuida);
                // Flecha hacia el player
                Gizmos.color = Color.white;
                Gizmos.DrawLine(transform.position, _playerObjetivo.transform.position);
                if (_estaHuyendo)
                {
                    // si estamos huyendo, necesitamos saber a qué posición estamos huyendo
                    Gizmos.color = Color.magenta;
                    Gizmos.DrawSphere(_posicionHuidaObjetivo, 1.0f);
                }
                Gizmos.color = Color.red;
                Vector3 posicionEncimaDelPersonaje = transform.position + 2.0f * transform.up;
                Gizmos.DrawCube(posicionEncimaDelPersonaje, Vector3.one);
                break;
            }
            case EEstados.Cansado:
            {
                Gizmos.color = Color.lightGray;
                Vector3 posicionEncimaDelPersonaje = transform.position + 2.0f * transform.up;
                Gizmos.DrawCube(posicionEncimaDelPersonaje, Vector3.one);
                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void DetectarJugador()
    {
        List<GameObject> objetosConocidos = SentidoDeVision.GetObjetosConocidos();
        foreach (var objeto in objetosConocidos)
        {
            if(objeto.layer == LayerMask.NameToLayer("Jugador"))
            {
                // si sí es igual, entonces este objeto es un Player, y ya lo habremos detectado
                _haDetectadoAlPlayer = true;
                _playerObjetivo = objeto;
                _estado = EEstados.Activo; // hacemos que deje de estar tranquilo (idle) y pasa a Activo.

                // Medida de seguridad Y buena práctica. 
                if (_corrutinaDisparo != null) // Si no es null es porque está activa, y que detenerla antes de volverla a iniciar.
                {
                    StopCoroutine(_corrutinaDisparo);
                    _corrutinaDisparo = null;
                }

                _corrutinaDisparo = StartCoroutine(CorrutinaDisparo()); // en cuanto lo vemos, iniciamos el proceso continuo de disparo.
                break;
            }
        }
    }
    
    private void ActualizarLineaDeVision()
    {
        // la corrutina puede continuar siempre,
        // y nada más que se salte un disparo si no tiene línea de visión directa hacia el player (podemos usar un raycast)
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
}
