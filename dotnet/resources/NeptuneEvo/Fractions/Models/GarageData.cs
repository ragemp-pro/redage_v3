using GTANetworkAPI;
using NeptuneEvo.Handles;

namespace NeptuneEvo.Fractions.Models
{
    public class GarageData
    {
        /// <summary>
        /// 
        /// </summary>
        public int FractionId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Vector3 PlayerEnterPos { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Vector3 PlayerEnterRot { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Vector3 PlayerExitPos { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Vector3 PlayerExitRot { get; set; }
        
        
        
        
        /// <summary>
        /// 
        /// </summary>
        public Vector3 VehEnterPosPoint { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Vector3 VehEnterPos { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Vector3 VehEnterRot { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Vector3 VehExitPosPoint { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Vector3 VehExitPos { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Vector3 VehExitRot { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int DefaultDimension { get; set; }
        
        public GarageData(int FractionId, Vector3 PlayerEnterPos, Vector3 PlayerEnterRot, Vector3 PlayerExitPos, Vector3 PlayerExitRot, Vector3 VehEnterPosPoint, Vector3 VehEnterPos, Vector3 VehEnterRot, Vector3 VehExitPosPoint, Vector3 VehExitPos, Vector3 VehExitRot, int DefaultDimension = 1750)
        {
            this.FractionId = FractionId;
            
            this.PlayerEnterPos = PlayerEnterPos;
            this.PlayerEnterRot = PlayerEnterRot;
            this.PlayerExitPos = PlayerExitPos;
            this.PlayerExitRot = PlayerExitRot;
            //
            this.VehEnterPosPoint = VehEnterPosPoint;
            this.VehEnterPos = VehEnterPos;
            this.VehEnterRot = VehEnterRot;
            this.VehExitPosPoint = VehExitPosPoint;
            this.VehExitPos = VehExitPos;
            this.VehExitRot = VehExitRot;
            
            this.DefaultDimension = DefaultDimension;
        }
    }
}