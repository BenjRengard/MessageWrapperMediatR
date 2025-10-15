namespace MessageWrapperMediatR.Application.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DeserializePropertyAttribute : Attribute
    {
        public DeserializePropertyAttribute() { }

    }
}
