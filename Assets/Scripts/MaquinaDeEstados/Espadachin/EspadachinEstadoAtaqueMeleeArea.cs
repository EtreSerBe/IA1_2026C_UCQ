using UnityEngine;

public class EspadachinEstadoAtaqueMeleeArea : EstadoBase
{
    public override string Nombre => "EspadachinEstadoAtaqueMeleeArea";
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
        
        // Activa el trigger que hará que el animator haga la animación del ataque melee básico.
        _context.GetAnimator.SetTrigger("MeleeArea");
    }

    public override void OnUpdate()
    {
        
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}
