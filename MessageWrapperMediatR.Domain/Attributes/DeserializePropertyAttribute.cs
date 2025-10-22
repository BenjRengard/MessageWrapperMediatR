namespace MessageWrapperMediatR.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DeserializePropertyAttribute : Attribute
    {
        public DeserializePropertyAttribute() { }

    }
}
