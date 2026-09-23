using UnityEngine;

public class SteeringBehaviors 
{
    public static Vector3 Seek(Vector3 objetivo, Vector3 posicionActual, 
        float maximaVelocidadParam, Vector3 velocidadActual, float maximaFuerzaParam)
    {
        // punta menos cola desde mí hacia el objetivo
        Vector3 puntaMenosCola = objetivo - posicionActual; // Seek
        
        // Para obtener la pura dirección de una flecha, usamos la Normalización
        // También se le llama Steering Direction.
        Vector3 direccionDeseada = puntaMenosCola.normalized;
            
        // Esta flecha me dice: voy hacia mi objetivo lo más rápido que puedo.
        Vector3 velocidadDeseada = direccionDeseada * maximaVelocidadParam;
        
        // Steering = velocidad deseada – velocidad actual
        Vector3 steeringVelocity = velocidadDeseada - velocidadActual;
            
        // SteeringForce es truncate(direccionDeseada, maximaFuerza)
        Vector3 steeringForce = Vector3.ClampMagnitude(steeringVelocity, maximaFuerzaParam);

        return steeringForce;
    }

    public static Vector3 Flee(Vector3 objetivo, Vector3 posicionActual,
        float maximaVelocidad, Vector3 velocidadActual, float maximaFuerza)
    {
        // Lo mismo que seek, pero en la dirección opuesta.
        return Seek(objetivo, posicionActual, maximaVelocidad, velocidadActual, maximaFuerza) * -1f;
    }
    
    public static Vector3 ActualizarAceleracionVelocidadYPosicion(Vector3 steeringForce, float masaParam, 
        float maximaVelocidad, ref Vector3 velocidadActual, Vector3 posicion)
    {
        Vector3 aceleracion = steeringForce / masaParam;

        velocidadActual = Vector3.ClampMagnitude(velocidadActual + aceleracion * Time.deltaTime,
            maximaVelocidad);
        
        return posicion + velocidadActual * Time.deltaTime;
    }
    
    // en C++ podríamos usar & para pasar un parámetro "por referencia", que quiere decir: los cambios que sufra 
    // dicha variable dentro del método permanecerán aún tras salir de dicho método.
    
}
