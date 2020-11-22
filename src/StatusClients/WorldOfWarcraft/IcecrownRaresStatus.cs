using AtriarchStatus.StatusClients.WorldOfWarcraft.Constants;
using System;
using System.Linq;

namespace AtriarchStatus.StatusClients.WorldOfWarcraft
{
    public class IcecrownRaresStatus
    {
        private const int RareSpawnOffset = 10; // minutes
        private static int RareRotationTotal = (IcecrownRareConstants.Rares.Length*RareSpawnOffset); //minutes
        private static readonly DateTime BaseDate = new DateTime(2020, 11, 16, 23, 40, 0); //noth-the-plaguebringer start utc
        private const string PrepatchInfoWowHeadUrl = @"https://www.wowhead.com/guides/shadowlands-deaths-rising-prelaunch-event-scourge-invasions#icecrown-bosses";
        
        // modulo function that doesn't output negative numbers. Pretty much lets the array loop.
        private static int Mod(int x, int m)
        {
            var r = x % m;
            return r < 0 ? r + m : r;
        }

        private string GetTableMarker(int index, int spawnsIntoRotation)
        {
            return (index -Mod(spawnsIntoRotation, IcecrownRareConstants.Rares.Length)) switch
            {
                0 => "Last",// Last
                1 => "Upcoming",// Upcoming
                2 => "Next",// Next
                _ => "",
            };

            //(index == Mod(spawnsIntoRotation + 1, IcecrownRareConstants.Rares.Length) ? ">" : "");
        }
        public string GetUpcomingRare()
        {
            // All time is in UTC
            var now = DateTime.UtcNow;
            var minutesApart = (int)(now - BaseDate).TotalMinutes;
            var rotationsSinceBase = minutesApart / RareRotationTotal;
            var spawnsIntoRotation = Mod(minutesApart, RareRotationTotal) / RareSpawnOffset;
            var offsetBaseDate = BaseDate.AddMinutes(rotationsSinceBase * RareRotationTotal);
            
            var lastStartTime = offsetBaseDate.AddMinutes(spawnsIntoRotation * RareSpawnOffset);
            var upcomingStartTime = offsetBaseDate.AddMinutes((spawnsIntoRotation + 1) * RareSpawnOffset);
            var nextStartTime = offsetBaseDate.AddMinutes((spawnsIntoRotation + 2) * RareSpawnOffset);
            
            var lastRare = IcecrownRareConstants.Rares[Mod(spawnsIntoRotation, IcecrownRareConstants.Rares.Length)];
            var upcomingRare = IcecrownRareConstants.Rares[Mod(spawnsIntoRotation + 1, IcecrownRareConstants.Rares.Length)];
            var nextRare = IcecrownRareConstants.Rares[Mod(spawnsIntoRotation + 2, IcecrownRareConstants.Rares.Length)];

            var tabelString = string.Join(
                "",
                IcecrownRareConstants.Rares.Select((record, index) =>
                $@"<tr><td style=""text-align:right"">{GetTableMarker(index,spawnsIntoRotation)}</td><td><a href=""{record.WowHeadUrl}"" target=""_blank"">{record.Name}</a></td><td>{offsetBaseDate.AddMinutes(index * RareSpawnOffset)} UTC</td><td><button onclick=""copyMacroToClipboard({record.Xcoord},{record.Ycoord})"" > Copy Macro </button></td><td> <button onclick=""copySharableMacroToClipboard({record.Xcoord},{record.Ycoord},'{record.Name} in {(int)(offsetBaseDate.AddMinutes(index * RareSpawnOffset) - now).TotalMinutes}m')"">Copy Macro</button> </td></tr>")
                );


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
                  width: 90%;
                  border: 1px solid #ddd;
                  margin-left: auto;
                  margin-right: auto;
                }}

                th, td {{
                  text-align: left;
                  padding: 8px;
                }}

                tr:nth-child(even){{background-color: #f2f2f2}}
            </style>
                </head>
                <body>
                <a href=""{PrepatchInfoWowHeadUrl}"" target=""_blank""><h2>Click for Prepatch Wowhead Info</h2></a>
                <h2>Rare Schedule</h2>
                <div style=""overflow - x:auto; "">
                <table>
   
                <tr>
                    <th></th>
                    <th> Name </th>
                    <th> Spawn Time ( Local )</th>
                    <th> Countdown </th>
                    <th> Map Waypoint </th>
                    <th> Announce Waypoint to Chat </th>
                </tr>
         
                <tr>
                    <td> Last Rare </td>
                    <td> <a href=""{lastRare.WowHeadUrl}"" target=""_blank"">{lastRare.Name}</a> </td>
                    <td id = ""lastTime""> </td>
                    <td id = ""last""></td>
                    <td> <button onclick=""copyMacroToClipboard({lastRare.Xcoord},{lastRare.Ycoord})"">Copy Macro</button> </td>
                    <td> <button onclick=""copySharableMacroToClipboard({lastRare.Xcoord},{lastRare.Ycoord},'{lastRare.Name}')"">Copy Macro</button> </td>
                </tr>
         
                <tr>
                    <td> Upcoming Rare </td>
                    <td> <a href=""{upcomingRare.WowHeadUrl}"" target=""_blank"">{upcomingRare.Name}</a> </td>
                    <td id = ""upcomingTime""> </td>
                    <td id = ""upcoming""></td>
                    <td> <button onclick=""copyMacroToClipboard({upcomingRare.Xcoord},{upcomingRare.Ycoord})"">Copy Macro</button> </td>
                    <td> <button onclick=""copySharableMacroToClipboard({upcomingRare.Xcoord},{upcomingRare.Ycoord},'{upcomingRare.Name} in {(int)(upcomingStartTime - now).TotalMinutes}m')"">Copy Macro</button> </td>
                </tr>
         
                <tr>
                    <td> Next Rare </td>
                    <td> <a href=""{nextRare.WowHeadUrl}"" target=""_blank"">{nextRare.Name}</a> </td>
                    <td id = ""nextTime""> </td>
                    <td id = ""next""></td>
                    <td> <button onclick=""copyMacroToClipboard({nextRare.Xcoord},{nextRare.Ycoord})"">Copy Macro</button> </td>
                    <td> <button onclick=""copySharableMacroToClipboard({nextRare.Xcoord},{nextRare.Ycoord},'{nextRare.Name} in {(int)(nextStartTime - now).TotalMinutes}m')"">Copy Macro</button> </td>
                </tr>

                </table>
                </div>
                </br>
                </br>
                <h2>Rare Spawn List</h2>
                <div style=""overflow - x:auto; "">
                <table>
   
                <tr>
                    <th> </th>
                    <th> Name </th>
                    <th> Current Cycle Spawn Time </th>
                    <th> Map Waypoint </th>
                    <th> Announce Waypoint to Chat </th>
                </tr>
                {tabelString}
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
                }}
                function copySharableMacroToClipboard(x,y, name, minutes=null) {{
                  var macroText = `/run C_Map.SetUserWaypoint(UiMapPoint.CreateFromCoordinates(118,${{x}}/100,${{y}}/100));C_SuperTrack.SetSuperTrackedUserWaypoint(true);SendChatMessage(format(""%s %s${{minutes!=null?' in i%m':''}}"",C_Map.GetUserWaypointHyperlink(),""${{name}}""${{minutes!=null?','+minutes:''}}),""CHANNEL"",nil,1)`;
                  navigator.clipboard.writeText(macroText);
                }}
                function convertToLocalTime(dateTimeString) {{
                  return new Date(dateTimeString+"" UTC"").getTime();
                }}
            </script>
            </body>
            </html>";
        }
    }
}
