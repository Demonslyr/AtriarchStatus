using System;
using System.Collections.Immutable;
using AtriarchStatus.StatusClients.WorldOfWarcraft.Constants;
using AtriarchStatus.StatusClients.WorldOfWarcraft.Models;
using TimeZoneConverter;

namespace AtriarchStatus.StatusClients.WorldOfWarcraft
{
    public class IcecrownRaresStatus
    {
        private const int RareRotationTotal = 400; //minutes
        private const int RareSpawnOffset = 20; // minutes
        private static readonly DateTime BaseDate = new DateTime(2020, 11, 12, 23, 0, 0); //noth-the-plaguebringer start utc
        private const string PrepatchInfoWowHeadUrl = @"https://www.wowhead.com/guides/shadowlands-deaths-rising-prelaunch-event-scourge-invasions#icecrown-bosses";
        
        // modulo function that doesn't output negative numbers. Pretty much lets the array loop.
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
            var offsetBaseDate = BaseDate.AddMinutes(rotationsSinceBase * RareRotationTotal);
            
            var lastStartTime = offsetBaseDate.AddMinutes(spawnsIntoRotation * RareSpawnOffset);
            var upcomingStartTime = offsetBaseDate.AddMinutes((spawnsIntoRotation + 1) * RareSpawnOffset);
            var nextStartTime = offsetBaseDate.AddMinutes((spawnsIntoRotation + 2) * RareSpawnOffset);
            
            var lastRare = IcecrownRareConstants.Rares[Mod(spawnsIntoRotation, RareSpawnOffset)];
            var upcomingRare = IcecrownRareConstants.Rares[Mod(spawnsIntoRotation + 1, RareSpawnOffset)];
            var nextRare = IcecrownRareConstants.Rares[Mod(spawnsIntoRotation + 2, RareSpawnOffset)];

            // get current time
            // offset the base time to current rotation
            // solve for current rare
            // return previous Name, start time, countdown, map pin macro, wowhead link to mob
            // return current Name, start time, countdown, map pin macro, wowhead link to mob
            // return next Name, start time, countdown, map pin macro, wowhead link to mob

            // Link to wowhead page detailing the event
            // https://www.w3schools.com amalgamation
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
                    <td id = ""lastTime""> </td>
                    <td id = ""last""></td>
                    <td> <button onclick=""copyMacroToClipboard({lastRare.Xcoord},{lastRare.Ycoord})"">Copy Macro</button> </td>
                </tr>
         
                <tr>
                    <td> Upcoming Rare </td>
                    <td> <a href=""{upcomingRare.WowHeadUrl}"" target=""_blank"">{upcomingRare.Name}</a> </td>
                    <td id = ""upcomingTime""> </td>
                    <td id = ""upcoming""></td>
                    <td> <button onclick=""copyMacroToClipboard({upcomingRare.Xcoord},{upcomingRare.Ycoord})"">Copy Macro</button> </td>
                </tr>
         
                <tr>
                    <td> Next Rare </td>
                    <td> <a href=""{nextRare.WowHeadUrl}"" target=""_blank"">{nextRare.Name}</a> </td>
                    <td id = ""nextTime""> </td>
                    <td id = ""next""></td>
                    <td> <button onclick=""copyMacroToClipboard({nextRare.Xcoord},{nextRare.Ycoord})"">Copy Macro</button> </td>
                </tr>

                </table>
                </div>
             <script>
                // Set the date we're counting down to
                var lastRareDate = new Date(""{lastStartTime} UTC"").getTime();
                var upcomingRareDate = new Date(""{upcomingStartTime} UTC"").getTime();
                var nextRareDate = new Date(""{nextStartTime} UTC"").getTime();

                document.getElementById(""lastTime"").innerHTML = `${{new Date(lastRareDate).toLocaleTimeString('en-US')}}`;
                document.getElementById(""upcomingTime"").innerHTML = `${{new Date(upcomingRareDate).toLocaleTimeString('en-US')}}`;
                document.getElementById(""nextTime"").innerHTML = `${{new Date(nextRareDate).toLocaleTimeString('en-US')}}`;

                // Update the count down every 1 second
                var x = setInterval(function() {{

                    // Get today's date and time
                    var now = new Date().getTime();

                    // Find the distance between now and the count down date
                    var upcomingDistance = upcomingRareDate - now;
                    var nextDistance = nextRareDate - now;

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
                        document.getElementById(""next"").innerHTML = ""Spawned"";
                        clearInterval(x);
                    }}
                }}, 1000);
                function copyMacroToClipboard(x,y) {{
                  var macroText = `/run C_Map.SetUserWaypoint(UiMapPoint.CreateFromCoordinates(118,${{x}}/100,${{y}}/100));C_SuperTrack.SetSuperTrackedUserWaypoint(true);`;
                  navigator.clipboard.writeText(macroText);
                  alert(""Copied to clipboard: "" + macroText);
                }}
            </script>
            </body>
            </html>";
        }
    }
}
