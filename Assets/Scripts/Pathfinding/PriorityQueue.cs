using System;
using System.Collections.Generic;
using UnityEngine;


public class PriorityQueue
{
    private LinkedList<Nodo> _listaLigada = new LinkedList<Nodo>();
    
    // Cada que nosotros pongamos o quitemos un elemento en la lista ligada, tiene que ser con base en una prioridad
    // dada, no solamente la variable de Distancia del nodo.

    // Es el enqueue que se usa para Best-first search
    public void Enqueue(Nodo nodoAInsertar)
    {
        if (_listaLigada.Count == 0)
        {
            _listaLigada.AddFirst(nodoAInsertar);
            return;
        }
        
        // Buscar el nodo en la lista que tenga una prioridad menor (más alta) que el nodo a insertar.
        LinkedListNode<Nodo> actual = _listaLigada.First;
        while (actual != null) // Corrección, era comparar que no sea null, para que visite a TODOS los nodos, incluso al último
        {
            // si la prioridad de este elemento es más alta que la mía, entonces este nodo va antes que él
            if (actual.Value.ObtenerPrioridad() > nodoAInsertar.ObtenerPrioridad())
            {
                // entonces yo inserto a este nodo aquí y ya acabé la inserción
                _listaLigada.AddBefore(actual, new LinkedListNode<Nodo>(nodoAInsertar));
                return;
            }

            actual = actual.Next; // si no lo hemos encontrado, avanzamos al siguiente nodo
        } 
        
        // si se acaba el while, es porque este nodo tiene la peor prioridad de todos, y se inserta al final
        _listaLigada.AddLast(nodoAInsertar);
    }
    
    public void EnqueueDjikstra(Nodo nodoAInsertar)
    {
        if (_listaLigada.Count == 0)
        {
            _listaLigada.AddFirst(nodoAInsertar);
            return;
        }
        
        // Buscar el nodo en la lista que tenga una prioridad menor (más alta) que el nodo a insertar.
        LinkedListNode<Nodo> actual = _listaLigada.First;
        while (actual != null) // Corrección, era comparar que no sea null, para que visite a TODOS los nodos, incluso al último
        {
            // si la prioridad de este elemento es más alta que la mía, entonces este nodo va antes que él
            if (actual.Value.ObtenerPrioridadDjikstra() > nodoAInsertar.ObtenerPrioridadDjikstra())
            {
                // entonces yo inserto a este nodo aquí y ya acabé la inserción
                _listaLigada.AddBefore(actual, new LinkedListNode<Nodo>(nodoAInsertar));
                return;
            }

            actual = actual.Next; // si no lo hemos encontrado, avanzamos al siguiente nodo
        } 
        
        // si se acaba el while, es porque este nodo tiene la peor prioridad de todos, y se inserta al final
        _listaLigada.AddLast(nodoAInsertar);
    }
    
    // No necesitamos una función para quitar elementos por búsqueda porque esta es una Queue, entonces solo
    // necesitamos poder quitar el de hasta el frente.
    public Nodo Dequeue()
    {
        Nodo frente = _listaLigada.First.Value; // primero lo guardamos en una variable para que no se borre al sacarlo de la lista
        _listaLigada.RemoveFirst(); // ahora sí lo borramos
        return frente; // lo regresamos.
    }

    public bool BorrarNodo(Nodo nodo)
    {
        LinkedListNode<Nodo> nodoEncontrado = _listaLigada.Find(nodo);
        if (nodoEncontrado == null)
            return false;

        _listaLigada.Remove(nodoEncontrado);
        return true;
    }

    public int Count => _listaLigada.Count;

    public void Imprimir()
    {
        string texto = "";
        foreach (var nodo in _listaLigada)
        {
            texto += $"Prio: {nodo.ObtenerPrioridad()} -> nodo: {nodo.ID} ; " + "\n";
        }
        Debug.Log(texto);
    }
    
    public Nodo Buscar(Nodo nodo)
    {
        LinkedListNode<Nodo> resultado = _listaLigada.Find(nodo);
        if (resultado != null)
        {
            return resultado.Value;
        }

        return null;
    }

    public bool Contiene(Nodo nodo)
    {
        return _listaLigada.Contains(nodo);
    }

    public void Limpiar()
    {
        _listaLigada.Clear();
    }
    
}
