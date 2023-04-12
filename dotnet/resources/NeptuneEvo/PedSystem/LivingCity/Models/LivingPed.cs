using NeptuneEvo.Handles;
using Redage.SDK;

namespace NeptuneEvo.PedSystem.LivingCity.Models
{
    public class LivingPed
    {
        public ExtPed Ped;
        public ExtVehicle Vehicle;
        public ExtPlayer Controller;
        public bool IsSpawned = false;
        private string InitTimer = null;

        public LivingPed(ExtPed ped, ExtVehicle vehicle, ExtPlayer controller)
        {
            Ped = ped;
            Ped.SetSharedData("LCNPC", vehicle.Value);
            Vehicle = vehicle;
            Controller = controller;
            InitTimer = Timers.StartOnce(2500, () =>
            {
                IsSpawned = true;
                if (Ped == null || !Ped.Exists) return;
                Ped.Controller = Controller;
            }, true);
        }

        public void Destroy()
        {
            DestroyTimer();
            IsSpawned = false;
            DestroyPed();
            DestroyVehicle();
            Controller = null;
        }

        private void DestroyTimer()
        {
            if (InitTimer == null) return;
            Timers.Stop(InitTimer);
            InitTimer = null;
        }

        private void DestroyPed()
        {
            if (Ped == null) return;
            Ped.SetSharedData("LCNPC", 0);
            Ped.Delete();
            Ped = null;
        }
        private void DestroyVehicle()
        {
            if (Vehicle == null) return;
            Vehicle.Delete();
            Vehicle = null;
        }
    }
}
