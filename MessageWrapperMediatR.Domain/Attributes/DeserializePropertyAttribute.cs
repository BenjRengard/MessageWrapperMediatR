namespace MessageWrapperMediatR.Domain.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DeserializePropertyAttribute : Attribute
    {
        public DeserializePropertyAttribute() { }

    }
}
