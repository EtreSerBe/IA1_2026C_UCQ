using UnityEngine;

public class EspadachinEstadoIdle : EstadoBase
{
    public override string Nombre => "EspadachinEstadoIdle";
    private EnemigoEspadachin _context;
    
    public override void OnStart()
    {
        if(!_owner.Context.TryGetComponent(out _context))
            Debug.LogError("El context gameObject no tiene un script de EnemigoEspadachin");
    }
    
    public override void OnEnter()
    {
        base.OnEnter();
    }


    public override void OnUpdate()
    {
        if (!_context.HaDetectadoAlPlayer) // NOTA este if ni siquiera sería necesario realmente como está ahorita
        {
            _context.DetectarJugador();
            if (_context.HaDetectadoAlPlayer)
            {
                _owner.CambiarEstado<EspadachinEstadoAtaqueMeleeBasico>();
                return; // siempre siempre siempre después de solicitar un Cambio de estado, mandamos a llamar Return.
            }
        }
        
        
    }

    public override void OnExit()
    {
        base.OnExit();
    }
    
    public override void DrawGizmosEstado()
    {
        Gizmos.DrawWireSphere(transform.position, _context.ObtenerRadioDeDeteccion());
        Gizmos.color = Color.lightGreen;
        Vector3 posicionEncimaDelPersonaje = transform.position + 2.0f * transform.up;
        Gizmos.DrawCube(posicionEncimaDelPersonaje, Vector3.one);
    }
}
