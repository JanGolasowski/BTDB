using System;
using System.Reflection;
using System.Reflection.Emit;

namespace BTDB.IL
{
    class ILDynamicTypeImpl : IILDynamicType
    {
        readonly AssemblyBuilder _assemblyBuilder;
        readonly ModuleBuilder _moduleBuilder;
        readonly TypeBuilder _typeBuilder;
        readonly IILGenForbidenInstructions _forbidenInstructions;
        readonly string _name;
        static int counter;

        public ILDynamicTypeImpl(string name, Type baseType, Type[] interfaces)
        {
            _name = name + (counter++) + ".dll";
            _assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(name), AssemblyBuilderAccess.RunAndCollect);
            _moduleBuilder = _assemblyBuilder.DefineDynamicModule(_name);
            _typeBuilder = _moduleBuilder.DefineType(name, TypeAttributes.Public, baseType, interfaces);
            _forbidenInstructions = new ILGenForbidenInstructionsCheating(_typeBuilder);
        }

        public IILMethod DefineMethod(string name, Type returns, Type[] parameters, MethodAttributes methodAttributes = MethodAttributes.Public)
        {
            var methodBuilder = _typeBuilder.DefineMethod(name, methodAttributes, returns, parameters);
            return new ILMethodImpl(methodBuilder, _forbidenInstructions);
        }

        public IILField DefineField(string name, Type type, FieldAttributes fieldAttributes)
        {
            return new ILFieldImpl(_typeBuilder.DefineField(name, type, fieldAttributes));
        }

        public IILEvent DefineEvent(string name, EventAttributes eventAttributes, Type type)
        {
            return new ILEventImpl(_typeBuilder.DefineEvent(name, eventAttributes, type));
        }

        public IILMethod DefineConstructor(Type[] parameters)
        {
            return new ILConstructorImpl(_typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, parameters), _forbidenInstructions);
        }

        public void DefineMethodOverride(IILMethod methodBuilder, MethodInfo baseMethod)
        {
            _typeBuilder.DefineMethodOverride(((IILMethodPrivate)methodBuilder).TrueMethodInfo, baseMethod);
        }

        public Type CreateType()
        {
            var finalType = _typeBuilder.CreateType();
            _forbidenInstructions.FinishType(finalType);
            //_assemblyBuilder.Save(_name);
            return finalType;
        }
    }
}