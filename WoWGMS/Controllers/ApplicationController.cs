using Microsoft.AspNetCore.Mvc;
using System;
namespace WoWGMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApplicationController : ControllerBase
    {
        [HttpPost]
        public IActionResult ReceiveApplication([FromBody] RawApplicationDto raw)
        {
            try
            {
                var parsed = new ApplicationDto
                {
                    DiscordUsername = raw.DiscordUsername,
                    CharacterName = raw.CharacterName,
                    ServerName = Enum.Parse<ServerName>(Sanitize(raw.ServerName), true),
                    Class = Enum.Parse<Class>(Sanitize(raw.Class), true),
                    MainRole = Enum.Parse<MainRole>(Sanitize(raw.MainRole), true)
                };

                // Process or save parsed data here
                Console.WriteLine($"New application: {parsed.DiscordUsername}, {parsed.CharacterName}, {parsed.ServerName}, {parsed.Class}, {parsed.MainRole}");

                return Ok(new { status = "Application received" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Invalid data received", details = ex.Message });
            }
        }

        // Simple sanitizer: removes spaces
        private string Sanitize(string? input)
        {
            return (input ?? string.Empty)
                   .Replace(" ", "")
                   .Trim();
        }
    }

    public class RawApplicationDto
    {
        public string? DiscordUsername { get; set; }
        public string? CharacterName { get; set; }
        public string? ServerName { get; set; }
        public string? Class { get; set; }
        public string? MainRole { get; set; }
    }

    public class ApplicationDto
    {
        public string? DiscordUsername { get; set; }
        public string? CharacterName { get; set; }
        public ServerName ServerName { get; set; }
        public Class Class { get; set; }
        public MainRole MainRole { get; set; }
    }

    // Enums
    public enum Class { Mage, Warrior, Rogue, Monk, Priest, DeathKnight, Evoker, Druid, Warlock, Paladin, Hunter, Shaman, DemonHunter }
    public enum MainRole { Tank, Healer, MeleeDPS, RangedDPS }
    public enum ServerName
        {
        Aegwynn,
        Antonidas,
        Archimonde,
        ArgentDawn,
        Aggramar,
        AlAkir,
        Alexstrasza,
        Alleria,
        Alonsus,
        Anetheron,
        Anubarak,
        Area52,
        Arthas,
        AzjolNerub,
        Blackrock,
        Blackmoore,
        Bloodhoof,
        ChamberOfAspects,
        ChantsEternels,
        ConfrerieDuThorium,
        Dalaran,
        DefiasBrotherhood,
        Destromath,
        DieAldor,
        Doomhammer,
        Draenor,
        Drakthul,
        DunModr,
        DunMorogh,
        Elune,
        EmeraldDream,
        Eonar,
        Eredar,
        Exodar,
        Frostwhisper,
        Frostwolf,
        Garrosh,
        GrimBatol,
        Hyjal,
        Illidan,
        KaelThas,
        Kargath,
        Kazzak,
        Khazgoroth,
        KhazModan,
        Kilrogg,
        KultDerVerdammten,
        Lightbringer,
        Lothar,
        Magtheridon,
        MalGanis,
        Malfurion,
        Medivh,
        Moonglade,
        Nemesis,
        Nordrassil,
        Onyxia,
        Outland,
        Ragnaros,
        Ravencrest,
        Sanguino,
        Sargeras,
        Shadowsong,
        ShatteredHand,
        Silvermoon,
        Stormrage,
        Stormreaver,
        Stormscale,
        Sunstrider,
        Sylvanas,
        TarrenMill,
        TheMaelstrom,
        Thunderhorn,
        TwilightsHammer,
        Tyrande,
        Uldaman,
        TwistingNether,
        WellOfEternity,
        Ysera,
        Ysondre,
        ZirkelDesCenarius,
        Разувий,
        ПиратскаяБухта,
        КорольЛич,
        Азурегос,
        ВечнаяПесня,
        Гордунни,
        Дракономор,
        РевущийФьорд,
        СвежевательДуш,
        СтражСмерти,
        ЯсеневыйЛес
    }
}