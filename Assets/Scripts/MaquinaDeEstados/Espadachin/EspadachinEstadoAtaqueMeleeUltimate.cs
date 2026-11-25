using UnityEngine;

public class EspadachinEstadoAtaqueMeleeUltimate : EstadoBase
{
    public override string Nombre => "EspadachinEstadoAtaqueMeleeUltimate";
    private EnemigoEspadachin _context;
    
    public override void OnStart()
    {
        base.OnStart();
        if(!_owner.Context.TryGetComponent(out _context))
            Debug.LogError("El context gameObject no tiene un script de EnemigoEspadachin", gameObject);

    }
    
    public override void OnEnter()
    {
        base.OnEnter();
    }


    public override void OnUpdate()
    {
        
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}