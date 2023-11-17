namespace SKELETON_KING;

using Microsoft.EntityFrameworkCore;
using KINESIS;
using PUZZLEBOX;
using System.Collections.Concurrent;
using ZORGATH;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        builder.Services.AddControllers();

        string connectionString = builder.Configuration.GetConnectionString("BOUNTY")!;
        builder.Services.AddDbContextFactory<BountyContext>(options =>
        {
            options.UseSqlServer(connectionString, connection => connection.MigrationsAssembly("SKELETON-KING")).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        });

        // TODO: store in a configuration file.
        ChatServerConfiguration chatServerConfiguration = new ChatServerConfiguration("localhost", 11031, 11032, 11033);
        ConcurrentDictionary<string, SrpAuthSessionData> srpAuthSessions = new();

        builder.Services.AddSingleton<IReadOnlyDictionary<string, IClientRequestHandler>>(
            new Dictionary<string, IClientRequestHandler>()
            {
                // NOTE: Please keep this list alphabetized by the string literal in the key.
                {"autocompleteNicks", new AutoCompleteNicksHandler() },
                {"get_match_stats", new GetMatchStatsHandler(replayServerUrl: "http://api.kongor.online") },
                {"match_history_overview", new MatchHistoryOverviewHandler() },
                {"pre_auth", new PreAuthHandler(srpAuthSessions) },
                {"show_simple_stats", new ShowSimpleStatsHandler() },
                {"srpAuth", new SrpAuthHandler(srpAuthSessions, new(), chatServerUrl: chatServerConfiguration.Address, icbUrl: "kongor.online") },
            }
        );

        VersionProvider versionProvider = new VersionProvider("http://gitea.kongor.online", "/administrator/KONGOR/raw/branch/main/patch/");
        builder.Services.AddSingleton<IReadOnlyDictionary<string, IServerRequestHandler>>(
            new Dictionary<string, IServerRequestHandler>()
            {
                // NOTE: Please keep this list alphabetized by the string literal in the key.
                {"new_session", new NewSessionHandler(versionProvider, chatServerConfiguration.Address, chatServerConfiguration.ServerPort) },
                {"replay_auth", new ReplayAuthHandler(chatServerConfiguration.Address, chatServerConfiguration.ManagerPort) },
                {"set_online", new SetOnlineHandler() },
            }
        );

        builder.Services.AddSingleton<ChatServerConfiguration>(chatServerConfiguration);
        builder.Services.AddSingleton<ChatServer>();
        builder.Services.AddControllers().AddApplicationPart(typeof(ClientRequesterController).Assembly);

        var app = builder.Build();
        app.MapControllers();
        app.Services.GetRequiredService<ChatServer>().Start();
        app.Run();
    }
}
