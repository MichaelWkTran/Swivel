using UnityEngine;

static class ComponentUtils
{
    static public Component CopyComponent(Component _original, GameObject _destination)
    {
        System.Type type = _original.GetType();
        Component copy = _destination.AddComponent(type);
        // Copied fields can be restricted with BindingFlags
        System.Reflection.FieldInfo[] fields = type.GetFields();
        foreach (System.Reflection.FieldInfo field in fields)
        {
            field.SetValue(copy, field.GetValue(_original));
        }
        return copy;
    }

    static public T CopyComponent<T>(T _original, GameObject _destination) where T : Component
    {
        System.Type type = _original.GetType();
        Component copy = _destination.AddComponent(type);
        System.Reflection.FieldInfo[] fields = type.GetFields();
        foreach (System.Reflection.FieldInfo field in fields)
        {
            field.SetValue(copy, field.GetValue(_original));
        }
        return copy as T;
    }
}

