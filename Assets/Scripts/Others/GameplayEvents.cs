using UnityEngine;
using UnityEngine.Events;

public static class GameplayEvents
{
    public static UnityEvent takeOff = new();
    public static UnityEvent startFlying = new();
}
