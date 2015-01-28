using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.Composition.Hosting;
using Ocean.Entity;
using Ocean.Communication.Model;

namespace Ocean.Communication.Commands
{
    public class CommandManager
    {
        private readonly Caller _caller;

        private static Dictionary<string, ICommand> _commandCache;
        private static readonly Lazy<IList<ICommand>> _commands = new Lazy<IList<ICommand>>(GetCommands);

        public CommandManager(Caller caller)
        {
            _caller = caller;
        }

        public bool TryHandleCommand(string commandName, string[] args, ref string errorMsg)
        {
            commandName = commandName.Trim();

            if (commandName.StartsWith("/"))
            {
                return false;
            }

            var callerContext = new CallerContext
            {
                Caller = _caller
            };

            ICommand command;

            if (!TryMatchCommand(commandName, out command))
            {
                throw new InvalidOperationException(String.Format("'{0}' is not a valid command.", commandName));
            }

            command.Execute(callerContext, args, ref errorMsg);

            return true;
        }

        private bool TryMatchCommand(string commandName, out ICommand command)
        {
            if (_commandCache == null)
            {
                var commands = from c in _commands.Value
                               let commandAttribute = c.GetType()
                                                       .GetCustomAttributes(true)
                                                       .OfType<CommandAttribute>()
                                                       .FirstOrDefault()
                               where commandAttribute != null
                               select new
                               {
                                   Name = commandAttribute.CommandName,
                                   Command = c
                               };

                _commandCache = commands.ToDictionary(c => c.Name,
                                                      c => c.Command,
                                                      StringComparer.OrdinalIgnoreCase);
            }

            return _commandCache.TryGetValue(commandName, out command);
        }

        private static IList<ICommand> GetCommands()
        {
            //使用MEF加载命令
            var catalog = new AssemblyCatalog(typeof(CommandManager).Assembly);
            var compositionContainer = new CompositionContainer(catalog);
            return compositionContainer.GetExportedValues<ICommand>().ToList();
        }

        public static IEnumerable<CommandMetaData> GetCommandsMetaData()
        {
            var commands = from c in _commands.Value
                           let commandAttribute = c.GetType()
                                                   .GetCustomAttributes(true)
                                                   .OfType<CommandAttribute>()
                                                   .FirstOrDefault()
                           where commandAttribute != null
                           select new CommandMetaData
                           {
                               Name = commandAttribute.CommandName,
                               Description = commandAttribute.Description,
                               Arguments = commandAttribute.Arguments,
                               Group = commandAttribute.Group
                           };
            return commands;
        }
    }
}