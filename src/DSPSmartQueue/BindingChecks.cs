using System;
using System.Collections.Generic;
using System.Reflection;

namespace DSPSmartQueue
{
    // Reflection only: no target construction, field reads/writes, or method invocation.
    // The same checks work in a MetadataLoadContext during offline verification.
    internal static class BindingChecks
    {
        private const BindingFlags Declared = BindingFlags.Public | BindingFlags.NonPublic |
                                              BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;

        internal static List<string> Validate(Assembly game, Assembly ui, Assembly unity, Assembly core)
        {
            var errors = new List<string>();
            var window = RequiredType(game, "UIReplicatorWindow");
            var forge = RequiredType(game, "MechaForge");
            var task = RequiredType(game, "ForgeTask");
            var integer = RequiredType(core, "System.Int32");
            var voidType = RequiredType(core, "System.Void");
            var unsignedArray = RequiredType(core, "System.UInt32").MakeArrayType();
            var tasks = RequiredType(core, "System.Collections.Generic.List`1").MakeGenericType(task);
            var text = RequiredType(ui, "UnityEngine.UI.Text");

            RequireMethod(errors, window, "SetBufferData", voidType, true, Type.EmptyTypes);
            RequireMethod(errors, window, "OnQueueMouseDown", voidType, true,
                new[] { RequiredType(ui, "UnityEngine.EventSystems.BaseEventData") });
            foreach (var name in new[] { "_OnOpen", "_OnClose", "_OnFree", "_OnDestroy" })
                RequireMethod(errors, window, name, voidType, false, Type.EmptyTypes);

            RequireField(errors, window, "taskQueue", tasks, true);
            RequireField(errors, window, "mechaForge", forge, true);
            RequireField(errors, window, "mouseQueueIndex", integer, true);
            RequireField(errors, window, "queueIndexArray", unsignedArray, true);
            RequireField(errors, window, "queueStateArray", unsignedArray, true);
            RequireField(errors, window, "queueNumTexts", text.MakeArrayType(), true);
            RequireField(errors, window, "queueCountText", text, false);
            RequireField(errors, window, "mainTaskTextColor", RequiredType(unity, "UnityEngine.Color"), false);
            RequireField(errors, forge, "tasks", tasks, false);
            foreach (var name in new[] { "parentTaskIndex", "recipeId", "count" })
                RequireField(errors, task, name, integer, false);
            return errors;
        }

        private static Type RequiredType(Assembly assembly, string name)
        {
            return assembly.GetType(name, false) ?? throw new TypeLoadException(
                "Required binding type missing: " + name + " in " + assembly.GetName().Name);
        }

        internal static void RequireField(List<string> errors, Type owner, string name, Type type, bool isPrivate)
        {
            var field = owner.GetField(name, Declared);
            if (field == null || field.IsStatic || field.IsInitOnly || field.IsLiteral ||
                field.FieldType != type || (isPrivate ? !field.IsPrivate : !field.IsPublic))
                errors.Add(owner.FullName + "." + name + ": expected " +
                    (isPrivate ? "private" : "public") + " mutable instance field " + type.FullName);
        }

        internal static void RequireMethod(List<string> errors, Type owner, string name, Type result,
            bool isPrivate, Type[] parameters)
        {
            var method = owner.GetMethod(name, Declared, null, parameters, null);
            // The default reflection binder can accept widening conversions; hooks require exact types.
            var actualParameters = method == null ? null : method.GetParameters();
            bool exactParameters = actualParameters != null && actualParameters.Length == parameters.Length;
            if (actualParameters != null && exactParameters)
                for (int i = 0; i < parameters.Length; i++)
                    exactParameters &= actualParameters[i].ParameterType == parameters[i];
            if (method == null || !exactParameters || method.IsStatic || method.IsGenericMethod || method.ReturnType != result ||
                (isPrivate ? !method.IsPrivate : !method.IsFamily))
                errors.Add(owner.FullName + "." + name + ": expected " +
                    (isPrivate ? "private" : "protected") + " instance method returning " + result.FullName +
                    " with parameters (" + string.Join(", ", Array.ConvertAll(parameters, p => p.FullName)) + ")");
        }
    }
}
