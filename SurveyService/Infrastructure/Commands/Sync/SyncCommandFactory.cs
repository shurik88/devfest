using Common.Infrastructure.Commands;

namespace Infrastructure.Commands.Sync
{
    /// <summary>
    ///     Фабрика по созданию команды <seealso cref="SyncCommand"/>
    /// </summary>
    public class SyncCommandFactory: ICommandLineFactory
    {
        /// <inheritdoc/>
        public string HelpMessage => $"{Verb} \n " +
                                     $"  -f, --force, not requeired \t Force database update \n" +
                                     $"Example: \n" +
                                     $"{Verb} -f";

        /// <inheritdoc/>
        public string Verb => "sync";

        /// <inheritdoc/>
        public Command Create(string[] args)
        {
            if (args.Length > 1)
                return null;

            var commandArgs = new SyncCommandArguments();
            if (args.Length == 1)
                commandArgs.Force = args[0] == "-f" || args[0] == "--force";

            return new SyncCommand(commandArgs);
        }
    }
}
