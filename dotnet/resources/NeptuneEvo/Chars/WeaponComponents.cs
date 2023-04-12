using GTANetworkAPI;
using NeptuneEvo.Handles;
using NeptuneEvo.Chars.Models;
using Newtonsoft.Json;
using Redage.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using NeptuneEvo.Character;

namespace NeptuneEvo.Chars
{
    class WeaponComponents : Script
    {
		private static readonly nLog Log = new nLog("Chars.WeaponComponents");

		public static IReadOnlyDictionary<uint, wComponentsData> WeaponsComponents = new Dictionary<uint, wComponentsData>()
		{
			{ NAPI.Util.GetHashKey("WEAPON_SNIPERRIFLE"), new wComponentsData(4, new Dictionary<uint, wComponentData>()
				{
					{ NAPI.Util.GetHashKey("COMPONENT_SNIPERRIFLE_VARMOD_LUXE"), new wComponentData("WCT_VAR_WOOD", "WCD_VAR_WOOD", 30000, wComponentsType.Varmod) },
					//{ NAPI.Util.GetHashKey("COMPONENT_SNIPERRIFLE_CLIP_01"), new wComponentData("WCT_CLIP1", "WCD_SR_CLIP1", 0, wComponentsType.Clip) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_AR_SUPP_02"), new wComponentData("WCT_SUPP", "WCD_AR_SUPP2", 9000, wComponentsType.Suppressor) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_SCOPE_MAX"), new wComponentData("WCT_SCOPE_MAX", "WCD_SCOPE_MAX", 17000, wComponentsType.Scope) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_SCOPE_LARGE"), new wComponentData("WCT_SCOPE_LRG", "WCD_SCOPE_LRG", 13000, wComponentsType.Scope) },

				})
			},
			/*{ NAPI.Util.GetHashKey("WEAPON_VINTAGEPISTOL"), new wComponentsData(2, new Dictionary<uint, wComponentData>()
				{
					{ NAPI.Util.GetHashKey("COMPONENT_VINTAGEPISTOL_CLIP_02"), new wComponentData("WCT_CLIP2", "WCD_VPST_CLIP2", 0, wComponentsType.Clip) },
					{ NAPI.Util.GetHashKey("COMPONENT_VINTAGEPISTOL_CLIP_01"), new wComponentData("WCT_CLIP1", "WCD_VPST_CLIP1", 0, wComponentsType.Clip) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_PI_SUPP"), new wComponentData("WCT_SUPP", "WCD_PI_SUPP", 0, wComponentsType.Suppressor) },

				})
			},*/
			{ NAPI.Util.GetHashKey("WEAPON_COMBATPDW"), new wComponentsData(3, new Dictionary<uint, wComponentData>()
				{
					{ NAPI.Util.GetHashKey("COMPONENT_AT_AR_AFGRIP"), new wComponentData("WCT_GRIP", "WCD_GRIP", 900, wComponentsType.Grip) },
					//{ NAPI.Util.GetHashKey("COMPONENT_COMBATPDW_CLIP_02"), new wComponentData("WCT_CLIP2", "WCD_PDW_CLIP2", 0, wComponentsType.Clip) },
					//{ NAPI.Util.GetHashKey("COMPONENT_COMBATPDW_CLIP_01"), new wComponentData("WCT_CLIP1", "WCD_PDW_CLIP1", 0, wComponentsType.Clip) },
					//{ NAPI.Util.GetHashKey("COMPONENT_COMBATPDW_CLIP_03"), new wComponentData("WCT_CLIP_DRM", "WCD_CLIP3", 0, wComponentsType.Clip) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_AR_FLSH"), new wComponentData("WCT_FLASH", "WCD_FLASH", 1200, wComponentsType.Flashlight) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_SCOPE_SMALL"), new wComponentData("WCT_SCOPE_SML", "WCD_SCOPE_SML", 1500, wComponentsType.Scope) },

				})
			},
			/*{ NAPI.Util.GetHashKey("WEAPON_HEAVYSNIPER_MK2"), new wComponentsData(5, new Dictionary<uint, wComponentData>()
				{
					{ NAPI.Util.GetHashKey("COMPONENT_HEAVYSNIPER_MK2_CLIP_INCENDIARY"), new wComponentData("WCT_CLIP_INC", "WCD_CLIP_INC", 0, wComponentsType.Clip) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_SR_BARREL_02"), new wComponentData("WCT_BARR2", "WCD_BARR2", 0, wComponentsType.Barrel) },
					{ NAPI.Util.GetHashKey("COMPONENT_HEAVYSNIPER_MK2_CAMO_04"), new wComponentData("WCT_CAMO_4", "", 0, wComponentsType.Varmod) },
					{ NAPI.Util.GetHashKey("COMPONENT_HEAVYSNIPER_MK2_CAMO_05"), new wComponentData("WCT_CAMO_5", "", 0, wComponentsType.Varmod) },
					{ NAPI.Util.GetHashKey("COMPONENT_HEAVYSNIPER_MK2_CLIP_02"), new wComponentData("WCT_CLIP2", "WCD_CLIP2", 0, wComponentsType.Clip) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_SCOPE_THERMAL"), new wComponentData("WCT_SCOPE_TH", "WCD_SCOPE_TH", 0, wComponentsType.Scope) },
					{ NAPI.Util.GetHashKey("COMPONENT_HEAVYSNIPER_MK2_CAMO_06"), new wComponentData("WCT_CAMO_6", "", 0, wComponentsType.Varmod) },
					{ NAPI.Util.GetHashKey("COMPONENT_HEAVYSNIPER_MK2_CLIP_FMJ"), new wComponentData("WCT_CLIP_FMJ", "WCD_CLIP_FMJ", 0, wComponentsType.Clip) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_MUZZLE_08"), new wComponentData("WCT_MUZZ8", "WCD_MUZZ_SR", 0, wComponentsType.Suppressor) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_MUZZLE_09"), new wComponentData("WCT_MUZZ9", "WCD_MUZZ_SR", 0, wComponentsType.Suppressor) },
					{ NAPI.Util.GetHashKey("COMPONENT_HEAVYSNIPER_MK2_CAMO_IND_01"), new wComponentData("WCT_CAMO_IND", "", 0, wComponentsType.Varmod) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_SCOPE_LARGE_MK2"), new wComponentData("WCT_SCOPE_LRG2", "WCD_SCOPE_LRG", 0, wComponentsType.Scope) },
					{ NAPI.Util.GetHashKey("COMPONENT_HEAVYSNIPER_MK2_CLIP_EXPLOSIVE"), new wComponentData("WCT_CLIP_EX", "WCD_CLIP_EX", 0, wComponentsType.Clip) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_SR_BARREL_01"), new wComponentData("WCT_BARR", "WCD_BARR", 0, wComponentsType.Barrel) },
					{ NAPI.Util.GetHashKey("COMPONENT_HEAVYSNIPER_MK2_CAMO_10"), new wComponentData("WCT_CAMO_10", "", 0, wComponentsType.Varmod) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_SR_SUPP_03"), new wComponentData("WCT_SUPP", "WCD_SR_SUPP", 0, wComponentsType.Suppressor) },
					{ NAPI.Util.GetHashKey("COMPONENT_HEAVYSNIPER_MK2_CAMO_07"), new wComponentData("WCT_CAMO_7", "", 0, wComponentsType.Varmod) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_SCOPE_NV"), new wComponentData("WCT_SCOPE_NV", "WCD_SCOPE_NV", 0, wComponentsType.Scope) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_SCOPE_MAX"), new wComponentData("WCT_SCOPE_MAX", "WCD_SCOPE_MAX", 0, wComponentsType.Scope) },
					{ NAPI.Util.GetHashKey("COMPONENT_HEAVYSNIPER_MK2_CAMO_08"), new wComponentData("WCT_CAMO_8", "", 0, wComponentsType.Varmod) },
					{ NAPI.Util.GetHashKey("COMPONENT_HEAVYSNIPER_MK2_CAMO_02"), new wComponentData("WCT_CAMO_2", "", 0, wComponentsType.Varmod) },
					{ NAPI.Util.GetHashKey("COMPONENT_HEAVYSNIPER_MK2_CAMO_09"), new wComponentData("WCT_CAMO_9", "", 0, wComponentsType.Varmod) },
					{ NAPI.Util.GetHashKey("COMPONENT_HEAVYSNIPER_MK2_CAMO_03"), new wComponentData("WCT_CAMO_3", "", 0, wComponentsType.Varmod) },
					{ NAPI.Util.GetHashKey("COMPONENT_HEAVYSNIPER_MK2_CAMO"), new wComponentData("WCT_CAMO_1", "", 0, wComponentsType.Varmod) },
					{ NAPI.Util.GetHashKey("COMPONENT_HEAVYSNIPER_MK2_CLIP_ARMORPIERCING"), new wComponentData("WCT_CLIP_AP", "WCD_CLIP_AP", 0, wComponentsType.Clip) },
					{ NAPI.Util.GetHashKey("COMPONENT_HEAVYSNIPER_MK2_CLIP_01"), new wComponentData("WCT_CLIP1", "WCD_CLIP1", 0, wComponentsType.Clip) },

				})
			},
			{ NAPI.Util.GetHashKey("WEAPON_HEAVYSNIPER"), new wComponentsData(2, new Dictionary<uint, wComponentData>()
				{
					{ NAPI.Util.GetHashKey("COMPONENT_HEAVYSNIPER_CLIP_01"), new wComponentData("WCT_CLIP1", "WCD_HS_CLIP1", 0, wComponentsType.Clip) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_SCOPE_MAX"), new wComponentData("WCT_SCOPE_MAX", "WCD_SCOPE_MAX", 0, wComponentsType.Scope) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_SCOPE_LARGE"), new wComponentData("WCT_SCOPE_LRG", "WCD_SCOPE_LRG", 0, wComponentsType.Scope) },

				})
			},*/
			{ NAPI.Util.GetHashKey("WEAPON_MICROSMG"), new wComponentsData(4, new Dictionary<uint, wComponentData>()
				{
					//{ NAPI.Util.GetHashKey("COMPONENT_MICROSMG_CLIP_02"), new wComponentData("WCT_CLIP2", "WCDMSMG_CLIP2", 0, wComponentsType.Clip) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_PI_FLSH"), new wComponentData("WCT_FLASH", "WCD_FLASH", 800, wComponentsType.Flashlight) },
					{ NAPI.Util.GetHashKey("COMPONENT_MICROSMG_VARMOD_LUXE"), new wComponentData("WCT_VAR_GOLD", "WCD_VAR_GOLD", 900, wComponentsType.Varmod) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_SCOPE_MACRO"), new wComponentData("WCT_SCOPE_MAC", "WCD_SCOPE_MAC", 1100, wComponentsType.Scope) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_AR_SUPP_02"), new wComponentData("WCT_SUPP", "WCD_AR_SUPP2", 1000, wComponentsType.Suppressor) },
					//{ NAPI.Util.GetHashKey("COMPONENT_MICROSMG_CLIP_01"), new wComponentData("WCT_CLIP1", "WCDMSMG_CLIP1", 0, wComponentsType.Clip) },

				})
			},
			{ NAPI.Util.GetHashKey("WEAPON_PISTOL"), new wComponentsData(3, new Dictionary<uint, wComponentData>()
				{
					{ NAPI.Util.GetHashKey("COMPONENT_AT_PI_FLSH"), new wComponentData("WCT_FLASH", "WCD_FLASH", 400, wComponentsType.Flashlight) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_PI_SUPP_02"), new wComponentData("WCT_SUPP", "WCD_PI_SUPP", 600, wComponentsType.Suppressor) },
					{ NAPI.Util.GetHashKey("COMPONENT_PISTOL_VARMOD_LUXE"), new wComponentData("WCT_VAR_GOLD", "WCD_VAR_GOLD", 500, wComponentsType.Varmod) },
					//{ NAPI.Util.GetHashKey("COMPONENT_PISTOL_CLIP_02"), new wComponentData("WCT_CLIP2", "WCD_P_CLIP2", 0, wComponentsType.Clip) },
					//{ NAPI.Util.GetHashKey("COMPONENT_PISTOL_CLIP_01"), new wComponentData("WCT_CLIP1", "WCD_P_CLIP1", 0, wComponentsType.Clip) },

				})
			},
			{ NAPI.Util.GetHashKey("WEAPON_PUMPSHOTGUN"), new wComponentsData(3, new Dictionary<uint, wComponentData>()
				{
					{ NAPI.Util.GetHashKey("COMPONENT_AT_AR_FLSH"), new wComponentData("WCT_FLASH", "WCD_FLASH", 500, wComponentsType.Flashlight) },
					{ NAPI.Util.GetHashKey("COMPONENT_PUMPSHOTGUN_VARMOD_LOWRIDER"), new wComponentData("WCT_VAR_GOLD", "WCD_VAR_GOLD", 600, wComponentsType.Varmod) },
					//{ NAPI.Util.GetHashKey("COMPONENT_PUMPSHOTGUN_CLIP_01"), new wComponentData("WCT_INVALID", "WCD_INVALID", 0, wComponentsType.Clip) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_SR_SUPP"), new wComponentData("WCT_SUPP", "WCD_SR_SUPP", 700, wComponentsType.Suppressor) },

				})
			},
			{ NAPI.Util.GetHashKey("WEAPON_APPISTOL"), new wComponentsData(3, new Dictionary<uint, wComponentData>()
				{
					//{ NAPI.Util.GetHashKey("COMPONENT_APPISTOL_CLIP_02"), new wComponentData("WCT_CLIP2", "WCD_AP_CLIP2", 0, wComponentsType.Clip) },
					//{ NAPI.Util.GetHashKey("COMPONENT_APPISTOL_CLIP_01"), new wComponentData("WCT_CLIP1", "WCD_AP_CLIP1", 0, wComponentsType.Clip) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_PI_FLSH"), new wComponentData("WCT_FLASH", "WCD_FLASH", 900, wComponentsType.Flashlight) },
					{ NAPI.Util.GetHashKey("COMPONENT_APPISTOL_VARMOD_LUXE"), new wComponentData("WCT_VAR_METAL", "WCD_VAR_METAL", 1000, wComponentsType.Varmod) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_PI_SUPP"), new wComponentData("WCT_SUPP", "WCD_PI_SUPP", 1100, wComponentsType.Suppressor) },

				})
			},
			{ NAPI.Util.GetHashKey("WEAPON_SMG"), new wComponentsData(5, new Dictionary<uint, wComponentData>()
				{
					{ NAPI.Util.GetHashKey("COMPONENT_AT_AR_AFGRIP"), new wComponentData("WCT_GRIP", "WCD_GRIP", 900, wComponentsType.Grip) }, //Invalid
					//{ NAPI.Util.GetHashKey("COMPONENT_SMG_CLIP_01"), new wComponentData("WCT_CLIP1", "WCD_SMG_CLIP1", 0, wComponentsType.Clip) },
					{ NAPI.Util.GetHashKey("COMPONENT_SMG_VARMOD_LUXE"), new wComponentData("WCT_VAR_GOLD", "WCD_VAR_GOLD", 900, wComponentsType.Varmod) },
					//{ NAPI.Util.GetHashKey("COMPONENT_SMG_CLIP_02"), new wComponentData("WCT_CLIP2", "WCD_SMG_CLIP2", 0, wComponentsType.Clip) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_SCOPE_MACRO_02"), new wComponentData("WCT_SCOPE_MAC", "WCD_SCOPE_MAC", 1100, wComponentsType.Scope) },
					//{ NAPI.Util.GetHashKey("COMPONENT_SMG_CLIP_03"), new wComponentData("WCT_CLIP_DRM", "WCD_CLIP_DRM", 0, wComponentsType.Clip) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_AR_FLSH"), new wComponentData("WCT_FLASH", "WCD_FLASH", 1200, wComponentsType.Flashlight) }, //Invalid
					{ NAPI.Util.GetHashKey("COMPONENT_AT_SCOPE_MACRO"), new wComponentData("WCT_SCOPE_MAC", "WCD_SCOPE_MAC", 1100, wComponentsType.Scope) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_PI_SUPP"), new wComponentData("WCT_SUPP", "WCD_PI_SUPP", 1400, wComponentsType.Suppressor) },

				})
			},
			/*{ NAPI.Util.GetHashKey("WEAPON_ASSAULTRIFLE_MK2"), new wComponentsData(7, new Dictionary<uint, wComponentData>()
				{
					{ NAPI.Util.GetHashKey("COMPONENT_AT_MUZZLE_05"), new wComponentData("WCT_MUZZ5", "WCD_MUZZ", 0, wComponentsType.Suppressor) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_SCOPE_MACRO_MK2"), new wComponentData("WCT_SCOPE_MAC2", "WCD_SCOPE_MAC", 0, wComponentsType.Scope2) },
					{ NAPI.Util.GetHashKey("COMPONENT_ASSAULTRIFLE_MK2_CAMO_04"), new wComponentData("WCT_CAMO_4", "", 0, wComponentsType.Varmod) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_MUZZLE_06"), new wComponentData("WCT_MUZZ6", "WCD_MUZZ", 0, wComponentsType.Suppressor) },
					{ NAPI.Util.GetHashKey("COMPONENT_ASSAULTRIFLE_MK2_CAMO_02"), new wComponentData("WCT_CAMO_2", "", 0, wComponentsType.Varmod) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_SIGHTS"), new wComponentData("WCT_HOLO", "WCD_HOLO", 0, wComponentsType.Scope2) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_AR_BARREL_01"), new wComponentData("WCT_BARR", "WCD_BARR", 0, wComponentsType.Barrel) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_MUZZLE_07"), new wComponentData("WCT_MUZZ7", "WCD_MUZZ", 0, wComponentsType.Suppressor) },
					{ NAPI.Util.GetHashKey("COMPONENT_ASSAULTRIFLE_MK2_CAMO_03"), new wComponentData("WCT_CAMO_3", "", 0, wComponentsType.Varmod) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_AR_BARREL_02"), new wComponentData("WCT_BARR2", "WCD_BARR2", 0, wComponentsType.Barrel) },
					{ NAPI.Util.GetHashKey("COMPONENT_ASSAULTRIFLE_MK2_CLIP_FMJ"), new wComponentData("WCT_CLIP_FMJ", "WCD_CLIP_FMJ", 0, wComponentsType.Clip) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_AR_FLSH"), new wComponentData("WCT_FLASH", "WCD_FLASH", 0, wComponentsType.Invalid) },
					{ NAPI.Util.GetHashKey("COMPONENT_ASSAULTRIFLE_MK2_CAMO_09"), new wComponentData("WCT_CAMO_9", "", 0, wComponentsType.Varmod) },
					{ NAPI.Util.GetHashKey("COMPONENT_ASSAULTRIFLE_MK2_CLIP_01"), new wComponentData("WCT_CLIP1", "WCD_CLIP1", 0, wComponentsType.Clip) },
					{ NAPI.Util.GetHashKey("COMPONENT_ASSAULTRIFLE_MK2_CAMO"), new wComponentData("WCT_CAMO_1", "", 0, wComponentsType.Varmod) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_AR_AFGRIP_02"), new wComponentData("WCT_GRIP", "WCD_GRIP", 0, wComponentsType.Grip2) },
					{ NAPI.Util.GetHashKey("COMPONENT_ASSAULTRIFLE_MK2_CAMO_10"), new wComponentData("WCT_CAMO_10", "", 0, wComponentsType.Varmod) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_AR_SUPP_02"), new wComponentData("WCT_SUPP", "WCD_AR_SUPP2", 0, wComponentsType.Suppressor) },
					{ NAPI.Util.GetHashKey("COMPONENT_ASSAULTRIFLE_MK2_CLIP_ARMORPIERCING"), new wComponentData("WCT_CLIP_AP", "WCD_CLIP_AP", 0, wComponentsType.Clip) },
					{ NAPI.Util.GetHashKey("COMPONENT_ASSAULTRIFLE_MK2_CAMO_07"), new wComponentData("WCT_CAMO_7", "", 0, wComponentsType.Varmod) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_MUZZLE_01"), new wComponentData("WCT_MUZZ1", "WCD_MUZZ", 0, wComponentsType.Suppressor) },
					{ NAPI.Util.GetHashKey("COMPONENT_ASSAULTRIFLE_MK2_CAMO_IND_01"), new wComponentData("WCT_CAMO_IND", "", 0, wComponentsType.Varmod) },
					{ NAPI.Util.GetHashKey("COMPONENT_ASSAULTRIFLE_MK2_CAMO_05"), new wComponentData("WCT_CAMO_5", "", 0, wComponentsType.Varmod) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_SCOPE_MEDIUM_MK2"), new wComponentData("WCT_SCOPE_MED2", "WCD_SCOPE_MED", 0, wComponentsType.Scope2) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_MUZZLE_02"), new wComponentData("WCT_MUZZ2", "WCD_MUZZ", 0, wComponentsType.Suppressor) },
					{ NAPI.Util.GetHashKey("COMPONENT_ASSAULTRIFLE_MK2_CAMO_06"), new wComponentData("WCT_CAMO_6", "", 0, wComponentsType.Varmod) },
					{ NAPI.Util.GetHashKey("COMPONENT_ASSAULTRIFLE_MK2_CLIP_02"), new wComponentData("WCT_CLIP2", "WCD_CLIP2", 0, wComponentsType.Clip) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_MUZZLE_03"), new wComponentData("WCT_MUZZ3", "WCD_MUZZ", 0, wComponentsType.Suppressor) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_MUZZLE_04"), new wComponentData("WCT_MUZZ4", "WCD_MUZZ", 0, wComponentsType.Suppressor) },
					{ NAPI.Util.GetHashKey("COMPONENT_ASSAULTRIFLE_MK2_CLIP_TRACER"), new wComponentData("WCT_CLIP_TR", "WCD_CLIP_TR", 0, wComponentsType.Clip) },
					{ NAPI.Util.GetHashKey("COMPONENT_ASSAULTRIFLE_MK2_CLIP_INCENDIARY"), new wComponentData("WCT_CLIP_INC", "WCD_CLIP_INC", 0, wComponentsType.Clip) },
					{ NAPI.Util.GetHashKey("COMPONENT_ASSAULTRIFLE_MK2_CAMO_08"), new wComponentData("WCT_CAMO_8", "", 0, wComponentsType.Varmod) },

				})
			},
			{ NAPI.Util.GetHashKey("WEAPON_HEAVYSHOTGUN"), new wComponentsData(3, new Dictionary<uint, wComponentData>()
				{
					{ NAPI.Util.GetHashKey("COMPONENT_AT_AR_AFGRIP"), new wComponentData("WCT_GRIP", "WCD_GRIP", 0, wComponentsType.Invalid) },
					{ NAPI.Util.GetHashKey("COMPONENT_HEAVYSHOTGUN_CLIP_01"), new wComponentData("WCT_CLIP1", "WCD_HVSG_CLIP1", 0, wComponentsType.Clip) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_AR_FLSH"), new wComponentData("WCT_FLASH", "WCD_FLASH", 0, wComponentsType.Invalid) },
					{ NAPI.Util.GetHashKey("COMPONENT_HEAVYSHOTGUN_CLIP_03"), new wComponentData("WCT_CLIP_DRM", "WCD_CLIP3", 0, wComponentsType.Clip) },
					{ NAPI.Util.GetHashKey("COMPONENT_HEAVYSHOTGUN_CLIP_02"), new wComponentData("WCT_CLIP2", "WCD_HVSG_CLIP2", 0, wComponentsType.Clip) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_AR_SUPP_02"), new wComponentData("WCT_SUPP", "WCD_AR_SUPP2", 0, wComponentsType.Suppressor) },

				})
			},
			{ NAPI.Util.GetHashKey("WEAPON_MINIGUN"), new wComponentsData(1, new Dictionary<uint, wComponentData>()
				{
					{ NAPI.Util.GetHashKey("COMPONENT_MINIGUN_CLIP_01"), new wComponentData("WCT_INVALID", "WCD_INVALID", 0, wComponentsType.Clip) },

				})
			},
			{ NAPI.Util.GetHashKey("WEAPON_GRENADELAUNCHER_SMOKE"), new wComponentsData(1, new Dictionary<uint, wComponentData>()
				{
					{ NAPI.Util.GetHashKey("COMPONENT_AT_AR_AFGRIP"), new wComponentData("WCT_GRIP", "WCD_GRIP", 0, wComponentsType.Invalid) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_AR_FLSH"), new wComponentData("WCT_FLASH", "WCD_FLASH", 0, wComponentsType.Invalid) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_SCOPE_SMALL"), new wComponentData("WCT_SCOPE_SML", "WCD_SCOPE_SML", 0, wComponentsType.Invalid) },

				})
			},*/
			{ NAPI.Util.GetHashKey("WEAPON_COMBATPISTOL"), new wComponentsData(3, new Dictionary<uint, wComponentData>()
				{
					//{ NAPI.Util.GetHashKey("COMPONENT_COMBATPISTOL_CLIP_01"), new wComponentData("WCT_CLIP1", "WCD_CP_CLIP1", 0, wComponentsType.Clip) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_PI_FLSH"), new wComponentData("WCT_FLASH", "WCD_FLASH", 400, wComponentsType.Flashlight) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_PI_SUPP"), new wComponentData("WCT_SUPP", "WCD_PI_SUPP", 600, wComponentsType.Suppressor) },
					{ NAPI.Util.GetHashKey("COMPONENT_COMBATPISTOL_VARMOD_LOWRIDER"), new wComponentData("WCT_VAR_GOLD", "WCD_VAR_GOLD", 500, wComponentsType.Varmod) },
					//{ NAPI.Util.GetHashKey("COMPONENT_COMBATPISTOL_CLIP_02"), new wComponentData("WCT_CLIP2", "WCD_CP_CLIP2", 0, wComponentsType.Clip) },

				})
			},
			/*{ NAPI.Util.GetHashKey("WEAPON_GUSENBERG"), new wComponentsData(1, new Dictionary<uint, wComponentData>()
				{
					{ NAPI.Util.GetHashKey("COMPONENT_GUSENBERG_CLIP_01"), new wComponentData("WCT_CLIP1", "WCD_GSNB_CLIP1", 0, wComponentsType.Clip) },
					{ NAPI.Util.GetHashKey("COMPONENT_GUSENBERG_CLIP_02"), new wComponentData("WCT_CLIP2", "WCD_GSNB_CLIP2", 0, wComponentsType.Clip) },

				})
			},*/
			/*{ NAPI.Util.GetHashKey("WEAPON_COMPACTRIFLE"), new wComponentsData(1, new Dictionary<uint, wComponentData>()
				{
					//{ NAPI.Util.GetHashKey("COMPONENT_COMPACTRIFLE_CLIP_01"), new wComponentData("WCT_CLIP1", "WCD_CMPR_CLIP1", 0, wComponentsType.Clip) },
					//{ NAPI.Util.GetHashKey("COMPONENT_COMPACTRIFLE_CLIP_02"), new wComponentData("WCT_CLIP2", "WCD_CMPR_CLIP2", 0, wComponentsType.Clip) },
					//{ NAPI.Util.GetHashKey("COMPONENT_COMPACTRIFLE_CLIP_03"), new wComponentData("WCT_CLIP_DRM", "WCD_CLIP3", 0, wComponentsType.Clip) },

				})
			},*/
			{ NAPI.Util.GetHashKey("WEAPON_SAWNOFFSHOTGUN"), new wComponentsData(1, new Dictionary<uint, wComponentData>()
				{
					{ NAPI.Util.GetHashKey("COMPONENT_SAWNOFFSHOTGUN_VARMOD_LUXE"), new wComponentData("WCT_VAR_METAL", "WCD_VAR_METAL", 600, wComponentsType.Varmod) },
					//{ NAPI.Util.GetHashKey("COMPONENT_SAWNOFFSHOTGUN_CLIP_01"), new wComponentData("WCT_INVALID", "WCD_INVALID", 0, wComponentsType.Clip) },

				})
			},
			{ NAPI.Util.GetHashKey("WEAPON_BULLPUPRIFLE"), new wComponentsData(5, new Dictionary<uint, wComponentData>()
				{
					{ NAPI.Util.GetHashKey("COMPONENT_AT_AR_AFGRIP"), new wComponentData("WCT_GRIP", "WCD_GRIP", 900, wComponentsType.Grip) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_AR_FLSH"), new wComponentData("WCT_FLASH", "WCD_FLASH", 1200, wComponentsType.Flashlight) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_AR_SUPP"), new wComponentData("WCT_SUPP", "WCD_AR_SUPP", 700, wComponentsType.Suppressor) },
					{ NAPI.Util.GetHashKey("COMPONENT_BULLPUPRIFLE_VARMOD_LOW"), new wComponentData("WCT_VAR_METAL", "WCD_VAR_BPR", 1600, wComponentsType.Varmod) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_SCOPE_SMALL"), new wComponentData("WCT_SCOPE_SML", "WCD_SCOPE_SML", 900, wComponentsType.Scope) },
					//{ NAPI.Util.GetHashKey("COMPONENT_BULLPUPRIFLE_CLIP_02"), new wComponentData("WCT_CLIP2", "WCD_BRIF_CLIP2", 0, wComponentsType.Clip) },
					//{ NAPI.Util.GetHashKey("COMPONENT_BULLPUPRIFLE_CLIP_01"), new wComponentData("WCT_CLIP1", "WCD_BRIF_CLIP1", 0, wComponentsType.Clip) },

				})
			},
			/*{ NAPI.Util.GetHashKey("WEAPON_FIREWORK"), new wComponentsData(1, new Dictionary<uint, wComponentData>()
				{
					{ NAPI.Util.GetHashKey("COMPONENT_FIREWORK_CLIP_01"), new wComponentData("WCT_CLIP1", "WCD_FWRK_CLIP1", 0, wComponentsType.Invalid) },

				})
			},
			{ NAPI.Util.GetHashKey("WEAPON_COMBATMG"), new wComponentsData(4, new Dictionary<uint, wComponentData>()
				{
					{ NAPI.Util.GetHashKey("COMPONENT_AT_AR_AFGRIP"), new wComponentData("WCT_GRIP", "WCD_GRIP", 0, wComponentsType.Invalid) },
					{ NAPI.Util.GetHashKey("COMPONENT_COMBATMG_VARMOD_LOWRIDER"), new wComponentData("WCT_VAR_ETCHM", "WCD_VAR_ETCHM", 0, wComponentsType.Varmod) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_SCOPE_MEDIUM"), new wComponentData("WCT_SCOPE_LRG", "WCD_SCOPE_LRG", 0, wComponentsType.Scope) },
					{ NAPI.Util.GetHashKey("COMPONENT_COMBATMG_CLIP_02"), new wComponentData("WCT_CLIP2", "WCDCMG_CLIP2", 0, wComponentsType.Clip) },
					{ NAPI.Util.GetHashKey("COMPONENT_COMBATMG_CLIP_01"), new wComponentData("WCT_CLIP1", "WCDCMG_CLIP1", 0, wComponentsType.Clip) },

				})
			},*/
			{ NAPI.Util.GetHashKey("WEAPON_REVOLVER"), new wComponentsData(1, new Dictionary<uint, wComponentData>()
				{
					{ NAPI.Util.GetHashKey("COMPONENT_REVOLVER_VARMOD_BOSS"), new wComponentData("WCT_REV_VARB", "WCD_REV_VARB", 1300, wComponentsType.Varmod) },
					{ NAPI.Util.GetHashKey("COMPONENT_REVOLVER_VARMOD_GOON"), new wComponentData("WCT_REV_VARG", "WCD_REV_VARG", 1000, wComponentsType.Varmod) },
					//{ NAPI.Util.GetHashKey("COMPONENT_REVOLVER_CLIP_01"), new wComponentData("WCT_CLIP1", "WCD_CLIP1", 0, wComponentsType.Clip) },

				})
			},
			/*{ NAPI.Util.GetHashKey("WEAPON_SWITCHBLADE"), new wComponentsData(1, new Dictionary<uint, wComponentData>()
				{
					{ NAPI.Util.GetHashKey("COMPONENT_SWITCHBLADE_VARMOD_VAR1"), new wComponentData("WCT_SB_VAR1", "WCD_VAR_DESC", 0, wComponentsType.Varmod) },
					{ NAPI.Util.GetHashKey("COMPONENT_SWITCHBLADE_VARMOD_BASE"), new wComponentData("WCT_SB_BASE", "WCD_VAR_DESC", 0, wComponentsType.Varmod) },
					{ NAPI.Util.GetHashKey("COMPONENT_SWITCHBLADE_VARMOD_VAR2"), new wComponentData("WCT_SB_VAR2", "WCD_VAR_DESC", 0, wComponentsType.Varmod) },

				})
			},*/
			/*{ NAPI.Util.GetHashKey("WEAPON_MINISMG"), new wComponentsData(1, new Dictionary<uint, wComponentData>()
				{
					//{ NAPI.Util.GetHashKey("COMPONENT_MINISMG_CLIP_01"), new wComponentData("WCT_CLIP1", "WCD_MIMG_CLIP1", 0, wComponentsType.Clip) },
					//{ NAPI.Util.GetHashKey("COMPONENT_MINISMG_CLIP_02"), new wComponentData("WCT_CLIP2", "WCD_MIMG_CLIP2", 0, wComponentsType.Clip) },

				})
			},*/
			/*{ NAPI.Util.GetHashKey("WEAPON_PISTOL_MK2"), new wComponentsData(5, new Dictionary<uint, wComponentData>()
				{
					{ NAPI.Util.GetHashKey("COMPONENT_PISTOL_MK2_CAMO_04"), new wComponentData("WCT_CAMO_4", "", 0, wComponentsType.Varmod) },
					{ NAPI.Util.GetHashKey("COMPONENT_PISTOL_MK2_CAMO_02"), new wComponentData("WCT_CAMO_2", "", 0, wComponentsType.Varmod) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_PI_COMP"), new wComponentData("WCT_COMP", "WCD_COMP", 0, wComponentsType.Suppressor) },
					{ NAPI.Util.GetHashKey("COMPONENT_PISTOL_MK2_CLIP_TRACER"), new wComponentData("WCT_CLIP_TR", "WCD_CLIP_TR", 0, wComponentsType.Clip) },
					{ NAPI.Util.GetHashKey("COMPONENT_PISTOL_MK2_CLIP_INCENDIARY"), new wComponentData("WCT_CLIP_INC", "WCD_CLIP_INC", 0, wComponentsType.Clip) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_PI_FLSH_02"), new wComponentData("WCT_FLASH", "WCD_FLASH", 0, wComponentsType.Flashlight) },
					{ NAPI.Util.GetHashKey("COMPONENT_PISTOL_MK2_CAMO_08"), new wComponentData("WCT_CAMO_8", "", 0, wComponentsType.Varmod) },
					{ NAPI.Util.GetHashKey("COMPONENT_PISTOL_MK2_CLIP_FMJ"), new wComponentData("WCT_CLIP_FMJ", "WCD_CLIP_FMJ", 0, wComponentsType.Clip) },
					{ NAPI.Util.GetHashKey("COMPONENT_PISTOL_MK2_CAMO"), new wComponentData("WCT_CAMO_1", "", 0, wComponentsType.Varmod) },
					{ NAPI.Util.GetHashKey("COMPONENT_PISTOL_MK2_CLIP_02"), new wComponentData("WCT_CLIP2", "WCD_CLIP2", 0, wComponentsType.Clip) },
					{ NAPI.Util.GetHashKey("COMPONENT_PISTOL_MK2_CAMO_09"), new wComponentData("WCT_CAMO_9", "", 0, wComponentsType.Varmod) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_PI_SUPP_02"), new wComponentData("WCT_SUPP", "WCD_PI_SUPP", 0, wComponentsType.Suppressor) },
					{ NAPI.Util.GetHashKey("COMPONENT_PISTOL_MK2_CAMO_10"), new wComponentData("WCT_CAMO_10", "", 0, wComponentsType.Varmod) },
					{ NAPI.Util.GetHashKey("COMPONENT_PISTOL_MK2_CLIP_HOLLOWPOINT"), new wComponentData("WCT_CLIP_HP", "WCD_CLIP_HP", 0, wComponentsType.Clip) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_PI_RAIL"), new wComponentData("WCT_SCOPE_PI", "WCD_SCOPE_PI", 0, wComponentsType.Scope) },
					{ NAPI.Util.GetHashKey("COMPONENT_PISTOL_MK2_CAMO_IND_01"), new wComponentData("WCT_CAMO_IND", "", 0, wComponentsType.Varmod) },
					{ NAPI.Util.GetHashKey("COMPONENT_PISTOL_MK2_CLIP_01"), new wComponentData("WCT_CLIP1", "WCD_CLIP1", 0, wComponentsType.Clip) },
					{ NAPI.Util.GetHashKey("COMPONENT_PISTOL_MK2_CAMO_03"), new wComponentData("WCT_CAMO_3", "", 0, wComponentsType.Varmod) },
					{ NAPI.Util.GetHashKey("COMPONENT_PISTOL_MK2_CAMO_07"), new wComponentData("WCT_CAMO_7", "", 0, wComponentsType.Varmod) },
					{ NAPI.Util.GetHashKey("COMPONENT_PISTOL_MK2_CAMO_06"), new wComponentData("WCT_CAMO_6", "", 0, wComponentsType.Varmod) },
					{ NAPI.Util.GetHashKey("COMPONENT_PISTOL_MK2_CAMO_05"), new wComponentData("WCT_CAMO_5", "", 0, wComponentsType.Varmod) },

				})
			},
			{ NAPI.Util.GetHashKey("WEAPON_CARBINERIFLE_MK2"), new wComponentsData(7, new Dictionary<uint, wComponentData>()
				{
					{ NAPI.Util.GetHashKey("COMPONENT_AT_MUZZLE_05"), new wComponentData("WCT_MUZZ5", "WCD_MUZZ", 0, wComponentsType.Suppressor) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_SCOPE_MACRO_MK2"), new wComponentData("WCT_SCOPE_MAC2", "WCD_SCOPE_MAC", 0, wComponentsType.Scope2) },
					{ NAPI.Util.GetHashKey("COMPONENT_CARBINERIFLE_MK2_CLIP_TRACER"), new wComponentData("WCT_CLIP_TR", "WCD_CLIP_TR", 0, wComponentsType.Clip) },
					{ NAPI.Util.GetHashKey("COMPONENT_CARBINERIFLE_MK2_CLIP_ARMORPIERCING"), new wComponentData("WCT_CLIP_AP", "WCD_CLIP_AP", 0, wComponentsType.Clip) },
					{ NAPI.Util.GetHashKey("COMPONENT_CARBINERIFLE_MK2_CAMO_03"), new wComponentData("WCT_CAMO_3", "", 0, wComponentsType.Varmod) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_MUZZLE_06"), new wComponentData("WCT_MUZZ6", "WCD_MUZZ", 0, wComponentsType.Suppressor) },
					{ NAPI.Util.GetHashKey("COMPONENT_CARBINERIFLE_MK2_CLIP_INCENDIARY"), new wComponentData("WCT_CLIP_INC", "WCD_CLIP_INC", 0, wComponentsType.Clip) },
					{ NAPI.Util.GetHashKey("COMPONENT_CARBINERIFLE_MK2_CAMO_02"), new wComponentData("WCT_CAMO_2", "", 0, wComponentsType.Varmod) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_SIGHTS"), new wComponentData("WCT_HOLO", "WCD_HOLO", 0, wComponentsType.Scope2) },
					{ NAPI.Util.GetHashKey("COMPONENT_CARBINERIFLE_MK2_CLIP_FMJ"), new wComponentData("WCT_CLIP_FMJ", "WCD_CLIP_FMJ", 0, wComponentsType.Clip) },
					{ NAPI.Util.GetHashKey("COMPONENT_CARBINERIFLE_MK2_CAMO"), new wComponentData("WCT_CAMO_1", "", 0, wComponentsType.Varmod) },
					{ NAPI.Util.GetHashKey("COMPONENT_CARBINERIFLE_MK2_CLIP_01"), new wComponentData("WCT_CLIP1", "WCD_CLIP1", 0, wComponentsType.Clip) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_MUZZLE_07"), new wComponentData("WCT_MUZZ7", "WCD_MUZZ", 0, wComponentsType.Suppressor) },
					{ NAPI.Util.GetHashKey("COMPONENT_CARBINERIFLE_MK2_CLIP_02"), new wComponentData("WCT_CLIP2", "WCD_CLIP2", 0, wComponentsType.Clip) },
					{ NAPI.Util.GetHashKey("COMPONENT_CARBINERIFLE_MK2_CAMO_10"), new wComponentData("WCT_CAMO_10", "", 0, wComponentsType.Varmod) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_AR_FLSH"), new wComponentData("WCT_FLASH", "WCD_FLASH", 0, wComponentsType.Invalid) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_CR_BARREL_01"), new wComponentData("WCT_BARR", "WCD_BARR", 0, wComponentsType.Barrel) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_AR_SUPP"), new wComponentData("WCT_SUPP", "WCD_AR_SUPP", 0, wComponentsType.Suppressor) },
					{ NAPI.Util.GetHashKey("COMPONENT_CARBINERIFLE_MK2_CAMO_06"), new wComponentData("WCT_CAMO_6", "", 0, wComponentsType.Varmod) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_CR_BARREL_02"), new wComponentData("WCT_BARR2", "WCD_BARR2", 0, wComponentsType.Barrel) },
					{ NAPI.Util.GetHashKey("COMPONENT_CARBINERIFLE_MK2_CAMO_09"), new wComponentData("WCT_CAMO_9", "", 0, wComponentsType.Varmod) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_AR_AFGRIP_02"), new wComponentData("WCT_GRIP", "WCD_GRIP", 0, wComponentsType.Grip2) },
					{ NAPI.Util.GetHashKey("COMPONENT_CARBINERIFLE_MK2_CAMO_08"), new wComponentData("WCT_CAMO_8", "", 0, wComponentsType.Varmod) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_MUZZLE_01"), new wComponentData("WCT_MUZZ1", "WCD_MUZZ", 0, wComponentsType.Suppressor) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_SCOPE_MEDIUM_MK2"), new wComponentData("WCT_SCOPE_MED2", "WCD_SCOPE_MED", 0, wComponentsType.Scope2) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_MUZZLE_02"), new wComponentData("WCT_MUZZ2", "WCD_MUZZ", 0, wComponentsType.Suppressor) },
					{ NAPI.Util.GetHashKey("COMPONENT_CARBINERIFLE_MK2_CAMO_05"), new wComponentData("WCT_CAMO_5", "", 0, wComponentsType.Varmod) },
					{ NAPI.Util.GetHashKey("COMPONENT_CARBINERIFLE_MK2_CAMO_IND_01"), new wComponentData("WCT_CAMO_IND", "", 0, wComponentsType.Varmod) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_MUZZLE_03"), new wComponentData("WCT_MUZZ3", "WCD_MUZZ", 0, wComponentsType.Suppressor) },
					{ NAPI.Util.GetHashKey("COMPONENT_CARBINERIFLE_MK2_CAMO_04"), new wComponentData("WCT_CAMO_4", "", 0, wComponentsType.Varmod) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_MUZZLE_04"), new wComponentData("WCT_MUZZ4", "WCD_MUZZ", 0, wComponentsType.Suppressor) },
					{ NAPI.Util.GetHashKey("COMPONENT_CARBINERIFLE_MK2_CAMO_07"), new wComponentData("WCT_CAMO_7", "", 0, wComponentsType.Varmod) },

				})
			},
			{ NAPI.Util.GetHashKey("WEAPON_COMBATMG_MK2"), new wComponentsData(6, new Dictionary<uint, wComponentData>()
				{
					{ NAPI.Util.GetHashKey("COMPONENT_COMBATMG_MK2_CAMO_10"), new wComponentData("WCT_CAMO_10", "", 0, wComponentsType.Varmod) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_MUZZLE_05"), new wComponentData("WCT_MUZZ5", "WCD_MUZZ", 0, wComponentsType.Suppressor) },
					{ NAPI.Util.GetHashKey("COMPONENT_COMBATMG_MK2_CAMO_09"), new wComponentData("WCT_CAMO_9", "", 0, wComponentsType.Varmod) },
					{ NAPI.Util.GetHashKey("COMPONENT_COMBATMG_MK2_CLIP_02"), new wComponentData("WCT_CLIP2", "WCD_CLIP2", 0, wComponentsType.Clip) },
					{ NAPI.Util.GetHashKey("COMPONENT_COMBATMG_MK2_CAMO_08"), new wComponentData("WCT_CAMO_8", "", 0, wComponentsType.Varmod) },
					{ NAPI.Util.GetHashKey("COMPONENT_COMBATMG_MK2_CLIP_ARMORPIERCING"), new wComponentData("WCT_CLIP_AP", "WCD_CLIP_AP", 0, wComponentsType.Clip) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_MUZZLE_06"), new wComponentData("WCT_MUZZ6", "WCD_MUZZ", 0, wComponentsType.Suppressor) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_SCOPE_SMALL_MK2"), new wComponentData("WCT_SCOPE_SML2", "WCD_SCOPE_SML", 0, wComponentsType.Scope2) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_SIGHTS"), new wComponentData("WCT_HOLO", "WCD_HOLO", 0, wComponentsType.Scope2) },
					{ NAPI.Util.GetHashKey("COMPONENT_COMBATMG_MK2_CAMO_05"), new wComponentData("WCT_CAMO_5", "", 0, wComponentsType.Varmod) },
					{ NAPI.Util.GetHashKey("COMPONENT_COMBATMG_MK2_CLIP_01"), new wComponentData("WCT_CLIP1", "WCD_CLIP1", 0, wComponentsType.Clip) },
					{ NAPI.Util.GetHashKey("COMPONENT_COMBATMG_MK2_CAMO"), new wComponentData("WCT_CAMO_1", "", 0, wComponentsType.Varmod) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_MUZZLE_07"), new wComponentData("WCT_MUZZ7", "WCD_MUZZ", 0, wComponentsType.Suppressor) },
					{ NAPI.Util.GetHashKey("COMPONENT_COMBATMG_MK2_CLIP_FMJ"), new wComponentData("WCT_CLIP_FMJ", "WCD_CLIP_FMJ", 0, wComponentsType.Clip) },
					{ NAPI.Util.GetHashKey("COMPONENT_COMBATMG_MK2_CAMO_04"), new wComponentData("WCT_CAMO_4", "", 0, wComponentsType.Varmod) },
					{ NAPI.Util.GetHashKey("COMPONENT_COMBATMG_MK2_CAMO_07"), new wComponentData("WCT_CAMO_7", "", 0, wComponentsType.Varmod) },
					{ NAPI.Util.GetHashKey("COMPONENT_COMBATMG_MK2_CAMO_06"), new wComponentData("WCT_CAMO_6", "", 0, wComponentsType.Varmod) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_AR_AFGRIP_02"), new wComponentData("WCT_GRIP", "WCD_GRIP", 0, wComponentsType.Grip2) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_MG_BARREL_02"), new wComponentData("WCT_BARR2", "WCD_BARR2", 0, wComponentsType.Barrel) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_MUZZLE_01"), new wComponentData("WCT_MUZZ1", "WCD_MUZZ", 0, wComponentsType.Suppressor) },
					{ NAPI.Util.GetHashKey("COMPONENT_COMBATMG_MK2_CAMO_03"), new wComponentData("WCT_CAMO_3", "", 0, wComponentsType.Varmod) },
					{ NAPI.Util.GetHashKey("COMPONENT_COMBATMG_MK2_CLIP_INCENDIARY"), new wComponentData("WCT_CLIP_INC", "WCD_CLIP_INC", 0, wComponentsType.Clip) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_MG_BARREL_01"), new wComponentData("WCT_BARR", "WCD_BARR", 0, wComponentsType.Barrel) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_SCOPE_MEDIUM_MK2"), new wComponentData("WCT_SCOPE_MED2", "WCD_SCOPE_MED", 0, wComponentsType.Scope2) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_MUZZLE_02"), new wComponentData("WCT_MUZZ2", "WCD_MUZZ", 0, wComponentsType.Suppressor) },
					{ NAPI.Util.GetHashKey("COMPONENT_COMBATMG_MK2_CAMO_02"), new wComponentData("WCT_CAMO_2", "", 0, wComponentsType.Varmod) },
					{ NAPI.Util.GetHashKey("COMPONENT_COMBATMG_MK2_CAMO_IND_01"), new wComponentData("WCT_CAMO_IND", "", 0, wComponentsType.Varmod) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_MUZZLE_03"), new wComponentData("WCT_MUZZ3", "WCD_MUZZ", 0, wComponentsType.Suppressor) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_MUZZLE_04"), new wComponentData("WCT_MUZZ4", "WCD_MUZZ", 0, wComponentsType.Suppressor) },
					{ NAPI.Util.GetHashKey("COMPONENT_COMBATMG_MK2_CLIP_TRACER"), new wComponentData("WCT_CLIP_TR", "WCD_CLIP_TR", 0, wComponentsType.Clip) },

				})
			},*/
			{ NAPI.Util.GetHashKey("WEAPON_MACHINEPISTOL"), new wComponentsData(1, new Dictionary<uint, wComponentData>()
				{
					//{ NAPI.Util.GetHashKey("COMPONENT_MACHINEPISTOL_CLIP_01"), new wComponentData("WCT_CLIP1", "WCD_MCHP_CLIP1", 0, wComponentsType.Clip) },
					//{ NAPI.Util.GetHashKey("COMPONENT_MACHINEPISTOL_CLIP_03"), new wComponentData("WCT_CLIP_DRM", "WCD_CLIP3", 0, wComponentsType.Clip) },
					//{ NAPI.Util.GetHashKey("COMPONENT_MACHINEPISTOL_CLIP_02"), new wComponentData("WCT_CLIP2", "WCD_MCHP_CLIP2", 0, wComponentsType.Clip) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_PI_SUPP"), new wComponentData("WCT_SUPP", "WCD_PI_SUPP", 900, wComponentsType.Suppressor) },

				})
			},
			/*{ NAPI.Util.GetHashKey("WEAPON_KNUCKLE"), new wComponentsData(1, new Dictionary<uint, wComponentData>()
				{
					{ NAPI.Util.GetHashKey("COMPONENT_KNUCKLE_VARMOD_PLAYER"), new wComponentData("WCT_KNUCK_PC", "WCD_VAR_DESC", 0, wComponentsType.Varmod) },
					{ NAPI.Util.GetHashKey("COMPONENT_KNUCKLE_VARMOD_LOVE"), new wComponentData("WCT_KNUCK_LV", "WCD_VAR_DESC", 0, wComponentsType.Varmod) },
					{ NAPI.Util.GetHashKey("COMPONENT_KNUCKLE_VARMOD_DOLLAR"), new wComponentData("WCT_KNUCK_DLR", "WCD_VAR_DESC", 0, wComponentsType.Varmod) },
					{ NAPI.Util.GetHashKey("COMPONENT_KNUCKLE_VARMOD_VAGOS"), new wComponentData("WCT_KNUCK_VG", "WCD_VAR_DESC", 0, wComponentsType.Varmod) },
					{ NAPI.Util.GetHashKey("COMPONENT_KNUCKLE_VARMOD_HATE"), new wComponentData("WCT_KNUCK_HT", "WCD_VAR_DESC", 0, wComponentsType.Varmod) },
					{ NAPI.Util.GetHashKey("COMPONENT_KNUCKLE_VARMOD_DIAMOND"), new wComponentData("WCT_KNUCK_DMD", "WCD_VAR_DESC", 0, wComponentsType.Varmod) },
					{ NAPI.Util.GetHashKey("COMPONENT_KNUCKLE_VARMOD_PIMP"), new wComponentData("WCT_KNUCK_02", "WCD_VAR_DESC", 0, wComponentsType.Varmod) },
					{ NAPI.Util.GetHashKey("COMPONENT_KNUCKLE_VARMOD_KING"), new wComponentData("WCT_KNUCK_SLG", "WCD_VAR_DESC", 0, wComponentsType.Varmod) },
					{ NAPI.Util.GetHashKey("COMPONENT_KNUCKLE_VARMOD_BALLAS"), new wComponentData("WCT_KNUCK_BG", "WCD_VAR_DESC", 0, wComponentsType.Varmod) },
					{ NAPI.Util.GetHashKey("COMPONENT_KNUCKLE_VARMOD_BASE"), new wComponentData("WCT_KNUCK_01", "WCD_VAR_DESC", 0, wComponentsType.Varmod) },

				})
			},
			{ NAPI.Util.GetHashKey("WEAPON_SNSPISTOL"), new wComponentsData(2, new Dictionary<uint, wComponentData>()
				{
					{ NAPI.Util.GetHashKey("COMPONENT_SNSPISTOL_CLIP_02"), new wComponentData("WCT_CLIP2", "WCD_SNSP_CLIP2", 0, wComponentsType.Clip) },
					{ NAPI.Util.GetHashKey("COMPONENT_SNSPISTOL_VARMOD_LOWRIDER"), new wComponentData("WCT_VAR_WOOD", "WCD_VAR_SNS", 0, wComponentsType.Varmod) },
					{ NAPI.Util.GetHashKey("COMPONENT_SNSPISTOL_CLIP_01"), new wComponentData("WCT_CLIP1", "WCD_SNSP_CLIP1", 0, wComponentsType.Clip) },

				})
			},*/
			{ NAPI.Util.GetHashKey("WEAPON_HEAVYPISTOL"), new wComponentsData(3, new Dictionary<uint, wComponentData>()
				{
					//{ NAPI.Util.GetHashKey("COMPONENT_HEAVYPISTOL_CLIP_01"), new wComponentData("WCT_CLIP1", "WCD_HPST_CLIP1", 0, wComponentsType.Clip) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_PI_FLSH"), new wComponentData("WCT_FLASH", "WCD_FLASH", 900, wComponentsType.Flashlight) },
					//{ NAPI.Util.GetHashKey("COMPONENT_HEAVYPISTOL_CLIP_02"), new wComponentData("WCT_CLIP2", "WCD_HPST_CLIP2", 0, wComponentsType.Clip) },
					{ NAPI.Util.GetHashKey("COMPONENT_HEAVYPISTOL_VARMOD_LUXE"), new wComponentData("WCT_VAR_WOOD", "WCD_VAR_HPST", 1000, wComponentsType.Varmod) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_PI_SUPP"), new wComponentData("WCT_SUPP", "WCD_PI_SUPP", 1100, wComponentsType.Suppressor) },

				})
			},
			/*{ NAPI.Util.GetHashKey("WEAPON_SPECIALCARBINE"), new wComponentsData(5, new Dictionary<uint, wComponentData>()
				{
					{ NAPI.Util.GetHashKey("COMPONENT_AT_AR_AFGRIP"), new wComponentData("WCT_GRIP", "WCD_GRIP", 0, wComponentsType.Invalid) },
					{ NAPI.Util.GetHashKey("COMPONENT_SPECIALCARBINE_CLIP_03"), new wComponentData("WCT_CLIP_DRM", "WCD_CLIP3", 0, wComponentsType.Clip) },
					{ NAPI.Util.GetHashKey("COMPONENT_SPECIALCARBINE_VARMOD_LOWRIDER"), new wComponentData("WCT_VAR_ETCHM", "WCD_VAR_SCAR", 0, wComponentsType.Varmod) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_AR_FLSH"), new wComponentData("WCT_FLASH", "WCD_FLASH", 0, wComponentsType.Invalid) },
					{ NAPI.Util.GetHashKey("COMPONENT_SPECIALCARBINE_CLIP_02"), new wComponentData("WCT_CLIP2", "WCD_SCRB_CLIP2", 0, wComponentsType.Clip) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_SCOPE_MEDIUM"), new wComponentData("WCT_SCOPE_LRG", "WCD_SCOPE_LRG", 0, wComponentsType.Scope) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_AR_SUPP_02"), new wComponentData("WCT_SUPP", "WCD_AR_SUPP2", 0, wComponentsType.Suppressor) },
					{ NAPI.Util.GetHashKey("COMPONENT_SPECIALCARBINE_CLIP_01"), new wComponentData("WCT_CLIP1", "WCD_SCRB_CLIP1", 0, wComponentsType.Clip) },

				})
			},
			{ NAPI.Util.GetHashKey("WEAPON_MUSKET"), new wComponentsData(1, new Dictionary<uint, wComponentData>()
				{
					{ NAPI.Util.GetHashKey("COMPONENT_MUSKET_CLIP_01"), new wComponentData("WCT_CLIP1", "WCD_MSKT_CLIP1", 0, wComponentsType.Invalid) },

				})
			},*/
			{ NAPI.Util.GetHashKey("WEAPON_MARKSMANRIFLE"), new wComponentsData(6, new Dictionary<uint, wComponentData>()
				{
					{ NAPI.Util.GetHashKey("COMPONENT_AT_AR_AFGRIP"), new wComponentData("WCT_GRIP", "WCD_GRIP", 9000, wComponentsType.Grip) },
					{ NAPI.Util.GetHashKey("COMPONENT_MARKSMANRIFLE_VARMOD_LUXE"), new wComponentData("WCT_VAR_GOLD", "WCD_VAR_MKRF", 35000, wComponentsType.Varmod) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_SCOPE_LARGE_FIXED_ZOOM"), new wComponentData("WCT_SCOPE_LRG", "WCD_SCOPE_LRF", 16000, wComponentsType.Scope) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_AR_FLSH"), new wComponentData("WCT_FLASH", "WCD_FLASH", 11000, wComponentsType.Flashlight) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_AR_SUPP"), new wComponentData("WCT_SUPP", "WCD_AR_SUPP", 14000, wComponentsType.Suppressor) },
					//{ NAPI.Util.GetHashKey("COMPONENT_MARKSMANRIFLE_CLIP_02"), new wComponentData("WCT_CLIP2", "WCD_MKRF_CLIP2", 0, wComponentsType.Clip) },
					//{ NAPI.Util.GetHashKey("COMPONENT_MARKSMANRIFLE_CLIP_01"), new wComponentData("WCT_CLIP1", "WCD_MKRF_CLIP1", 0, wComponentsType.Clip) },

				})
			},
			{ NAPI.Util.GetHashKey("WEAPON_PISTOL50"), new wComponentsData(3, new Dictionary<uint, wComponentData>()
				{
					//{ NAPI.Util.GetHashKey("COMPONENT_PISTOL50_CLIP_01"), new wComponentData("WCT_CLIP1", "WCD_P50_CLIP1", 0, wComponentsType.Clip) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_PI_FLSH"), new wComponentData("WCT_FLASH", "WCD_FLASH", 400, wComponentsType.Flashlight) },
					{ NAPI.Util.GetHashKey("COMPONENT_PISTOL50_VARMOD_LUXE"), new wComponentData("WCT_VAR_SIL", "WCD_VAR_SIL", 500, wComponentsType.Varmod) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_AR_SUPP_02"), new wComponentData("WCT_SUPP", "WCD_AR_SUPP2", 600, wComponentsType.Suppressor) },
					//{ NAPI.Util.GetHashKey("COMPONENT_PISTOL50_CLIP_02"), new wComponentData("WCT_CLIP2", "WCD_P50_CLIP2", 0, wComponentsType.Clip) },

				})
			},
			/*{ NAPI.Util.GetHashKey("WEAPON_ASSAULTSMG"), new wComponentsData(5, new Dictionary<uint, wComponentData>()
				{
					{ NAPI.Util.GetHashKey("COMPONENT_ASSAULTSMG_VARMOD_LOWRIDER"), new wComponentData("WCT_VAR_GOLD", "WCD_VAR_GOLD", 0, wComponentsType.Varmod) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_AR_FLSH"), new wComponentData("WCT_FLASH", "WCD_FLASH", 0, wComponentsType.Invalid) },
					{ NAPI.Util.GetHashKey("COMPONENT_ASSAULTSMG_CLIP_01"), new wComponentData("WCT_CLIP1", "WCD_INVALID", 0, wComponentsType.Clip) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_SCOPE_MACRO"), new wComponentData("WCT_SCOPE_MAC", "WCD_SCOPE_MAC", 0, wComponentsType.Scope) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_AR_SUPP_02"), new wComponentData("WCT_SUPP", "WCD_AR_SUPP2", 0, wComponentsType.Suppressor) },
					{ NAPI.Util.GetHashKey("COMPONENT_ASSAULTSMG_CLIP_02"), new wComponentData("WCT_CLIP2", "WCD_INVALID", 0, wComponentsType.Clip) },

				})
			},*/
			{ NAPI.Util.GetHashKey("WEAPON_ASSAULTRIFLE"), new wComponentsData(5, new Dictionary<uint, wComponentData>()
				{
					{ NAPI.Util.GetHashKey("COMPONENT_AT_AR_AFGRIP"), new wComponentData("WCT_GRIP", "WCD_GRIP", 1200, wComponentsType.Grip) }, //Invalid
					{ NAPI.Util.GetHashKey("COMPONENT_ASSAULTRIFLE_VARMOD_LUXE"), new wComponentData("WCT_VAR_GOLD", "WCD_VAR_GOLD", 2000, wComponentsType.Varmod) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_AR_FLSH"), new wComponentData("WCT_FLASH", "WCD_FLASH", 900, wComponentsType.Flashlight) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_SCOPE_MACRO"), new wComponentData("WCT_SCOPE_MAC", "WCD_SCOPE_MAC", 1400, wComponentsType.Scope) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_AR_SUPP_02"), new wComponentData("WCT_SUPP", "WCD_AR_SUPP2", 1100, wComponentsType.Suppressor) },
					//{ NAPI.Util.GetHashKey("COMPONENT_ASSAULTRIFLE_CLIP_02"), new wComponentData("WCT_CLIP2", "WCD_AR_CLIP2", 0, wComponentsType.Clip) },
					//{ NAPI.Util.GetHashKey("COMPONENT_ASSAULTRIFLE_CLIP_01"), new wComponentData("WCT_CLIP1", "WCD_AR_CLIP1", 0, wComponentsType.Clip) },
					//{ NAPI.Util.GetHashKey("COMPONENT_ASSAULTRIFLE_CLIP_03"), new wComponentData("WCT_CLIP_DRM", "WCD_CLIP_DRM", 0, wComponentsType.Clip) },

				})
			},
			{ NAPI.Util.GetHashKey("WEAPON_CARBINERIFLE"), new wComponentsData(5, new Dictionary<uint, wComponentData>()
				{
					{ NAPI.Util.GetHashKey("COMPONENT_AT_AR_AFGRIP"), new wComponentData("WCT_GRIP", "WCD_GRIP", 1300, wComponentsType.Grip) },
					//{ NAPI.Util.GetHashKey("COMPONENT_AT_RAILCOVER_01"), new wComponentData("WCT_RAIL", "WCD_AT_RAIL", 2, wComponentsType.Rail) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_AR_FLSH"), new wComponentData("WCT_FLASH", "WCD_FLASH", 900, wComponentsType.Flashlight) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_AR_SUPP"), new wComponentData("WCT_SUPP", "WCD_AR_SUPP", 1100, wComponentsType.Suppressor) },
					//{ NAPI.Util.GetHashKey("COMPONENT_CARBINERIFLE_CLIP_02"), new wComponentData("WCT_CLIP2", "WCD_CR_CLIP2", 0, wComponentsType.Clip) },
					//{ NAPI.Util.GetHashKey("COMPONENT_CARBINERIFLE_CLIP_01"), new wComponentData("WCT_CLIP1", "WCD_CR_CLIP1", 0, wComponentsType.Clip) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_SCOPE_MEDIUM"), new wComponentData("WCT_SCOPE_LRG", "WCD_SCOPE_LRG", 1400, wComponentsType.Scope) },
					//{ NAPI.Util.GetHashKey("COMPONENT_CARBINERIFLE_CLIP_03"), new wComponentData("WCT_CLIP_BOX", "WCD_CLIP_BOX", 0, wComponentsType.Clip) },
					{ NAPI.Util.GetHashKey("COMPONENT_CARBINERIFLE_VARMOD_LUXE"), new wComponentData("WCT_VAR_GOLD", "WCD_VAR_GOLD", 2000, wComponentsType.Varmod) },

				})
			},
			{ NAPI.Util.GetHashKey("WEAPON_ADVANCEDRIFLE"), new wComponentsData(4, new Dictionary<uint, wComponentData>()
				{
					{ NAPI.Util.GetHashKey("COMPONENT_ADVANCEDRIFLE_VARMOD_LUXE"), new wComponentData("WCT_VAR_METAL", "WCD_VAR_METAL", 2000, wComponentsType.Varmod) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_AR_FLSH"), new wComponentData("WCT_FLASH", "WCD_FLASH", 900, wComponentsType.Flashlight) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_AR_SUPP"), new wComponentData("WCT_SUPP", "WCD_AR_SUPP", 1100, wComponentsType.Suppressor) },
					//{ NAPI.Util.GetHashKey("COMPONENT_ADVANCEDRIFLE_CLIP_02"), new wComponentData("WCT_CLIP2", "WCD_AR_CLIP2", 0, wComponentsType.Clip) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_SCOPE_SMALL"), new wComponentData("WCT_SCOPE_SML", "WCD_SCOPE_SML", 1400, wComponentsType.Scope) },
					//{ NAPI.Util.GetHashKey("COMPONENT_ADVANCEDRIFLE_CLIP_01"), new wComponentData("WCT_CLIP1", "WCD_AR_CLIP1", 0, wComponentsType.Clip) },

				})
			},
			/*{ NAPI.Util.GetHashKey("WEAPON_MG"), new wComponentsData(4, new Dictionary<uint, wComponentData>()
				{
					{ NAPI.Util.GetHashKey("COMPONENT_AT_AR_AFGRIP"), new wComponentData("WCT_GRIP", "WCD_GRIP", 0, wComponentsType.Invalid) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_SCOPE_SMALL_02"), new wComponentData("WCT_SCOPE_SML", "WCD_SCOPE_SML", 0, wComponentsType.Scope) },
					{ NAPI.Util.GetHashKey("COMPONENT_MG_CLIP_02"), new wComponentData("WCT_CLIP2", "WCD_MG_CLIP2", 0, wComponentsType.Clip) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_SCOPE_SMALL"), new wComponentData("WCT_SCOPE_SML", "WCD_SCOPE_SML", 0, wComponentsType.Invalid) },
					{ NAPI.Util.GetHashKey("COMPONENT_MG_VARMOD_LOWRIDER"), new wComponentData("WCT_VAR_GOLD", "WCD_VAR_GOLD", 0, wComponentsType.Varmod) },
					{ NAPI.Util.GetHashKey("COMPONENT_MG_CLIP_01"), new wComponentData("WCT_CLIP1", "WCD_MG_CLIP1", 0, wComponentsType.Clip) },

				})
			},
			{ NAPI.Util.GetHashKey("WEAPON_ASSAULTSHOTGUN"), new wComponentsData(3, new Dictionary<uint, wComponentData>()
				{
					{ NAPI.Util.GetHashKey("COMPONENT_AT_AR_AFGRIP"), new wComponentData("WCT_GRIP", "WCD_GRIP", 0, wComponentsType.Invalid) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_AR_FLSH"), new wComponentData("WCT_FLASH", "WCD_FLASH", 0, wComponentsType.Invalid) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_AR_SUPP"), new wComponentData("WCT_SUPP", "WCD_AR_SUPP", 0, wComponentsType.Suppressor) },
					{ NAPI.Util.GetHashKey("COMPONENT_ASSAULTSHOTGUN_CLIP_02"), new wComponentData("WCT_CLIP2", "WCD_AS_CLIP2", 0, wComponentsType.Clip) },
					{ NAPI.Util.GetHashKey("COMPONENT_ASSAULTSHOTGUN_CLIP_01"), new wComponentData("WCT_CLIP1", "WCD_AS_CLIP1", 0, wComponentsType.Clip) },

				})
			},*/
			{ NAPI.Util.GetHashKey("WEAPON_BULLPUPSHOTGUN"), new wComponentsData(3, new Dictionary<uint, wComponentData>()
				{
					{ NAPI.Util.GetHashKey("COMPONENT_AT_AR_AFGRIP"), new wComponentData("WCT_GRIP", "WCD_GRIP", 400, wComponentsType.Grip) }, //Invalid
					{ NAPI.Util.GetHashKey("COMPONENT_AT_AR_FLSH"), new wComponentData("WCT_FLASH", "WCD_FLASH", 600, wComponentsType.Flashlight) }, //Invalid
					{ NAPI.Util.GetHashKey("COMPONENT_AT_AR_SUPP_02"), new wComponentData("WCT_SUPP", "WCD_AR_SUPP2", 800, wComponentsType.Suppressor) },
					//{ NAPI.Util.GetHashKey("COMPONENT_BULLPUPSHOTGUN_CLIP_01"), new wComponentData("WCT_INVALID", "WCD_INVALID", 0, wComponentsType.Clip) },

				})
			},
			{ NAPI.Util.GetHashKey("WEAPON_PUMPSHOTGUN_MK2"), new wComponentsData(5, new Dictionary<uint, wComponentData>()
				{
					//{ NAPI.Util.GetHashKey("COMPONENT_PUMPSHOTGUN_MK2_CLIP_01"), new wComponentData("WCT_SHELL", "WCD_SHELL", 0, wComponentsType.Clip) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_SIGHTS"), new wComponentData("WCT_HOLO", "WCD_HOLO", 1200, wComponentsType.Scope) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_SCOPE_MACRO_MK2"), new wComponentData("WCT_SCOPE_MAC2", "WCD_SCOPE_MAC", 1200, wComponentsType.Scope) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_SCOPE_SMALL_MK2"), new wComponentData("WCT_SCOPE_SML2", "WCD_SCOPE_SML", 1200, wComponentsType.Scope) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_AR_FLSH"), new wComponentData("WCT_FLASH", "WCD_FLASH", 600, wComponentsType.Flashlight) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_SR_SUPP_03"), new wComponentData("WCT_SUPP", "WCD_SR_SUPP", 800, wComponentsType.Suppressor) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_MUZZLE_08"), new wComponentData("WCT_MUZZ8", "WCD_MUZZ_SR", 1800, wComponentsType.Muzzlebrake) },
					{ NAPI.Util.GetHashKey("COMPONENT_PUMPSHOTGUN_MK2_CAMO"), new wComponentData("WCT_CAMO_1", "WCD_INVALID", 1100, wComponentsType.Camo) },
					{ NAPI.Util.GetHashKey("COMPONENT_PUMPSHOTGUN_MK2_CAMO_02"), new wComponentData("WCT_CAMO_2", "WCD_INVALID", 1100, wComponentsType.Camo) },
					{ NAPI.Util.GetHashKey("COMPONENT_PUMPSHOTGUN_MK2_CAMO_03"), new wComponentData("WCT_CAMO_3", "WCD_INVALID", 1100, wComponentsType.Camo) },
					{ NAPI.Util.GetHashKey("COMPONENT_PUMPSHOTGUN_MK2_CAMO_04"), new wComponentData("WCT_CAMO_4", "WCD_INVALID", 1100, wComponentsType.Camo) },
					{ NAPI.Util.GetHashKey("COMPONENT_PUMPSHOTGUN_MK2_CAMO_05"), new wComponentData("WCT_CAMO_5", "WCD_INVALID", 1100, wComponentsType.Camo) },
					{ NAPI.Util.GetHashKey("COMPONENT_PUMPSHOTGUN_MK2_CAMO_06"), new wComponentData("WCT_CAMO_6", "WCD_INVALID", 1100, wComponentsType.Camo) },
					{ NAPI.Util.GetHashKey("COMPONENT_PUMPSHOTGUN_MK2_CAMO_07"), new wComponentData("WCT_CAMO_7", "WCD_INVALID", 1100, wComponentsType.Camo) },
					{ NAPI.Util.GetHashKey("COMPONENT_PUMPSHOTGUN_MK2_CAMO_08"), new wComponentData("WCT_CAMO_8", "WCD_INVALID", 1100, wComponentsType.Camo) },
					{ NAPI.Util.GetHashKey("COMPONENT_PUMPSHOTGUN_MK2_CAMO_09"), new wComponentData("WCT_CAMO_9", "WCD_INVALID", 1100, wComponentsType.Camo) },
					{ NAPI.Util.GetHashKey("COMPONENT_PUMPSHOTGUN_MK2_CAMO_10"), new wComponentData("WCT_CAMO_10", "WCD_INVALID", 1100, wComponentsType.Camo) },
					{ NAPI.Util.GetHashKey("COMPONENT_PUMPSHOTGUN_MK2_CAMO_IND_01"), new wComponentData("WCT_CAMO_IND", "WCD_INVALID", 1100, wComponentsType.Camo) },

				})
			},
			{ NAPI.Util.GetHashKey("WEAPON_SMG_MK2"), new wComponentsData(6, new Dictionary<uint, wComponentData>()
				{
					//{ NAPI.Util.GetHashKey("COMPONENT_SMG_MK2_CLIP_01"), new wComponentData("WCT_CLIP1", "WCD_CLIP1", 0, wComponentsType.Clip) },
					//{ NAPI.Util.GetHashKey("COMPONENT_SMG_MK2_CLIP_02"), new wComponentData("WCT_CLIP2", "WCD_CLIP2", 0, wComponentsType.Clip) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_AR_FLSH"), new wComponentData("WCT_FLASH", "WCD_FLASH", 1200, wComponentsType.Flashlight) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_SIGHTS_SMG"), new wComponentData("WCT_HOLO", "WCD_HOLO", 1500, wComponentsType.Scope) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_SCOPE_MACRO_02_SMG_MK2"), new wComponentData("WCT_SCOPE_MAC2", "WCD_SCOPE_MAC", 1500, wComponentsType.Scope) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_SCOPE_SMALL_SMG_MK2"), new wComponentData("WCT_SCOPE_SML2", "WCD_SCOPE_SML", 1500, wComponentsType.Scope) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_PI_SUPP"), new wComponentData("WCT_SUPP", "WCD_PI_SUPP", 1000, wComponentsType.Suppressor) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_MUZZLE_01"), new wComponentData("WCT_MUZZ1", "WCD_MUZZ", 1800, wComponentsType.Muzzlebrake) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_MUZZLE_02"), new wComponentData("WCT_MUZZ2", "WCD_MUZZ", 1800, wComponentsType.Muzzlebrake) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_MUZZLE_03"), new wComponentData("WCT_MUZZ3", "WCD_MUZZ", 1800, wComponentsType.Muzzlebrake) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_MUZZLE_04"), new wComponentData("WCT_MUZZ4", "WCD_MUZZ", 1800, wComponentsType.Muzzlebrake) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_MUZZLE_05"), new wComponentData("WCT_MUZZ5", "WCD_MUZZ", 1800, wComponentsType.Muzzlebrake) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_MUZZLE_06"), new wComponentData("WCT_MUZZ6", "WCD_MUZZ", 1800, wComponentsType.Muzzlebrake) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_MUZZLE_07"), new wComponentData("WCT_MUZZ7", "WCD_MUZZ", 1800, wComponentsType.Muzzlebrake) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_SB_BARREL_01"), new wComponentData("WCT_BARR", "WCD_BARR", 2100, wComponentsType.Barrel) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_SB_BARREL_02"), new wComponentData("WCT_BARR2", "WCD_BARR2", 2100, wComponentsType.Barrel) },
					{ NAPI.Util.GetHashKey("COMPONENT_SMG_MK2_CAMO"), new wComponentData("WCT_CAMO_1", "WCD_INVALID", 1100, wComponentsType.Camo) },
					{ NAPI.Util.GetHashKey("COMPONENT_SMG_MK2_CAMO_02"), new wComponentData("WCT_CAMO_2", "WCD_INVALID", 1100, wComponentsType.Camo) },
					{ NAPI.Util.GetHashKey("COMPONENT_SMG_MK2_CAMO_03"), new wComponentData("WCT_CAMO_3", "WCD_INVALID", 1100, wComponentsType.Camo) },
					{ NAPI.Util.GetHashKey("COMPONENT_SMG_MK2_CAMO_04"), new wComponentData("WCT_CAMO_4", "WCD_INVALID", 1100, wComponentsType.Camo) },
					{ NAPI.Util.GetHashKey("COMPONENT_SMG_MK2_CAMO_05"), new wComponentData("WCT_CAMO_5", "WCD_INVALID", 1100, wComponentsType.Camo) },
					{ NAPI.Util.GetHashKey("COMPONENT_SMG_MK2_CAMO_06"), new wComponentData("WCT_CAMO_6", "WCD_INVALID", 1100, wComponentsType.Camo) },
					{ NAPI.Util.GetHashKey("COMPONENT_SMG_MK2_CAMO_07"), new wComponentData("WCT_CAMO_7", "WCD_INVALID", 1100, wComponentsType.Camo) },
					{ NAPI.Util.GetHashKey("COMPONENT_SMG_MK2_CAMO_08"), new wComponentData("WCT_CAMO_8", "WCD_INVALID", 1100, wComponentsType.Camo) },
					{ NAPI.Util.GetHashKey("COMPONENT_SMG_MK2_CAMO_09"), new wComponentData("WCT_CAMO_9", "WCD_INVALID", 1100, wComponentsType.Camo) },
					{ NAPI.Util.GetHashKey("COMPONENT_SMG_MK2_CAMO_10"), new wComponentData("WCT_CAMO_10", "WCD_INVALID", 1100, wComponentsType.Camo) },
					{ NAPI.Util.GetHashKey("COMPONENT_SMG_MK2_CAMO_IND_01"), new wComponentData("WCT_CAMO_IND", "WCD_INVALID", 1100, wComponentsType.Camo) },

				})
			},
			/*{ NAPI.Util.GetHashKey("WEAPON_DBSHOTGUN"), new wComponentsData(1, new Dictionary<uint, wComponentData>()
				{
					//{ NAPI.Util.GetHashKey("COMPONENT_DBSHOTGUN_CLIP_01"), new wComponentData("WCT_INVALID", "WCD_INVALID", 0, wComponentsType.Clip) },

				})
			},*/
			/*{ NAPI.Util.GetHashKey("WEAPON_GRENADELAUNCHER"), new wComponentsData(2, new Dictionary<uint, wComponentData>()
				{
					{ NAPI.Util.GetHashKey("COMPONENT_AT_AR_AFGRIP"), new wComponentData("WCT_GRIP", "WCD_GRIP", 0, wComponentsType.Invalid) },
					{ NAPI.Util.GetHashKey("COMPONENT_GRENADELAUNCHER_CLIP_01"), new wComponentData("WCT_INVALID", "WCD_INVALID", 0, wComponentsType.Grip) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_AR_FLSH"), new wComponentData("WCT_FLASH", "WCD_FLASH", 0, wComponentsType.Invalid) },
					{ NAPI.Util.GetHashKey("COMPONENT_AT_SCOPE_SMALL"), new wComponentData("WCT_SCOPE_SML", "WCD_SCOPE_SML", 0, wComponentsType.Invalid) },

				})
			},
			{ NAPI.Util.GetHashKey("WEAPON_RPG"), new wComponentsData(1, new Dictionary<uint, wComponentData>()
				{
					{ NAPI.Util.GetHashKey("COMPONENT_RPG_CLIP_01"), new wComponentData("WCT_INVALID", "WCD_INVALID", 0, wComponentsType.Invalid) },

				})
			},*/
		};

		public static void Give(ExtPlayer player, uint weaponHash, string locationName, string Location)
        {
			try
			{
				if (!player.IsCharacterData()) return;
				else if (!WeaponsComponents.ContainsKey(weaponHash)) return;
				if (Repository.ItemsData.ContainsKey(locationName) && Repository.ItemsData[locationName].ContainsKey(Location) && Repository.ItemsData[locationName][Location].Count > 0)
				{
					Dictionary<uint, wComponentData> Components = WeaponsComponents[weaponHash].Components;
					List<uint> _JsonInventoryItemData = new List<uint>();

					foreach (InventoryItemData item in Repository.ItemsData[locationName][Location].Values)//Todo
					{
						if (item.ItemId != ItemId.Debug && item.Data != null && item.Data.Split('_').Length > 0 && Components.ContainsKey(Convert.ToUInt32(item.Data.Split('_')[1])))
						{
							_JsonInventoryItemData.Add(Convert.ToUInt32(item.Data.Split('_')[1]));
						}
					}
					player.SetSharedData("weaponComponents", $"{weaponHash}|" + JsonConvert.SerializeObject(_JsonInventoryItemData));
				}
			}
			catch (Exception e)
			{
				Log.Write($"Give Exception: {e.ToString()}");
			}
        }
        public static void Remove(ExtPlayer player)
        {
			try
			{
				if (!player.IsCharacterData()) return;
				player.SetSharedData("weaponComponents", "null");
			}
			catch (Exception e)
			{
				Log.Write($"Remove Exception: {e.ToString()}");
			}
        }
	}
}
