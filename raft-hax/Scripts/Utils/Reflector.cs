using System;
using System.Diagnostics;
using System.Reflection;

namespace Hax;
public class Reflector {
    const BindingFlags Internal = BindingFlags.NonPublic | BindingFlags.Instance;
    const BindingFlags Public = BindingFlags.Public | BindingFlags.Instance;
    const BindingFlags InternalStatic = BindingFlags.NonPublic | BindingFlags.Static;
    const BindingFlags PublicStatic = BindingFlags.Public | BindingFlags.Static;

    const BindingFlags InternalField = Reflector.Internal | BindingFlags.GetField;
    const BindingFlags InternalProperty = Reflector.Internal | BindingFlags.GetProperty;
    const BindingFlags InternalMethod = Reflector.Internal | BindingFlags.InvokeMethod;
    const BindingFlags InternalStaticField = Reflector.InternalStatic | BindingFlags.GetField;
    const BindingFlags InternalStaticProperty = Reflector.InternalStatic | BindingFlags.GetProperty;
    const BindingFlags InternalStaticMethod = Reflector.InternalStatic | BindingFlags.InvokeMethod;

    const BindingFlags PublicField = Reflector.Public | BindingFlags.GetField;
    const BindingFlags PublicProperty = Reflector.Public | BindingFlags.GetProperty;
    const BindingFlags PublicMethod = Reflector.Public | BindingFlags.InvokeMethod;
    const BindingFlags PublicStaticField = Reflector.PublicStatic | BindingFlags.GetField;
    const BindingFlags PublicStaticProperty = Reflector.PublicStatic | BindingFlags.GetProperty;
    const BindingFlags PublicStaticMethod = Reflector.PublicStatic | BindingFlags.InvokeMethod;

    object Obj { get; }
    Type ObjType { get; }

    Reflector(object obj) {
        this.Obj = obj;
    }

    Reflector(Type objType) {
        this.ObjType = objType;
    }

    Type GetObjectType() {
        if (this.ObjType != null) return this.ObjType;
        return this.Obj.GetType();
    }

    T GetField<T>(string variableName, BindingFlags flags) {
        try {
            return (T)this.GetObjectType()
                          .GetField(variableName, flags)
                          .GetValue(this.Obj);
        }

        catch (Exception e) {
            this.LogReflectionError(e);
            return default;
        }
    }

    Reflector GetProperty(string propertyName, BindingFlags flags) {
        try {
            return new Reflector(
                this.GetObjectType()
                    .GetProperty(propertyName, flags)
                    .GetValue(this.Obj, null)
            );
        }

        catch (Exception e) {
            this.LogReflectionError(e);
            return default;
        }
    }

    Reflector SetField(string variableName, object value, BindingFlags flags) {
        try {
            this.GetObjectType()
                .GetField(variableName, flags)
                .SetValue(this.Obj, value);
        }

        catch (Exception e) {
            this.LogReflectionError(e);
        }

        return this;
    }

    Reflector SetProperty(string propertyName, object value, BindingFlags flags) {
        try {
            this.GetObjectType()
                .GetProperty(propertyName, flags)
                .SetValue(this.Obj, value, null);
        }

        catch (Exception e) {
            this.LogReflectionError(e);
        }

        return this;
    }

    T InvokeMethod<T>(string methodName, BindingFlags flags, params object[] args) {
        try {
            return (T)this.GetObjectType()
                          .GetMethod(methodName, flags)
                          .Invoke(this.Obj, args);
        }

        catch (Exception e) {
            this.LogReflectionError(e);
            return default;
        }
    }

    // Field extension methods
    public T GetInternalField<T>(string variableName) => this.GetField<T>(variableName, Reflector.InternalField);

    public T GetInternalStaticField<T>(string variableName) => this.GetField<T>(variableName, Reflector.InternalStaticField);

    public T GetPublicField<T>(string variableName) => this.GetField<T>(variableName, Reflector.PublicField);

    public T GetPublicStaticField<T>(string variableName) => this.GetField<T>(variableName, Reflector.PublicStaticField);

    public Reflector GetInternalField(string variableName) => new Reflector(this.GetInternalField<object>(variableName));

    public Reflector GetInternalStaticField(string variableName) => new Reflector(this.GetInternalStaticField<object>(variableName));

    public Reflector GetPublicField(string variableName) => new Reflector(this.GetPublicField<object>(variableName));

    public Reflector GetPublicStaticField(string variableName) => new Reflector(this.GetPublicStaticField<object>(variableName));

    public Reflector SetInternalField(string variableName, object value) => this.SetField(variableName, value, Reflector.InternalField);

    public Reflector SetInternalStaticField(string variableName, object value) => this.SetField(variableName, value, Reflector.InternalStaticField);

    public Reflector SetPublicField(string variableName, object value) => this.SetField(variableName, value, Reflector.PublicField);

    public Reflector SetPublicStaticField(string variableName, object value) => this.SetField(variableName, value, Reflector.PublicStaticField);

    // Property extension methods
    public Reflector GetInternalProperty(string propertyName) => this.GetProperty(propertyName, Reflector.InternalProperty);

    public Reflector GetPublicProperty(string propertyName) => this.GetProperty(propertyName, Reflector.PublicProperty);

    public Reflector SetInternalProperty(string propertyName, object value) => this.SetProperty(propertyName, value, Reflector.InternalProperty);

    public Reflector SetPublicProperty(string propertyName, object value) => this.SetProperty(propertyName, value, Reflector.PublicProperty);

    // Method extension methods
    public T InvokeInternalMethod<T>(string methodName, params object[] args) => this.InvokeMethod<T>(methodName, Reflector.InternalMethod, args);

    public T InvokePublicMethod<T>(string methodName, params object[] args) => this.InvokeMethod<T>(methodName, Reflector.PublicMethod, args);

    public T InvokePublicStaticMethod<T>(string methodName, params object[] args) => this.InvokeMethod<T>(methodName, Reflector.PublicStaticMethod, args);

    public Reflector InvokeInternalMethod(string methodName, params object[] args) => new Reflector(this.InvokeInternalMethod<object>(methodName, args));

    public Reflector InvokePublicMethod(string methodName, params object[] args) => new Reflector(this.InvokePublicMethod<object>(methodName, args));

    public Reflector InvokePublicStaticMethod(string methodName, params object[] args) => new Reflector(this.InvokePublicStaticMethod<object>(methodName, args));

    // Miscellaneous methods
    void LogReflectionError(Exception e) => Console.Print($"Reflection Error in {new StackFrame(5).GetMethod().Name}:\n{e}");

    public static Reflector Target(object obj) => new Reflector(obj);

    public static Reflector Target(Type objType) => new Reflector(objType);
}