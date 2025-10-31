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
        public static float realtimeSinceStartup { get; set; } = 0f;
    }

    public enum KeyCode
    {
        None = 0,
        F11 = 302,
        F12 = 303,
        Space = 32,
        Return = 13,
        Escape = 27
    }

    public static class Input
    {
        public static bool GetKeyDown(KeyCode key)
        {
            // Mock implementation - always return false
            return false;
        }

        public static bool GetKey(KeyCode key)
        {
            // Mock implementation - always return false
            return false;
        }

        public static bool GetKeyUp(KeyCode key)
        {
            // Mock implementation - always return false
            return false;
        }
    }

    public static class Screen
    {
        public static int width => 1920;
        public static int height => 1080;
    }

    public static class GUI
    {
        public static void Label(Rect position, string text)
        {
            // Mock implementation
        }

        public static bool Button(Rect position, string text)
        {
            // Mock implementation - return false (not clicked)
            return false;
        }

        public static string TextField(Rect position, string text)
        {
            // Mock implementation - return unchanged text
            return text;
        }

        public static void Box(Rect position, string text)
        {
            // Mock implementation
        }
    }

    public static class GUILayout
    {
        public static void Label(string text, params object[] options)
        {
            // Mock implementation
        }

        public static bool Button(string text, params object[] options)
        {
            // Mock implementation - return false (not clicked)
            return false;
        }

        public static string TextField(string text, params object[] options)
        {
            // Mock implementation - return unchanged text
            return text;
        }

        public static void Box(string text, params object[] options)
        {
            // Mock implementation
        }

        public static void Space(float pixels)
        {
            // Mock implementation
        }

        public static void BeginHorizontal(params object[] options)
        {
            // Mock implementation
        }

        public static void EndHorizontal()
        {
            // Mock implementation
        }

        public static void BeginVertical(params object[] options)
        {
            // Mock implementation
        }

        public static void EndVertical()
        {
            // Mock implementation
        }

        public static void BeginArea(Rect screenRect)
        {
            // Mock implementation
        }

        public static void EndArea()
        {
            // Mock implementation
        }
    }

    public static class Application
    {
        public static bool isPlaying => true;
        public static string persistentDataPath => "/tmp/unity_data";
    }

    public static class Random
    {
        private static System.Random _rnd = new System.Random();

        public static int Range(int min, int max)
        {
            return _rnd.Next(min, max);
        }

        public static float Range(float min, float max)
        {
            return (float)(_rnd.NextDouble() * (max - min) + min);
        }

        public static float value => (float)_rnd.NextDouble();
    }

    public static class Debug
    {
        public static void Log(object message) { Console.WriteLine($"[Unity] {message}"); }
        public static void LogWarning(object message) { Console.WriteLine($"[Unity WARN] {message}"); }
        public static void LogError(object message) { Console.WriteLine($"[Unity ERROR] {message}"); }
    }
}
