using IoT.RPiController.Services.Enums;
using IoT.RPiController.Services.Models;
using IoT.RPiController.Services.Options;
using IoT.RPiController.Services.Services.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Device.Gpio;

namespace IoT.RPiController.Services.Services.Implementations
{
    public class PowerBusService : IPowerBusService, IDisposable
    {
        private readonly BusConfigOptions _busConfig;
        private readonly ILogger<PowerBusService> _logger;
        private GpioController _gpioController;

        public PowerBusService(
            IOptions<BusConfigOptions> config,
            ILogger<PowerBusService> logger
        )
        {
            _busConfig = config.Value;
            _logger = logger;
            _gpioController = new GpioController();
        }

        public void Dispose()
        {
            _gpioController?.Dispose();
        }

        public List<PowerBusDto> ReadAllStates()
        {
            var allStates = new List<PowerBusDto>();
            try
            {
                foreach (var powerBusEnum in Enum.GetValues(typeof(PowerBusEnum)).Cast<PowerBusEnum>())
                {
                    var state = ReadState(powerBusEnum);
                    if (state != null)
                    {
                        allStates.Add(state);
                    }
                }

                return allStates;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while reading all states: {ex.Message}", ex);
                return allStates;
            }
        }

        public PowerBusDto? ReadState(PowerBusEnum powerBusEnum)
        {
            int pin = -1;
            try
            {
                var pinIsUndefinedMessage = $"Pin for {powerBusEnum.ToString()} is undefined.";
                var pinProperty = _busConfig?.GPIO?.GetType().GetProperty(powerBusEnum.ToString());
                if (pinProperty == null)
                {
                    throw new Exception(pinIsUndefinedMessage);
                }

                var pinValue = pinProperty.GetValue(_busConfig?.GPIO);
                if (pinValue == null || !(pinValue is int))
                {
                    throw new Exception(pinIsUndefinedMessage);
                }

                pin = (int)pinValue;

                var isPinOpen = _gpioController.IsPinOpen(pin);
                _logger.LogInformation($"IsPinOpen {pin}: {isPinOpen}");
                if (!isPinOpen)
                {
                    _gpioController.OpenPin(pin, PinMode.Output);
                }

                var state = _gpioController.Read(pin) == PinValue.High;
                _logger.LogInformation($"Read state of pin {pin}: {(state ? "High" : "Low")}");

                return new PowerBusDto
                {
                    BusName = powerBusEnum.ToString(),
                    State = state
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while reading state: {ex.Message}", ex);
                return null;
            }
            finally
            {
                if (pin != -1)
                {
                    _gpioController?.ClosePin(pin);
                }
            }
        }

        public bool SwitchState(PowerBusEnum powerBusEnum, bool state)
        {
            var pin = -1; // Initialize pin variable outside the try block
            try
            {
                var pinIsUndefinedMessage = $"Pin for {powerBusEnum.ToString()} is undefined.";
                var pinProperty = _busConfig?.GPIO?.GetType().GetProperty(powerBusEnum.ToString());
                if (pinProperty == null)
                {
                    throw new Exception(pinIsUndefinedMessage);
                }

                var pinValue = pinProperty.GetValue(_busConfig?.GPIO);
                if (pinValue == null || !(pinValue is int))
                {
                    throw new Exception(pinIsUndefinedMessage);
                }

                pin = (int) pinValue;

                var isPinOpen = _gpioController.IsPinOpen(pin);
                if (!isPinOpen)
                {
                    _gpioController.OpenPin(pin, PinMode.Output);
                    _logger.LogInformation($"Pin {pin} opened in output mode.");
                }

                _gpioController.Write(pin, state ? PinValue.High : PinValue.Low);
                _logger.LogInformation($"Switched state of pin {pin} to {(state ? "High" : "Low")}");

                return state;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while switching state: {ex.Message}", ex);
                throw new Exception(ex.Message);
            }
            finally
            {
                // Always ensure to close the pin after use
                if (pin != -1)
                {
                    _gpioController?.ClosePin(pin);
                }
            }
        }
    }
}