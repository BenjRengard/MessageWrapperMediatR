namespace MessageWrapperMediatR.Application.Contracts.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DeserializePropertyAttribute : Attribute
    {
        public DeserializePropertyAttribute() { }

    }
}
