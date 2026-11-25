using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EscapistaFSMNavMesh : EnemigoBaseFSM
{

    private float _tiempoEntreDisparosActual;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();

    }

    // Update is called once per frame
    protected override void Update()
    {
        // Aquí podríamos controlar "manualmente" el update de la FSM si le quitáramos su Update y se lo cambiáramos por
        // una función, digamos, OnUpdate().
        //_FSM.OnUpdate();
        
        // Pero ahorita no haremos eso, la FSM se actualizará solita con su Update de monobehaviour.
    }

   
}
