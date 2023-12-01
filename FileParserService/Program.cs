using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Services;
using Logger;

public class Program
{
    private static ILogger _logger {get;set;} = null;
    public static async Task Main(string[] args)
    {
        int size = args.Count();

        if(size < 5)
        {
            Console.WriteLine("Input 4 parameters: \"Directory\" : string, \"host\" : string, \"RabbitName\" : string, \"RabbitPassword\" : string, \"loggerPath\" : string");
            Environment.Exit(0);
        }

        string directory = args[0];
        string host = args[1];
        string UserName = args[2];
        string Password = args[3];
        string loggerPath = args[4];

        Console.WriteLine(loggerPath);

        ConnectionFactory factory = new ConnectionFactory
        {
            HostName = host,
            UserName = UserName,
            Password = Password,
            Port = 5672
        };

        _logger = new FileLogger(loggerPath);

        IConnection connection = await WaitUntilRabbitMqStart(factory);

        RabbitMqService rabbit = new RabbitMqService(connection);

        FileParserService fileParser = new FileParserService(_logger);
        
        while(true)
        {
            var list = await fileParser.DeserializeXmlFiles(directory);
            var json = fileParser.SerializeToJson(list);
            rabbit.SendMessage(json); 
            System.Threading.Thread.Sleep(1000);
        } 
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
}