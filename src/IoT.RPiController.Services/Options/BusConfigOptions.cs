using System.Text.Json.Serialization;
using IoT.RPiController.Services.Constants;

namespace IoT.RPiController.Services.Options
{
    /// <summary>
    /// Represents the bus configuration options. As the additional source of input data and manipulations.
    /// </summary>
    public class BusConfigOptions
    {
        public const string BusConfig = "BusConfig";

        public GPIOOptions? GPIO { get; set; }

        public DigitalPortOptions DigitalPorts { get; set; }
    }

    public class GPIOOptions
    {
        /// <summary>
        /// Gets or sets the power 5V out pin number.
        /// </summary>
        public int Power5VOut { get; set; }

        /// <summary>
        /// Gets or sets the buzzer pin number.
        /// </summary>
        public int Buzzer { get; set; }

        /// <summary>
        /// Gets or sets the power extension module pin number.
        /// </summary>
        public int PowerExtensionModule { get; set; }

        /// <summary>
        /// Gets interrupt for extension module pin number.
        /// </summary>
        public int InterruptExtensionModule { get; set; }
    }

    public class DigitalPortOptions
    {
        [JsonPropertyName(DigitalPortNames.A1)]
        public DigitalPort A1 { get; set; }

        [JsonPropertyName(DigitalPortNames.A2)]

        public DigitalPort A2 { get; set; }

        [JsonPropertyName(DigitalPortNames.A3)]
        public DigitalPort A3 { get; set; }

        [JsonPropertyName(DigitalPortNames.A4)]
        public DigitalPort A4 { get; set; }
    }


    /// <summary>
    /// Represents the digital port options of Universal inputs / outputs (UIO).
    /// </summary>
    public class DigitalPort
    {
        /// <summary>
        /// Gets or sets the GPIO pin number.
        /// </summary>
        public int GPIO { get; set; }

        public string Mode { get; set; }
    }
}