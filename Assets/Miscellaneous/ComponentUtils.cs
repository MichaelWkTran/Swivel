using UnityEngine;

static class ComponentUtils
{
    //https://discussions.unity.com/t/copy-a-component-at-runtime/71172/3
    static public Component CopyComponent(Component _original, GameObject _destination)
    {
        System.Type type = _original.GetType();
        Component copy = _destination.AddComponent(type);
        // Copied fields can be restricted with BindingFlags
        System.Reflection.FieldInfo[] fields = type.GetFields();
        foreach (System.Reflection.FieldInfo field in fields)
        {
            if (field.IsLiteral) continue;
            if (field.IsInitOnly) continue;

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
            if (field.IsLiteral) continue;
            if (field.IsInitOnly) continue;

            field.SetValue(copy, field.GetValue(_original));
        }
        return copy as T;
    }

    static public T CopyComponent<T>(T _original, T _destination) where T : Component
    {
        System.Type type = _original.GetType();
        System.Reflection.FieldInfo[] fields = type.GetFields();
        foreach (System.Reflection.FieldInfo field in fields)
        {
            if (field.IsLiteral) continue;
            if (field.IsInitOnly) continue;
                
            field.SetValue(_destination, field.GetValue(_original));
        }
        return _destination;
    }
}

