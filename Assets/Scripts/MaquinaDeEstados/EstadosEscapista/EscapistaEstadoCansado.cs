using System;
using System.Collections;
using UnityEngine;

public class EscapistaEstadoCansado : EstadoBase
{
    public override string Nombre => "EscapistaEstadoCansado";
    private EscapistaFSMNavMesh _context;

    
    [SerializeField] private float tiempoEntreDisparos = 1.0f;
    [SerializeField] private float tiempoDeDescanso = 3.0f;

    private Coroutine _corrutinaDisparo;

    public override void OnStart()
    {
        if(!_owner.Context.TryGetComponent(out _context))
            Debug.LogError("El context gameObject no tiene un script de EscapistaFSMNavMesh");
    }

    public override void OnEnter()
    {
        base.OnEnter(); // mandar a llamar el OnEnter tal cual de la clase padre.
        
        // _navMeshAgent.destination = transform.position;
        _context.AsignarIsStopped(true);
        
        StartCoroutine(CorrutinaCambiarAActivo());
    }

    public override void OnExit()
    {
        base.OnExit();
        
        _context.AsignarDestino(transform.position); // seguridad extra para que ya no se mueva hasta que se le diga.
        _context.AsignarIsStopped(false); // quiere decir que me puedo mover de nuevo cuando me lo pidan
    }

    IEnumerator CorrutinaCambiarAActivo()
    {
        yield return new WaitForSeconds(tiempoDeDescanso);
        
        // ahora sí viene el cambio de estado, necesitamos pedirle a la máquina de estados que cambie al nuevo estado.
        _owner.CambiarEstado<EscapistaEstadoActivo>();
    }

    public override void DrawGizmosEstado()
    {
        Gizmos.color = Color.lightGray;
        Vector3 posicionEncimaDelPersonaje = transform.position + 2.0f * transform.up;
        Gizmos.DrawCube(posicionEncimaDelPersonaje, Vector3.one);
    }

}
