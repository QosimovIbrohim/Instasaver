
using ScheckBot;

class Project
{
    static async Task Main(string[] args)
    {
        string token = "6555322044:AAEH28_Qxs05IzdAOyNckZKhoStVRH6nr3k";
        BotHandler bt = new BotHandler(token);

        try
        {
            bt.BotHandle().Wait();
        }
        catch
        {
            bt.BotHandle().Wait();
        }
    }
}