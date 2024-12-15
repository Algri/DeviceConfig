using System.ComponentModel;
using System.Reflection;

namespace IoT.RPiController.Services.Enums
{
    public enum ModuleTypeEnum
    {
        // IE-I2C-RM15A-4
        [Description("RM4")]
        RM4,
        [Description("RM8")]
        RM8,
        [Description("RM16")]
        RM16,
        [Description("IM4")]
        IM4,
        [Description("IM8")]
        IM8,
        [Description("IM16")]
        IM16
    }

    public static class ModuleTypeEnumExtensions
    {
        private const char OutputModulePrefix = 'R';
        private const char InputModulePrefix = 'I';

        public static bool IsOutputModule(this ModuleTypeEnum moduleType)
        {
            return moduleType.ToString().StartsWith(OutputModulePrefix);
        }

        public static bool IsInputModule(this ModuleTypeEnum moduleType)
        {
            return moduleType.ToString().StartsWith(InputModulePrefix);
        }

        public static int PortsAmount(this ModuleTypeEnum moduleType)
        {
            var typeName = moduleType.ToString(); // Get the enum name as string
            var numberString = typeName.Substring(typeName.Length - 1); // Get the last character (which should be the number)
            if (int.TryParse(numberString, out int result))
            {
                return result; // Parse the number and return it
            }

            throw new InvalidOperationException("Failed to parse the number from the enum name.");
        }

        public static IEnumerable<string> GetModuleTypeDescriptions()
        {
            var enumType = typeof(ModuleTypeEnum);
            if (!enumType.IsEnum) return Array.Empty<string>();

            var fields = enumType.GetFields(BindingFlags.Public | BindingFlags.Static);

            return fields.Select(field => GetDescription((Enum)field.GetValue(null))).ToArray();
        }

        //TODO: move to common utils?
        private static string GetDescription(this Enum genericEnum)
        {
            var genericEnumType = genericEnum.GetType();
            var memberInfo = genericEnumType.GetMember(genericEnum.ToString());

            if ((memberInfo == null || memberInfo.Length <= 0)) return genericEnum.ToString();

            var attributes = memberInfo[0].GetCustomAttributes(
                typeof(System.ComponentModel.DescriptionAttribute),
                false
            );

            if ((attributes != null && attributes.Length > 0))
            {
                return ((System.ComponentModel.DescriptionAttribute)attributes[0]).Description;
            }

            return genericEnum.ToString();
        }
    }
}