const CONTROL = {
	INPUT_NEXT_CAMERA: 0,
	INPUT_LOOK_LR: 1,
	INPUT_LOOK_UD: 2,
	INPUT_LOOK_UP_ONLY: 3,
	INPUT_LOOK_DOWN_ONLY: 4,
	INPUT_LOOK_LEFT_ONLY: 5,
	INPUT_LOOK_RIGHT_ONLY: 6,
	INPUT_CINEMATIC_SLOWMO: 7,
	INPUT_SCRIPTED_FLY_UD: 8,
	INPUT_SCRIPTED_FLY_LR: 9,
	INPUT_SCRIPTED_FLY_ZUP: 10,
	INPUT_SCRIPTED_FLY_ZDOWN: 11,
	INPUT_WEAPON_WHEEL_UD: 12,
	INPUT_WEAPON_WHEEL_LR: 13,
	INPUT_WEAPON_WHEEL_NEXT: 14,
	INPUT_WEAPON_WHEEL_PREV: 15,
	INPUT_SELECT_NEXT_WEAPON: 16,
	INPUT_SELECT_PREV_WEAPON: 17,
	INPUT_SKIP_CUTSCENE: 18,
	INPUT_CHARACTER_WHEEL: 19,
	INPUT_MULTIPLAYER_INFO: 20,
	INPUT_SPRINT: 21,
	INPUT_JUMP: 22,
	INPUT_ENTER: 23,
	INPUT_ATTACK: 24,
	INPUT_AIM: 25,
	INPUT_LOOK_BEHIND: 26,
	INPUT_PHONE: 27,
	INPUT_SPECIAL_ABILITY: 28,
	INPUT_SPECIAL_ABILITY_SECONDARY: 29,
	INPUT_MOVE_LR: 30,
	INPUT_MOVE_UD: 31,
	INPUT_MOVE_UP_ONLY: 32,
	INPUT_MOVE_DOWN_ONLY: 33,
	INPUT_MOVE_LEFT_ONLY: 34,
	INPUT_MOVE_RIGHT_ONLY: 35,
	INPUT_DUCK: 36,
	INPUT_SELECT_WEAPON: 37,
	INPUT_PICKUP: 38,
	INPUT_SNIPER_ZOOM: 39,
	INPUT_SNIPER_ZOOM_IN_ONLY: 40,
	INPUT_SNIPER_ZOOM_OUT_ONLY: 41,
	INPUT_SNIPER_ZOOM_IN_SECONDARY: 42,
	INPUT_SNIPER_ZOOM_OUT_SECONDARY: 43,
	INPUT_COVER: 44,
	INPUT_RELOAD: 45,
	INPUT_TALK: 46,
	INPUT_DETONATE: 47,
	INPUT_HUD_SPECIAL: 48,
	INPUT_ARREST: 49,
	INPUT_ACCURATE_AIM: 50,
	INPUT_CONTEXT: 51,
	INPUT_CONTEXT_SECONDARY: 52,
	INPUT_WEAPON_SPECIAL: 53,
	INPUT_WEAPON_SPECIAL_TWO: 54,
	INPUT_DIVE: 55,
	INPUT_DROP_WEAPON: 56,
	INPUT_DROP_AMMO: 57,
	INPUT_THROW_GRENADE: 58,
	INPUT_VEH_MOVE_LR: 59,
	INPUT_VEH_MOVE_UD: 60,
	INPUT_VEH_MOVE_UP_ONLY: 61,
	INPUT_VEH_MOVE_DOWN_ONLY: 62,
	INPUT_VEH_MOVE_LEFT_ONLY: 63,
	INPUT_VEH_MOVE_RIGHT_ONLY: 64,
	INPUT_VEH_SPECIAL: 65,
	INPUT_VEH_GUN_LR: 66,
	INPUT_VEH_GUN_UD: 67,
	INPUT_VEH_AIM: 68,
	INPUT_VEH_ATTACK: 69,
	INPUT_VEH_ATTACK2: 70,
	INPUT_VEH_ACCELERATE: 71,
	INPUT_VEH_BRAKE: 72,
	INPUT_VEH_DUCK: 73,
	INPUT_VEH_HEADLIGHT: 74,
	INPUT_VEH_EXIT: 75,
	INPUT_VEH_HANDBRAKE: 76,
	INPUT_VEH_HOTWIRE_LEFT: 77,
	INPUT_VEH_HOTWIRE_RIGHT: 78,
	INPUT_VEH_LOOK_BEHIND: 79,
	INPUT_VEH_CIN_CAM: 80,
	INPUT_VEH_NEXT_RADIO: 81,
	INPUT_VEH_PREV_RADIO: 82,
	INPUT_VEH_NEXT_RADIO_TRACK: 83,
	INPUT_VEH_PREV_RADIO_TRACK: 84,
	INPUT_VEH_RADIO_WHEEL: 85,
	INPUT_VEH_HORN: 86,
	INPUT_VEH_FLY_THROTTLE_UP: 87,
	INPUT_VEH_FLY_THROTTLE_DOWN: 88,
	INPUT_VEH_FLY_YAW_LEFT: 89,
	INPUT_VEH_FLY_YAW_RIGHT: 90,
	INPUT_VEH_PASSENGER_AIM: 91,
	INPUT_VEH_PASSENGER_ATTACK: 92,
	INPUT_VEH_SPECIAL_ABILITY_FRANKLIN: 93,
	INPUT_VEH_STUNT_UD: 94,
	INPUT_VEH_CINEMATIC_UD: 95,
	INPUT_VEH_CINEMATIC_UP_ONLY: 96,
	INPUT_VEH_CINEMATIC_DOWN_ONLY: 97,
	INPUT_VEH_CINEMATIC_LR: 98,
	INPUT_VEH_SELECT_NEXT_WEAPON: 99,
	INPUT_VEH_SELECT_PREV_WEAPON: 100,
	INPUT_VEH_ROOF: 101,
	INPUT_VEH_JUMP: 102,
	INPUT_VEH_GRAPPLING_HOOK: 103,
	INPUT_VEH_SHUFFLE: 104,
	INPUT_VEH_DROP_PROJECTILE: 105,
	INPUT_VEH_MOUSE_CONTROL_OVERRIDE: 106,
	INPUT_VEH_FLY_ROLL_LR: 107,
	INPUT_VEH_FLY_ROLL_LEFT_ONLY: 108,
	INPUT_VEH_FLY_ROLL_RIGHT_ONLY: 109,
	INPUT_VEH_FLY_PITCH_UD: 110,
	INPUT_VEH_FLY_PITCH_UP_ONLY: 111,
	INPUT_VEH_FLY_PITCH_DOWN_ONLY: 112,
	INPUT_VEH_FLY_UNDERCARRIAGE: 113,
	INPUT_VEH_FLY_ATTACK: 114,
	INPUT_VEH_FLY_SELECT_NEXT_WEAPON: 115,
	INPUT_VEH_FLY_SELECT_PREV_WEAPON: 116,
	INPUT_VEH_FLY_SELECT_TARGET_LEFT: 117,
	INPUT_VEH_FLY_SELECT_TARGET_RIGHT: 118,
	INPUT_VEH_FLY_VERTICAL_FLIGHT_MODE: 119,
	INPUT_VEH_FLY_DUCK: 120,
	INPUT_VEH_FLY_ATTACK_CAMERA: 121,
	INPUT_VEH_FLY_MOUSE_CONTROL_OVERRIDE: 122,
	INPUT_VEH_SUB_TURN_LR: 123,
	INPUT_VEH_SUB_TURN_LEFT_ONLY: 124,
	INPUT_VEH_SUB_TURN_RIGHT_ONLY: 125,
	INPUT_VEH_SUB_PITCH_UD: 126,
	INPUT_VEH_SUB_PITCH_UP_ONLY: 127,
	INPUT_VEH_SUB_PITCH_DOWN_ONLY: 128,
	INPUT_VEH_SUB_THROTTLE_UP: 129,
	INPUT_VEH_SUB_THROTTLE_DOWN: 130,
	INPUT_VEH_SUB_ASCEND: 131,
	INPUT_VEH_SUB_DESCEND: 132,
	INPUT_VEH_SUB_TURN_HARD_LEFT: 133,
	INPUT_VEH_SUB_TURN_HARD_RIGHT: 134,
	INPUT_VEH_SUB_MOUSE_CONTROL_OVERRIDE: 135,
	INPUT_VEH_PUSHBIKE_PEDAL: 136,
	INPUT_VEH_PUSHBIKE_SPRINT: 137,
	INPUT_VEH_PUSHBIKE_FRONT_BRAKE: 138,
	INPUT_VEH_PUSHBIKE_REAR_BRAKE: 139,
	INPUT_MELEE_ATTACK_LIGHT: 140,
	INPUT_MELEE_ATTACK_HEAVY: 141,
	INPUT_MELEE_ATTACK_ALTERNATE: 142,
	INPUT_MELEE_BLOCK: 143,
	INPUT_PARACHUTE_DEPLOY: 144,
	INPUT_PARACHUTE_DETACH: 145,
	INPUT_PARACHUTE_TURN_LR: 146,
	INPUT_PARACHUTE_TURN_LEFT_ONLY: 147,
	INPUT_PARACHUTE_TURN_RIGHT_ONLY: 148,
	INPUT_PARACHUTE_PITCH_UD: 149,
	INPUT_PARACHUTE_PITCH_UP_ONLY: 150,
	INPUT_PARACHUTE_PITCH_DOWN_ONLY: 151,
	INPUT_PARACHUTE_BRAKE_LEFT: 152,
	INPUT_PARACHUTE_BRAKE_RIGHT: 153,
	INPUT_PARACHUTE_SMOKE: 154,
	INPUT_PARACHUTE_PRECISION_LANDING: 155,
	INPUT_MAP: 156,
	INPUT_SELECT_WEAPON_UNARMED: 157,
	INPUT_SELECT_WEAPON_MELEE: 158,
	INPUT_SELECT_WEAPON_HANDGUN: 159,
	INPUT_SELECT_WEAPON_SHOTGUN: 160,
	INPUT_SELECT_WEAPON_SMG: 161,
	INPUT_SELECT_WEAPON_AUTO_RIFLE: 162,
	INPUT_SELECT_WEAPON_SNIPER: 163,
	INPUT_SELECT_WEAPON_HEAVY: 164,
	INPUT_SELECT_WEAPON_SPECIAL: 165,
	INPUT_SELECT_CHARACTER_MICHAEL: 166,
	INPUT_SELECT_CHARACTER_FRANKLIN: 167,
	INPUT_SELECT_CHARACTER_TREVOR: 168,
	INPUT_SELECT_CHARACTER_MULTIPLAYER: 169,
	INPUT_SAVE_REPLAY_CLIP: 170,
	INPUT_SPECIAL_ABILITY_PC: 171,
	INPUT_CELLPHONE_UP: 172,
	INPUT_CELLPHONE_DOWN: 173,
	INPUT_CELLPHONE_LEFT: 174,
	INPUT_CELLPHONE_RIGHT: 175,
	INPUT_CELLPHONE_SELECT: 176,
	INPUT_CELLPHONE_CANCEL: 177,
	INPUT_CELLPHONE_OPTION: 178,
	INPUT_CELLPHONE_EXTRA_OPTION: 179,
	INPUT_CELLPHONE_SCROLL_FORWARD: 180,
	INPUT_CELLPHONE_SCROLL_BACKWARD: 181,
	INPUT_CELLPHONE_CAMERA_FOCUS_LOCK: 182,
	INPUT_CELLPHONE_CAMERA_GRID: 183,
	INPUT_CELLPHONE_CAMERA_SELFIE: 184,
	INPUT_CELLPHONE_CAMERA_DOF: 185,
	INPUT_CELLPHONE_CAMERA_EXPRESSION: 186,
	INPUT_FRONTEND_DOWN: 187,
	INPUT_FRONTEND_UP: 188,
	INPUT_FRONTEND_LEFT: 189,
	INPUT_FRONTEND_RIGHT: 190,
	INPUT_FRONTEND_RDOWN: 191,
	INPUT_FRONTEND_RUP: 192,
	INPUT_FRONTEND_RLEFT: 193,
	INPUT_FRONTEND_RRIGHT: 194,
	INPUT_FRONTEND_AXIS_X: 195,
	INPUT_FRONTEND_AXIS_Y: 196,
	INPUT_FRONTEND_RIGHT_AXIS_X: 197,
	INPUT_FRONTEND_RIGHT_AXIS_Y: 198,
	INPUT_FRONTEND_PAUSE: 199,
	INPUT_FRONTEND_PAUSE_ALTERNATE: 200,
	INPUT_FRONTEND_ACCEPT: 201,
	INPUT_FRONTEND_CANCEL: 202,
	INPUT_FRONTEND_X: 203,
	INPUT_FRONTEND_Y: 204,
	INPUT_FRONTEND_LB: 205,
	INPUT_FRONTEND_RB: 206,
	INPUT_FRONTEND_LT: 207,
	INPUT_FRONTEND_RT: 208,
	INPUT_FRONTEND_LS: 209,
	INPUT_FRONTEND_RS: 210,
	INPUT_FRONTEND_LEADERBOARD: 211,
	INPUT_FRONTEND_SOCIAL_CLUB: 212,
	INPUT_FRONTEND_SOCIAL_CLUB_SECONDARY: 213,
	INPUT_FRONTEND_DELETE: 214,
	INPUT_FRONTEND_ENDSCREEN_ACCEPT: 215,
	INPUT_FRONTEND_ENDSCREEN_EXPAND: 216,
	INPUT_FRONTEND_SELECT: 217,
	INPUT_SCRIPT_LEFT_AXIS_X: 218,
	INPUT_SCRIPT_LEFT_AXIS_Y: 219,
	INPUT_SCRIPT_RIGHT_AXIS_X: 220,
	INPUT_SCRIPT_RIGHT_AXIS_Y: 221,
	INPUT_SCRIPT_RUP: 222,
	INPUT_SCRIPT_RDOWN: 223,
	INPUT_SCRIPT_RLEFT: 224,
	INPUT_SCRIPT_RRIGHT: 225,
	INPUT_SCRIPT_LB: 226,
	INPUT_SCRIPT_RB: 227,
	INPUT_SCRIPT_LT: 228,
	INPUT_SCRIPT_RT: 229,
	INPUT_SCRIPT_LS: 230,
	INPUT_SCRIPT_RS: 231,
	INPUT_SCRIPT_PAD_UP: 232,
	INPUT_SCRIPT_PAD_DOWN: 233,
	INPUT_SCRIPT_PAD_LEFT: 234,
	INPUT_SCRIPT_PAD_RIGHT: 235,
	INPUT_SCRIPT_SELECT: 236,
	INPUT_CURSOR_ACCEPT: 237,
	INPUT_CURSOR_CANCEL: 238,
	INPUT_CURSOR_X: 239,
	INPUT_CURSOR_Y: 240,
	INPUT_CURSOR_SCROLL_UP: 241,
	INPUT_CURSOR_SCROLL_DOWN: 242,
	INPUT_ENTER_CHEAT_CODE: 243,
	INPUT_INTERACTION_MENU: 244,
	INPUT_MP_TEXT_CHAT_ALL: 245,
	INPUT_MP_TEXT_CHAT_TEAM: 246,
	INPUT_MP_TEXT_CHAT_FRIENDS: 247,
	INPUT_MP_TEXT_CHAT_CREW: 248,
	INPUT_PUSH_TO_TALK: 249,
	INPUT_CREATOR_LS: 250,
	INPUT_CREATOR_RS: 251,
	INPUT_CREATOR_LT: 252,
	INPUT_CREATOR_RT: 253,
	INPUT_CREATOR_MENU_TOGGLE: 254,
	INPUT_CREATOR_ACCEPT: 255,
	INPUT_CREATOR_DELETE: 256,
	INPUT_ATTACK2: 257,
	INPUT_RAPPEL_JUMP: 258,
	INPUT_RAPPEL_LONG_JUMP: 259,
	INPUT_RAPPEL_SMASH_WINDOW: 260,
	INPUT_PREV_WEAPON: 261,
	INPUT_NEXT_WEAPON: 262,
	INPUT_MELEE_ATTACK1: 263,
	INPUT_MELEE_ATTACK2: 264,
	INPUT_WHISTLE: 265,
	INPUT_MOVE_LEFT: 266,
	INPUT_MOVE_RIGHT: 267,
	INPUT_MOVE_UP: 268,
	INPUT_MOVE_DOWN: 269,
	INPUT_LOOK_LEFT: 270,
	INPUT_LOOK_RIGHT: 271,
	INPUT_LOOK_UP: 272,
	INPUT_LOOK_DOWN: 273,
	INPUT_SNIPER_ZOOM_IN: 274,
	INPUT_SNIPER_ZOOM_OUT: 275,
	INPUT_SNIPER_ZOOM_IN_ALTERNATE: 276,
	INPUT_SNIPER_ZOOM_OUT_ALTERNATE: 277,
	INPUT_VEH_MOVE_LEFT: 278,
	INPUT_VEH_MOVE_RIGHT: 279,
	INPUT_VEH_MOVE_UP: 280,
	INPUT_VEH_MOVE_DOWN: 281,
	INPUT_VEH_GUN_LEFT: 282,
	INPUT_VEH_GUN_RIGHT: 283,
	INPUT_VEH_GUN_UP: 284,
	INPUT_VEH_GUN_DOWN: 285,
	INPUT_VEH_LOOK_LEFT: 286,
	INPUT_VEH_LOOK_RIGHT: 287,
	INPUT_REPLAY_START_STOP_RECORDING: 288,
	INPUT_REPLAY_START_STOP_RECORDING_SECONDARY: 289,
	INPUT_SCALED_LOOK_LR: 290,
	INPUT_SCALED_LOOK_UD: 291,
	INPUT_SCALED_LOOK_UP_ONLY: 292,
	INPUT_SCALED_LOOK_DOWN_ONLY: 293,
	INPUT_SCALED_LOOK_LEFT_ONLY: 294,
	INPUT_SCALED_LOOK_RIGHT_ONLY: 295,
	INPUT_REPLAY_MARKER_DELETE: 296,
	INPUT_REPLAY_CLIP_DELETE: 297,
	INPUT_REPLAY_PAUSE: 298,
	INPUT_REPLAY_REWIND: 299,
	INPUT_REPLAY_FFWD: 300,
	INPUT_REPLAY_NEWMARKER: 301,
	INPUT_REPLAY_RECORD: 302,
	INPUT_REPLAY_SCREENSHOT: 303,
	INPUT_REPLAY_HIDEHUD: 304,
	INPUT_REPLAY_STARTPOINT: 305,
	INPUT_REPLAY_ENDPOINT: 306,
	INPUT_REPLAY_ADVANCE: 307,
	INPUT_REPLAY_BACK: 308,
	INPUT_REPLAY_TOOLS: 309,
	INPUT_REPLAY_RESTART: 310,
	INPUT_REPLAY_SHOWHOTKEY: 311,
	INPUT_REPLAY_CYCLEMARKERLEFT: 312,
	INPUT_REPLAY_CYCLEMARKERRIGHT: 313,
	INPUT_REPLAY_FOVINCREASE: 314,
	INPUT_REPLAY_FOVDECREASE: 315,
	INPUT_REPLAY_CAMERAUP: 316,
	INPUT_REPLAY_CAMERADOWN: 317,
	INPUT_REPLAY_SAVE: 318,
	INPUT_REPLAY_TOGGLETIME: 319,
	INPUT_REPLAY_TOGGLETIPS: 320,
	INPUT_REPLAY_PREVIEW: 321,
	INPUT_REPLAY_TOGGLE_TIMELINE: 322,
	INPUT_REPLAY_TIMELINE_PICKUP_CLIP: 323,
	INPUT_REPLAY_TIMELINE_DUPLICATE_CLIP: 324,
	INPUT_REPLAY_TIMELINE_PLACE_CLIP: 325,
	INPUT_REPLAY_CTRL: 326,
	INPUT_REPLAY_TIMELINE_SAVE: 327,
	INPUT_REPLAY_PREVIEW_AUDIO: 328,
	INPUT_VEH_DRIVE_LOOK: 329,
	INPUT_VEH_DRIVE_LOOK2: 330,
	INPUT_VEH_FLY_ATTACK2: 331,
	INPUT_RADIO_WHEEL_UD: 332,
	INPUT_RADIO_WHEEL_LR: 333,
	INPUT_VEH_SLOWMO_UD: 334,
	INPUT_VEH_SLOWMO_UP_ONLY: 335,
	INPUT_VEH_SLOWMO_DOWN_ONLY: 336,
	INPUT_MAP_POI: 337
};

global.isSeat = false;

class SitClass {
    constructor() {

        this.disableControls = [
            CONTROL.INPUT_MOVE_LEFT,
            CONTROL.INPUT_MOVE_RIGHT,
            CONTROL.INPUT_MOVE_UP,
            CONTROL.INPUT_MOVE_DOWN,
            CONTROL.INPUT_MOVE_LR,
            CONTROL.INPUT_MOVE_UD,
            CONTROL.INPUT_MOVE_UP_ONLY,
            CONTROL.INPUT_MOVE_DOWN_ONLY,
            CONTROL.INPUT_MOVE_LEFT_ONLY,
            CONTROL.INPUT_MOVE_RIGHT_ONLY,
            CONTROL.INPUT_VEH_EXIT,
            CONTROL.INPUT_JUMP,
            CONTROL.INPUT_RAPPEL_JUMP,
            CONTROL.INPUT_SPRINT,
            CONTROL.INPUT_COVER,
        ];

        this.objectsActions = { 
            text: translateText("Сесть"),
            anim: 
            { 
                dict: "switch@michael@sitting", 
                name: "idle" 
            } 
        };

        this.defaultData = {
            None: 0,      
            yMinus: 1,
            yPlus: 2,//Задняя сторона
            xMinus: 4,
            xPlus: 8
        }

        this.objectsInfo = {
            [mp.game.joaat("xm_prop_x17_Corp_OffChair")]: { typeFlags: this.defaultData.yMinus },//1
            [mp.game.joaat("apa_mp_h_din_chair_04")]: { typeFlags: this.defaultData.yMinus },//2 -
            [mp.game.joaat("apa_mp_h_din_chair_08")]: { typeFlags: this.defaultData.yMinus },//3 -
            [mp.game.joaat("apa_mp_h_din_chair_09")]: { typeFlags: this.defaultData.yMinus },//4 -
            [mp.game.joaat("apa_mp_h_din_chair_12")]: { typeFlags: this.defaultData.yMinus },//5 -
            [mp.game.joaat("apa_mp_h_din_stool_04")]: { typeFlags: this.defaultData.yMinus },//6 -
            [mp.game.joaat("apa_mp_h_stn_chairarm_01")]: { typeFlags: this.defaultData.yMinus },//7 -
            [mp.game.joaat("apa_mp_h_stn_chairarm_02")]: { typeFlags: this.defaultData.yMinus },//8 -
            [mp.game.joaat("apa_mp_h_stn_chairarm_03")]: { typeFlags: this.defaultData.yMinus },//9 -
            [mp.game.joaat("apa_mp_h_stn_chairarm_09")]: { typeFlags: this.defaultData.yMinus },//10 -
            [mp.game.joaat("apa_mp_h_stn_chairarm_11")]: { typeFlags: this.defaultData.yMinus },//11 -
            [mp.game.joaat("apa_mp_h_stn_chairarm_12")]: { typeFlags: this.defaultData.yMinus },//12 -
            [mp.game.joaat("apa_mp_h_stn_chairarm_13")]: { typeFlags: this.defaultData.yMinus },//13 -
            [mp.game.joaat("apa_mp_h_stn_chairarm_23")]: { typeFlags: this.defaultData.yMinus },//14 -
            [mp.game.joaat("apa_mp_h_stn_chairarm_24")]: { typeFlags: this.defaultData.yMinus },//15 -
            [mp.game.joaat("apa_mp_h_stn_chairarm_25")]: { typeFlags: this.defaultData.yMinus },//16 -
            [mp.game.joaat("apa_mp_h_stn_chairarm_26")]: { typeFlags: this.defaultData.yMinus },//17 -
            [mp.game.joaat("apa_mp_h_stn_chairstrip_01")]: { typeFlags: this.defaultData.yMinus },//18 -
            [mp.game.joaat("apa_mp_h_stn_chairstrip_02")]: { typeFlags: this.defaultData.yMinus },//19 -
            [mp.game.joaat("apa_mp_h_stn_chairstrip_03")]: { typeFlags: this.defaultData.yMinus },//20 -
            [mp.game.joaat("apa_mp_h_stn_chairstrip_04")]: { typeFlags: this.defaultData.yMinus },//21
            [mp.game.joaat("apa_mp_h_stn_chairstrip_05")]: { typeFlags: this.defaultData.yMinus },//22
            [mp.game.joaat("apa_mp_h_stn_chairstrip_06")]: { typeFlags: this.defaultData.yMinus },//23
            [mp.game.joaat("apa_mp_h_stn_chairstrip_07")]: { typeFlags: this.defaultData.yMinus },//24
            [mp.game.joaat("apa_mp_h_stn_chairstrip_08")]: { typeFlags: this.defaultData.yMinus },//25
            [mp.game.joaat("apa_mp_h_yacht_armchair_01")]: { typeFlags: this.defaultData.yMinus },//26
            [mp.game.joaat("apa_mp_h_yacht_armchair_03")]: { typeFlags: this.defaultData.yMinus },//27
            [mp.game.joaat("apa_mp_h_yacht_armchair_04")]: { typeFlags: this.defaultData.yMinus },//28
            [mp.game.joaat("apa_mp_h_yacht_barstool_01")]: { typeFlags: this.defaultData.yMinus },//29 - Высоты
            [mp.game.joaat("apa_mp_h_yacht_strip_chair_01")]: { typeFlags: this.defaultData.yMinus },//30
            [mp.game.joaat("bkr_prop_biker_barstool_01")]: { typeFlags: this.defaultData.yMinus },//31 Высоты
            [mp.game.joaat("bkr_prop_biker_barstool_02")]: { typeFlags: this.defaultData.yMinus },//32
            [mp.game.joaat("bkr_prop_biker_barstool_03")]: { },//33 не работает
            [mp.game.joaat("bkr_prop_biker_barstool_04")]: { typeFlags: this.defaultData.yMinus | this.defaultData.yPlus | this.defaultData.xMinus | this.defaultData.xPlus },//34 -
            [mp.game.joaat("bkr_prop_biker_boardchair01")]: { typeFlags: this.defaultData.yMinus },//35
            //[mp.game.joaat("bkr_prop_biker_chair_01")]: { typeFlags: this.defaultData.yMinus },//36 не работает
            [mp.game.joaat("bkr_prop_biker_chairstrip_01")]: { typeFlags: this.defaultData.yMinus },//37
            [mp.game.joaat("bkr_prop_biker_chairstrip_02")]: { typeFlags: this.defaultData.yMinus },//38
            [mp.game.joaat("bkr_prop_clubhouse_armchair_01a")]: { typeFlags: this.defaultData.yMinus },//39
            [mp.game.joaat("bkr_prop_clubhouse_chair_01")]: { typeFlags: this.defaultData.yMinus },//41
            [mp.game.joaat("bkr_prop_clubhouse_chair_03")]: { typeFlags: this.defaultData.yMinus },//42
            [mp.game.joaat("bkr_prop_clubhouse_offchair_01a")]: { typeFlags: this.defaultData.yMinus },//43
            [mp.game.joaat("bkr_prop_clubhouse_sofa_01a")]: { typeFlags: this.defaultData.yMinus },//44
            [mp.game.joaat("bkr_prop_weed_chair_01a")]: { typeFlags: this.defaultData.yMinus },//45
            //[mp.game.joaat("ex_mp_h_din_chair_04")]: { typeFlags: this.defaultData.yMinus },//46 Не работает
            //[mp.game.joaat("ex_mp_h_din_chair_08")]: { typeFlags: this.defaultData.yMinus },//47 Не работает
            //[mp.game.joaat("ex_mp_h_din_chair_09")]: { typeFlags: this.defaultData.yMinus },//48 Не работает
            //[mp.game.joaat("ex_mp_h_din_chair_12")]: { typeFlags: this.defaultData.yMinus },//49 Не работает
            [mp.game.joaat("ex_mp_h_din_stool_04")]: { typeFlags: this.defaultData.yMinus },//50 Высоты
            [mp.game.joaat("ex_mp_h_off_chairstrip_01")]: { typeFlags: this.defaultData.yMinus },//51
            [mp.game.joaat("ex_mp_h_off_easychair_01")]: { typeFlags: this.defaultData.yMinus },//52
            [mp.game.joaat("ex_mp_h_stn_chairarm_03")]: { typeFlags: this.defaultData.yMinus },//53
            [mp.game.joaat("ex_mp_h_stn_chairarm_24")]: { typeFlags: this.defaultData.yMinus },//54
            [mp.game.joaat("ex_mp_h_stn_chairstrip_01")]: { typeFlags: this.defaultData.yMinus },//55
            [mp.game.joaat("ex_mp_h_stn_chairstrip_010")]: { typeFlags: this.defaultData.yMinus },//56
            [mp.game.joaat("ex_mp_h_stn_chairstrip_011")]: { typeFlags: this.defaultData.yMinus },//58
            [mp.game.joaat("ex_mp_h_stn_chairstrip_05")]: { typeFlags: this.defaultData.yMinus },//59
            [mp.game.joaat("ex_mp_h_stn_chairstrip_07")]: { typeFlags: this.defaultData.yMinus },//60
            [mp.game.joaat("ex_prop_offchair_exec_01")]: { typeFlags: this.defaultData.yMinus },//61
            [mp.game.joaat("ex_prop_offchair_exec_02")]: { typeFlags: this.defaultData.yMinus },//62
            [mp.game.joaat("ex_prop_offchair_exec_03")]: { typeFlags: this.defaultData.yMinus },//63
            [mp.game.joaat("ex_prop_offchair_exec_04")]: { typeFlags: this.defaultData.yMinus },//64
            //[mp.game.joaat("hei_heist_din_chair_01")]: { typeFlags: this.defaultData.yMinus },//65 Не работает
            [mp.game.joaat("hei_heist_din_chair_02")]: { typeFlags: this.defaultData.yMinus },//66
            [mp.game.joaat("hei_heist_din_chair_03")]: { typeFlags: this.defaultData.yMinus },//67
            [mp.game.joaat("hei_heist_din_chair_04")]: { typeFlags: this.defaultData.yMinus },//68
            [mp.game.joaat("hei_heist_din_chair_05")]: { typeFlags: this.defaultData.yMinus },//69
            [mp.game.joaat("hei_heist_din_chair_06")]: { typeFlags: this.defaultData.yMinus },//70
            [mp.game.joaat("hei_heist_din_chair_08")]: { typeFlags: this.defaultData.yMinus },//71
            [mp.game.joaat("hei_heist_din_chair_09")]: { typeFlags: this.defaultData.yMinus },//72
            [mp.game.joaat("hei_heist_stn_benchshort")]: { typeFlags: this.defaultData.yMinus | this.defaultData.yPlus, zOffset: 0.25 },//73
            [mp.game.joaat("hei_heist_stn_chairarm_01")]: { typeFlags: this.defaultData.yMinus },//74
            [mp.game.joaat("hei_heist_stn_chairarm_03")]: { typeFlags: this.defaultData.yMinus },//75
            [mp.game.joaat("hei_heist_stn_chairarm_04")]: { typeFlags: this.defaultData.yMinus },//76
            [mp.game.joaat("hei_heist_stn_chairarm_06")]: { typeFlags: this.defaultData.yMinus },//77
            [mp.game.joaat("hei_heist_stn_chairstrip_01")]: { typeFlags: this.defaultData.yMinus },//78
            [mp.game.joaat("hei_prop_yah_lounger")]: { typeFlags: this.defaultData.yMinus },//80 Лежак
            [mp.game.joaat("hei_prop_yah_seat_01")]: { typeFlags: this.defaultData.yMinus },//81
            [mp.game.joaat("hei_prop_yah_seat_02")]: { typeFlags: this.defaultData.yMinus },//82 -
            [mp.game.joaat("hei_prop_yah_seat_03")]: { typeFlags: this.defaultData.yMinus },//83 -
            [mp.game.joaat("lr_prop_clubstool_01")]: { typeFlags: this.defaultData.yMinus | this.defaultData.yPlus | this.defaultData.xMinus | this.defaultData.xPlus },//84 -
            [mp.game.joaat("p_dinechair_01_s")]: { typeFlags: this.defaultData.yMinus },//85 -
            [mp.game.joaat("p_patio_lounger1_s")]: { typeFlags: this.defaultData.yMinus },//86 -
            [mp.game.joaat("p_v_med_p_sofa_s")]: { typeFlags: this.defaultData.yMinus },//88 -
            [mp.game.joaat("p_yacht_chair_01_s")]: { typeFlags: this.defaultData.yMinus },//89 -
            [mp.game.joaat("p_yacht_sofa_01_s")]: { typeFlags: this.defaultData.yMinus },//90 -
            [mp.game.joaat("prop_air_bench_01")]: { typeFlags: this.defaultData.yPlus },//91 -
            [mp.game.joaat("prop_air_bench_02")]: { typeFlags: this.defaultData.yMinus | this.defaultData.yPlus },//92 -
            [mp.game.joaat("prop_armchair_01")]: { typeFlags: this.defaultData.yMinus },//93 -
            [mp.game.joaat("prop_bar_stool_01")]: { typeFlags: this.defaultData.yMinus | this.defaultData.yPlus | this.defaultData.xMinus | this.defaultData.xPlus },//94 -
            //[mp.game.joaat("prop_busstop_02")]: { typeFlags: this.defaultData.yMinus },//95 - Спорно
            //[mp.game.joaat("prop_busstop_04")]: { typeFlags: this.defaultData.yMinus },//96 - Спорно
            //[mp.game.joaat("prop_busstop_05")]: { typeFlags: this.defaultData.yMinus },//97 - Спорно
            [mp.game.joaat("prop_couch_01")]: { typeFlags: this.defaultData.yMinus },//98 -
            [mp.game.joaat("prop_couch_03")]: { typeFlags: this.defaultData.yMinus },//99 -
            [mp.game.joaat("prop_couch_04")]: { typeFlags: this.defaultData.yMinus },//100 -
            [mp.game.joaat("prop_couch_lg_02")]: { typeFlags: this.defaultData.yMinus },//101 -
            [mp.game.joaat("prop_couch_lg_05")]: { typeFlags: this.defaultData.yMinus },//102 -
            [mp.game.joaat("prop_couch_lg_06")]: { typeFlags: this.defaultData.yMinus },//103 -
            //[mp.game.joaat("prop_couch_lg_07")]: { typeFlags: this.defaultData.yMinus },//104 - Баганая
            [mp.game.joaat("prop_couch_lg_08")]: { typeFlags: this.defaultData.yMinus },//105 -
            [mp.game.joaat("prop_couch_sm2_07")]: { typeFlags: this.defaultData.yMinus },//106 -
            [mp.game.joaat("prop_couch_sm_02")]: { typeFlags: this.defaultData.yMinus },//107 -
            [mp.game.joaat("prop_couch_sm_05")]: { typeFlags: this.defaultData.yMinus },//108 -
            [mp.game.joaat("prop_couch_sm_06")]: { typeFlags: this.defaultData.yMinus },//109 -
            [mp.game.joaat("prop_couch_sm_07")]: { typeFlags: this.defaultData.yMinus },//110 -
            [mp.game.joaat("prop_cs_office_chair")]: { typeFlags: this.defaultData.yMinus },//111 -
            [mp.game.joaat("prop_direct_chair_01")]: { typeFlags: this.defaultData.yMinus },//112 -
            [mp.game.joaat("prop_direct_chair_02")]: { typeFlags: this.defaultData.yMinus },//113 - вЫсота
            [mp.game.joaat("prop_fib_3b_bench")]: { typeFlags: this.defaultData.yMinus },//114 -
            [mp.game.joaat("prop_gc_chair02")]: { typeFlags: this.defaultData.yMinus },//115 -
            [mp.game.joaat("prop_hwbowl_pseat_6x1")]: { typeFlags: this.defaultData.yMinus },//116 - Низко
            [mp.game.joaat("prop_ld_farm_chair01")]: { typeFlags: this.defaultData.yPlus },//117 -
            [mp.game.joaat("v_ilev_ph_bench")]: { typeFlags: this.defaultData.yMinus },//118 -
            [mp.game.joaat("v_res_fh_benchlong")]: { typeFlags: this.defaultData.yMinus | this.defaultData.yPlus },//119

            [mp.game.joaat("v_ret_chair")]: { typeFlags: this.defaultData.yMinus },//144 -
            [mp.game.joaat("v_ret_chair_white")]: { typeFlags: this.defaultData.yMinus },//145 -
            [mp.game.joaat("v_ret_fh_chair01")]: { typeFlags: this.defaultData.yMinus },//146 -
            [mp.game.joaat("v_ret_gc_chair01")]: { typeFlags: this.defaultData.yMinus },//147 -
            [mp.game.joaat("v_ret_gc_chair02")]: { typeFlags: this.defaultData.yMinus },//148 -
            [mp.game.joaat("v_ret_gc_chair03")]: { typeFlags: this.defaultData.yMinus },//149 -
            [mp.game.joaat("v_ret_ps_chair")]: { typeFlags: this.defaultData.yMinus },//150 - 

            [mp.game.joaat("xm_lab_chairarm_02")]: { typeFlags: this.defaultData.yMinus },//151 -
            [mp.game.joaat("xm_lab_chairarm_03")]: { typeFlags: this.defaultData.yMinus },//152 -
            [mp.game.joaat("xm_lab_chairarm_11")]: { typeFlags: this.defaultData.yMinus },//153 -
            [mp.game.joaat("xm_lab_chairarm_12")]: { typeFlags: this.defaultData.yMinus },//154 -
            [mp.game.joaat("xm_lab_chairarm_24")]: { typeFlags: this.defaultData.yMinus },//155 -
            [mp.game.joaat("xm_lab_chairarm_25")]: { typeFlags: this.defaultData.yMinus },//156 -
            [mp.game.joaat("xm_lab_chairarm_26")]: { typeFlags: this.defaultData.yMinus },//157 -
            
            [mp.game.joaat("prop_bench_01a")]: { typeFlags: this.defaultData.yMinus, zOffset: 0.25 },
            [mp.game.joaat("prop_bench_01b")]: { typeFlags: this.defaultData.yMinus, zOffset: 0.25 },
            [mp.game.joaat("prop_bench_01c")]: { typeFlags: this.defaultData.yMinus, zOffset: 0.25 },
            [mp.game.joaat("prop_bench_02")]: { typeFlags: this.defaultData.yMinus, zOffset: 0.25 },
            [mp.game.joaat("prop_bench_03")]: { typeFlags: this.defaultData.yMinus, zOffset: 0.25 },
            [mp.game.joaat("prop_bench_04")]: { typeFlags: this.defaultData.yMinus, zOffset: 0.25 },
            [mp.game.joaat("prop_bench_05")]: { typeFlags: this.defaultData.yMinus, zOffset: 0.25 },
            [mp.game.joaat("prop_bench_06")]: { typeFlags: this.defaultData.yMinus, zOffset: 0.25 },
            [mp.game.joaat("prop_bench_07")]: { typeFlags: this.defaultData.yMinus, zOffset: 0.25 },
            [mp.game.joaat("prop_bench_08")]: { typeFlags: this.defaultData.yMinus, zOffset: 0.25 },
            [mp.game.joaat("prop_bench_09")]: { typeFlags: this.defaultData.yMinus, zOffset: 0.25 },
            [mp.game.joaat("prop_bench_10")]: { typeFlags: this.defaultData.yMinus, zOffset: 0.25 },
            [mp.game.joaat("prop_bench_11")]: { typeFlags: this.defaultData.yMinus, zOffset: 0.25 },
            //[mp.game.joaat("prop_ld_bench01")]: { typeFlags: this.defaultData.yMinus },//176 - Спорно
            [mp.game.joaat("prop_wait_bench_01")]: { typeFlags: this.defaultData.yMinus | this.defaultData.yPlus },//177 -
            [mp.game.joaat("hei_prop_heist_off_chair")]: { typeFlags: this.defaultData.yMinus },//178 -
            [mp.game.joaat("hei_prop_hei_skid_chair")]: { typeFlags: this.defaultData.yMinus },//179 -
            [mp.game.joaat("prop_chair_01a")]: { typeFlags: this.defaultData.yMinus },//180 -
            [mp.game.joaat("prop_chair_01b")]: { typeFlags: this.defaultData.yMinus },//181 -
            [mp.game.joaat("prop_chair_02")]: { typeFlags: this.defaultData.yMinus },//182 -
            [mp.game.joaat("prop_chair_03")]: { typeFlags: this.defaultData.yMinus },//183 -
            [mp.game.joaat("prop_chair_04a")]: { typeFlags: this.defaultData.yMinus },//184 -
            [mp.game.joaat("prop_chair_04b")]: { typeFlags: this.defaultData.yMinus },//185 -
            [mp.game.joaat("prop_chair_05")]: { typeFlags: this.defaultData.yMinus },//186 -
            [mp.game.joaat("prop_chair_06")]: { typeFlags: this.defaultData.yMinus },//187 -
            [mp.game.joaat("prop_chair_07")]: { typeFlags: this.defaultData.yMinus },//188 -
            [mp.game.joaat("prop_chair_08")]: { typeFlags: this.defaultData.yMinus },//189 -
            [mp.game.joaat("prop_chair_09")]: { typeFlags: this.defaultData.yMinus },//190 -
            [mp.game.joaat("prop_chair_10")]: { typeFlags: this.defaultData.yMinus },//191 -
            [mp.game.joaat("prop_chateau_chair_01")]: { typeFlags: this.defaultData.yMinus },//192 -
            [mp.game.joaat("prop_clown_chair")]: { typeFlags: this.defaultData.yMinus },//193 -
            [mp.game.joaat("prop_off_chair_01")]: { typeFlags: this.defaultData.yMinus },//198 -
            [mp.game.joaat("prop_off_chair_03")]: { typeFlags: this.defaultData.yMinus },//199 -
            [mp.game.joaat("prop_off_chair_04")]: { typeFlags: this.defaultData.yMinus },//200 -
            [mp.game.joaat("prop_off_chair_04b")]: { typeFlags: this.defaultData.yMinus },//201 -
            [mp.game.joaat("prop_off_chair_04_s")]: { typeFlags: this.defaultData.yMinus },//202 -
            [mp.game.joaat("prop_off_chair_05")]: { typeFlags: this.defaultData.yMinus },//203 -
            [mp.game.joaat("prop_old_deck_chair")]: { typeFlags: this.defaultData.yMinus },//204 -
            [mp.game.joaat("prop_old_wood_chair")]: { typeFlags: this.defaultData.yMinus },//205 -
            [mp.game.joaat("prop_rock_chair_01")]: { typeFlags: this.defaultData.yMinus },//206 -
            [mp.game.joaat("prop_skid_chair_01")]: { typeFlags: this.defaultData.yMinus },//207 -
            [mp.game.joaat("prop_skid_chair_02")]: { typeFlags: this.defaultData.yMinus },//208 -
            [mp.game.joaat("prop_skid_chair_03")]: { typeFlags: this.defaultData.yMinus },//209 -
            [mp.game.joaat("prop_sol_chair")]: { typeFlags: this.defaultData.yMinus },//210 -
            [mp.game.joaat("prop_wheelchair_01")]: { typeFlags: this.defaultData.yMinus },//211 -
            [mp.game.joaat("prop_wheelchair_01_s")]: { typeFlags: this.defaultData.yMinus },//212 -
            [mp.game.joaat("p_armchair_01_s")]: { typeFlags: this.defaultData.yMinus },//213 -
            [mp.game.joaat("p_clb_officechair_s")]: { typeFlags: this.defaultData.yMinus },//214 -
            [mp.game.joaat("p_ilev_p_easychair_s")]: { typeFlags: this.defaultData.yMinus },//216 -
            //[mp.game.joaat("p_soloffchair_s")]: { typeFlags: this.defaultData.yMinus },//217 - Баганый
            [mp.game.joaat("v_club_officechair")]: { typeFlags: this.defaultData.yMinus },//219 -
            [mp.game.joaat("v_corp_bk_chair3")]: { typeFlags: this.defaultData.yMinus },//220 -
            [mp.game.joaat("v_corp_cd_chair")]: { typeFlags: this.defaultData.yMinus },//221 -
            [mp.game.joaat("v_corp_offchair")]: { typeFlags: this.defaultData.yMinus },//222 -
            [mp.game.joaat("v_ilev_chair02_ped")]: { typeFlags: this.defaultData.yMinus },//223 -
            [mp.game.joaat("v_ilev_hd_chair")]: { typeFlags: this.defaultData.yMinus },//224 -
            [mp.game.joaat("v_ilev_p_easychair")]: { typeFlags: this.defaultData.yMinus },//225 -
            [mp.game.joaat("prop_table_04_chr")]: { typeFlags: this.defaultData.yMinus },//228 -
            [mp.game.joaat("prop_table_05_chr")]: { typeFlags: this.defaultData.yMinus },//229 -
            [mp.game.joaat("prop_table_06_chr")]: { typeFlags: this.defaultData.yMinus },//230 -
            [mp.game.joaat("v_ilev_leath_chr")]: { typeFlags: this.defaultData.yMinus },//231 -
            [mp.game.joaat("prop_table_01_chr_a")]: { typeFlags: this.defaultData.yMinus },//232 -
            [mp.game.joaat("prop_table_01_chr_b")]: { typeFlags: this.defaultData.xPlus },//233 -
            [mp.game.joaat("prop_table_02_chr")]: { typeFlags: this.defaultData.yMinus },//234 -
            [mp.game.joaat("prop_table_03b_chr")]: { typeFlags: this.defaultData.yMinus },//235 -
            [mp.game.joaat("prop_table_03_chr")]: { typeFlags: this.defaultData.yMinus },//236 -
            [mp.game.joaat("prop_torture_ch_01")]: { typeFlags: this.defaultData.yMinus },//237 -
            [mp.game.joaat("v_ilev_fh_dineeamesa")]: { typeFlags: this.defaultData.yMinus },//238 -
            [mp.game.joaat("v_ilev_fh_kitchenstool")]: { typeFlags: this.defaultData.yMinus | this.defaultData.yPlus | this.defaultData.xMinus | this.defaultData.xPlus },//239 -
            [mp.game.joaat("v_ilev_tort_stool")]: { typeFlags: this.defaultData.yMinus | this.defaultData.yPlus | this.defaultData.xMinus | this.defaultData.xPlus},//240 -
            [mp.game.joaat("prop_waiting_seat_01")]: { typeFlags: this.defaultData.yMinus },//248 -
            [mp.game.joaat("prop_yacht_seat_01")]: { typeFlags: this.defaultData.yMinus },//249 -
            [mp.game.joaat("prop_yacht_seat_02")]: { typeFlags: this.defaultData.yMinus },//250 -
            [mp.game.joaat("prop_yacht_seat_03")]: { typeFlags: this.defaultData.yMinus },//251 -
            [mp.game.joaat("prop_hobo_seat_01")]: { typeFlags: this.defaultData.yMinus },//252 -
            [mp.game.joaat("prop_rub_couch01")]: { typeFlags: this.defaultData.yMinus },//253 -
            [mp.game.joaat("miss_rub_couch_01")]: { typeFlags: this.defaultData.yMinus },//254 -
            [mp.game.joaat("prop_ld_farm_couch01")]: { typeFlags: this.defaultData.xPlus },//255 -
            [mp.game.joaat("prop_ld_farm_couch02")]: { typeFlags: this.defaultData.xPlus },//256 -
            [mp.game.joaat("prop_rub_couch02")]: { typeFlags: this.defaultData.yMinus },//257 -
            [mp.game.joaat("prop_rub_couch03")]: { typeFlags: this.defaultData.yMinus },//258 -
            [mp.game.joaat("prop_rub_couch04")]: { typeFlags: this.defaultData.yMinus },//259 -
            //[mp.game.joaat("p_lev_sofa_s")]: { typeFlags: this.defaultData.yMinus },//260 - Не работает
            [mp.game.joaat("p_res_sofa_l_s")]: { typeFlags: this.defaultData.yMinus },//261 -
            [mp.game.joaat("v_ilev_m_sofa")]: {  },//264 - Баг
            //[mp.game.joaat("v_res_tre_sofa_s")]: { typeFlags: this.defaultData.yMinus },//265 - Не работает
            [mp.game.joaat("v_tre_sofa_mess_a_s")]: { },//266 - Баг
            [mp.game.joaat("v_tre_sofa_mess_b_s")]: { typeFlags: this.defaultData.yMinus },//267 -
            //[mp.game.joaat("v_tre_sofa_mess_c_s")]: { typeFlags: this.defaultData.yMinus },//268 - Не работает
            [mp.game.joaat("prop_muscle_bench_03")]: { typeFlags: this.defaultData.yMinus },//275 -
            [mp.game.joaat("prop_pris_bench_01")]: { typeFlags: this.defaultData.yMinus },//276 -
            [mp.game.joaat("prop_weight_bench_02")]: { typeFlags: this.defaultData.yMinus },//277 -
            [mp.game.joaat("prop_patio_lounger1")]: { typeFlags: this.defaultData.yMinus },//278 -
            [mp.game.joaat("prop_patio_lounger_2")]: { typeFlags: this.defaultData.yMinus },//279 -
            [mp.game.joaat("prop_patio_lounger_3")]: { typeFlags: this.defaultData.yMinus },//280 -
            [mp.game.joaat("prop_chair_pile_01")]: { typeFlags: this.defaultData.yMinus },//281 -
            [mp.game.joaat("v_med_bed2")]: { typeFlags: this.defaultData.xMinus | this.defaultData.xPlus },//282 -
            [mp.game.joaat("v_med_bed1")]: { typeFlags: this.defaultData.xMinus | this.defaultData.xPlus },//283 -
            [mp.game.joaat("v_med_emptybed")]: { typeFlags: this.defaultData.xMinus | this.defaultData.xPlus },//284 -      
            [mp.game.joaat("v_med_cor_medstool")]: { typeFlags: this.defaultData.yMinus | this.defaultData.yPlus | this.defaultData.xMinus | this.defaultData.xPlus, zOffset: 0.5  },   
            [mp.game.joaat("v_ind_meatbench")]: { typeFlags: this.defaultData.yMinus | this.defaultData.yPlus },  
            [mp.game.joaat("v_corp_lngestool")]: { typeFlags: this.defaultData.yMinus | this.defaultData.yPlus | this.defaultData.xMinus | this.defaultData.xPlus },   
            [mp.game.joaat("v_corp_lngestoolfd")]: { typeFlags: this.defaultData.yMinus | this.defaultData.yPlus | this.defaultData.xMinus | this.defaultData.xPlus },            
            [mp.game.joaat("prop_bench_07")]: { typeFlags: this.defaultData.yMinus },//289 -
            [mp.game.joaat("v_corp_lazychair")]: { typeFlags: this.defaultData.yMinus },//290 -
            [mp.game.joaat("prop_ven_market_stool")]: { typeFlags: this.defaultData.yMinus | this.defaultData.yPlus },//291 -
            [mp.game.joaat("ba_prop_int_edgy_stool")]: { typeFlags: this.defaultData.yMinus | this.defaultData.yPlus | this.defaultData.xMinus | this.defaultData.xPlus },//292 -
            [mp.game.joaat("apa_mp_h_yacht_stool_01")]: { typeFlags: this.defaultData.yMinus | this.defaultData.yPlus | this.defaultData.xMinus | this.defaultData.xPlus },//293 -
            [mp.game.joaat("v_ilev_m_dinechair")]: { typeFlags: this.defaultData.yMinus },//294 -
            [mp.game.joaat("prop_chair_07")]: { typeFlags: this.defaultData.yMinus },//295 -
            [mp.game.joaat("gr_prop_gr_chair02_ped")]: { typeFlags: this.defaultData.yMinus },//296 -
            [mp.game.joaat("xm_prop_x17_avengerchair_02")]: { typeFlags: this.defaultData.yMinus },//297 -
            [mp.game.joaat("xm_lab_easychair_01")]: { typeFlags: this.defaultData.yMinus },//298 -
            [mp.game.joaat("xm_int_lev_sub_chair_01")]: { typeFlags: this.defaultData.yMinus },//299 -
            [mp.game.joaat("vw_prop_vw_offchair_03")]: { typeFlags: this.defaultData.yMinus },//300 -
            [mp.game.joaat("sm_prop_smug_offchair_01a")]: { typeFlags: this.defaultData.yMinus },//301 -
            [mp.game.joaat("sm_prop_offchair_smug_02")]: { typeFlags: this.defaultData.yMinus },//302 -
            [mp.game.joaat("sm_prop_offchair_smug_01")]: { typeFlags: this.defaultData.yMinus },//303 -
            [mp.game.joaat("prop_yaught_chair_01")]: { typeFlags: this.defaultData.yMinus },//304 -
            [mp.game.joaat("imp_prop_impexp_offchair_01a")]: { typeFlags: this.defaultData.yMinus },//305 -
            [mp.game.joaat("ch_prop_casino_track_chair_01")]: { typeFlags: this.defaultData.yMinus },//306 -
            [mp.game.joaat("ba_prop_battle_club_chair_03")]: { typeFlags: this.defaultData.yMinus },//307 -
            [mp.game.joaat("ba_prop_battle_club_chair_02")]: { typeFlags: this.defaultData.yMinus },//308 -
            [mp.game.joaat("ba_prop_battle_club_chair_01")]: { typeFlags: this.defaultData.yMinus },//309 -
        }

        this.currentObjectHandler = null;
		this.releaseScriptGuids = new Set();
        /*gm.events.add("render", () => {
            try {
                if (!global.loggedin || true) return;
                if (null !== this.currentObjectHandler) {
                    const getNearSitInfo = this.getNearSitInfo(this.currentObjectHandler);
                    if (getNearSitInfo && getNearSitInfo.position) {
                        mp.game.graphics.drawLine(
                            null == getNearSitInfo ? void 0 : getNearSitInfo.position.x,
                            null == getNearSitInfo ? void 0 : getNearSitInfo.position.y,
                            0,
                            null == getNearSitInfo ? void 0 : getNearSitInfo.position.x,
                            null == getNearSitInfo ? void 0 : getNearSitInfo.position.y,
                            1000,
                            255,
                            0,
                            0,
                            255
                        );
                    }
                }
                if (this.sitting) {
                    this.disableControls.forEach((control) => {
                        mp.game.controls.disableControlAction(0, control, true);
                    });
                }
            }
            catch (e) 
            {
                mp.events.callRemote("client_trycatch", "synchronization/sit", "render", e.toString());
            }
        });*/
        gm.events.add("client.seat", () => {
            if (this.isCanSitDown())
                return this.sitDown();
            else if (this.isCanStandUp())
                return this.standUp();
            
        });

        gm.events.add("client.seat.yes", (posX, posY, posZ, heading) => {
            mp.players.local.setPosition(new mp.Vector3(posX, posY, posZ));
            mp.players.local.setHeading(heading);
            mp.events.call('hud.oEnter', "SeatingUp");
            global.isSeat = true;
            global.freeze = true;
            this.sitting = true;
            gm.discord(translateText("Сидит"));
        });
    }
    sitDown() {
        try {
            if (!this.isCanSitDown()) return false;
            if (global.ANTIANIM) return;
            const getNearSitInfo = this.getNearSitInfo(this.currentObjectHandler);
            if (!getNearSitInfo) return false;
            const rotation = Natives.GET_ENTITY_ROTATION(this.currentObjectHandler, 2);
            getNearSitInfo.position.z = mp.players.local.position.z;
            //mp.players.local.setPosition(getNearSitInfo.position);
            //mp.players.local.setHeading(rotation.z + getNearSitInfo.angleOffset);
            //mp.players.local.setCollision(false, true);
            //mp.players.local.freezePosition(true);
            //global.requestAnimDict(this.objectsActions.anim.dict).then(async () => {
            //    mp.players.local.taskPlayAnim (this.objectsActions.anim.dict, this.objectsActions.anim.name, 2.0, 8, -1, 39, 0, false, false, false);
            //});
            
		    mp.events.callRemote("server.landing.sit", getNearSitInfo.position.x, getNearSitInfo.position.y, getNearSitInfo.position.z, rotation.z + getNearSitInfo.angleOffset);
            return true;
        }
        catch (e) 
        {
            mp.events.callRemote("client_trycatch", "synchronization/sit", "render", e.toString());
        } 
        return false;
    }
    standUp() {
        try {
            if (this.isCanStandUp()) {
                this.sitting = false;                
                global.freeze = false;
                mp.events.callRemote("server.landing.end");

                //mp.players.local.stopAnimTask(this.objectsActions.anim.dict, this.objectsActions.anim.name, 3);
                //mp.players.local.setCollision(true, true);
                //mp.players.local.freezePosition(false);
                //gm.player.actions.resetAnim();
    
                if (this.currentObject)                    
                    mp.events.call('hud.oEnter', "Seating");
            }
        }
        catch (e) 
        {
            mp.events.callRemote("client_trycatch", "synchronization/sit", "standUp", e.toString());
        }
        return true;
    }
    isNearSitPosition(entity) {
        try {
            if (null === entity) return false;
            const getNearSitInfo = this.getNearSitInfo(entity);
            if (!getNearSitInfo) return false;
            return utils.getDistance(mp.players.local.position, getNearSitInfo.position) < 1;
        }
        catch (e) 
        {
            mp.events.callRemote("client_trycatch", "synchronization/sit", "isNearSitPosition", e.toString());
        }
        return false;
    }
    getNearSitInfo(entity) {
        try {

            let zOffset;
            if (null === entity) return null;
            const rotation = Natives.GET_ENTITY_ROTATION(entity, 2);
            if (rotation.x > 30 || rotation.x < -30 || rotation.y > 30 || rotation.y < -30) return null;

            const model = utils.fixHash(Natives.GET_ENTITY_MODEL(entity)),
                currentObject = null !== this.currentObject && undefined !== this.currentObject ? this.currentObject : this.objectsInfo[model];
            
		    //mp.gui.chat.push(model + " = " + JSON.stringify (this.objectsInfo[model]));
            if (!currentObject) return null;
            const { x: xPos, y: yPos, z: zPos } = mp.players.local.position;
            const worldCoords = Natives.GET_OFFSET_FROM_ENTITY_GIVEN_WORLD_COORDS(entity, xPos, yPos, zPos);

            const dimensions = mp.game.gameplay.getModelDimensions(model);
            const xAbs = Math.abs(dimensions.min.x) + Math.abs(dimensions.max.x);
            const yAbs = Math.abs(dimensions.min.y) + Math.abs(dimensions.max.y);
            const xMin = Math.min(0.4, xAbs / 2);
            const yMin = Math.min(0.4, yAbs / 2);
    
            worldCoords.x = global.clamp(worldCoords.x, dimensions.min.x + xMin, dimensions.max.x - xMin);
            worldCoords.y = global.clamp(worldCoords.y, dimensions.min.y + yMin, dimensions.max.y - yMin);

            const currentZOffset = null !== (zOffset = null == currentObject ? undefined : currentObject.zOffset) && undefined !== zOffset ? zOffset : 0,
                getInteractionObjectTypeOffsets = this.getInteractionObjectTypeOffsets(currentObject, worldCoords, dimensions),
                getNearestPositionAndOffset = this.getNearestPositionAndOffset(entity, currentZOffset, getInteractionObjectTypeOffsets);

            if (!getNearestPositionAndOffset) return null;
    
            let angleOffset = 0;
            if (getNearestPositionAndOffset.offset.x !== worldCoords.x) {
                if (getNearestPositionAndOffset.offset.x <= 0)
                    angleOffset = 90;
                else if (getNearestPositionAndOffset.offset.x > 0)
                    angleOffset -= 90;
            }
             
            if (getNearestPositionAndOffset.offset.y !== worldCoords.y) {
                if (getNearestPositionAndOffset.offset.y < 0)
                    angleOffset = 180;
            }
    
            getNearestPositionAndOffset.angleOffset = angleOffset;

            return getNearestPositionAndOffset;
        }
        catch (e) 
        {
            mp.events.callRemote("client_trycatch", "synchronization/sit", "getNearSitInfo", e.toString());
        }
        return null;
    }
    getNearestPositionAndOffset(entity, zPos, positions) {
        try {
            let returnPosition = null,
                returnOffset = null,
                lastDist = Number.MAX_SAFE_INTEGER;
            positions.forEach((offset) => {
                const worldCoords = Natives.GET_OFFSET_FROM_ENTITY_IN_WORLD_COORDS(entity, offset.x, offset.y, offset.z),
                    position = new mp.Vector3(worldCoords.x, worldCoords.y, worldCoords.z + zPos),
                    dist = utils.getDistance(position, mp.players.local.position);

                if (dist < lastDist) {
                    lastDist = dist;
                    returnPosition = position;
                    returnOffset = offset;
                };
            });
            return returnPosition ? { position: returnPosition, offset: returnOffset } : null;
        }
        catch (e) 
        {
            mp.events.callRemote("client_trycatch", "synchronization/sit", "getNearestPositionAndOffset", e.toString());
        }
        return null;
    }
    getInteractionObjectTypeOffsets(currentObject, worldCoords, dimensions) {
        try {
            if (!currentObject) return [];
            const diff = Math.abs(dimensions.min.z) + Math.abs(dimensions.max.z),
                returnPosition = [];
    
            if (currentObject.typeFlags & this.defaultData.yMinus) 
                returnPosition.push(new mp.Vector3(worldCoords.x, -0.2 + dimensions.min.y, diff / 2));
    
            if (currentObject.typeFlags & this.defaultData.yPlus) 
                returnPosition.push(new mp.Vector3(worldCoords.x, 0.2 + dimensions.max.y, diff / 2));
    
            if (currentObject.typeFlags & this.defaultData.xMinus)
                returnPosition.push(new mp.Vector3(-0.2 + dimensions.min.x, worldCoords.y, diff / 2));
    
            if (currentObject.typeFlags & this.defaultData.xPlus) 
                returnPosition.push(new mp.Vector3(0.2 + dimensions.max.x, worldCoords.y, diff / 2));
    
            return returnPosition;
        }
        catch (e) 
        {
            mp.events.callRemote("client_trycatch", "synchronization/sit", "getInteractionObjectTypeOffsets", e.toString());
        }
        return [];
    }
    isCanSitDown(toggled = false) {
        return (
            !this.isCanStandUp() &&
            !(!toggled && !this.currentObject) &&
            !mp.players.local.vehicle &&
            !mp.players.local.isFaint &&
            !mp.players.local.isJailed
        );
    }
    isCanStandUp() {
        return this.sitting;
    }
	async start() {
        try {
            if (!global.loggedin) return;
			//global.debugLog(" sit start 2", true);
			//await mp.game.waitAsync(1500);
            if (this.isCanStandUp()) return;

            if (!this.isCanSitDown(true)) return;


            const { x: xPos, y: yPos, z: zPos } = mp.players.local.position,
                playerPos = new mp.Vector3(xPos, yPos, zPos - 0.4),
                heading = mp.players.local.getHeading(),
                front_vector = global.utils.getFrontVector(playerPos, heading, 0.3),
                right_vector = global.utils.getFrontVector(playerPos, heading, 1),
                testCapsule = mp.raycasting.testCapsule(front_vector, right_vector, 0.5, mp.players.local, 1 | 16);


            let model,
                object = null,
                entity = null;

			if (testCapsule) {
                if ("object" == typeof testCapsule.entity) {
                    entity = testCapsule.entity.handle;
                    model = testCapsule.entity.model;
                    //model = global.utils.fixHash(testCapsule.entity.model);
                } else if ("number" == typeof testCapsule.entity && testCapsule.entity !== 0 && nativeInvoke("IS_ENTITY_AN_OBJECT", testCapsule.entity)) {
                    entity = testCapsule.entity;
					model = global.utils.fixHash(nativeInvoke("GET_ENTITY_MODEL", testCapsule.entity));
                }
                object = this.objectsInfo[model];

				if (object && this.isNearSitPosition(entity)) {
					this.releaseScriptGuids.add(entity);
					this.currentObject = object;
					this.currentObjectHandler = entity;
					mp.events.call('hud.oEnter', "Seating");
					global.isSeat = true;
					return;
				}
            }
            if (this.currentObject) {
				this.clearScriptGuids();
                this.currentObject = null;
                this.currentObjectHandler = null;
                mp.events.call('hud.cEnter');
                global.isSeat = false;
            }
        }
        catch (e) 
        {
            mp.events.callRemote("client_trycatch", "synchronization/sit", "start", e.toString());
        }
    }
	clearScriptGuids() {
		this.releaseScriptGuids.forEach((entity) => {
			Natives.RELEASE_SCRIPT_GUID_FROM_ENTITY(entity);
		});

		this.releaseScriptGuids.clear();
	}
}

const sitData = new SitClass ();

gm.events.add(global.renderName ["500ms"], () => {
    sitData.start ();
});

///




let testfsd = [
    mp.game.joaat("ex_office_swag_counterfeit2")        ,
    mp.game.joaat("xm_prop_x17_Corp_OffChair")          ,
    mp.game.joaat("apa_mp_h_din_chair_04")              ,
    mp.game.joaat("apa_mp_h_din_chair_08")              ,
    mp.game.joaat("apa_mp_h_din_chair_09")              ,
    mp.game.joaat("apa_mp_h_din_chair_12")              ,
    mp.game.joaat("apa_mp_h_din_stool_04")              ,
    mp.game.joaat("apa_mp_h_stn_chairarm_01")           ,
    mp.game.joaat("apa_mp_h_stn_chairarm_02")           ,
    mp.game.joaat("apa_mp_h_stn_chairarm_03")           ,
    mp.game.joaat("apa_mp_h_stn_chairarm_09")           ,
    mp.game.joaat("apa_mp_h_stn_chairarm_11")           ,
    mp.game.joaat("apa_mp_h_stn_chairarm_12")           ,
    mp.game.joaat("apa_mp_h_stn_chairarm_13")           ,
    mp.game.joaat("apa_mp_h_stn_chairarm_23")           ,
    mp.game.joaat("apa_mp_h_stn_chairarm_24")           ,
    mp.game.joaat("apa_mp_h_stn_chairarm_25")           ,
    mp.game.joaat("apa_mp_h_stn_chairarm_26")           ,
    mp.game.joaat("apa_mp_h_stn_chairstrip_01")         ,
    mp.game.joaat("apa_mp_h_stn_chairstrip_02")         ,
    mp.game.joaat("apa_mp_h_stn_chairstrip_03")         ,
    mp.game.joaat("apa_mp_h_stn_chairstrip_04")         ,
    mp.game.joaat("apa_mp_h_stn_chairstrip_05")         ,
    mp.game.joaat("apa_mp_h_stn_chairstrip_06")         ,
    mp.game.joaat("apa_mp_h_stn_chairstrip_07")         ,
    mp.game.joaat("apa_mp_h_stn_chairstrip_08")         ,
    mp.game.joaat("apa_mp_h_yacht_armchair_01")         ,
    mp.game.joaat("apa_mp_h_yacht_armchair_03")         ,
    mp.game.joaat("apa_mp_h_yacht_armchair_04")         ,
    mp.game.joaat("apa_mp_h_yacht_barstool_01")         ,
    mp.game.joaat("apa_mp_h_yacht_strip_chair_01")      ,
    mp.game.joaat("bkr_prop_biker_barstool_01")         ,
    mp.game.joaat("bkr_prop_biker_barstool_02")         ,
    mp.game.joaat("bkr_prop_biker_barstool_03")         ,
    mp.game.joaat("bkr_prop_biker_barstool_04")         ,
    mp.game.joaat("bkr_prop_biker_boardchair01")        ,
    mp.game.joaat("bkr_prop_biker_chair_01")            ,
    mp.game.joaat("bkr_prop_biker_chairstrip_01")       ,
    mp.game.joaat("bkr_prop_biker_chairstrip_02")       ,
    mp.game.joaat("bkr_prop_clubhouse_armchair_01a")    ,
    mp.game.joaat("bkr_prop_clubhouse_blackboard_01a")  ,
    mp.game.joaat("bkr_prop_clubhouse_chair_01")        ,
    mp.game.joaat("bkr_prop_clubhouse_chair_03")        ,
    mp.game.joaat("bkr_prop_clubhouse_offchair_01a")    ,
    mp.game.joaat("bkr_prop_clubhouse_sofa_01a")        ,
    mp.game.joaat("bkr_prop_weed_chair_01a")            ,
    mp.game.joaat("ex_mp_h_din_chair_04")               ,
    mp.game.joaat("ex_mp_h_din_chair_08")               ,
    mp.game.joaat("ex_mp_h_din_chair_09")               ,
    mp.game.joaat("ex_mp_h_din_chair_12")               ,
    mp.game.joaat("ex_mp_h_din_stool_04")               ,
    mp.game.joaat("ex_mp_h_off_chairstrip_01")          ,
    mp.game.joaat("ex_mp_h_off_easychair_01")           ,
    mp.game.joaat("ex_mp_h_stn_chairarm_03")            ,
    mp.game.joaat("ex_mp_h_stn_chairarm_24")            ,
    mp.game.joaat("ex_mp_h_stn_chairstrip_01")          ,
    mp.game.joaat("ex_mp_h_stn_chairstrip_010")         ,
    mp.game.joaat("ex_mp_h_stn_chairstrip_010")         ,
    mp.game.joaat("ex_mp_h_stn_chairstrip_011")         ,
    mp.game.joaat("ex_mp_h_stn_chairstrip_05")          ,
    mp.game.joaat("ex_mp_h_stn_chairstrip_07")          ,
    mp.game.joaat("ex_prop_offchair_exec_01")           ,
    mp.game.joaat("ex_prop_offchair_exec_02")           ,
    mp.game.joaat("ex_prop_offchair_exec_03")           ,
    mp.game.joaat("ex_prop_offchair_exec_04")           ,
    mp.game.joaat("hei_heist_din_chair_01")             ,
    mp.game.joaat("hei_heist_din_chair_02")             ,
    mp.game.joaat("hei_heist_din_chair_03")             ,
    mp.game.joaat("hei_heist_din_chair_04")             ,
    mp.game.joaat("hei_heist_din_chair_05")             ,
    mp.game.joaat("hei_heist_din_chair_06")             ,
    mp.game.joaat("hei_heist_din_chair_08")             ,
    mp.game.joaat("hei_heist_din_chair_09")             ,
    mp.game.joaat("hei_heist_stn_benchshort")           ,
    mp.game.joaat("hei_heist_stn_chairarm_01")          ,
    mp.game.joaat("hei_heist_stn_chairarm_03")          ,
    mp.game.joaat("hei_heist_stn_chairarm_04")          ,
    mp.game.joaat("hei_heist_stn_chairarm_06")          ,
    mp.game.joaat("hei_heist_stn_chairstrip_01")        ,
    mp.game.joaat("hei_heist_stn_chairstrip_01")        ,
    mp.game.joaat("hei_prop_yah_lounger")               ,
    mp.game.joaat("hei_prop_yah_seat_01")               ,
    mp.game.joaat("hei_prop_yah_seat_02")               ,
    mp.game.joaat("hei_prop_yah_seat_03")               ,
    mp.game.joaat("lr_prop_clubstool_01")               ,
    mp.game.joaat("p_dinechair_01_s")                   ,
    mp.game.joaat("p_patio_lounger1_s")                 ,
    //mp.game.joaat("p_solooffchair_s")                   , // ?
    mp.game.joaat("p_v_med_p_sofa_s")                   ,
    mp.game.joaat("p_yacht_chair_01_s")                 ,
    mp.game.joaat("p_yacht_sofa_01_s")                  ,
    mp.game.joaat("prop_air_bench_01")                  ,
    mp.game.joaat("prop_air_bench_02")                  ,
    mp.game.joaat("prop_armchair_01")                   ,
    mp.game.joaat("prop_bar_stool_01")                  ,
    mp.game.joaat("prop_busstop_02")                    ,
    mp.game.joaat("prop_busstop_04")                    ,
    mp.game.joaat("prop_busstop_05")                    ,
    mp.game.joaat("prop_couch_01")                      ,
    mp.game.joaat("prop_couch_03")                      ,
    mp.game.joaat("prop_couch_04")                      ,
    mp.game.joaat("prop_couch_lg_02")                   ,
    mp.game.joaat("prop_couch_lg_05")                   ,
    mp.game.joaat("prop_couch_lg_06")                   ,
    mp.game.joaat("prop_couch_lg_07")                   ,
    mp.game.joaat("prop_couch_lg_08")                   ,
    mp.game.joaat("prop_couch_sm2_07")                  ,
    mp.game.joaat("prop_couch_sm_02")                   ,
    mp.game.joaat("prop_couch_sm_05")                   ,
    mp.game.joaat("prop_couch_sm_06")                   ,
    mp.game.joaat("prop_couch_sm_07")                   ,
    mp.game.joaat("prop_cs_office_chair")               ,
    mp.game.joaat("prop_direct_chair_01")               ,
    mp.game.joaat("prop_direct_chair_02")               ,
    mp.game.joaat("prop_fib_3b_bench")                  ,
    mp.game.joaat("prop_gc_chair02")                    ,
    mp.game.joaat("prop_hwbowl_pseat_6x1")              ,
    mp.game.joaat("prop_ld_farm_chair01")               ,
    mp.game.joaat("v_ilev_ph_bench")                    ,
    mp.game.joaat("v_res_fh_benchlong")                 ,
    mp.game.joaat("v_res_fh_benchshort")                ,
    //mp.game.joaat("v_res_fh_dineemesa")                 , // ?
    //mp.game.joaat("v_res_fh_dineemesb")                 , // ?
    //mp.game.joaat("v_res_fh_dineemesc")                 , // ?
    mp.game.joaat("v_res_fh_easychair")                 ,
    mp.game.joaat("v_res_fh_singleseat")                ,
    mp.game.joaat("v_res_j_dinechair")                  ,
    mp.game.joaat("v_res_j_stool")                      ,
    mp.game.joaat("v_res_m_armchair")                   ,
    mp.game.joaat("v_res_m_dinechair")                  ,
    mp.game.joaat("v_res_m_h_sofa")                     ,
    mp.game.joaat("v_res_m_h_sofa_sml")                 ,
    mp.game.joaat("v_res_m_l_chair1")                   ,
    mp.game.joaat("v_res_mbchair")                      ,
    mp.game.joaat("v_res_mbottoman")                    ,
    mp.game.joaat("v_res_mp_stripchair")                ,
    mp.game.joaat("v_res_study_chair")                  ,
    mp.game.joaat("v_res_tre_chair")                    ,
    mp.game.joaat("v_res_tre_officechair")              ,
    //mp.game.joaat("v_res_tre_sofa")                     , // ?
    //mp.game.joaat("v_res_tre_sofa_mess_b")              , // ?
    //mp.game.joaat("v_res_tre_sofa _s")                  , // ?
    mp.game.joaat("v_res_tre_stool")                    ,
    mp.game.joaat("v_res_tre_stool_leather")            ,
    mp.game.joaat("v_ret_chair")                        ,
    mp.game.joaat("v_ret_chair_white")                  ,
    mp.game.joaat("v_ret_fh_chair01")                   ,
    mp.game.joaat("v_ret_gc_chair01")                   ,
    mp.game.joaat("v_ret_gc_chair02")                   ,
    mp.game.joaat("v_ret_gc_chair03")                   ,
    mp.game.joaat("v_ret_ps_chair")                     ,
    mp.game.joaat("xm_lab_chairarm_02")                 ,
    mp.game.joaat("xm_lab_chairarm_03")                 ,
    mp.game.joaat("xm_lab_chairarm_11")                 ,
    mp.game.joaat("xm_lab_chairarm_12")                 ,
    mp.game.joaat("xm_lab_chairarm_24")                 ,
    mp.game.joaat("xm_lab_chairarm_25")                 ,
    mp.game.joaat("xm_lab_chairarm_26")                 ,
    mp.game.joaat("v_res_fh_barcchair")                 ,
    mp.game.joaat("v_res_fh_dineeamesb")                ,
    mp.game.joaat("v_res_fh_dineeamesa")                ,
    mp.game.joaat("v_res_fh_easychair")                 ,
    mp.game.joaat("prop_bench_01a")                     ,
    mp.game.joaat("prop_bench_01b")                     ,
    mp.game.joaat("prop_bench_01c")                     ,
    mp.game.joaat("prop_bench_02")                      ,
    mp.game.joaat("prop_bench_03")                      ,
    mp.game.joaat("prop_bench_04")                      ,
    mp.game.joaat("prop_bench_05")                      ,
    mp.game.joaat("prop_bench_06")                      ,
    mp.game.joaat("prop_bench_05")                      ,
    mp.game.joaat("prop_bench_08")                      ,
    mp.game.joaat("prop_bench_09")                      ,
    mp.game.joaat("prop_bench_10")                      ,
    mp.game.joaat("prop_bench_11")                      ,
    mp.game.joaat("prop_fib_3b_bench")                  ,
    mp.game.joaat("prop_ld_bench01")                    ,
    mp.game.joaat("prop_wait_bench_01")                 ,
    mp.game.joaat("hei_prop_heist_off_chair")           ,
    mp.game.joaat("hei_prop_hei_skid_chair")            ,
    mp.game.joaat("prop_chair_01a")                     ,
    mp.game.joaat("prop_chair_01b")                     ,
    mp.game.joaat("prop_chair_02")                      ,
    mp.game.joaat("prop_chair_03")                      ,
    mp.game.joaat("prop_chair_04a")                     ,
    mp.game.joaat("prop_chair_04b")                     ,
    mp.game.joaat("prop_chair_05")                      ,
    mp.game.joaat("prop_chair_06")                      ,
    mp.game.joaat("prop_chair_05")                      ,
    mp.game.joaat("prop_chair_08")                      ,
    mp.game.joaat("prop_chair_09")                      ,
    mp.game.joaat("prop_chair_10")                      ,
    mp.game.joaat("prop_chateau_chair_01")              ,
    mp.game.joaat("prop_clown_chair")                   ,
    mp.game.joaat("prop_cs_office_chair")               ,
    mp.game.joaat("prop_direct_chair_01")               ,
    mp.game.joaat("prop_direct_chair_02")               ,
    mp.game.joaat("prop_gc_chair02")                    ,
    mp.game.joaat("prop_off_chair_01")                  ,
    mp.game.joaat("prop_off_chair_03")                  ,
    mp.game.joaat("prop_off_chair_04")                  ,
    mp.game.joaat("prop_off_chair_04b")                 ,
    mp.game.joaat("prop_off_chair_04_s")                ,
    mp.game.joaat("prop_off_chair_05")                  ,
    mp.game.joaat("prop_old_deck_chair")                ,
    mp.game.joaat("prop_old_wood_chair")                ,
    mp.game.joaat("prop_rock_chair_01")                 ,
    mp.game.joaat("prop_skid_chair_01")                 ,
    mp.game.joaat("prop_skid_chair_02")                 ,
    mp.game.joaat("prop_skid_chair_03")                 ,
    mp.game.joaat("prop_sol_chair")                     ,
    mp.game.joaat("prop_wheelchair_01")                 ,
    mp.game.joaat("prop_wheelchair_01_s")               ,
    mp.game.joaat("p_armchair_01_s")                    ,
    mp.game.joaat("p_clb_officechair_s")                ,
    mp.game.joaat("p_dinechair_01_s")                   ,
    mp.game.joaat("p_ilev_p_easychair_s")               ,
    mp.game.joaat("p_soloffchair_s")                    ,
    mp.game.joaat("p_yacht_chair_01_s")                 ,
    mp.game.joaat("v_club_officechair")                 ,
    mp.game.joaat("v_corp_bk_chair3")                   ,
    mp.game.joaat("v_corp_cd_chair")                    ,
    mp.game.joaat("v_corp_offchair")                    ,
    mp.game.joaat("v_ilev_chair02_ped")                 ,
    mp.game.joaat("v_ilev_hd_chair")                    ,
    mp.game.joaat("v_ilev_p_easychair")                 ,
    mp.game.joaat("v_ret_gc_chair03")                   ,
    mp.game.joaat("prop_ld_farm_chair01")               ,
    mp.game.joaat("prop_table_04_chr")                  ,
    mp.game.joaat("prop_table_05_chr")                  ,
    mp.game.joaat("prop_table_06_chr")                  ,
    mp.game.joaat("v_ilev_leath_chr")                   ,
    mp.game.joaat("prop_table_01_chr_a")                ,
    mp.game.joaat("prop_table_01_chr_b")                ,
    mp.game.joaat("prop_table_02_chr")                  ,
    mp.game.joaat("prop_table_03b_chr")                 ,
    mp.game.joaat("prop_table_03_chr")                  ,
    mp.game.joaat("prop_torture_ch_01")                 ,
    mp.game.joaat("v_ilev_fh_dineeamesa")               ,
    mp.game.joaat("v_ilev_fh_kitchenstool")             ,
    mp.game.joaat("v_ilev_tort_stool")                  ,
    mp.game.joaat("v_ilev_fh_kitchenstool")             ,
    mp.game.joaat("v_ilev_fh_kitchenstool")             ,
    mp.game.joaat("v_ilev_fh_kitchenstool")             ,
    mp.game.joaat("v_ilev_fh_kitchenstool")             ,
    mp.game.joaat("hei_prop_yah_seat_01")               ,
    mp.game.joaat("hei_prop_yah_seat_02")               ,
    mp.game.joaat("hei_prop_yah_seat_03")               ,
    mp.game.joaat("prop_waiting_seat_01")               ,
    mp.game.joaat("prop_yacht_seat_01")                 ,
    mp.game.joaat("prop_yacht_seat_02")                 ,
    mp.game.joaat("prop_yacht_seat_03")                 ,
    mp.game.joaat("prop_hobo_seat_01")                  ,
    mp.game.joaat("prop_rub_couch01")                   ,
    mp.game.joaat("miss_rub_couch_01")                  ,
    mp.game.joaat("prop_ld_farm_couch01")               ,
    mp.game.joaat("prop_ld_farm_couch02")               ,
    mp.game.joaat("prop_rub_couch02")                   ,
    mp.game.joaat("prop_rub_couch03")                   ,
    mp.game.joaat("prop_rub_couch04")                   ,
    mp.game.joaat("p_lev_sofa_s")                       ,
    mp.game.joaat("p_res_sofa_l_s")                     ,
    mp.game.joaat("p_v_med_p_sofa_s")                   ,
    mp.game.joaat("p_yacht_sofa_01_s")                  ,
    mp.game.joaat("v_ilev_m_sofa")                      ,
    mp.game.joaat("v_res_tre_sofa_s")                   ,
    mp.game.joaat("v_tre_sofa_mess_a_s")                ,
    mp.game.joaat("v_tre_sofa_mess_b_s")                ,
    mp.game.joaat("v_tre_sofa_mess_c_s")                ,
    mp.game.joaat("prop_roller_car_01")                 ,
    mp.game.joaat("prop_roller_car_02")                 ,
    mp.game.joaat("prop_a_base_bars_01")                ,
    mp.game.joaat("prop_beach_bars_02")                 ,
    mp.game.joaat("prop_pris_bars_01")                  ,
    mp.game.joaat("prop_beach_bars_01")                 ,
    mp.game.joaat("prop_muscle_bench_03")               ,
    mp.game.joaat("prop_pris_bench_01")                 ,
    mp.game.joaat("prop_weight_bench_02")               ,
    mp.game.joaat("prop_patio_lounger1")                ,
    mp.game.joaat("prop_patio_lounger_2")               ,
    mp.game.joaat("prop_patio_lounger_3")               ,
    mp.game.joaat("prop_chair_pile_01")                 ,
    mp.game.joaat("v_med_bed2")                         ,
    mp.game.joaat("v_med_bed1")                         ,
    mp.game.joaat("v_med_emptybed")                     ,
    mp.game.joaat("v_med_cor_medstool")                 ,
    mp.game.joaat("v_ind_meatbench")                    ,
    mp.game.joaat("v_corp_lngestool")                   ,
    mp.game.joaat("v_corp_lngestoolfd")                 ,
    mp.game.joaat("prop_bench_07")                                   ,
    mp.game.joaat("v_corp_lazychair")                                ,
    mp.game.joaat("prop_ven_market_stool")                           ,
    mp.game.joaat("ba_prop_int_edgy_stool")                          ,
    mp.game.joaat("apa_mp_h_yacht_stool_01")                         ,
    mp.game.joaat("v_ilev_m_dinechair")                              ,
    mp.game.joaat("prop_chair_07")                                   ,
    mp.game.joaat("gr_prop_gr_chair02_ped")                          ,
    mp.game.joaat("xm_prop_x17_avengerchair_02")                     ,
    mp.game.joaat("xm_lab_easychair_01")                             ,
    mp.game.joaat("xm_int_lev_sub_chair_01")                         ,
    mp.game.joaat("vw_prop_vw_offchair_03")                          ,
    mp.game.joaat("sm_prop_smug_offchair_01a")                       ,
    mp.game.joaat("sm_prop_offchair_smug_02")                        ,
    mp.game.joaat("sm_prop_offchair_smug_01")                        ,
    mp.game.joaat("prop_yaught_chair_01")                            ,
    mp.game.joaat("imp_prop_impexp_offchair_01a")                    ,
    mp.game.joaat("ch_prop_casino_track_chair_01")                   ,
    mp.game.joaat("ba_prop_battle_club_chair_03")                    ,
    mp.game.joaat("ba_prop_battle_club_chair_02")                    ,
    mp.game.joaat("ba_prop_battle_club_chair_01")                    ,    
]
let selecttestfsd = 0;
let objdata = null

let objdatap = null
let objdatar = null

async function RecreateObjectEditor() {
    mp.events.call('client.dropinfo.close');

    await global.loadModel(testfsd[selecttestfsd]);

    //mp.console.logError(`[RedAge] Debug loadModel: ${selecttestfsd}`, true);

    global.OnObjectEditor (testfsd[selecttestfsd], null, (pos, rot, _) => {
        objdata = mp.objects.new(testfsd[selecttestfsd], pos, {
            'rotation': new mp.Vector3(0, 0, rot),
            'dimension': global.localplayer.dimension
        });
        objdatap = pos;
        objdatar = rot;
    })
}

gm.events.add('client.editor.sit', () => {
	try
	{
        selecttestfsd = 0;

        if (objdata && mp.objects.exists(objdata))
            objdata.destroy();

		RecreateObjectEditor();
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "inventory/dropEditor", "client.editor.start", e.toString());
	}
});

mp.keys.bind(global.Keys.VK_Q, true, function () {
    if(!global.isEditor)
        return;

    if (selecttestfsd > 0)
        selecttestfsd--;

    RecreateObjectEditor();
});

mp.keys.bind(global.Keys.VK_E, true, function () { 
    if(!global.isEditor)
        return;

    if (selecttestfsd < testfsd.length - 1)
        selecttestfsd++;
        
    RecreateObjectEditor();
});

mp.keys.bind(global.Keys.VK_LEFT, true, function () {
    if(!sitData.objectsInfo[testfsd[selecttestfsd]])
        return;

    if (!sitData.objectsInfo[testfsd[selecttestfsd]] || !sitData.objectsInfo[testfsd[selecttestfsd]].zOffset)
        sitData.objectsInfo[testfsd[selecttestfsd]].zOffset = 0;

    sitData.objectsInfo[testfsd[selecttestfsd]].zOffset -= 0.1;
    if (sitData.objectsInfo[testfsd[selecttestfsd]].zOffset < -5)
        sitData.objectsInfo[testfsd[selecttestfsd]].zOffset = 0;
});

mp.keys.bind(global.Keys.VK_RIGHT, true, function () { 
    if(!sitData.objectsInfo[testfsd[selecttestfsd]])
        return;

    if (!sitData.objectsInfo[testfsd[selecttestfsd]].zOffset)
        sitData.objectsInfo[testfsd[selecttestfsd]].zOffset = 0;

    sitData.objectsInfo[testfsd[selecttestfsd]].zOffset += 0.1;
    if (sitData.objectsInfo[testfsd[selecttestfsd]].zOffset > 15)
        sitData.objectsInfo[testfsd[selecttestfsd]].zOffset = 0;
});


let selectsda = 0;
mp.keys.bind(global.Keys.VK_UP, true, function () {
    if(!global.isEditor)
        return;

    selectsda--;

    switch (selectsda) {
        case 1: {
            sitData.objectsInfo[testfsd[selecttestfsd]].typeFlags = sitData.defaultData.yMinus;
            break;
        }
        case 2: {
            sitData.objectsInfo[testfsd[selecttestfsd]].typeFlags = sitData.defaultData.yPlus;
            break;
        }
        case 3: {
            sitData.objectsInfo[testfsd[selecttestfsd]].typeFlags = sitData.defaultData.xMinus;
            break;
        }
        case 4: {
            sitData.objectsInfo[testfsd[selecttestfsd]].typeFlags = sitData.defaultData.xPlus;
            break;
        }
        default: {
            selectsda = 0;
            delete sitData.objectsInfo[testfsd[selecttestfsd]].typeFlags;
            break;
        }
    }
});

mp.keys.bind(global.Keys.VK_DOWN, true, function () {
    if(!global.isEditor)
        return;

    selectsda++;

    switch (selectsda) {
        case 1: {
            sitData.objectsInfo[testfsd[selecttestfsd]].typeFlags = sitData.defaultData.yMinus;
            break;
        }
        case 2: {
            sitData.objectsInfo[testfsd[selecttestfsd]].typeFlags = sitData.defaultData.yPlus;
            break;
        }
        case 3: {
            sitData.objectsInfo[testfsd[selecttestfsd]].typeFlags = sitData.defaultData.xMinus;
            break;
        }
        case 4: {
            sitData.objectsInfo[testfsd[selecttestfsd]].typeFlags = sitData.defaultData.xPlus;
            break;
        }
        default: {
            selectsda = 0;
            delete sitData.objectsInfo[testfsd[selecttestfsd]].typeFlags;
            break;
        }
    }
});

/*
new class extends debugRender {
	constructor() {
		super("r_synchronization_sit");
	}

	render () {
		if (!objdata)
			return;
		mp.game.graphics.drawText(`index - ${selecttestfsd}~n~zOffset: ${sitData.objectsInfo[testfsd[selecttestfsd]].zOffset}~n~typeFlags: ${sitData.objectsInfo[testfsd[selecttestfsd]].typeFlags}`, [0.5, 0.9], {
			font: 4,
			color: [255, 255, 255, 255],
			scale: [0.4, 0.4],
			centre: true,
		});
	}
};*/