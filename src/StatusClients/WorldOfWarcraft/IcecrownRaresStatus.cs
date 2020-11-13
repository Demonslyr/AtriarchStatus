using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace AtriarchStatus.StatusClients.WorldOfWarcraft
{
    public class IcecrownRaresStatus : Controller
    {
        private readonly IMemoryCache _cache;
        public IcecrownRaresStatus(IMemoryCache cache)
        {
            _cache = cache;
        }
        private class RareDescription
        {
            public string Name;
            public double Xcoord;
            public double Ycoord;
            public string WowHeadUrl;
            public string? AltName;
            public double? AltXcoord;
            public double? AltYcoord;

        }
        private static readonly ImmutableArray<RareDescription> Rares = ImmutableArray.Create<RareDescription>
        (
            new RareDescription{Name = "Noth the Plaguebringer",Xcoord = 31.6,Ycoord = 70.5,WowHeadUrl = @"https://www.wowhead.com/npc=174067/noth-the-plaguebringer"},
            new RareDescription{Name = "Patchwerk",Xcoord = 36.5,Ycoord = 67.4,WowHeadUrl = @"https://www.wowhead.com/npc=174066/patchwerk", AltName = "Patchwerk CAVE ENTRANCE", AltXcoord = 34.4, AltYcoord = 68.5},
            new RareDescription{Name = "Blood Queen Lana'thel",Xcoord = 49.7,Ycoord = 32.7,WowHeadUrl = @"https://www.wowhead.com/npc=174065/blood-queen-lanathel"},
            new RareDescription{Name = "Professor Putricide",Xcoord = 57.1,Ycoord = 30.3,WowHeadUrl = @"https://www.wowhead.com/npc=174064/professor-putricide"},
            new RareDescription{Name = "Lady Deathwhisper",Xcoord = 51.1,Ycoord = 78.5,WowHeadUrl = @"https://www.wowhead.com/npc=174063/lady-deathwhisper"},
            new RareDescription{Name = "Skadi the Ruthless",Xcoord = 57.8,Ycoord = 56.1,WowHeadUrl = @"https://www.wowhead.com/npc=174062/skadi-the-ruthless"},
            new RareDescription{Name = "Ingvar the Plunderer",Xcoord = 52.4,Ycoord = 52.6,WowHeadUrl = @"https://www.wowhead.com/npc=174061/ingvar-the-plunderer"},
            new RareDescription{Name = "Prince Keleseth",Xcoord = 54,Ycoord = 44.7,WowHeadUrl = @"https://www.wowhead.com/npc=174060/prince-keleseth"},
            new RareDescription{Name = "The Black Knight",Xcoord = 64.8,Ycoord = 22.1,WowHeadUrl = @"https://www.wowhead.com/npc=174059/the-black-knight"},
            new RareDescription{Name = "Bronjahm",Xcoord = 70.7,Ycoord = 38.4,WowHeadUrl = @"https://www.wowhead.com/npc=174058/bronjahm"},
            new RareDescription{Name = "Scourgelord Tyrannus",Xcoord = 47.2,Ycoord = 66.1,WowHeadUrl = @"https://www.wowhead.com/npc=174057/scourgelord-tyrannus"},
            new RareDescription{Name = "Forgemaster Garfrost",Xcoord = 58.6,Ycoord = 72.5,WowHeadUrl = @"https://www.wowhead.com/npc=174056/forgemaster-garfrost"},
            new RareDescription{Name = "Marwyn",Xcoord = 58.2,Ycoord = 83.4,WowHeadUrl = @"https://www.wowhead.com/npc=174055/marwyn"},
            new RareDescription{Name = "Falric",Xcoord = 50.2,Ycoord = 87.9,WowHeadUrl = @"https://www.wowhead.com/npc=174054/falric"},
            new RareDescription{Name = "The Prophet Tharon'ja",Xcoord = 80.1,Ycoord = 61.2,WowHeadUrl = @"https://www.wowhead.com/npc=174053/the-prophet-tharonja"},
            new RareDescription{Name = "Novos the Summoner",Xcoord = 77.8,Ycoord = 66.1,WowHeadUrl = @"https://www.wowhead.com/npc=174052/novos-the-summoner"},
            new RareDescription{Name = "Trollgore",Xcoord = 58.3,Ycoord = 39.4,WowHeadUrl = @"https://www.wowhead.com/npc=174051/trollgore"},
            new RareDescription{Name = "Krik'thir the Gatewatcher",Xcoord = 67.5,Ycoord = 58,WowHeadUrl = @"https://www.wowhead.com/npc=174050/krikthir-the-gatewatcher"},
            new RareDescription{Name = "Prince Taldaram",Xcoord = 29.6,Ycoord = 62.2,WowHeadUrl = @"https://www.wowhead.com/npc=174049/prince-taldaram"},
            new RareDescription{Name = "Elder Nadox",Xcoord = 44.2,Ycoord = 49.1,WowHeadUrl = @"https://www.wowhead.com/npc=174048/elder-nadox"}
        );

        private const int RareRotationTotal = 400; //minutes
        private const int RareSpawnOffset = 20; // minutes
        private static readonly DateTime BaseDate = new DateTime(2020, 11, 12, 23, 0, 0); //noth-the-plaguebringer start utc
        private readonly TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
        private const string PrepatchInfoWowHeadUrl = @"https://www.wowhead.com/guides/shadowlands-deaths-rising-prelaunch-event-scourge-invasions#icecrown-bosses";

        [HttpGet]
        [Route("wow/icrares")]
        public async Task<IActionResult> GetCachedUpcomingRares()
        {
            var resultString = "<html><body>Failed to get status</body></html>";
            try
            {
                var cacheEntry = _cache.GetOrCreate<string>("wow/icrares", entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(15);
                    var statusResult = GetUpcomingRare();
                    return string.IsNullOrWhiteSpace(statusResult)
                        ? "<html><body>Failed to get status</body></html>"
                        : statusResult;
                });
                resultString = cacheEntry;
            }
            catch
            {
                // I don't care if this fails
            }

            ViewBag.HtmlOutput(resultString);
            return View("~/StatusClients/WorldOfWarcraft/icrares.cshtml");
        }
        public string GetUpcomingRare()
        {
            // All time is in UTC
            var now = DateTime.UtcNow;
            int minutesApart = (now - BaseDate).Minutes;
            var rotationsSinceBase = minutesApart / RareRotationTotal;
            int spawnsIntoRotation = (minutesApart % RareRotationTotal) / RareSpawnOffset;
            DateTime lastStartTime = BaseDate.AddMinutes(rotationsSinceBase * RareRotationTotal).AddMinutes((spawnsIntoRotation - 1) * RareSpawnOffset);
            DateTime upcomingStartTime = BaseDate.AddMinutes(rotationsSinceBase * RareRotationTotal).AddMinutes(spawnsIntoRotation * RareSpawnOffset);
            DateTime nextStartTime = BaseDate.AddMinutes(rotationsSinceBase * RareRotationTotal).AddMinutes((spawnsIntoRotation + 1) * RareSpawnOffset);
            var lastRare = Rares[Math.Abs((spawnsIntoRotation - 1) % 20)];
            var upcomingRare = Rares[(spawnsIntoRotation) % 20];
            var nextRare = Rares[(spawnsIntoRotation + 1) % 20];

            // get current time
            // offset the base time to current rotation
            // solve for current rare
            // return previous Name, start time, countdown, map pin macro, wowhead link to mob
            // return current Name, start time, countdown, map pin macro, wowhead link to mob
            // return next Name, start time, countdown, map pin macro, wowhead link to mob

            // Link to wowhead page detailing the event
            // https://www.w3schools.com/ amalgamation
            var responsePage = $@"<!DOCTYPE HTML>
            <html>
            <head>
            <meta name=""viewport"" content=""width = device - width, initial - scale = 1"">
            < style >
                p {{
                text - align: center;
                font - size: 60px;
                margin - top: 0px;
                }}
                table {{
                  border-collapse: collapse;
                  border-spacing: 0;
                  width: 100%;
                  border: 1px solid #ddd;
                }}

                th, td {{
                  text-align: left;
                  padding: 8px;
                }}

                tr:nth-child(even){{background-color: #f2f2f2}}
            </ style >
                </ head >
                < body >
                <a href=""{PrepatchInfoWowHeadUrl}"" target=""_blank""><h2>Prepatch Wowhead Info</h2></a>
                <div style=""overflow - x:auto; "">
                < table >
   
                < tr >
                    < th ></ th >
                    < th > Name </ th >
                    < th > Start Time </ th >
                    < th > Countdown </ th >
                    < th > Wowhead Mob Info </ th >
                    < th > Map Waypoint </ th >
                </ tr >
         
                < tr >
                    < td > Last Rare </ td >
                    < td > {lastRare.Name} </ td >
                    < td > {TimeZoneInfo.ConvertTimeFromUtc(lastStartTime, cstZone).ToShortTimeString()} CST </ td >
                    < td id = ""last""></ td >
                    < td > <a href=""{lastRare.WowHeadUrl}"" target=""_blank"">Link</a> </ td >
                    < td > <button onclick=""copyMacroToClipboard({lastRare.Xcoord},{lastRare.Ycoord})"" >Copy Macro</button> </ td >
                </ tr >
         
                < tr >
                    < td > Upcoming Rare </ td >
                    < td > {upcomingRare.Name} </ td >
                    < td > {TimeZoneInfo.ConvertTimeFromUtc(upcomingStartTime, cstZone).ToShortTimeString()} CST </ td >
                    < td id = ""upcoming""></ td >
                    < td > <a href=""{upcomingRare.WowHeadUrl}"" target=""_blank"">Link</a> </ td >
                    < td > <button onclick=""copyMacroToClipboard({upcomingRare.Xcoord},{upcomingRare.Ycoord})"">Copy Macro</button> </ td >
                </ tr >
         
                < tr >
                    < td > Next Rare </ td >
                    < td > {nextRare.Name} </ td >
                    < td > {TimeZoneInfo.ConvertTimeFromUtc(nextStartTime, cstZone).ToShortTimeString()} CST </ td >
                    < td id = ""next""></ td >
                    < td > <a href=""{nextRare.WowHeadUrl}"" target=""_blank"">Link</a> </ td >
                    < td > <button onclick=""copyMacroToClipboard({nextRare.Xcoord},{nextRare.Ycoord})"">Copy Macro</button> </ td >
                </ tr >

                </ table >
                </ div >
             < script >
                // Set the date we're counting down to
                var countDownDate = new Date(""Jan 5, 2021 15:37:25"").getTime();

            // Update the count down every 1 second
            var x = setInterval(function() {{

                // Get today's date and time
                var now = new Date().getTime();

                // Find the distance between now and the count down date
                var distance = countDownDate - now;

                // Time calculations for days, hours, minutes and seconds
                var minutes = Math.floor((distance % (1000 * 60 * 60)) / (1000 * 60));
                var seconds = Math.floor((distance % (1000 * 60)) / 1000);

                // Output the result in an element
                document.getElementById(""last"").innerHTML = minutes + ""m "" + seconds + ""s "";
                document.getElementById(""upcoming"").innerHTML = minutes + ""m "" + seconds + ""s "";
                document.getElementById(""next"").innerHTML = minutes + ""m "" + seconds + ""s "";

                // If the count down is over, write some text 
                if (distance < 0)
                {{
                    clearInterval(x);
                    document.getElementById(""demo"").innerHTML = ""Spawned"";
                }}
            }}, 1000);
            function copyMacroToClipboard(x,y) {{
              var macroText = `/run b=C_Map;c='player';d=b.GetBestMapForUnit(c);b.SetUserWaypoint(UiMapPoint.CreateFromCoordinates(d,${{x}},${{y}}));`;
              navigator.clipboard.writeText(macroText);
              alert(""Copied to clipboard: "" + macroText);
            }}
            </ script >
            </ body >
            </ html >"; 
            return responsePage;
        }
    }
}
