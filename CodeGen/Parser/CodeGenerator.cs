using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;

namespace Parser
{
    internal class CodeGenerator
    {
        private static ILGenerator _il;
        private static string _moduleName;
        private static AssemblyBuilder _asmb;
        private static ModuleBuilder _modb;
        private static MethodBuilder _methb;
        private static TypeBuilder _typeBuilder;
        Dictionary<string, LocalBuilder> _symbol_Table;
        Dictionary<string, string> _symbols_Types;
        Node _root;

        public CodeGenerator(Node root)
        {
            var asmName = new AssemblyName("example.exe");
            _asmb = AppDomain.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.Save);
            _modb = _asmb.DefineDynamicModule(_moduleName);
            _typeBuilder = _modb.DefineType("_code_gen_");
            _methb = _typeBuilder.DefineMethod("Main", MethodAttributes.Static, typeof(void), Type.EmptyTypes);
            _il = _methb.GetILGenerator();
            _symbol_Table = new Dictionary<string, LocalBuilder>();
            _symbols_Types = new Dictionary<string, string>();
            _root = root;
        }

        public void run() {
            v_Generic(_root);
        }

        void v_BoolOp(Node BOOL_OP)
        {
            v_Generic(BOOL_OP.Children[0]);
            string op = BOOL_OP.Children[1].Value;
            v_Generic(BOOL_OP.Children[2]);

        }

        string v_DECLARATION(Node Dec)
        {
            string DEC_TYPE = Dec.Value;
            string Name = Dec.Children[0].Value;
            _symbols_Types.Add(Name, DEC_TYPE);
            return Name;
        }

        void v_ASSIGNMENT(Node ASSIGNMENT) {
            string AssignTo;
            if (ASSIGNMENT.Children[0].nodeType == Node.NodeType.DECLARATION)
            {
                AssignTo = v_DECLARATION(ASSIGNMENT.Children[0]);
            }
            else if(ASSIGNMENT.Children[0].nodeType == Node.NodeType.VAR)
            {
                AssignTo = v_DECLARATION(ASSIGNMENT.Children[0]);
            }
            string assignmentType = ASSIGNMENT.Children[1].Value;
            v_Generic(ASSIGNMENT.Children[2]);
        }

        void v_Var(Node var)
        {
            if (!_symbols_Types.ContainsKey(var.Value))
            {
                throw new Exception("Symbol '' not found!".Insert(8, var.Value));
            }
            string symbolType = _symbols_Types[var.Value];
            _il.Emit(OpCodes.Ldarg_0,_symbols_Types[var.Value]);
        }

        void v_CODE_BLOCK(Node BLOCK)
        {
            foreach(var statement in BLOCK.Children)
            {
                v_Generic(statement);
            }
        }

        void v_INTERGER(Node Interger)
        {
            _il.Emit(OpCodes.Ldarg_0, int.Parse(Interger.Value));
        }

        void v_Generic(Node UNKOWN)
        {
            switch (UNKOWN.nodeType)
            {
                case Node.NodeType.ASSIGNMENT:
                    v_ASSIGNMENT(UNKOWN);
                    break;
                case Node.NodeType.DECLARATION:
                    v_DECLARATION(UNKOWN);
                    break;
                case Node.NodeType.BOOLEAN_OP:
                    v_BoolOp(UNKOWN);
                    break;
                case Node.NodeType.CODE_BLOCK:
                    v_CODE_BLOCK(UNKOWN);
                    break;
                case Node.NodeType.VAR:
                    v_Var(UNKOWN);
                    break;
                default:
                    break;
            }
        }
    }
}
