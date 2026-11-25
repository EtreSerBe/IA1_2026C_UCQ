using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EscapistaEstadoActivo : EstadoBase
{
    public override string Nombre => "EscapistaEstadoActivo";
    private EscapistaFSMNavMesh _context;
    

    [SerializeField] private float tiempoHastaElCansancio = 3.0f;
    private bool _estaHuyendo = false;
    
    [SerializeField] private float distanciaDeEscape = 5.0f;
    [SerializeField] private float tiempoEntreDisparos = 1.0f;
    [SerializeField] private float radioDeDeteccionDeHuida = 8.0f;


    private Coroutine _corrutinaDisparo;
    private Coroutine _corrutinaTiempoDeCansancio;

    private Vector3 _posicionHuidaObjetivo; // Nos sirve para dibujar un gizmo en caso de que fuera una posición no-válida.


    // Esto sí va en el start (a pesar de que es un estado) porque solo tiene que hacerse una vez.
    public override void OnStart()
    {
        if(!_owner.Context.TryGetComponent(out _context))
            Debug.LogError("El context gameObject no tiene un script de EscapistaFSMNavMesh");
    }
    
    public override void OnEnter()
    {
        Debug.Log("OnEnter del estado perrón: " + Nombre, gameObject);
        // StartCoroutine(CorrutinaCambiarACansado());
        
        // Medida de seguridad Y buena práctica. 
        if (_corrutinaDisparo != null) // Si no es null es porque está activa, y que detenerla antes de volverla a iniciar.
        {
            StopCoroutine(_corrutinaDisparo);
            _corrutinaDisparo = null;
        }
        

        
        _corrutinaDisparo = StartCoroutine(CorrutinaDisparo()); // en cuanto lo vemos, iniciamos el proceso continuo de disparo.
    }

    // Es OnUpdate para que no sea la Update normal del monobehaviour, ya que solo queremos que se actualicen los 
    // estados mediante el update de la máquina de estados (BaseFSM).
    // no deben de ser usadas por nadie, excepto por la clase BaseFSM.
    public override void OnUpdate()
    {
        // Debug.Log("OnUpdate del estado perrón: " + Nombre, gameObject);

         // Esta parte de que haya línea de visión solamente importa mientras el agente no está huyendo.
        if(!_estaHuyendo)
        {
            // checamos si la distancia entre la posición de este agente y la posición del jugador
            // es menor o igual que nuestro radio de detección para empezar a huir
            float distanciaAlPlayer = Vector3.Distance(transform.position, _context.ObtenerPosicionPlayerObjetivo());
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
            else if (!_context.HayLineaDeVisionDirecta) // Si no hay línea de visión,
            {
                // entonces ponemos la posición del player como destino de nuestro navMeshAgent.
                _context.AsignarDestino(_context.ObtenerPosicionPlayerObjetivo());
                _context.AsignarIsStopped(false);
            }
            else // si sí hay línea de visión, deja de moverse y continúa disparando
            {
                // _navMeshAgent.destination = transform.position; // esta antes era una manera confiable de detenerlo.
                _context.AsignarIsStopped(true);
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

    // Virtual es: esta función está hecha para que se PUEDA cambiar en las clases hijas, pero no es de a fuerzas.
    // no deben de ser usadas por nadie, excepto por la clase BaseFSM.
    public override void OnExit()
    {
        Debug.Log("OnExit del estado perrón: " + Nombre, gameObject);
        _estaHuyendo = false;

    }

    private IEnumerator CorrutinaTiempoDeCansancio()
    {
        yield return new WaitForSeconds(tiempoHastaElCansancio);

        // ponemos que su está es cansado
        // _estado = EEstados.Cansado;
        _owner.CambiarEstado<EscapistaEstadoCansado>();
        // SIEMPRE después de un cambio de estado, tienes que salirte inmediatamente de la función donde estés
        yield break;
        // Si no te salieras después del CambiarEstado que está arriba, aquí diría que sigues en el estado activo,
        // a pesar de que ya te saliste de él y entraste al cansado.
        Debug.Log("Estoy en el estado Activo");
    }
    
    // El enemigo siempre va a estar disparando mientras tenga un objetivo, independientemente de si está cansado o no.
    private IEnumerator CorrutinaDisparo()
    {
        while(true)
        {
            yield return new WaitForSeconds(tiempoEntreDisparos);
            
            _context.ActualizarLineaDeVision();
            
            // Si sí hay línea de visión ya podemos disparar (porque el temporizador de disparo está impuesto en la corrutina)
            if (_context.HayLineaDeVisionDirecta)
            {
                // entonces, disparamos (por ahora es el puro debug)
                // Debug.Log($"Disparando al Player: {_context._playerObjetivo.name}", gameObject);
                Debug.Log($"Disparando al Player", gameObject);
            }
        }
    }
    
    private void CalcularPosicionObjetivoDeHuida()
    {
        // Obtenemos la dirección que va desde el player hacia este agente (que sería la dirección opuesta de la que viene el player)
        Vector3 direccionDePlayerAAgente = (transform.position - _context.ObtenerPosicionPlayerObjetivo()).normalized;

        Vector3 posicionHuidaObjetivo = transform.position + distanciaDeEscape * direccionDePlayerAAgente;
        
        NavMeshHit navHit;
        // Si esa posición es válida dentro del navMesh, 
        if (NavMesh.SamplePosition(posicionHuidaObjetivo, out navHit, Mathf.Infinity, NavMesh.AllAreas))
        {
            // Entonces ponemos dicha posición como el objetivo del navmeshAgent
            _context.AsignarDestino(posicionHuidaObjetivo);
        }
        // si no fuera válida, podemos hacer varias cosas, pero por ahora simplemente voy a imprimir que no fue una posición válida
        else
        {
            Debug.LogWarning($"La posición: {posicionHuidaObjetivo} No fue una posición válida dentro del navMesh");
        }
    }

    public override void DrawGizmosEstado()
    {
        // Radio de huída
        Gizmos.color = Color.orange;
        Gizmos.DrawWireSphere(transform.position, radioDeDeteccionDeHuida);
        // Flecha hacia el player
        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position, _context.ObtenerPosicionPlayerObjetivo());
        if (_estaHuyendo)
        {
            // si estamos huyendo, necesitamos saber a qué posición estamos huyendo
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(_posicionHuidaObjetivo, 1.0f);
        }
        Gizmos.color = Color.red;
        Vector3 posicionEncimaDelPersonaje = transform.position + 2.0f * transform.up;
        Gizmos.DrawCube(posicionEncimaDelPersonaje, Vector3.one);
    }
}
