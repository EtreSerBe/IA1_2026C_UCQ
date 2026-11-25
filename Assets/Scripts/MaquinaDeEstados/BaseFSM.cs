using System;
using System.Collections.Generic;
using UnityEngine;

public class BaseFSM : MonoBehaviour
{
    [SerializeField] private GameObject _context;
    public GameObject Context => _context;
    
    
    // Estado inicial que se asigna en el editor.
    [SerializeField] private EstadoBase estadoInicial;
    private EstadoBase _estadoActual;

    [SerializeField] private List<EstadoBase> estados = new List<EstadoBase>();
    
    private Dictionary<Type, EstadoBase> _dicEstados = new Dictionary<Type, EstadoBase>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (_context == null)
        {
            Debug.LogError("El context es null", gameObject);
            return;
        }
        
        // Si dentro de los estados disponibles NO está el estado inicial, pues es un error
        if (!estados.Contains(estadoInicial))
        {
            Debug.LogError($"No está el estado inicial: {estadoInicial.GetType()} en los estados de la FSM", gameObject);
            return;
        }

        foreach (var estado in estados)
        {
            estado.AsignarOwner(this); // le decimos a este estado que esta máquina de estados es su dueña.
            estado.OnStart(); // los inicializamos cuando ya sabemos que es seguro porque la FSM ya está inicializada.
            _dicEstados.Add(estado.GetType(), estado); // los metemos al diccionario para identificarlos por su tipo.
        }
        
        _estadoActual = estadoInicial;
        _estadoActual.OnEnter();

    }

    // Update is called once per frame
    void Update()
    {
        // Llamar el update del estado actual de la máquina
        _estadoActual.OnUpdate();
    }
    
    // CambiarDeEstado.  
    // La <T> es que solo le pasemos el Tipo del estado, no una instancia o referencia a un estado de dicha clase en sí,
    // porque la máquina de estados ya lo debe de conocer, es más, debe de ser su dueña.
    public void CambiarEstado<T>() where T : EstadoBase
    {
        // Tenemos que checar que la máquina tiene un estado de la clase T solicitada.
        if (!_dicEstados.ContainsKey(typeof(T)))
        {
            // Si no tiene de dicho estado, entonces hay un error.
            Debug.LogError($"Se intentó cambiar a un estado de tipo: {typeof(T)} " +
                           $"pero esta FSM no lo contiene, se ignoró este cambio de estado a partir del" +
                           $" estado actual {_estadoActual.GetType()}", gameObject);
            return;
        }

        EstadoBase nuevoEstado = _dicEstados[typeof(T)];
        
        // Salir del estado actual;
        _estadoActual.OnExit();
        
        // Cambiar el estado actual por el nuevo
        _estadoActual = nuevoEstado;

        // entrar al estado nuevo
        _estadoActual.OnEnter();
    }

    protected void OnDrawGizmos()
    {
        if(_estadoActual != null)
            // Lo único que hace es mandar a llamar el DrawGizmos del estado actual.
            _estadoActual.DrawGizmosEstado();
    }

    protected void OnGUI()
    {
        string content = _estadoActual == null ? "NO CURRENT STATE" : _estadoActual.Nombre;
        GUILayout.Label($"<color='black'><size=40>{content}</size></color>");
    }
}
