// Mock references for UnityEngine classes
// Used for Docker compilation when RimWorld assemblies are not available

using System;

namespace UnityEngine
{
    public struct Rect
    {
        public float x;
        public float y;
        public float width;
        public float height;
        
        public Rect(float x, float y, float width, float height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }
    }

    public struct Vector2
    {
        public float x;
        public float y;
        
        public Vector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
        
        public static Vector2 zero => new Vector2(0, 0);
    }

    public struct Vector3
    {
        public float x;
        public float y;
        public float z;
        
        public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        
        public static Vector3 zero => new Vector3(0, 0, 0);
    }

    public class MonoBehaviour
    {
        public virtual void Start() { }
        public virtual void Update() { }
        public virtual void OnDestroy() { }
    }

    public class GameObject
    {
        public string name;
        public bool activeInHierarchy { get; set; }
        
        public GameObject(string name)
        {
            this.name = name;
        }
        
        public T GetComponent<T>() where T : class { return null; }
        public T AddComponent<T>() where T : class, new() { return new T(); }
    }

    public static class Time
    {
        public static float time { get; set; }
        public static float deltaTime { get; set; } = 0.016f; // 60 FPS
        public static float unscaledTime { get; set; }
        public static float unscaledDeltaTime { get; set; } = 0.016f;
    }

    public static class Debug
    {
        public static void Log(object message)
        {
            Console.WriteLine($"[Unity] {message}");
        }
        
        public static void LogWarning(object message)
        {
            Console.WriteLine($"[Unity Warning] {message}");
        }
        
        public static void LogError(object message)
        {
            Console.WriteLine($"[Unity Error] {message}");
        }
    }
} 