using IoT.RPiController.Services.Enums;
using IoT.RPiController.Services.Models;
using IoT.RPiController.Services.Options;
using IoT.RPiController.Services.Services.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IoT.RPiController.Services.Services.Implementations.Mocks
{
    public class PowerBusServiceMock : IPowerBusService
    {
        private readonly BusConfigOptions _busConfig;
        private readonly ILogger<PowerBusServiceMock> _logger;

        public PowerBusServiceMock(
            IOptions<BusConfigOptions> config,
            ILogger<PowerBusServiceMock> logger
        )
        {
            _busConfig = config.Value;
            _logger = logger;
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
                _logger.LogError(ex, "Error occurred while reading all states:");
                return allStates;
            }
        }

        public PowerBusDto? ReadState(PowerBusEnum powerBusEnum)
        {
            return new PowerBusDto
            {
                BusName = powerBusEnum.ToString(),
                State = true,
            };
        }

        public bool SwitchState(PowerBusEnum powerBusEnum, bool state)
        {
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

                _logger.LogInformation($"Switched state of pin {pinValue} to {(state ? "High" : "Low")}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while switching state:");
                return false;
            }           
        }
    }
}
