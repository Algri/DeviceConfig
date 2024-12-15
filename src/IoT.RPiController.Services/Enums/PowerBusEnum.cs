using System.ComponentModel;

namespace IoT.RPiController.Services.Enums
{
    public enum PowerBusEnum
    {
        [Description("OneWire 5V")]
        Power5VOut,
        PowerExtensionModule,
        Buzzer
    }
}