using CounterStrikeSharp.API; 
using CounterStrikeSharp.API.Core; 
using CounterStrikeSharp.API.Core.Attributes.Registration; 
using MenuManager;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Menu;
using CounterStrikeSharp.API.Modules.Utils;
using Newtonsoft.Json;
using CounterStrikeSharp.API.Core.Capabilities;
using Microsoft.Extensions.Logging;

namespace dContact; 
 
public class dContact : BasePlugin 
{ 
    public override string ModuleName => "[CS2] D3X - [ Kontakt z administracją ]";
    public override string ModuleAuthor => "D3X";
    public override string ModuleDescription => " Plugin dodaje na kontakt do administracji.";
    public override string ModuleVersion => "1.0.0";
    public static dContact Instance { get; private set; }
    public static List<Config.ContactData> adminsList;
    private IMenuApi? _api;
    private readonly PluginCapability<IMenuApi?> _pluginCapability = new("menu:nfcore");   

    public override void Load(bool hotReload)
    {
        Instance = this;
        Config.Initialize();

        if (Config.config.AdminContacts == null || !Config.config.AdminContacts.Any())
        {
            Logger.LogError("W Pliku konfiguracyjnym nie znaleziono żadnych administratorów.");
            return;
        }

        adminsList = Config.config.AdminContacts;

        var commands = new Dictionary<IEnumerable<string>, (string description, CommandInfo.CommandCallback handler)>
        {
            { SplitCommands(Config.config.Settings.Contact_Command), ("Displays contact menu", Command_Contact) }
        };

        foreach (var commandPair in commands)
        {
            foreach (var command in commandPair.Key)
            {
                Instance.AddCommand($"css_{command}", commandPair.Value.description, commandPair.Value.handler);
            }
        }
    }

    public override void OnAllPluginsLoaded(bool hotReload)
    {
        _api = _pluginCapability.Get();
        if (_api == null) Logger.LogWarning("MenuManager Core nie znaleziono...");
    }

    [CommandHelper(whoCanExecute: CommandUsage.CLIENT_ONLY)]
    public void Command_Contact(CCSPlayerController? player, CommandInfo info)
    {
        if (adminsList == null)
        {
            Logger.LogError($"Lista kontaktów nie została załadowana");
            return;
        }

        if (_api == null)
        {
            player.PrintToChat($"Błąd: Menu API niedostępne.");
            return;
        }

        var menu = _api.NewMenu($"<font color='{Config.config.Settings.TitleColor}'>{Config.config.Settings.Title}</font>");

        foreach (var contact in adminsList)
        {
            menu.AddMenuOption($" <font color='yellow'>★</font> <font color='red'>{contact.rank}:</font> <font color='yellow'>{contact.name}</font>", (player, option) =>
            {
                player.PrintToChat($" {ChatColors.Green}―――――――――――{ChatColors.DarkRed}◥◣◆◢◤{ChatColors.Green}―――――――――――");
                player.PrintToChat($" {ChatColors.Lime}[ {ChatColors.DarkRed}★ {ChatColors.Lime}Kontakt do administratora: {ChatColors.LightRed}{contact.name} {ChatColors.DarkRed}★ {ChatColors.Lime}]");
                player.PrintToChat($"➪ {ChatColors.Lime}Nick: {ChatColors.LightRed}{contact.name}");
                player.PrintToChat($"➪ {ChatColors.Lime}Ranga: {ChatColors.LightRed}{contact.rank}");
                player.PrintToChat($"➪ {ChatColors.Lime}Steam: {ChatColors.LightRed}{contact.steam}");
                player.PrintToChat($"➪ {ChatColors.Lime}Forum: {ChatColors.LightRed}{contact.forum}");
                player.PrintToChat($"➪ {ChatColors.Lime}Discord: {ChatColors.LightRed}{contact.discord}");
                player.PrintToChat($" {ChatColors.Green}―――――――――――{ChatColors.DarkRed}◥◣◆◢◤{ChatColors.Green}―――――――――――");
            });
        }

        menu.Open(player);
    }

    private static IEnumerable<string> SplitCommands(string commands)
    {
        if (string.IsNullOrWhiteSpace(commands)) return Enumerable.Empty<string>();
        return commands.Split(',').Select(c => c.Trim());
    }
} 
