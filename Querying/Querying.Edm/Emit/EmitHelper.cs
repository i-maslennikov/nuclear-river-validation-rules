using System;
using System.Reflection;
using System.Reflection.Emit;

namespace NuClear.Querying.Edm.Emit
{
    internal static class EmitHelper
    {
        // the property set and property get methods require a special set of attributes
        private const MethodAttributes PropertyAccessorAttributes = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;

        public static AssemblyBuilder DefineAssembly(string assemblyName)
        {
            return AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(assemblyName), AssemblyBuilderAccess.Run);
        }

        public static ModuleBuilder DefineModule(this AssemblyBuilder assemblyBuilder, string moduleName)
        {
            return assemblyBuilder.DefineDynamicModule(moduleName);
        }

        public static TypeBuilder DefineType(this ModuleBuilder moduleBuilder, string name)
        {
            return moduleBuilder.DefineType(name, TypeAttributes.Public);
        }

        public static EnumBuilder DefineEnum(this ModuleBuilder moduleBuilder, string name, Type underlyingType)
        {
            return moduleBuilder.DefineEnum(name, TypeAttributes.Public, underlyingType);
        }

        public static void DefineProperty(this TypeBuilder typeBuilder, string propertyName, Type propertyType)
        {
            var fieldBuilder = typeBuilder.DefineField("_" + propertyName, propertyType, FieldAttributes.Private);

            var propertyBuilder = typeBuilder.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);

            // define the "get" accessor method
            var getMethodBuilder = typeBuilder.DefineMethod("get_" + propertyName, PropertyAccessorAttributes, propertyType, Type.EmptyTypes);
            var getMethodIL = getMethodBuilder.GetILGenerator();
            getMethodIL.Emit(OpCodes.Ldarg_0);
            getMethodIL.Emit(OpCodes.Ldfld, fieldBuilder);
            getMethodIL.Emit(OpCodes.Ret);

            // define the "set" accessor method
            var setMethodBuilder = typeBuilder.DefineMethod("set_" + propertyName, PropertyAccessorAttributes, null, new [] { propertyType });
            var setMethodIL = setMethodBuilder.GetILGenerator();
            setMethodIL.Emit(OpCodes.Ldarg_0);
            setMethodIL.Emit(OpCodes.Ldarg_1);
            setMethodIL.Emit(OpCodes.Stfld, fieldBuilder);
            setMethodIL.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getMethodBuilder);
            propertyBuilder.SetSetMethod(setMethodBuilder);
        }
    }
}