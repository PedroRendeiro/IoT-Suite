using IoTSuite.Server.Controllers;
using IoTSuite.Server.Hubs;
using IoTSuite.Server.Services;
using IoTSuite.Shared;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Net.Mqtt;
using System.Reactive.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;

namespace IoTSuite.Server.Tasks
{
    public class MQTTTask : BackgroundService
    {
        //private static ApplicationDbContext _context;
        //private static PositionsController _positions;
        //private static MeasuresController _measures;
        //private static PowerMeterController _powermeter;
        //private static AlarmsController _alarm;

        private static IServiceScopeFactory _serviceScopeFactory;
        private static IHubContext<SignalR> _hubContext;

        private static TelegramBotClient _telegramBotClient;
        private static int _telegramBotChatId;

        private static ILogger<MQTTTask> _logger;

        private static IMqttClient client;
        private static string broker;
        private static string username;
        private static string password;
        private static int port;
        private static string clientId;

        public MQTTTask(IConfigurationSection configuration, IServiceScopeFactory serviceScopeFactory, IHubContext<SignalR> hubContext, ITelegramBotService telegramBotService, ILogger<MQTTTask> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _hubContext = hubContext;
            
            //_context = serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<ApplicationDbContext>();
            //_positions = new PositionsController(_context, hubContext);
            //_measures = new MeasuresController(_context, hubContext);
            //_powermeter = new PowerMeterController(_context, hubContext);
            //_alarm = new AlarmsController(_context, hubContext);

            _telegramBotClient = telegramBotService.Client;
            _telegramBotChatId = telegramBotService.ChatId;

            _logger = logger;

            broker = configuration["Broker"];
            username = configuration["Username"];
            password = configuration["Password"];
            port = Convert.ToInt32(configuration["Port"]);
        }

        private static void Client_Disconnected(object sender, MqttEndpointDisconnected e)
        {
            _logger.LogError($"{e.Reason}: Client was disconnected from server.");
        }

        private static void Client_MqttMsgPublished(IObservable<MqttApplicationMessage> mqttApplicationMessage)
        {
            foreach (var message in mqttApplicationMessage)
            {
                _logger.LogInformation($"Payload {message.Payload} published to topic {message.Topic}.");
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            if (client.IsConnected)
            {
                await client.DisconnectAsync();
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var configuration = new MqttConfiguration()
            {
                Port = port,
                AllowWildcardsInTopicFilters = true,
                KeepAliveSecs = 30,
                ConnectionTimeoutSecs = 30,
                WaitTimeoutSecs = 30
            };
            client = await MqttClient.CreateAsync(broker, configuration);
            clientId = Guid.NewGuid().ToString().Replace("-", "");

            client.Disconnected += Client_Disconnected;

            do
            {
                try
                {
                    if (!client.IsConnected)
                    {
                        await Connect();
                    }

                    await Task.Delay(5000, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.GetType().ToString());
                    _logger.LogError(ex.Message);
                }
            }
            while (!stoppingToken.IsCancellationRequested);
        }

        private static async Task Connect()
        {
            var credentials = new MqttClientCredentials(clientId, username, password);

            var sessionState = await client.ConnectAsync(credentials);

            _logger.LogInformation($"Client {client.Id} connected to server with {sessionState}.");

            client.MessageStream.Subscribe(async msg => await Client_MqttMsgReceived(msg));

            await client.SubscribeAsync("telemetry/#", MqttQualityOfService.AtMostOnce);
            await client.SubscribeAsync("command/reply/#", MqttQualityOfService.ExactlyOnce);
        }

        public static async Task PublishCommand(Guid thingId, ThingActions thingAction, MqttQualityOfService qos = MqttQualityOfService.AtMostOnce, bool retained = false)
        {
            var message = new MqttApplicationMessage($"command/{thingId}/{thingAction.ToString().ToLower()}", Array.Empty<byte>());

            await client.PublishAsync(message,
                       qos,
                       retained);
        }

        private static async Task Client_MqttMsgReceived(MqttApplicationMessage mqttApplicationMessage)
        {
            string message = Encoding.UTF8.GetString(mqttApplicationMessage.Payload);
            string[] topic = mqttApplicationMessage.Topic.Split("/", StringSplitOptions.RemoveEmptyEntries);

            _logger.LogInformation($"Received message on topic {mqttApplicationMessage.Topic}: {message}");

            Guid thingId;

            switch (topic.First())
            {
                case "telemetry":
                    thingId = Guid.Parse(topic[1]);

                    switch (topic[2].ToLower().Trim())
                    {
                        case "tracker":
                            if (!(await ProcessTracker(thingId, mqttApplicationMessage.Payload)))
                            {
                                return;
                            }
                            break;
                        case "energymonitor":
                            if (!(await ProcessEnergyMonitor(thingId, mqttApplicationMessage.Payload)))
                            {
                                return;
                            }
                            break;
                        case "powermeter":
                            if (Enum.TryParse(topic[3], true, out PowerMeterMeasureType measureType))
                            {
                                if (!(await ProcessPowerMeter(thingId, mqttApplicationMessage.Payload, measureType)))
                                {
                                    return;
                                }
                            }
                            break;
                        case "alarm":
                            if (!(await ProcessAlarm(thingId, mqttApplicationMessage.Payload)))
                            {
                                return;
                            }
                            break;
                        default:
                            return;
                    }

                    break;
                case "commands":
                    if (topic.Length != 4)
                    {
                        _logger.LogWarning($"Invalid topic {mqttApplicationMessage.Topic}");
                        return;
                    }
                    thingId = Guid.Parse(topic[2]);
                    break;
                default:
                    return;
            }

            _logger.LogInformation($"Processed {topic.First()} message from device {thingId} to alter feature {topic.Last()}");
        }

        private async static Task<bool> ProcessTracker(Guid thingId, byte[] message)
        {
            var _context = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var _positions = new PositionsController(_context, _hubContext);

            var thing = _context.Thing.AsNoTracking().Where(thing => thing.ThingId.Equals(thingId)).FirstOrDefault();

            if (thing == null)
            {
                _logger.LogWarning($"Thing with Id {thingId} not found");
                return false;
            }
            else if (!thing.Features.HasFlag(FeatureType.Tracker))
            {
                _logger.LogWarning($"Thing with Id {thingId} does not have the specified feature {FeatureType.Tracker}");
                return false;
            }

            PositionDTO position;
            try
            {
                Stream positionStream = new MemoryStream(message);
                position = await JsonSerializer.DeserializeAsync<PositionDTO>(positionStream, new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true
                });
                position.ThingId = thingId;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }

            try
            {
                await _positions.PostPosition(position);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }

            return true;
        }

        private async static Task<bool> ProcessEnergyMonitor(Guid thingId, byte[] message)
        {
            var _context = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var _measures = new MeasuresController(_context, _hubContext);

            var thing = _context.Thing.AsNoTracking().Where(thing => thing.ThingId.Equals(thingId)).FirstOrDefault();

            MeasureDTO measure;
            try
            {
                Stream measureStream = new MemoryStream(message);
                var options = new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true
                };
                options.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
                measure = await JsonSerializer.DeserializeAsync<MeasureDTO>(measureStream, options);
                measure.ThingId = thingId;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }

            if (thing == null)
            {
                _logger.LogWarning($"Thing with Id {thingId} not found");
                return false;
            }
            else if (!thing.Features.ToString().Split(',', StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()).ToList().Contains(measure.Type.ToString().Trim()))
            {
                _logger.LogWarning($"Thing with Id {thingId} does not have the specified feature {measure.Type}");
                return false;
            }

            try
            {
                await _measures.PostMeasure(measure);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }

            return true;
        }

        private async static Task<bool> ProcessPowerMeter(Guid thingId, byte[] message, PowerMeterMeasureType measureType)
        {
            var _context = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var _powermeter = new PowerMeterController(_context, _hubContext);

            var thing = _context.Thing.AsNoTracking().Where(thing => thing.ThingId.Equals(thingId)).FirstOrDefault();

            if (thing == null)
            {
                _logger.LogWarning($"Thing with Id {thingId} not found");
                return false;
            }

            switch (measureType)
            {
                case PowerMeterMeasureType.Information:
                    break;
                case PowerMeterMeasureType.Total:
                    PowerMeterTotalDTO measureTotal;
                    try
                    {
                        Stream measureStream = new MemoryStream(message);
                        var options = new JsonSerializerOptions()
                        {
                            PropertyNameCaseInsensitive = true
                        };
                        options.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
                        measureTotal = await JsonSerializer.DeserializeAsync<PowerMeterTotalDTO>(measureStream, options);
                        measureTotal.ThingId = thingId;
                    }
                    catch (JsonException ex)
                    {
                        _logger.LogError(ex.Message);
                        return false;
                    }

                    try
                    {
                        await _powermeter.PostPowerMeterTotal(measureTotal);
                    }
                    catch (DbUpdateException ex)
                    {
                        _logger.LogError(ex.Message);
                        return false;
                    }
                    break;
                case PowerMeterMeasureType.Tariff:
                    PowerMeterTariffDTO measureTariff;
                    try
                    {
                        Stream measureStream = new MemoryStream(message);
                        var options = new JsonSerializerOptions()
                        {
                            PropertyNameCaseInsensitive = true
                        };
                        options.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
                        measureTariff = await JsonSerializer.DeserializeAsync<PowerMeterTariffDTO>(measureStream, options);
                        measureTariff.ThingId = thingId;
                    }
                    catch (JsonException ex)
                    {
                        _logger.LogError(ex.Message);
                        return false;
                    }

                    try
                    {
                        await _powermeter.PostPowerMeterTariff(measureTariff);
                    }
                    catch (DbUpdateException ex)
                    {
                        _logger.LogError(ex.Message);
                        return false;
                    }
                    break;
                case PowerMeterMeasureType.Instantaneous:
                    PowerMeterInstantaneousDTO measureInstantaneous;
                    try
                    {
                        Stream measureStream = new MemoryStream(message);
                        var options = new JsonSerializerOptions()
                        {
                            PropertyNameCaseInsensitive = true
                        };
                        options.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
                        measureInstantaneous = await JsonSerializer.DeserializeAsync<PowerMeterInstantaneousDTO>(measureStream, options);
                        measureInstantaneous.ThingId = thingId;
                    }
                    catch (JsonException ex)
                    {
                        _logger.LogError(ex.Message);
                        return false;
                    }

                    try
                    {
                        await _powermeter.PostPowerMeterInstantaneous(measureInstantaneous);
                    }
                    catch (DbUpdateException ex)
                    {
                        _logger.LogError(ex.Message);
                        return false;
                    }
                    break;
                default:
                    return false;
            }

            return true;
        }

        private async static Task<bool> ProcessAlarm(Guid thingId, byte[] message)
        {
            var _context = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var _alarm = new AlarmsController(_context, _hubContext);

            var thing = _context.Thing.AsNoTracking().Where(thing => thing.ThingId.Equals(thingId)).FirstOrDefault();

            if (thing == null)
            {
                _logger.LogWarning($"Thing with Id {thingId} not found");
                return false;
            }
            else if (!thing.Features.HasFlag(FeatureType.Alarm))
            {
                _logger.LogWarning($"Thing with Id {thingId} does not have the specified feature {FeatureType.Alarm}");
                return false;
            }

            try
            {
                string messageReceived = Encoding.Default.GetString(message);

                AlarmDTO alarm = new()
                {
                    ThingId = thingId
                };

                if (messageReceived.Contains("Open"))
                {
                    alarm.Type = AlarmType.Open;
                }
                else if (messageReceived.Contains("Closed"))
                {
                    alarm.Type = AlarmType.Close;
                }
                else if (messageReceived.Contains("id"))
                {
                    alarm.Type = AlarmType.Read;
                }
                else
                {
                    _logger.LogError($"Message from Thing with Id {thingId} not processed");
                    return false;
                }

                await _telegramBotClient.SendTextMessageAsync(_telegramBotChatId, messageReceived);

                try
                {
                    await _alarm.PostAlarm(alarm);
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex.Message);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }

            return true;
        }
    }
}