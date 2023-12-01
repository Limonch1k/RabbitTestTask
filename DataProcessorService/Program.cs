using System.Threading.Channels;
using api_fact_weather_by_city.Mapper;
using AutoMapper;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using Logger;

class Program
{
    public static async Task Main(string[] args)
    {
        int size = args.Count();

        if(size < 5)
        {
            Console.WriteLine("Input 4 parameters: \"host\" : string, \"RabbitName\" : string, \"RabbitPassword\" : string, \"loggetPath\" : string");
            Environment.Exit(0);
        }

        string host = args[0];
        string UserName = args[1];
        string Password = args[2];
        string loggerPath = args[3];
        string dbPath = args[4];

        Console.WriteLine(loggerPath);

        Console.WriteLine(dbPath);

        ConnectionFactory factory = new ConnectionFactory
        {
            HostName = host,
            UserName = UserName,
            Password = Password,
            Port = 5672
        };

        var mapperConfig = new MapperConfiguration(mc =>
        {       
            mc.AddProfile(new DeviceStatus_to_ModuleCategoty());
        });

        FileLogger fileLogger = new FileLogger(loggerPath);

        IMapper mapper = mapperConfig.CreateMapper();

        IConnection connection = await WaitUntilRabbitMqStart(factory);

        SqliteContext context = new SqliteContext(dbPath);

        Repository repository = new Repository(context);

        GetMessageRabbitMqService rabbitMq = new GetMessageRabbitMqService(connection, repository, mapper, fileLogger);

        try
        {
            rabbitMq.GetMessage();
        }
        catch (OperationInterruptedException ex)
        {
            fileLogger.LogInformation("Drop connection to RabbitMq");
        }

        while(true)
        {
            
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
                    Console.WriteLine("the method has been completed");
                    return null;
                }
                try
                {
                    var connection = factory.CreateConnection();
                    Console.WriteLine("Connection was successfull created");
                    return connection;
                }
                catch (RabbitMQ.Client.Exceptions.BrokerUnreachableException)
                {
                    Console.WriteLine("RabbitMq still does not start");
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