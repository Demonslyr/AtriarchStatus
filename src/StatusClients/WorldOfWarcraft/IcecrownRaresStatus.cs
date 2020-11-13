using System;
using System.Collections.Immutable;
using TimeZoneConverter;

namespace AtriarchStatus.StatusClients.WorldOfWarcraft
{
    public class IcecrownRaresStatus
    {
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
            new RareDescription{Name = "Noth the Plaguebringer",Xcoord = 31.6,Ycoord = 70.5,WowHeadUrl = @"https://www.wowhead.com/npc=174067/noth-the-plaguebringer" },
            new RareDescription{Name = "Patchwerk",Xcoord = 36.5,Ycoord = 67.4,WowHeadUrl = @"https://www.wowhead.com/npc=174066/patchwerk", AltName = "Patchwerk CAVE ENTRANCE", AltXcoord = 34.4, AltYcoord = 68.5},
            new RareDescription{Name = "Blood Queen Lana'thel",Xcoord = 49.7,Ycoord = 32.7,WowHeadUrl = @"https://www.wowhead.com/npc=174065/blood-queen-lanathel" },
            new RareDescription{Name = "Professor Putricide",Xcoord = 57.1,Ycoord = 30.3,WowHeadUrl = @"https://www.wowhead.com/npc=174064/professor-putricide" },
            new RareDescription{Name = "Lady Deathwhisper",Xcoord = 51.1,Ycoord = 78.5,WowHeadUrl = @"https://www.wowhead.com/npc=174063/lady-deathwhisper" },
            new RareDescription{Name = "Skadi the Ruthless",Xcoord = 57.8,Ycoord = 56.1,WowHeadUrl = @"https://www.wowhead.com/npc=174062/skadi-the-ruthless" },
            new RareDescription{Name = "Ingvar the Plunderer",Xcoord = 52.4,Ycoord = 52.6,WowHeadUrl = @"https://www.wowhead.com/npc=174061/ingvar-the-plunderer" },
            new RareDescription{Name = "Prince Keleseth",Xcoord = 54,Ycoord = 44.7,WowHeadUrl = @"https://www.wowhead.com/npc=174060/prince-keleseth" },
            new RareDescription{Name = "The Black Knight",Xcoord = 64.8,Ycoord = 22.1,WowHeadUrl = @"https://www.wowhead.com/npc=174059/the-black-knight" },
            new RareDescription{Name = "Bronjahm",Xcoord = 70.7,Ycoord = 38.4,WowHeadUrl = @"https://www.wowhead.com/npc=174058/bronjahm" },
            new RareDescription{Name = "Scourgelord Tyrannus",Xcoord = 47.2,Ycoord = 66.1,WowHeadUrl = @"https://www.wowhead.com/npc=174057/scourgelord-tyrannus" },
            new RareDescription{Name = "Forgemaster Garfrost",Xcoord = 58.6,Ycoord = 72.5,WowHeadUrl = @"https://www.wowhead.com/npc=174056/forgemaster-garfrost" },
            new RareDescription{Name = "Marwyn",Xcoord = 58.2,Ycoord = 83.4,WowHeadUrl = @"https://www.wowhead.com/npc=174055/marwyn" },
            new RareDescription{Name = "Falric",Xcoord = 50.2,Ycoord = 87.9,WowHeadUrl = @"https://www.wowhead.com/npc=174054/falric" },
            new RareDescription{Name = "The Prophet Tharon'ja",Xcoord = 80.1,Ycoord = 61.2,WowHeadUrl = @"https://www.wowhead.com/npc=174053/the-prophet-tharonja" },
            new RareDescription{Name = "Novos the Summoner",Xcoord = 77.8,Ycoord = 66.1,WowHeadUrl = @"https://www.wowhead.com/npc=174052/novos-the-summoner" },
            new RareDescription{Name = "Trollgore",Xcoord = 58.3,Ycoord = 39.4,WowHeadUrl = @"https://www.wowhead.com/npc=174051/trollgore" },
            new RareDescription{Name = "Krik'thir the Gatewatcher",Xcoord = 67.5,Ycoord = 58,WowHeadUrl = @"https://www.wowhead.com/npc=174050/krikthir-the-gatewatcher" },
            new RareDescription{Name = "Prince Taldaram",Xcoord = 29.6,Ycoord = 62.2,WowHeadUrl = @"https://www.wowhead.com/npc=174049/prince-taldaram" },
            new RareDescription{Name = "Elder Nadox",Xcoord = 44.2,Ycoord = 49.1,WowHeadUrl = @"https://www.wowhead.com/npc=174048/elder-nadox" }
        );

        private const int RareRotationTotal = 400; //minutes
        private const int RareSpawnOffset = 20; // minutes
        private static readonly DateTime BaseDate = new DateTime(2020, 11, 12, 23, 0, 0); //noth-the-plaguebringer start utc
        private readonly TimeZoneInfo _cstZone = TimeZoneInfo.CreateCustomTimeZone("Central Standard Time", new TimeSpan(-6,00,00), "(GMT - 06:00) America/Chicago", "Central Standard Time");
        private const string PrepatchInfoWowHeadUrl = @"https://www.wowhead.com/guides/shadowlands-deaths-rising-prelaunch-event-scourge-invasions#icecrown-bosses";
        private static int Mod(int x, int m)
        {
            var r = x % m;
            return r < 0 ? r + m : r;
        }
        public string GetUpcomingRare()
        {
            // All time is in UTC
            var minutesApart = (int)(DateTime.UtcNow - BaseDate).TotalMinutes;
            var rotationsSinceBase = minutesApart / RareRotationTotal;
            var spawnsIntoRotation = Mod(minutesApart, RareRotationTotal) / RareSpawnOffset;
            var lastStartTime = BaseDate.AddMinutes(rotationsSinceBase * RareRotationTotal).AddMinutes(spawnsIntoRotation * RareSpawnOffset);
            var upcomingStartTime = BaseDate.AddMinutes(rotationsSinceBase * RareRotationTotal).AddMinutes((spawnsIntoRotation+1) * RareSpawnOffset);
            var nextStartTime = BaseDate.AddMinutes(rotationsSinceBase * RareRotationTotal).AddMinutes((spawnsIntoRotation + 2) * RareSpawnOffset);
            var lastRare = Rares[Mod(spawnsIntoRotation, RareSpawnOffset)];
            var upcomingRare = Rares[Mod(spawnsIntoRotation+1, RareSpawnOffset)];
            var nextRare = Rares[Mod(spawnsIntoRotation + 2, RareSpawnOffset)];

            // get current time
            // offset the base time to current rotation
            // solve for current rare
            // return previous Name, start time, countdown, map pin macro, wowhead link to mob
            // return current Name, start time, countdown, map pin macro, wowhead link to mob
            // return next Name, start time, countdown, map pin macro, wowhead link to mob

            // Link to wowhead page detailing the event
            // https://www.w3schools.com/ amalgamation
            return $@"<!DOCTYPE HTML>
            <html>
            <head>
            <meta http-equiv=""refresh"" content=""30"">
            <style>
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
            </style>
                </head>
                <body>
                <a href=""{PrepatchInfoWowHeadUrl}"" target=""_blank""><h2>Prepatch Wowhead Info</h2></a>
                <div style=""overflow - x:auto; "">
                <table>
   
                <tr>
                    <th></th>
                    <th> Name </th>
                    <th> Spawn Time </th>
                    <th> Countdown </th>
                    <th> Map Waypoint </th>
                </tr>
         
                <tr>
                    <td> Last Rare </td>
                    <td> <a href=""{lastRare.WowHeadUrl}"" target=""_blank"">{lastRare.Name}</a> </td>
                    <td> {TimeZoneInfo.ConvertTimeFromUtc(lastStartTime, _cstZone).ToShortTimeString()} CST </td>
                    <td id = ""last""></td>
                    <td> <button onclick=""copyMacroToClipboard({lastRare.Xcoord},{lastRare.Ycoord})"">Copy Macro</button> </td>
                </tr>
         
                <tr>
                    <td> Upcoming Rare </td>
                    <td> <a href=""{upcomingRare.WowHeadUrl}"" target=""_blank"">{upcomingRare.Name}</a> </td>
                    <td> {TimeZoneInfo.ConvertTimeFromUtc(upcomingStartTime, _cstZone).ToShortTimeString()} CST </td>
                    <td id = ""upcoming""></td>
                    <td> <button onclick=""copyMacroToClipboard({upcomingRare.Xcoord},{upcomingRare.Ycoord})"">Copy Macro</button> </td>
                </tr>
         
                <tr>
                    <td> Next Rare </td>
                    <td> <a href=""{nextRare.WowHeadUrl}"" target=""_blank"">{nextRare.Name}</a> </td>
                    <td> {TimeZoneInfo.ConvertTimeFromUtc(nextStartTime, _cstZone).ToShortTimeString()} CST </td>
                    <td id = ""next""></td>
                    <td> <button onclick=""copyMacroToClipboard({nextRare.Xcoord},{nextRare.Ycoord})"">Copy Macro</button> </td>
                </tr>

                </table>
                </div>
             <script>
                // Set the date we're counting down to
                //var lastRareDate = new Date(""{lastStartTime} CST"").getTime();
                var upcomingRareDate = new Date(""{upcomingStartTime} CST"").getTime();
                var nextRareDate = new Date(""{nextStartTime} CST"").getTime();

            // Update the count down every 1 second
            var x = setInterval(function() {{

                // Get today's date and time
                var now = new Date().getTime();

                // Find the distance between now and the count down date
                //var lastDistance = lastRareDate - now;
                var upcomingDistance = upcomingRareDate - now;
                var nextDistance = nextRareDate - now;

                // Time calculations for minutes and seconds
                // Output the result in an element
                //document.getElementById(""last"").innerHTML = Math.floor((lastDistance % (1000 * 60 * 60)) / (1000 * 60)) + ""m "" + Math.floor((lastDistance % (1000 * 60)) / 1000) + ""s "";

                // Time calculations for minutes and seconds
                // Output the result in an element
                document.getElementById(""upcoming"").innerHTML = Math.floor((upcomingDistance % (1000 * 60 * 60)) / (1000 * 60)) + ""m "" + Math.floor((upcomingDistance % (1000 * 60)) / 1000) + ""s "";

                // Time calculations for minutes and seconds
                // Output the result in an element
                document.getElementById(""next"").innerHTML = Math.floor((nextDistance % (1000 * 60 * 60)) / (1000 * 60)) + ""m "" + Math.floor((nextDistance % (1000 * 60)) / 1000) + ""s "";

                // If the count down is over, write some text 
                document.getElementById(""last"").innerHTML = ""Spawned"";
                if (upcomingDistance <0)
                {{
                    document.getElementById(""upcoming"").innerHTML = ""Spawned"";
                }}
                if (nextDistance <0)
                {{
                    clearInterval(x);
                    document.getElementById(""next"").innerHTML = ""Spawned"";
                }}
            }}, 1000);
            function copyMacroToClipboard(x,y) {{
              var macroText = `/run C_Map.SetUserWaypoint(UiMapPoint.CreateFromCoordinates(118,${{x}}/100,${{y}}/100));`;
              navigator.clipboard.writeText(macroText);
              alert(""Copied to clipboard: "" + macroText);
            }}
            </script>
            </body>
            </html>";
        }
    }
}
