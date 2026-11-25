using UnityEngine;

/*
 * NOTA: A pesar de que las funciones OnEnter, OnUpdate y OnExit son públicas, no deben de ser usadas por nadie,
 * excepto por la clase BaseFSM.
 * 
 */
public class EstadoBase : MonoBehaviour
{
    // La máquina de estados que es dueña de este estado.
    protected BaseFSM _owner;

    public void AsignarOwner(BaseFSM owner)
    {
        _owner = owner;
    }

    public virtual string Nombre => "EstadoBase";
    
    public virtual void OnStart()
    {
        Debug.Log("OnStart del estado: " + Nombre, gameObject);
    }
    
    // no deben de ser usadas por nadie, excepto por la clase BaseFSM.
    public virtual void OnEnter()
    {
        Debug.Log("OnEnter del estado: " + Nombre, gameObject);
    }

    // Es OnUpdate para que no sea la Update normal del monobehaviour, ya que solo queremos que se actualicen los 
    // estados mediante el update de la máquina de estados (BaseFSM).
    // no deben de ser usadas por nadie, excepto por la clase BaseFSM.
    public virtual void OnUpdate()
    {
        // Debug.Log("OnUpdate del estado: " + Nombre, gameObject);

    }

    // Virtual es: esta función está hecha para que se PUEDA cambiar en las clases hijas, pero no es de a fuerzas.
    // no deben de ser usadas por nadie, excepto por la clase BaseFSM.
    public virtual void OnExit()
    {
        Debug.Log("OnExit del estado: " + Nombre, gameObject);
    }

    public virtual void DrawGizmosEstado()
    {
        
    }
}
