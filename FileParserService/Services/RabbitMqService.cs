using System.Text;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

public class RabbitMqService
{
    private IConnection _connection{get;set;}


    public RabbitMqService(IConnection connection)
    {
        _connection = connection;
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