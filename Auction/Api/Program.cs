namespace AuctionApi;

public class Program
{

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);  // MiniValidator 

        var app = builder.Build();

        app.Run();
    }
}


