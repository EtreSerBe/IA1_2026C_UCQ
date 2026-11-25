using UnityEngine;

public class EscapistaEstadoIdle : EstadoBase
{
    public override string Nombre => "EscapistaEstadoIdle";
    private EscapistaFSMNavMesh _context;
    
    public override void OnStart()
    {
        if(!_owner.Context.TryGetComponent(out _context))
            Debug.LogError("El context gameObject no tiene un script de EscapistaFSMNavMesh");

    }

    // Update is called once per frame
    public override void OnUpdate()
    {
        if (!_context.HaDetectadoAlPlayer) // NOTA este if ni siquiera sería necesario realmente como está ahorita
        {
            _context.DetectarJugador();
        }
    }

    public override void DrawGizmosEstado()
    {
        Gizmos.DrawWireSphere(transform.position, _context.ObtenerRadioDeDeteccion());
        Gizmos.color = Color.lightGreen;
        Vector3 posicionEncimaDelPersonaje = transform.position + 2.0f * transform.up;
        Gizmos.DrawCube(posicionEncimaDelPersonaje, Vector3.one);
    }
}
