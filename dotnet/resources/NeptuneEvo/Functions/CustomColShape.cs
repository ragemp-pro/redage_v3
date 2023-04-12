using GTANetworkAPI;
using NeptuneEvo.Handles;
using NeptuneEvo.Chars;
using Newtonsoft.Json;
using Redage.SDK;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NeptuneEvo.Character;
using NeptuneEvo.Players;
using NeptuneEvo.Players.Models;

namespace NeptuneEvo.Functions
{
    public enum ColShapeData
    {
        Error = -99
    }
    public enum ColShapeEnums
    {
        None = 0,
        GangZone,
        Warehouse,
        WarehouseEnter,
        WarehouseExit,
        Trucker,
        BusinessAction,
        Door,
        DriveSchool,
        DriveSchoolCoord,
        RentCar,
        Festive,
        SaluteShop,
        BlackMarket,
        SafeZone,
        Island,
        HouseSafe,
        Casino,
        Atm,
        Electrician,
        ElectricianPoint,
        Repair,
        SeatingArrangements,
        JobSelect,
        CrimeGang,
        CrimeMafia,
        CrimeBiker,
        GangEndDelivery,
        MafiaEndDelivery,
        PoliceDropDelivery,
        EnterAlcoShop,
        ExitAlcoShop,
        UnloadPoints,
        BuyPoints,
        War,

        DrugPoints,
        EnterPoints,
        ExitPoints,

        WarPoint,

        EnterGarage,
        ExitGarage,

        EnterHouse,
        ExitHouse,
        HealkitHouse,

        EnterHotel,
        ExitHotel,
        CarRentHotel,

        Organizations,

        FractionStock,
        FractionArmy,

        FractionCityhallBeginWorkDay,
        FractionCityhall,
        FractionCityhallGunMenu,
        FractionCityhallOld,

        FractionEms,
        FractionFbi,

        FractionPolic,
        FractionPolicArrest,

        FractionSheriff,
        FractionSheriffArrest,

        FractionLSNews,
        FractionGarageEnter,
        FractionGarageExit,

        FractionMerryweather,

        JobGoPosta,
        JobGoPostaCar,

        TakeMoney,

        GetProductTrucker,

        BusWays,

        MowerWays,

        ChangeAutoNumber,

        ElectionPoint,

        AdminMP,

        Racing,

        Parachute,

        EventsMenu,

        EmsQuests,
        EmsQuests2,

        QuestDoctor,
        QuestTracy,
        QuestGranny,

        QuestDada,
        QuestPavel,
        QuestZak,

        PedAirDrop,

        OresSell,
        GovMineStock,

        FracPolic,
        FracSheriff,
        FracLSNews,

        PremiumShop,

        ActionLabelShape,
        BoomboxShape,
        HookahShape,

        DonateAutoroom,

        CallGovMember,
        CallArmyMember,
        CallEmsMember,
        CallNewsMember,
        CallPoliceMember,
        CallSheriffMember,
        CallFibMember,

        LumberjackTree,
        HuntingShop,
        TreesSell,

        FracEms,
        FracEmsVeterinarian,

        Tent,
        SafeZoneTent,

        QuestWedding,
        OrgCreate,

        VoiceZone,

        PetShop,
        
        Rieltagency,
        
        QuestZdobich,
        FurnitureBuy,
        
        ImpoundLot,
        ImpoundLotPed,
        
        AirAutoRoom,
        AirSpawn,
        
        EliteAutoRoom,
        
        HeliCrash,
        
        Patrolling,
        
        FamilyZone,
        
        
        WarGangZone,
        
        QuestBonus,
    }
    class CustomColShape : Script
    {

        private static readonly nLog Log = new nLog("Functions.ColShape");

        private static ColShapeEnums[] ExceptionToNotifications = new ColShapeEnums[]
        {
            ColShapeEnums.EnterGarage
        };
        
        private static ColShapeEnums[] KeyClamp = new ColShapeEnums[]
        {
            ColShapeEnums.HeliCrash
        };
        
        public static void CreatCircleColShape(float x, float y, float range, uint dimension = uint.MaxValue, ColShapeEnums colShapeEnums = ColShapeEnums.None, int Index = (int)ColShapeData.Error)
        {
            var colShape = (ExtColShape) NAPI.ColShape.CreatCircleColShape(x, y, range, dimension);

            colShape.SetColShapeData(new ExtColShapeData(colShapeEnums, Index, (int)ColShapeData.Error));
        }
        public static void Create2DColShape(float x, float y, float width, float height, uint dimension = uint.MaxValue, ColShapeEnums colShapeEnums = ColShapeEnums.None, int Index = (int)ColShapeData.Error)
        {
            var colShape = (ExtColShape) NAPI.ColShape.Create2DColShape(x, y, width, height, dimension);

            colShape.SetColShapeData(new ExtColShapeData(colShapeEnums, Index, (int)ColShapeData.Error));
        }
        public static void Create3DColShape(Vector3 start, Vector3 end, uint dimension = uint.MaxValue, ColShapeEnums colShapeEnums = ColShapeEnums.None, int Index = (int)ColShapeData.Error)
        {
            var colShape = (ExtColShape) NAPI.ColShape.Create3DColShape(start, end, dimension);

            colShape.SetColShapeData(new ExtColShapeData(colShapeEnums, Index, (int)ColShapeData.Error));
        }
        public static ExtColShape CreateCylinderColShape(Vector3 position, float range, float height, uint dimension = uint.MaxValue, ColShapeEnums colShapeEnums = ColShapeEnums.None, int Index = (int)ColShapeData.Error, int ListId = (int)ColShapeData.Error)
        {
            var colShape = (ExtColShape) NAPI.ColShape.CreateCylinderColShape(position, range, height, dimension);

            colShape.SetColShapeData(new ExtColShapeData(colShapeEnums, Index, ListId));

            return colShape;
        }
        public static ExtColShape CreateSphereColShape(Vector3 position, float range, uint dimension = uint.MaxValue, ColShapeEnums colShapeEnums = ColShapeEnums.None, int Index = (int)ColShapeData.Error, int ListId = (int)ColShapeData.Error)
        {
            var colShape = (ExtColShape) NAPI.ColShape.CreateSphereColShape(position, range, dimension);

            colShape.SetColShapeData(new ExtColShapeData(colShapeEnums, Index, ListId));

            return colShape;
        }
        public static void DeleteColShape(ExtColShape shape)
        {
            if (shape == null)
                return;
            
            var colShape = shape.ColShapeData;

            foreach (var foreachPlayer in RAGE.Entities.Players.All.Cast<ExtPlayer>())
            {
                var foreachColShapesData = foreachPlayer.GetColShapesData();
                if (foreachColShapesData == null) 
                    continue;
                
                var sData = foreachColShapesData
                    .LastOrDefault(s => s.ColShapeId == colShape.ColShapeId && s.Index == colShape.Index);

                if (sData != null)
                {
                    if (InteractionCollection.isFunction(sData.ColShapeId.ToString()))
                        Trigger.ClientEvent(foreachPlayer, "hud.cEnter");
                    
                    foreachPlayer.DeleteColShapeData(sData);
                }
                
            }
            
            
            if (shape.Exists) 
                shape.Delete();
        }
        public static bool IsPointWithinColshape(ExtColShape shape, Vector3 point)
        {
            return NAPI.ColShape.IsPointWithinColshape(shape, point);
        }
        public static int GetDataToEnum(ExtPlayer player, ColShapeEnums colShapeEnums = ColShapeEnums.None)
        {
            var colShapesData = player.GetColShapesData();
            if (colShapesData == null) return (int)ColShapeData.Error;
            if (colShapesData.Count == 0) return (int)ColShapeData.Error;
            ExtColShapeData sData = null;

            if (colShapeEnums == ColShapeEnums.None) 
                sData = colShapesData.LastOrDefault();
            else 
                sData = colShapesData.FirstOrDefault(s => s.ColShapeId == colShapeEnums);
            
            if (sData == null) 
                return (int)ColShapeData.Error;
            return 
                sData.Index;
        }
        public static ExtColShapeData GetData(ExtPlayer player, ColShapeEnums colShapeEnums = ColShapeEnums.None)
        {
            var colShapesData = player.GetColShapesData();
            if (colShapesData == null) 
                return null;
            if (colShapesData.Count == 0) 
                return null;

            if (colShapeEnums == ColShapeEnums.None) 
                return colShapesData.LastOrDefault();
            
            return colShapesData.FirstOrDefault(s => s.ColShapeId == colShapeEnums);
        }

        [ServerEvent(Event.PlayerEnterColshape)]
        public void OnPlayerEnterColshape(ExtColShape shape, ExtPlayer player)
        {
            try
            {
                var sData = shape.GetColShapeData();
                if (sData == null) return;
                if (!player.IsCharacterData()) return;
                
                OnEnterColShape(player, sData.ColShapeId, sData.Index, sData.ListId);
            }
            catch (Exception e)
            {
                Log.Write($"OnPlayerEnterColshape Exception: {e.ToString()}");
            }
        }

        public static void SetColShapesData(ExtPlayer player, ColShapeEnums colShapeEnums,
            int index = (int) ColShapeData.Error, int listId = (int) ColShapeData.Error, bool isAddColShapeData = false)
        {
            if (!player.IsCharacterData()) 
                return;
            
            if (KeyClamp.Contains(colShapeEnums) && !isAddColShapeData) 
                return;
            
            var colShapesData = player.GetColShapesData();
            if (colShapesData != null)
            {
                var sData = colShapesData
                    .LastOrDefault(s => s.ColShapeId == colShapeEnums && s.Index == index);
                        
                if (sData != null) 
                    player.DeleteColShapeData(sData);
            }

            player.AddColShapeData(new ExtColShapeData(colShapeEnums, index, listId));
            //
            if (InteractionCollection.isFunction(colShapeEnums.ToString()) && !ExceptionToNotifications.Contains(colShapeEnums))
                Trigger.ClientEvent(player, "hud.oEnter", colShapeEnums.ToString());
        }

        private void OnEnterColShape(ExtPlayer player, ColShapeEnums colShapeEnums, int Index = (int)ColShapeData.Error, int ListId = (int)ColShapeData.Error)
        {
            try
            {
                if (!player.IsCharacterData()) return;

                var vehicle = (ExtVehicle) player.Vehicle;
                if (vehicle != null && vehicle.Model == (uint)VehicleHash.Rcbandito) return;

                //if (InteractionCollection.isFunction(colShapeEnums.ToString()))
                SetColShapesData(player, colShapeEnums, Index, ListId);

                var length = 1;
                if (Index != (int)ColShapeData.Error) length++;
                if (ListId != (int)ColShapeData.Error) length++;

                var parameters = new object[length];

                parameters[0] = player;
                if (Index != (int)ColShapeData.Error)
                {
                    parameters[1] = Index;
                    if (ListId != (int)ColShapeData.Error)
                        parameters[2] = ListId;
                }
                InteractionCollection.Call("In_" + colShapeEnums.ToString(), parameters);
            }
            catch (Exception e)
            {
                Log.Write($"OnEnterColShape Exception: {e.ToString()}");
            }
        }
        [ServerEvent(Event.PlayerExitColshape)]
        public void OnPlayerExitColShape(ExtColShape shape, ExtPlayer player)
        {
            try
            {
                var sData = shape.GetColShapeData();
                if (sData == null) return;
                if (!player.IsCharacterData()) return;
                OnExitColShape(player, sData.ColShapeId, sData.Index, sData.ListId);
            }
            catch (Exception e)
            {
                Log.Write($"OnPlayerExitColShape Exception: {e.ToString()}");
            }
        }
        private void OnExitColShape(ExtPlayer player, ColShapeEnums colShapeEnums, int Index = (int)ColShapeData.Error, int ListId = (int)ColShapeData.Error)
        {
            try
            {
                if (!player.IsCharacterData()) return;

                var colShapesData = player.GetColShapesData();
                
                if (colShapesData != null)
                {
                    var sData = colShapesData
                        .LastOrDefault(s => s.ColShapeId == colShapeEnums && s.Index == Index);
                    
                    if (sData != null)
                    {
                        player.DeleteColShapeData(sData);
                        
                        var cData = player.GetLastColShapeData();
                        if (cData != null && InteractionCollection.isFunction(cData.ColShapeId.ToString()) && !ExceptionToNotifications.Contains(cData.ColShapeId))
                        {
                            Trigger.ClientEvent(player, "hud.oEnter", cData.ColShapeId.ToString());
                        }
                        else if (InteractionCollection.isFunction(colShapeEnums.ToString()))
                        {
                            //Скрываем кнопку
                            Trigger.ClientEvent(player, "hud.cEnter");
                        }
                    }
                }
                int length = 1;
                if (Index != (int)ColShapeData.Error) length++;
                if (ListId != (int)ColShapeData.Error) length++;

                object[] parameters = new object[length];

                parameters[0] = player;
                if (Index != (int)ColShapeData.Error)
                {
                    parameters[1] = Index;
                    if (ListId != (int)ColShapeData.Error)
                        parameters[2] = ListId;
                }
                InteractionCollection.Call("Out_" + colShapeEnums.ToString(), parameters);

            }
            catch (Exception e)
            {
                Log.Write($"OnExitColShape Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("server.useEvent")]
        public void rEvent(ExtPlayer player)
        {
            var cData = player.GetLastColShapeData();
            if (cData == null) return;

            var vehicle = (ExtVehicle) player.Vehicle;
            if (vehicle != null && vehicle.Model == (uint)VehicleHash.Rcbandito) return;


            int length = 1;
            if (cData.Index != (int)ColShapeData.Error) length++;
            if (cData.ListId != (int)ColShapeData.Error) length++;

            object[] parameters = new object[length];

            parameters[0] = player;
            if (cData.Index != (int)ColShapeData.Error)
            {
                parameters[1] = cData.Index;
                if (cData.ListId != (int)ColShapeData.Error)
                    parameters[2] = cData.ListId;
            }
            InteractionCollection.Call(cData.ColShapeId.ToString(), parameters);
        }

    }

    [AttributeUsage(AttributeTargets.Method)]
    public class InteractionAttribute : Attribute
    {
        private readonly string name;
        public string Name
        {
            get { return name; }
        }

        public InteractionAttribute(ColShapeEnums name, bool In = false, bool Out = false)
        {
            if (In) this.name = "In_" + name.ToString();
            else if (Out) this.name = "Out_" + name.ToString();
            else this.name = name.ToString();
        }
    }
    class InteractionCollection : Script
    {
        class InteractionAttributeData
        {
            public MethodInfo Method { get; }
            public Delegate FastInvokeHandler { get; }
            public object Instance { get; internal set; }
            public InteractionAttributeData(MethodInfo method, Delegate fastInvokeHandler)
            {
                Method = method;
                FastInvokeHandler = fastInvokeHandler;
            }
        }

        private static readonly nLog Log = new nLog("Functions.InteractionCollection");

        private static ConcurrentDictionary<string, InteractionAttributeData> InteractionFunction = new ConcurrentDictionary<string, InteractionAttributeData>();

        private bool AddInstanceIfRequired(MethodInfo method, InteractionAttributeData methodData)
        {
            if (method.IsStatic) return true;

            var instance = GetMethodInstance(method);
            if (instance is null)
            {
                Console.WriteLine($"Method {method.Name} in class {method.DeclaringType!.FullName} can not be added because " +
                    $"it's neither static nor an object of the class can be created (e.g. because of missing parameterless constructor or being a static/abstract class.");
                return false;
            }
            methodData.Instance = instance;
            return true;
        }
        private readonly Dictionary<Type, object> _instancesPerClass = new Dictionary<Type, object>();
        private object GetMethodInstance(MethodInfo method)
        {
            var classType = method.DeclaringType!;
            if (_instancesPerClass.TryGetValue(classType, out var instance))
                return instance;

            instance = Activator.CreateInstance(classType);
            if (instance is null) return null;
            _instancesPerClass[classType] = instance;
            return instance;
        }

        public InteractionCollection()
        {
            /*var methods = AppDomain.CurrentDomain.GetAssemblies() // Returns all currenlty loaded assemblies
                .SelectMany(x => x.GetTypes()) // returns all types defined in this assemblies
                .Where(x => x.IsClass) // only yields classes
                .SelectMany(x => x.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)) // returns all methods defined in those classes
                .Where(x => x.GetCustomAttributes(typeof(InteractionAttribute), false).FirstOrDefault() != null); // returns only methods that have the InvokeAttribute
            */
            var _fastMethodInvoker = new FastMethodInvoker();
            var assembly = Assembly.GetExecutingAssembly();
            var methods = assembly.GetTypes().SelectMany(type => type
                .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)
               .Where(m => m.GetCustomAttribute<InteractionAttribute>(false) != null));


            foreach (MethodInfo method in methods) // iterate through all found methods
            {
                var cmdAttribute = method.GetCustomAttribute<InteractionAttribute>()!;
                if (!InteractionFunction.ContainsKey(cmdAttribute.Name))
                {
                    var fastInvokeHandler = _fastMethodInvoker.GetMethodInvoker(method);
                    var methodData = new InteractionAttributeData(method, fastInvokeHandler);
                    if (AddInstanceIfRequired(method, methodData))
                        InteractionFunction.TryAdd(cmdAttribute.Name, methodData); // Instantiate the class
                }
            }
        }
        public static void Call(string name, params object[] parameters)
        {
            try
            {
                if (!InteractionFunction.ContainsKey(name)) return;
                InteractionAttributeData methodData = InteractionFunction[name];

                if (methodData.FastInvokeHandler is FastInvokeHandler nonStaticHandler)
                    nonStaticHandler.Invoke(methodData.Instance, parameters);
                else if (methodData.FastInvokeHandler is FastInvokeHandlerStatic staticHandler)
                    staticHandler.Invoke(parameters);

            }
            catch (Exception e)
            {
                Log.Write($"Call Exception: {e.ToString()}");
            }
        }

        public static bool isFunction(string name)
        {
            return InteractionFunction.ContainsKey(name);
        }
    }
}
