using Discord;
using Lomztein.AdvDiscordCommands.Framework.Categories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lomztein.AdvDiscordCommands.Framework
{
    public class FuncCommand : Command
    {
        private Func<object[], Task<Result>>[] _functions;
        private string[] _funcDescriptions;
        private Type[][] _funcParamTypes;
        private string[][] _argNames;
        private CommandOverload.ExampleInfo[] _exampleInfos;

        public FuncCommand (string name, string desc, ICategory cat, List<GuildPermission> reqPerms,
            Func<object[], Task<Result>>[] functions, string[] funcDescs, Type[][] funcParamTypes, string[][] argNames,
            CommandOverload.ExampleInfo[] infos)
        {
            Name = name;
            Description = desc;
            Category = cat;
            RequiredPermissions = reqPerms;
            _functions = functions;
            _funcParamTypes = funcParamTypes;
            _funcDescriptions = funcDescs;
            _argNames = argNames;
            _exampleInfos = infos;
        }

        public override CommandOverload[] GetOverloads()
        {
            CommandOverload[] overloads = new CommandOverload[_functions.Length];
            for (int i = 0; i < overloads.Length; i++)
            {
                var func = _functions[i];
                string[] argNames = _argNames[i];

                CommandOverload.Parameter[] parameters = new CommandOverload.Parameter[func.Method.GetParameters().Length];
                for (int j = 0; j < parameters.Length; i++)
                {
                    CommandOverload.Parameter newParam = new CommandOverload.Parameter(
                        _argNames[i][j],
                        _funcParamTypes[i][j],
                        new Attribute[0]
                        );
                    parameters[i] = newParam;
                }

                overloads[i] = new CommandOverload(func.Method.ReturnType.GenericTypeArguments.First(),
                    parameters, _funcDescriptions[i], _exampleInfos[i], _functions[i]);
            }
            return overloads;
        }
    }

    public class FuncCommandBuilder
    {
        private readonly string _name;
        private readonly string _desc;
        private ICategory _category;
        private readonly List<GuildPermission> _requiredPermissions = new List<GuildPermission>();
        private readonly List<Func<object[], Task<Result>>> _functions = new List<Func<object[], Task<Result>>>();
        private readonly List<string> _funcDescriptions = new List<string>();
        private readonly List<Type[]> _argTypes = new List<Type[]>();
        private readonly List<string[]> _argNames = new List<string[]>();
        private readonly List<CommandOverload.ExampleInfo> _exampleInfos = new List<CommandOverload.ExampleInfo>();

        public FuncCommandBuilder (string name, string description)
        {
            _name = name;
            _desc = description;
        }

        public FuncCommandBuilder WithCategory (ICategory category)
        {
            _category = category;
            return this;
        }

        public FuncCommandBuilder AddRequiredPermission (GuildPermission permission)
        {
            _requiredPermissions.Add(permission);
            return this;
        }

        public FuncCommandBuilder AddOverload (Func<object[], Task<Result>> function, string description, Dictionary<Type, string> paramArgs)
        {
            return AddOverload(function, description, new CommandOverload.ExampleInfo(string.Empty, string.Empty), paramArgs);
        }

        public FuncCommandBuilder AddOverload(Func<object[], Task<Result>> function, string description, CommandOverload.ExampleInfo example, Dictionary<Type, string> paramArgs)
        {
            _functions.Add(function);
            _funcDescriptions.Add(description);
            _argTypes.Add(paramArgs.Keys.ToArray());
            _argNames.Add(paramArgs.Values.ToArray());
            _exampleInfos.Add(example);
            return this;
        }

        public FuncCommand Build ()
        {
            return new FuncCommand(_name, _desc, _category, _requiredPermissions, _functions.ToArray(), _funcDescriptions.ToArray(), _argTypes.ToArray (), _argNames.ToArray(), _exampleInfos.ToArray());
        }
    }
}
