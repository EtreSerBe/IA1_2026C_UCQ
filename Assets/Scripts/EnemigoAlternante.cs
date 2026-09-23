using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

// Para más explicaciones sobre corrutinas, este video está cortito y muy útil:
// https://youtu.be/kUP6OK36nrM?si=qSSAzcoA13nC6j8m

public class EnemigoAlternante : EnemigoBase
{
    
    private bool _estaUsandoSeek = true;
    [SerializeField] private float tiempoParaCambiarEntreSeekYFlee = 2.0f;
    private float _tiempoTranscurrido = 0.0f;

    private Coroutine _coroutineImprimirCada5SegundosHastaQueMeDetengan;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // base.Start quiere decir: Haz TODO lo que hace la función de la clase padre
        base.Start();

        StartCoroutine(ImprimirCadaSegundoDuranteDiezSegundos()); // Mejor forma de hacerlo
        // StartCoroutine(nameof(ImprimirCadaSegundoDuranteDiezSegundos)); // aceptable pero no la mejor
        // StartCoroutine("ImprimirCadaSegundoDuranteDiezSegundos"); // trata de evitarla, usa mejor las de arriba.

        // esta variable tiene una referencia a esa corrutina que se comenzó aquí, y con esta variable tú puedes
        // detener manualmente dicha corrutina.
        if(_coroutineImprimirCada5SegundosHastaQueMeDetengan == null) // solo si sí es nula comienzo esta corrutina.
                                                                      // Esto sirve para evitar que haya más de una
                                                                      // instancia de dicha corrutina ejecutándose al mismo tiempo.  
            _coroutineImprimirCada5SegundosHastaQueMeDetengan = StartCoroutine(
            ImprimirCada5SegundosHastaQueMeDetengan()); // Mejor forma de hacerlo

        
        // Este ejemplo muestra cómo todos los Start, Update, etc. de Unity están en el "Hilo" o "rutina" principal.
        // por lo que si tú detienes el hilo principal TODO pero absolutamente TODO lo de dicho hilo se detiene.
        // Las corrutinas combaten justo este problema, ejecutándose en otro hilo separado.
        // for (int i = 0; i < 10; i++)
        // {
        //     Thread.Sleep(1000);
        //     Debug.Log($"Han pasado: {i} segundos");
        // }
    }

    // Update is called once per frame
    void Update()
    {
        // Le pedimos al sentido de visión los objetos que conoce, y de esos nosotros vamos a perseguir a uno, si es que hay
        List<GameObject> objetosConocidos = SentidoDeVision.GetObjetosConocidos();
        // si la lista no está vacía, es que hay al menos un conocido, y entonces yo lo puedo poner como mi objetivo.
        if (objetosConocidos.Count > 0)
        {
            AlternarSeekYFlee();
            
            Objetivo = objetosConocidos[0];
            
            // Documento original de los steering behaviors: https://www.red3d.com/cwr/papers/1999/gdc99steer.pdf

            Vector3 steeringForce;
            if (_estaUsandoSeek)
            {
                steeringForce = Seek();
            }
            else
            {
                steeringForce = Flee();
            }
            
            ActualizarAceleracionVelocidadYPosicion(steeringForce);
        }



    }
    
    // Una corrutina sería una rutina adicional... ¿pero cuál es la rutina normal?
    // son flujos adicionales para no depender del flujo normal de los updates.

    // La principal fortaleza (y uso) es para manejar cosas que requieren del manejo de Tiempos, 
    // por ejemplo, cosas que se hacen periódicamente, cosas que necesitan una espera, o cosas que necesitan un órden,
    // especialmente si ese órden involucra tiempos de espera.
    private IEnumerator ImprimirCadaSegundoDuranteDiezSegundos()
    {
        for (int i = 0; i < 10; i++)
        {
            yield return new WaitForSeconds(1.0f);
            Debug.Log($"Han pasado: {i} segundos");
        }
        
        // ya que pasaron los diez segundos, detén la corrutina de ImprimirCada5SegundosHastaQueMeDetengan.
        // para ello uso StopCoroutine y le paso la variable que asigné cuando llamé el StartCoroutine.
        StopCoroutine(_coroutineImprimirCada5SegundosHastaQueMeDetengan);
        _coroutineImprimirCada5SegundosHastaQueMeDetengan = null; // y por buena práctica, conviene hacer dicha variable null tras detenerla.

        // yield, la traducción que corresponde aquí es: "ceder", porque "cede" el paso durante cierto "tiempo" o evento.

        // Por ejemplo:
        // yield return null; // es simplemente: continúa desde este punto el frame siguiente. Entonces cede la ejecución un frame.

        // yield break; //  — Termina la corrutina inmediatamente antes de llegar al final del método

        // yield return new WaitForSeconds(X); // detiene la ejecución de este código (es decir, el que está en esta corrutina)
        // por un dado tiempo X, y cuando se acaba dicho tiempo, continúa en la línea siguiente.

    }

    // Una corrutina se puede detener manualmente también.
    private IEnumerator ImprimirCada5SegundosHastaQueMeDetengan()
    {
        int tiempoTranscurrido = 0;
        while (true)
        {
            yield return new WaitForSeconds(5.0f);
            tiempoTranscurrido+=5;
            Debug.Log($"Han pasado: {tiempoTranscurrido} segundos");
        }
    } // el compilador nos advierte: oye, esta función nunca va a terminar, ten cuidado. No quiere decir que esté mal, solo 
    // te advierte para que te asegures de que está bien lo que estás haciendo.
    


    void AlternarSeekYFlee()
    {
        // el tiempo solo transcurre si sí está detectando un objetivo.
        _tiempoTranscurrido += Time.deltaTime;
        if (_tiempoTranscurrido >= tiempoParaCambiarEntreSeekYFlee)
        {
            _estaUsandoSeek = !_estaUsandoSeek; // invertimos el valor
            _tiempoTranscurrido = 0.0f; // reseteamos este para esperar otros X segundos.
        }
    }
}
