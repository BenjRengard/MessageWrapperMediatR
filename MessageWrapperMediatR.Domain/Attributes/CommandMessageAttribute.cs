namespace MessageWrapperMediatR.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CommandMessageAttribute : Attribute
    {
        public required string CommandName { get; set; }

        public CommandMessageAttribute(string commandName) => CommandName = commandName;
    }
}
