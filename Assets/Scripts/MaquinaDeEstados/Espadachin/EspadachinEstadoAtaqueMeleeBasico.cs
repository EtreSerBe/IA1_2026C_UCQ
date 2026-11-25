using UnityEngine;

public class EspadachinEstadoAtaqueMeleeBasico : EstadoBase
{
    public override string Nombre => "EspadachinEstadoAtaqueMeleeBasico";
    private EnemigoEspadachin _context;
    
    public override void OnStart()
    {
        base.OnStart();
        
        if(!_owner.Context.TryGetComponent(out _context))
            Debug.LogError("El context gameObject no tiene un script de EnemigoEspadachin");
    }
    
    public override void OnEnter()
    {
        base.OnEnter();
        
        // Activa el trigger que hará que el animator haga la animación del ataque melee básico.
        _context.GetAnimator.SetTrigger("MeleeBasico");
    }

    public override void OnUpdate()
    {

    }

    public override void OnExit()
    {
        base.OnExit();
    }
}
