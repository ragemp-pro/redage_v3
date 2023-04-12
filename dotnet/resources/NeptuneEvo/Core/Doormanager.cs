using System;
using System.Collections.Generic;
using GTANetworkAPI;
using NeptuneEvo.Handles;
using NeptuneEvo.Accounts;
using NeptuneEvo.Players.Models;
using NeptuneEvo.Players;
using NeptuneEvo.Character.Models;
using NeptuneEvo.Character;
using NeptuneEvo.Functions;
using Redage.SDK;

namespace NeptuneEvo.Core
{
    class Doormanager : Script
    {
        private static readonly nLog Log = new nLog("Core.Doormanager");

        [ServerEvent(Event.ResourceStart)]
        public void onResourceStart()
        {
            try
            {
                /*
                // Banks
                {id: 0,  name: 'Pacific Standard Bank Main Doors',                     hash: 110411286,    locked: true, position: new mp.Vector3(232.6054, 214.1584, 106.4049)},   // Right
                {id: 1,  name: 'Pacific Standard Bank Main Doors',                     hash: 110411286,    locked: true, position: new mp.Vector3(231.5123, 216.5177, 106.4049)},   // Left
                {id: 2,  name: 'Pacific Standard Bank Main Doors',                     hash: 110411286,    locked: true, position: new mp.Vector3(260.6432, 203.2052, 106.4049)},   // Right
                {id: 3,  name: 'Pacific Standard Bank Main Doors',                     hash: 110411286,    locked: true, position: new mp.Vector3(258.2022, 204.1005, 106.4049)},   // Left
                {id: 4,  name: 'Pacific Standard Bank Door To Upstair',                hash: 1956494919,   locked: true, position: new mp.Vector3(237.7704, 227.87, 106.426)}, 
                {id: 5,  name: 'Pacific Standard Bank Upstair Door',                   hash: 1956494919,   locked: true, position: new mp.Vector3(236.5488, 228.3147, 110.4328)}, 
                {id: 6,  name: 'Pacific Standard Bank Back To Hall Doors',             hash: 110411286,    locked: true, position: new mp.Vector3(259.9831, 215.2468, 106.4049)},   // Right
                {id: 7,  name: 'Pacific Standard Bank Back To Hall Doors',             hash: 110411286,    locked: true, position: new mp.Vector3(259.0879, 212.8062, 106.4049)},   // Left
                {id: 8,  name: 'Pacific Standard Bank Upstair Door To Offices',        hash: 1956494919,   locked: true, position: new mp.Vector3(256.6172, 206.1522, 110.4328)}, 
                {id: 9,  name: 'Pacific Standard Bank Big Office Door',                hash: 964838196,    locked: true, position: new mp.Vector3(260.8579, 210.4453, 110.4328)}, 
                {id: 10, name: 'Pacific Standard Bank Small Office Door',              hash: 964838196,    locked: true, position: new mp.Vector3(262.5366, 215.0576, 110.4328)}, 
   
                // Shops      
                {id: 100, name: 'Discount Store South Enter Door',                     hash: -1148826190,  locked: true, position: new mp.Vector3(82.38156, -1390.476, 29.52609)},   // Right
                {id: 101, name: 'Discount Store South Enter Door',                     hash: 868499217,    locked: true, position: new mp.Vector3(82.38156, -1390.752, 29.52609)},   // Left
                {id: 110, name: 'Los Santos Customs Popular Street Door',              hash: 270330101,    locked: true, position: new mp.Vector3(723.116, -1088.831, 23.23201)},  
                {id: 111, name: 'Los Santos Customs Carcer Way Door',                  hash: -550347177,   locked: true, position: new mp.Vector3(-356.0905, -134.7714, 40.01295)},  
                {id: 111, name: 'Los Santos Customs Greenwich Parkway Door',           hash: -550347177,   locked: true, position: new mp.Vector3(-1145.898, -1991.144, 14.18357)},  
                {id: 113, name: 'Los Santos Customs Route 68 Doors',                   hash: -822900180,   locked: true, position: new mp.Vector3(1174.656, 2644.159, 40.50673)},    // Right
                {id: 114, name: 'Los Santos Customs Route 68 Doors',                   hash: -822900180,   locked: true, position: new mp.Vector3(1182.307, 2644.166, 40.50784)},    // Left
                {id: 115, name: 'Los Santos Customs Route 68 Office Door',             hash: 1335311341,   locked: true, position: new mp.Vector3(1187.202, 2644.95, 38.55176)},  
                {id: 116, name: 'Los Santos Customs Route 68 Office Door',             hash: 1544229216,   locked: true, position: new mp.Vector3(1182.646, 2641.182, 39.31031)},  
                {id: 117, name: 'Beekers Garage Paleto Bay Doors',                     hash: -822900180,   locked: true, position: new mp.Vector3(114.3135, 6623.233, 32.67305)},    // Right
                {id: 118, name: 'Beekers Garage Paleto Bay Doors',                     hash: -822900180,   locked: true, position: new mp.Vector3(108.8502, 6617.877, 32.67305)},    // Left
                {id: 119, name: 'Beekers Garage Paleto Bay Office Door',               hash: 1335311341,   locked: true, position: new mp.Vector3(105.1518, 6614.655, 32.58521)},  
                {id: 120, name: 'Beekers Garage Paleto Bay Interior Door',             hash: 1544229216,   locked: true, position: new mp.Vector3(105.7772, 6620.532, 33.34266)},  
                {id: 121, name: 'Ammu Nation Vespucci Boulevard Doors',                hash: -8873588,     locked: true, position: new mp.Vector3(842.7685, -1024.539, 28.34478)},   // Right
                {id: 122, name: 'Ammu Nation Vespucci Boulevard Doors',                hash: 97297972,     locked: true, position: new mp.Vector3(845.3694, -1024.539, 28.34478)},   // Left
                {id: 123, name: 'Ammu Nation Lindsay Circus Doors',                    hash: -8873588,     locked: true, position: new mp.Vector3(-662.6415, -944.3256, 21.97915)},  // Right
                {id: 124, name: 'Ammu Nation Lindsay Circus Doors',                    hash: 97297972,     locked: true, position: new mp.Vector3(-665.2424, -944.3256, 21.97915)},  // Left
                {id: 125, name: 'Ammu Nation Popular Street Doors',                    hash: -8873588,     locked: true, position: new mp.Vector3(810.5769, -2148.27, 29.76892)},    // Right
                {id: 126, name: 'Ammu Nation Popular Street Doors',                    hash: 97297972,     locked: true, position: new mp.Vector3(813.1779, -2148.27, 29.76892)},     // Left
                {id: 128, name: 'Ammu Nation Popular Street Doors',                    hash: -8873588,     locked: true, position: new mp.Vector3(18.572, -1115.495, 29.94694)},    // Right
                {id: 129, name: 'Ammu Nation Popular Street Doors',                    hash: 97297972,     locked: true, position: new mp.Vector3(16.12787, -1114.606, 29.94694)},     // Left
                {id: 130, name: 'Ammu Nation Adams Apple Boulevard',                   hash: 452874391,    locked: true, position: new mp.Vector3(6.81789, -1098.209, 29.94685)},  
                {id: 131, name: 'Ammu Nation Vinewood Plaza Doors',                    hash: -8873588,     locked: true, position: new mp.Vector3(243.8379, -46.52324, 70.09098)},   // Right
                {id: 132, name: 'Ammu Nation Vinewood Plaza Doors',                    hash: 97297972,     locked: true, position: new mp.Vector3(244.7275, -44.07911, 70.09098)},   // Left
                {id: 150, name: 'Ponsonbys Portola Drive Door',                        hash: -1922281023,  locked: true, position: new mp.Vector3(-715.6154, -157.2561, 37.67493)},  // Right
                {id: 151, name: 'Ponsonbys Portola Drive Door',                        hash: -1922281023,  locked: true, position: new mp.Vector3(-716.6755, -155.42, 37.67493)},    // Left
                {id: 152, name: 'Ponsonbys Portola Drive Door',                        hash: -1922281023,  locked: true, position: new mp.Vector3(-1456.201, -233.3682, 50.05648)},  // Right
                {id: 153, name: 'Ponsonbys Portola Drive Door',                        hash: -1922281023,  locked: true, position: new mp.Vector3(-1454.782, -231.7927, 50.05649)},  // Left
                {id: 154, name: 'Ponsonbys Rockford Plaza Door',                       hash: -1922281023,  locked: true, position: new mp.Vector3(-156.439, -304.4294, 39.99308)},   // Right
                {id: 155, name: 'Ponsonbys Rockford Plaza Door',                       hash: -1922281023,  locked: true, position: new mp.Vector3(-157.1293, -306.4341, 39.99308)},  // Left
                {id: 156, name: 'Sub Urban Prosperity Street Promenade Door',          hash: 1780022985,   locked: true, position: new mp.Vector3(-1201.435, -776.8566, 17.99184)},  
                {id: 157, name: 'Sub Urban Hawick Avenue Door',                        hash: 1780022985,   locked: true, position: new mp.Vector3(127.8201, -211.8274, 55.22751)},  
                {id: 158, name: 'Sub Urban Route 68 Door',                             hash: 1780022985,   locked: true, position: new mp.Vector3(617.2458, 2751.022, 42.75777)},  
                {id: 159, name: 'Sub Urban Chumash Plaza Door',                        hash: 1780022985,   locked: true, position: new mp.Vector3(-3167.75, 1055.536, 21.53288)},  
                {id: 160, name: 'Robs Liquor Route 1 Main Enter Door',                 hash: -1212951353,  locked: true, position: new mp.Vector3(-2973.535, 390.1414, 15.18735)},  
                {id: 161, name: 'Robs Liquor Route 1 Personnal Door',                  hash: 1173348778,   locked: true, position: new mp.Vector3(-2965.648, 386.7928, 15.18735)},  
                {id: 162, name: 'Robs Liquor Route 1 Back Door',                       hash: 1173348778,   locked: true, position: new mp.Vector3(-2961.749, 390.2573, 15.19322)},  
                {id: 163, name: 'Robs Liquor Prosperity Street Main Enter Door',       hash: -1212951353,  locked: true, position: new mp.Vector3(-1490.411, -383.8453, 40.30745)},  
                {id: 164, name: 'Robs Liquor Prosperity Street Personnal Door',        hash: 1173348778,   locked: true, position: new mp.Vector3(-1482.679, -380.153, 40.30745)},  
                {id: 165, name: 'Robs Liquor Prosperity Street Back Door',             hash: 1173348778,   locked: true, position: new mp.Vector3(-1482.693, -374.9365, 40.31332)},  
                {id: 166, name: 'Robs Liquor San Andreas Avenue Main Enter Door',      hash: -1212951353,  locked: true, position: new mp.Vector3(-1226.894, -903.1218, 12.47039)},  
                {id: 167, name: 'Robs Liquor San Andreas Avenue Personnal Door',       hash: 1173348778,   locked: true, position: new mp.Vector3(-1224.755, -911.4182, 12.47039)},  
                {id: 168, name: 'Robs Liquor San Andreas Avenue Back Door',            hash: 1173348778,   locked: true, position: new mp.Vector3(-1219.633, -912.406, 12.47626)},  
                {id: 169, name: 'Robs Liquor El Rancho Boulevard Main Enter Door',     hash: -1212951353,  locked: true, position: new mp.Vector3(1141.038, -980.3225, 46.55986)},  
                {id: 170, name: 'Robs Liquor El Rancho Boulevard Personnal Door',      hash: 1173348778,   locked: true, position: new mp.Vector3(1132.645, -978.6059, 46.55986)},  
                {id: 171, name: 'Robs Liquor El Rancho Boulevard Back Door',           hash: 1173348778,   locked: true, position: new mp.Vector3(129.51, -982.7756, 46.56573)},  
                {id: 180, name: 'Bob Mulét Barber Shop Door',                          hash: 145369505,    locked: true, position: new mp.Vector3(-822.4442, -188.3924, 37.81895)},  // Right
                {id: 181, name: 'Bob Mulét Barber Shop Door',                          hash: -1663512092,  locked: true, position: new mp.Vector3(-823.2001, -187.0831, 37.81895)},  // Left
                {id: 182, name: 'Hair on Hawick Barber Shop Door',                     hash: -1844444717,  locked: true, position: new mp.Vector3(-29.86917, -148.1571, 57.22648)},  
                {id: 183, name: 'OSheas Barber Shop Door',                             hash: -1844444717,  locked: true, position: new mp.Vector3(1932.952, 3725.154, 32.9944)},    
                {id: 190, name: 'Premium Deluxe Motorsport Parking Doors',             hash: 1417577297,   locked: true, position: new mp.Vector3(-37.33113, -1108.873, 26.7198)},   // Right
                {id: 191, name: 'Premium Deluxe Motorsport Parking Doors',             hash: 2059227086,   locked: true, position: new mp.Vector3(-39.13366, -1108.218, 26.7198)},   // Left
                {id: 192, name: 'Premium Deluxe Motorsport Main Doors',                hash: 1417577297,   locked: true, position: new mp.Vector3(-60.54582, -1094.749, 26.88872)},  // Right
                {id: 193, name: 'Premium Deluxe Motorsport Main Doors',                hash: 2059227086,   locked: true, position: new mp.Vector3(-59.89302, -1092.952, 26.88362)},  // Left
                {id: 194, name: 'Premium Deluxe Motorsport Right Office Door',         hash: -2051651622,  locked: true, position: new mp.Vector3(-33.80989, -1107.579, 26.57225)},    
                {id: 195, name: 'Premium Deluxe Motorsport Left Office Door',          hash: -2051651622,  locked: true, position: new mp.Vector3(-31.72353, -1101.847, 26.57225)},    
   
                // Houses   
                {id: 300, name: 'Franklin House Enter Door',                           hash: 520341586,    locked: true, position: new mp.Vector3(-14.86892, -1441.182, 31.19323)},    
                {id: 301, name: 'Franklin House Garage Door',                          hash: 703855057,    locked: true, position: new mp.Vector3(-25.2784, -1431.061, 30.83955)},   
       
                // Police   
                {id: 1000, name: 'Mission Row Police Station Main Enter Doors',        hash: 320433149,    locked: true, position: new mp.Vector3(434.7479, -983.2151, 30.83926)},  // Right
                {id: 1001, name: 'Mission Row Police Station Main Enter Doors',        hash: -1215222675,  locked: true, position: new mp.Vector3(434.7479, -980.6184, 30.83926)},  // Left
                {id: 1002, name: 'Mission Row Police Station Back Enter Doors',        hash: -2023754432,  locked: true, position: new mp.Vector3(469.9679, -1014.452, 26.53623)},  // Right
                {id: 1003, name: 'Mission Row Police Station Back Enter Doors',        hash: -2023754432,  locked: true, position: new mp.Vector3(467.3716, -1014.452, 26.53623)},  // Left

                {id: 1004, name: 'Mission Row Police Station Back To Cells Door',      hash: -1033001619,  locked: true, position: new mp.Vector3(463.4782, -1003.538, 25.00599)}, 
                {id: 1005, name: 'Mission Row Police Station Cell Door 1',             hash: 631614199,    locked: true, position: new mp.Vector3(461.8065, -994.4086, 25.06443)}, 
                {id: 1006, name: 'Mission Row Police Station Cell Door 2',             hash: 631614199,    locked: true, position: new mp.Vector3(461.8065, -997.6583, 25.06443)}, 
                {id: 1007, name: 'Mission Row Police Station Cell Door 3',             hash: 631614199,    locked: true, position: new mp.Vector3(461.8065, -1001.302, 25.06443)}, 
                {id: 1008, name: 'Mission Row Police Station Door To Cells Door',      hash: 631614199,    locked: true, position: new mp.Vector3(464.5701, -992.6641, 25.06443)}, 
                {id: 1009, name: 'Mission Row Police Station Captans Office Door',     hash: -1320876379,  locked: true, position: new mp.Vector3(446.5728, -980.0106, 30.8393)}, 
                {id: 1010, name: 'Mission Row Police Station Armory Double Door',      hash: 185711165,    locked: true, position: new mp.Vector3(450.1041, -984.0915, 30.8393)},  // Right
                {id: 1011, name: 'Mission Row Police Station Armory Double Door',      hash: 185711165,    locked: true, position: new mp.Vector3(450.1041, -981.4915, 30.8393)},  // Left

                {id: 1012, name: 'Mission Row Police Station Armory Secure Door',      hash: 749848321,    locked: true, position: new mp.Vector3(453.0793, -983.1895, 30.83926)}, 
                {id: 1013, name: 'Mission Row Police Station Locker Rooms Door',       hash: 1557126584,   locked: true, position: new mp.Vector3(450.1041, -985.7384, 30.8393)}, 
                {id: 1014, name: 'Mission Row Police Station Locker Room 1 Door',      hash: -2023754432,  locked: true, position: new mp.Vector3(452.6248, -987.3626, 30.8393)}, 
                {id: 1015, name: 'Mission Row Police Station Roof Access Door',        hash: 749848321,    locked: true, position: new mp.Vector3(461.2865, -985.3206, 30.83926)}, 
                {id: 1016, name: 'Mission Row Police Station Roof Door',               hash: -340230128,   locked: true, position: new mp.Vector3(464.3613, -984.678, 43.83443)}, 
                {id: 1017, name: 'Mission Row Police Station Cell And Briefing Doors', hash: 185711165,    locked: true, position: new mp.Vector3(443.4078, -989.4454, 30.8393)},  // Right
                {id: 1018, name: 'Mission Row Police Station Cell And Briefing Doors', hash: 185711165,    locked: true, position: new mp.Vector3(446.0079, -989.4454, 30.8393)},  // Left
                {id: 1019, name: 'Mission Row Police Station Briefing Doors',          hash: -131296141,   locked: true, position: new mp.Vector3(443.0298, -991.941, 30.8393)},   // Right
                {id: 1020, name: 'Mission Row Police Station Briefing Doors',          hash: -131296141,   locked: true, position: new mp.Vector3(443.0298, -994.5412, 30.8393)},  // Left
                {id: 1021, name: 'Mission Row Police Station Back Gate Door',          hash: -1603817716,  locked: true, position: new mp.Vector3(489.301, -1020.029, 28.078)},  
    
                // Others
                {id: 500, name: 'Vanilla Unicorn Main Enter Door',                     hash: -1116041313,  locked: true, position: new mp.Vector3(127.9552, -1298.503, 29.41962)},  
                {id: 501, name: 'Vanilla Unicorn Back Enter Door',                     hash: 668467214,    locked: true, position: new mp.Vector3(96.09197, -1284.854, 29.43878)},  
                {id: 502, name: 'Vanilla Unicorn Office Door',                         hash: -626684119,   locked: true, position: new mp.Vector3(99.08321, -1293.701, 29.41868)},  
                {id: 503, name: 'Vanilla Unicorn Dress Door',                          hash: -495720969,   locked: true, position: new mp.Vector3(113.9822, -1297.43, 29.41868)},  
                {id: 504, name: 'Vanilla Unicorn Private Rooms Door',                  hash: -1881825907,  locked: true, position: new mp.Vector3(116.0046, -1294.692, 29.41947)},  
                {id: 510, name: 'Bolingbroke Penitentiary Main Enter First Door',      hash:  741314661,   locked: true, position: new mp.Vector3(1844.72, 2608.49, 46.0)},  
                {id: 511, name: 'Bolingbroke Penitentiary Main Enter Second Door',     hash:  741314661,   locked: true, position: new mp.Vector3(1818.252, 2608.384, 46.0)},  
                {id: 512, name: 'Bolingbroke Penitentiary Main Enter Third Door',      hash:  741314661,   locked: true, position: new mp.Vector3(1795.98, 2616.696, 45.565)},  

                // Added by me
                {id: -1, name: 'Pacific Standard Bank Main Safe',                      hash: 961976194,    locked: true, position: new mp.Vector3(254.230, 224.55, 101.87)},
                */
                //X:266,3624 Y:217,5697 Z:110,4328
                RegisterDoor(-1246222793, new Vector3(0, 0, 0)); // pacific standart staff door
                SetDoorLocked(0, true, 0);

                RegisterDoor(1956494919, new Vector3(0, 0, 0)); // pacific standart staff door
                SetDoorLocked(1, true, 0);

                RegisterDoor(961976194, new Vector3(255.2283, 223.976, 102.3932)); // safe door 
                SetDoorLocked(2, true, 0);

                RegisterDoor(110411286, new Vector3(232.6054, 214.1584, 106.4049)); // pacific standart main door 1
                SetDoorLocked(3, true, 0);

                RegisterDoor(110411286, new Vector3(231.5123, 216.5177, 106.4049)); // pacific standart main door 2
                SetDoorLocked(4, true, 0);

                RegisterDoor(631614199, new Vector3(461.8065, -997.6583, 25.06443)); // police prison door
                SetDoorLocked(5, true, 0);

                RegisterDoor(1335309163, new Vector3(260.6518, 203.2292, 106.4328)); // pacific exit door
                SetDoorLocked(6, true, 0);

                RegisterDoor(1335309163, new Vector3(258.2093, 204.119, 106.4328)); // pacific exit door 2
                SetDoorLocked(7, true, 0);

                if (Main.ServerSettings.IsDeleteProp)
                {
                    NAPI.World.DeleteWorldProp(-1148826190, new Vector3(82.38156, -1390.476, 29.52609), 30f);
                    NAPI.World.DeleteWorldProp(868499217, new Vector3(82.38156, -1390.752, 29.52609), 30f);
                    NAPI.World.DeleteWorldProp(-8873588, new Vector3(842.7685, -1024.539, 28.34478), 30f);
                    NAPI.World.DeleteWorldProp(97297972, new Vector3(845.3694, -1024.539, 28.34478), 30f);
                    NAPI.World.DeleteWorldProp(-8873588, new Vector3(-662.6415, -944.3256, 21.97915), 30f);
                    NAPI.World.DeleteWorldProp(97297972, new Vector3(-665.2424, -944.3256, 21.97915), 30f);
                    NAPI.World.DeleteWorldProp(-8873588, new Vector3(810.5769, -2148.27, 29.76892), 30f);
                    NAPI.World.DeleteWorldProp(97297972, new Vector3(813.1779, -2148.27, 29.76892), 30f);
                    NAPI.World.DeleteWorldProp(-8873588, new Vector3(18.572, -1115.495, 29.94694), 30f);
                    NAPI.World.DeleteWorldProp(97297972, new Vector3(16.12787, -1114.606, 29.94694), 30f);
                    NAPI.World.DeleteWorldProp(-8873588, new Vector3(243.8379, -46.52324, 70.09098), 30f);
                    NAPI.World.DeleteWorldProp(97297972, new Vector3(244.7275, -44.07911, 70.09098), 30f);
                    NAPI.World.DeleteWorldProp(-1922281023, new Vector3(-715.6154, -157.2561, 37.67493), 30f);
                    NAPI.World.DeleteWorldProp(-1922281023, new Vector3(-716.6755, -155.42, 37.67493), 30f);
                    NAPI.World.DeleteWorldProp(-1922281023, new Vector3(-1456.201, -233.3682, 50.05648), 30f);
                    NAPI.World.DeleteWorldProp(-1922281023, new Vector3(-1454.782, -231.7927, 50.05649), 30f);
                    NAPI.World.DeleteWorldProp(-1922281023, new Vector3(-156.439, -304.4294, 39.99308), 30f);
                    NAPI.World.DeleteWorldProp(-1922281023, new Vector3(-157.1293, -306.4341, 39.99308), 30f);
                    NAPI.World.DeleteWorldProp(1780022985, new Vector3(-1201.435, -776.8566, 17.99184), 30f);
                    NAPI.World.DeleteWorldProp(1780022985, new Vector3(127.8201, -211.8274, 55.22751), 30f);
                    NAPI.World.DeleteWorldProp(1780022985, new Vector3(617.2458, 2751.022, 42.75777), 30f);
                    NAPI.World.DeleteWorldProp(1780022985, new Vector3(-3167.75, 1055.536, 21.53288), 30f);
                    NAPI.World.DeleteWorldProp(145369505, new Vector3(-822.4442, -188.3924, 37.81895), 30f);
                    NAPI.World.DeleteWorldProp(-1663512092, new Vector3(-823.2001, -187.0831, 37.81895), 30f);
                    NAPI.World.DeleteWorldProp(-1844444717, new Vector3(-29.86917, -148.1571, 57.22648), 30f);
                    NAPI.World.DeleteWorldProp(-1844444717, new Vector3(1932.952, 3725.154, 32.9944), 30f);
                    NAPI.World.DeleteWorldProp(1417577297, new Vector3(-59.89302, -1092.952, 26.88362), 30f);
                    NAPI.World.DeleteWorldProp(2059227086, new Vector3(-39.13366, -1108.218, 26.7198), 30f);
                    NAPI.World.DeleteWorldProp(1417577297, new Vector3(-60.54582, -1094.749, 26.88872), 30f);
                    NAPI.World.DeleteWorldProp(2059227086, new Vector3(-59.89302, -1092.952, 26.88362), 30f);
                    NAPI.World.DeleteWorldProp(1765048490, new Vector3(1855.685, 3683.93, 34.59282), 30f);
                    NAPI.World.DeleteWorldProp(543652229, new Vector3(321.8085, 178.3599, 103.6782), 30f);
                    NAPI.World.DeleteWorldProp(868499217, new Vector3(-818.7643, -1079.545, 11.47806), 30f);
                    NAPI.World.DeleteWorldProp(3146141106, new Vector3(-816.7932, -1078.406, 11.47806), 30f);
                    NAPI.World.DeleteWorldProp(543652229, new Vector3(-1155.454, -1424.008, 5.046147), 30f);
                    NAPI.World.DeleteWorldProp(543652229, new Vector3(1321.286, -1650.597, 52.36629), 30f);

                    NAPI.World.DeleteWorldProp(NAPI.Util.GetHashKey("v_ilev_247door_r"),
                        new Vector3(1732.362, 6410.917, 35.18717), 30f);
                    NAPI.World.DeleteWorldProp(NAPI.Util.GetHashKey("v_ilev_247door"),
                        new Vector3(1730.032, 6412.072, 35.18717), 30f);

                    NAPI.World.DeleteWorldProp(NAPI.Util.GetHashKey("v_ilev_gasdoor_r"),
                        new Vector3(1698.172, 4928.146, 42.21359), 30f);
                    NAPI.World.DeleteWorldProp(NAPI.Util.GetHashKey("v_ilev_gasdoor"),
                        new Vector3(1699.661, 4930.278, 42.21359), 30f);


                    NAPI.World.DeleteWorldProp(NAPI.Util.GetHashKey("v_ilev_gasdoor"),
                        new Vector3(1699.661, 4930.278, 42.21359), 30f);
                }
                // X:-59,89302 Y:-1092,952 Z:26,88362
            }
            catch (Exception e)
            {
                Log.Write($"onResourceStart Exception: {e.ToString()}");
            }
        }

        private static List<Door> allDoors = new List<Door>();
        public static int RegisterDoor(int model, Vector3 Position)
        {
            try
            {
                allDoors.Add(new Door(model, Position));
                CustomColShape.CreateCylinderColShape(Position, 5, 5, 0, ColShapeEnums.Door, allDoors.Count - 1);
                return allDoors.Count - 1;
            }
            catch (Exception e)
            {
                Log.Write($"RegisterDoor Exception: {e.ToString()}");
                return allDoors.Count - 1;
            }
        }
        [Interaction(ColShapeEnums.Door, In: true)]
        public static void InDoor(ExtPlayer player, int index)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                Door door = allDoors[index];
                Trigger.ClientEvent(player, "setDoorLocked", door.Model, door.Position.X, door.Position.Y, door.Position.Z, door.Locked, door.Angle);
            }
            catch (Exception e)
            {
                Log.Write($"InDoor Exception: {e.ToString()}");
            }
        }

        public static void SetDoorLocked(int id, bool locked, float angle)
        {
            try
            {
                if (allDoors.Count < id + 1) return;
                allDoors[id].Locked = locked;
                allDoors[id].Angle = angle;
                Trigger.ClientEventForAll("setDoorLocked", allDoors[id].Model, allDoors[id].Position.X, allDoors[id].Position.Y, allDoors[id].Position.Z, allDoors[id].Locked, allDoors[id].Angle);
            }
            catch (Exception e)
            {
                Log.Write($"SetDoorLocked Exception: {e.ToString()}");
            }
        }

        public static bool GetDoorLocked(int id)
        {
            if (allDoors.Count < id + 1) return false;
            return allDoors[id].Locked;
        }

        internal class Door
        {
            public Door(int model, Vector3 position)
            {
                Model = model;
                Position = position;
                Locked = false;
                Angle = 50.0f;
            }
            
            public int Model { get; set; }
            public Vector3 Position { get; set; }
            public bool Locked { get; set; }
            public float Angle { get; set; }
        }
    }
}
