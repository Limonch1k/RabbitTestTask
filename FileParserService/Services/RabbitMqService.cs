using System.Text;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

public class RabbitMqService
{
    private IConnection _connection{get;set;}

    private ILogger _logger {get;set;}

    public RabbitMqService(IConnection connection, ILogger logger)
    {
        _connection = connection;

        _logger = logger;
    }

    public static async Task<IConnection> WaitUntilRabbitMqStart(ConnectionFactory factory)
    {
        CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
        CancellationToken token = cancelTokenSource.Token;

        Task<IConnection> t = Task.Run<IConnection>( () =>
        {
            bool b = true;
            while(b)
            {
                if (token.IsCancellationRequested)
                {
                    return null;
                }
                try
                {
                    var connection = factory.CreateConnection();
                    return connection;
                }
                catch (RabbitMQ.Client.Exceptions.BrokerUnreachableException)
                {
                    System.Threading.Thread.Sleep(TimeSpan.FromSeconds(2));
                }
            }

            return null;
        }, token);

        t.Wait(16000);

        if (t.IsCompletedSuccessfully)
        {
            return t.Result;
        }
        else
        {
            cancelTokenSource.Cancel();
            var return_obj = await t;
            return return_obj;
        }

        return null;
    }

    public void SendMessage(string message)
    {
        var channel = _connection.CreateModel();

        channel.QueueDeclare(queue: "person_queue",
                            durable: false,
                            exclusive: false,
                            autoDelete: false,
                            arguments: null);

        byte[] body = Encoding.UTF8.GetBytes(message);

        channel.BasicPublish(exchange: "",
                            routingKey: "person_queue",
                            basicProperties: null,
                            body: body);

        channel.Dispose();
    }

}