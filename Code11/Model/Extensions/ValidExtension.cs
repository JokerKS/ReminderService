using Code11.Model.Interfaces;

namespace Code11.Model.Extensions
{
    static class ValidExtension
    {
        public static bool IsValid(this IValidate entity)
        {
            return entity.Validate();
        }
    }
}