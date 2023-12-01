using System.Net.Http.Json;
using System.Text;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

public class GetMessageRabbitMqService
{
    private Repository _repo {get;set;}
    private IConnection _connection {get;set;}
    
    private IMapper _mapper {get;set;}

    private ILogger _logger {get;set;}

    public GetMessageRabbitMqService(IConnection connection, Repository repo, IMapper mapper, ILogger logger)
    {
        _connection = connection;
        _repo = repo;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task GetMessage()
    {
        var channel = _connection.CreateModel();
        
        channel.QueueDeclare(queue: "person_queue",
                            durable: false,
                            exclusive: false,
                            autoDelete: false,
                            arguments: null);

        EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
        consumer.Received += async (model, ea) =>
        {

            _logger.LogInformation( "I catch message");

            byte[] body = ea.Body.ToArray();

            string message = Encoding.UTF8.GetString(body);

            _logger.LogInformation("try convert data to object");

            List<InstrumentStatus> instruments = null;

            try
            {
                instruments = JsonConvert.DeserializeObject<List<InstrumentStatus>>(message);
            }
            catch
            {
                _logger.LogInformation("I could not convert data to object");
                return;
            }

            _logger.LogInformation("Pass data convert stage");

            List<DeviceStatus> dev_list = new List<DeviceStatus>();

            foreach(var ins in instruments)
            {
                foreach(var dev in ins.DeviceStatuses)
                {
                    dev_list.Add(dev);
                }
            }

            try
            {
                List<ModuleCategory> list = _mapper.Map<List<ModuleCategory>>(dev_list);

                _repo.AddModuleCategoty(list);

                await _repo.SaveChanges();  

                _logger.LogInformation("IF go to this point, then everythink ok");

            }
            catch
            {
                _logger.LogError("Somethink went wrong with data write in db");
                return;
            }        
        };

        channel.BasicConsume(queue: "person_queue",
                            autoAck: true,
                            consumer: consumer);
    }
}