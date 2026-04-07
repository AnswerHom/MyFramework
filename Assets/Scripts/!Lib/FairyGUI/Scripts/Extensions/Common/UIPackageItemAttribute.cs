using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace FairyGUI
{
    /// <summary>
    /// 标记GObject子类与UI资产的关联，并自动注册到UIObjectFactory
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class UIPackageItemAttribute : Attribute
    {
        public string url;

        public UIPackageItemAttribute(string url) { this.url = url; }

        /// <summary>
        /// 检查所有类型，将带有UIPackageItemExtension的GObject子类自动注册到UIObjectFactory
        /// </summary>
        public static void RegisterAll(Assembly assembly)
        {			
            foreach (var type in assembly.GetTypes())
            {
                if (!type.IsClass || type.IsAbstract || !typeof(GObject).IsAssignableFrom(type))
                    continue;

                var attributes = type.GetCustomAttributes(typeof(UIPackageItemAttribute), false);
                for (int i = 0; i < attributes.Length; i++)
                {
                    var attribute = (UIPackageItemAttribute)attributes[i];
                    UIObjectFactory.SetPackageItemExtension(attribute.url, type);
                }
            }
        }

        private static readonly string[] _systemAssemblyPrefixList = new string[] {
            "mscorlib","netstandard","System.", "Mono.","Microsoft.", "Unity.","UnityEngine.","UnityEditor.",
        };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsSystemAssembly(Assembly assembly)
        {
            var name = assembly.FullName;

            foreach (var prefix in _systemAssemblyPrefixList)
            {
                if (name.StartsWith(prefix))
                    return true;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                // Gets the array of classes that were defined in the module and loaded.
                return e.Types.Where(t => t != null);
            }
        }
    }
}