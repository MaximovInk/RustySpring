using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MaximovInk
{
    public static class CommandsDatabase
    {
        private static readonly Dictionary<string, Command> commands = new Dictionary<string, Command>();

        public static void RegisterCommand(Command command, bool replace = false)
        {
            if (command == null)
            {
                throw new System.Exception("Cannot register block , because its null");
            }

            if (commands.ContainsKey(command.Name))
            {
                if (!replace)
                {
                    Debug.LogError("Tile is already registered in database:" + command.Name);
                }
                else
                {
                    commands[command.Name] = command;
                }

                return;
            }

            commands.Add(command.Name, command);
        }

        public static List<string> GetAllCommands()
        {
            return commands.Keys.ToList();
        }

        public static Command GetCommand(string name)
        {
            return commands[name];
        }

        static CommandsDatabase()
        {
            RegisterCommand(new Command()
            {
                Name = "help",
                Description = "help you",
                Execute = (objs) =>
                    {
                        if (objs.Length > 0)
                            return "Params must be equals zero";
                        var str = new StringBuilder();
                        var commands = GetAllCommands();

                        str.Append("There is ").Append(commands.Count + 1).Append(" commnads");
                        for (int i = 0; i < commands.Count; i++)
                        {
                            str.Append(i + 1).Append(") ").Append(commands[i]);
                        }
                        return str.ToString();
                    }
            });
        }
    }
}