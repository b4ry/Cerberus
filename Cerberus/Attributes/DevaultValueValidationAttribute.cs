using System.ComponentModel.DataAnnotations;

namespace Cerberus.Attributes
{
    /// <summary>
    /// Validation attribute validating whether provided value is default, or not.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class DevaultValueValidationAttribute : ValidationAttribute
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="errorMessage">An error message passed, when the validation fails</param>
        public DevaultValueValidationAttribute(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        /// <summary>
        /// Verifies, if a value is valid or not, based on its underlying type.
        /// If it is default, then the validation fails. Otherwise, it is successful.
        /// Examples:
        ///     validation for string will fail, if it is either null or empty,
        ///     validation for int will fail, if it is 0
        /// </summary>
        /// <param name="value">Value boxed in an object, which is going to be tested for its validity</param>
        /// <returns>
        ///     False, when validation fails.
        ///     True otherwise.
        /// </returns>
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
