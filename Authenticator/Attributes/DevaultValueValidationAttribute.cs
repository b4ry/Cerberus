using System.ComponentModel.DataAnnotations;

namespace Authenticator.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class DevaultValueValidationAttribute : ValidationAttribute
    {
        public DevaultValueValidationAttribute(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        public override bool IsValid(object? value)
        {
            if (value == null)
            {
                return false;
            }

            Type type = value.GetType();

            if (type == typeof(string))
            {
                return !string.IsNullOrEmpty(value as string);
            }

            object? defaultValue = GetDefaultValue(type);

            return !value.Equals(defaultValue);
        }

        private static object? GetDefaultValue(Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            else
            {
                return null;
            }
        }
    }
}
