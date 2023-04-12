using Newtonsoft.Json;
using System;

namespace NeptuneEvo.Character.Models
{
	public class QuestData
	{
		[JsonIgnore]
		public int AutoId { get; set; } // int(11)
		public string ActorName { get; set; } // varchar(42)
		public short Line { get; set; } // smallint(6)
		[JsonIgnore]
		public sbyte Status { get; set; } = 0; // tinyint(3)
		[JsonIgnore]
		public DateTime Time { get; set; } = DateTime.MinValue; // datetime
		[JsonIgnore]
		public bool Complete { get; set; } = false; // tinyint(1)
		/// <summary>
		/// Сколько пройдено пунктов, нужно только для меню
		/// </summary>
		public sbyte Stage { get; set; } = 0; // tinyint(3)
		[JsonIgnore]
		public string Data { get; set; } = "0"; // varchar(50)
		/// <summary>
		/// Нуцжно для худа
		/// </summary>
		[JsonIgnore]
		public bool Use { get; set; } = false; // tinyint(1)
	}
}
