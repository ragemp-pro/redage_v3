using Database;
using GTANetworkAPI;
using NeptuneEvo.Handles;
using LinqToDB;
using NeptuneEvo.Accounts;
using NeptuneEvo.Players.Models;
using NeptuneEvo.Players;
using NeptuneEvo.Character.Models;
using NeptuneEvo.Character;
using NeptuneEvo.Chars;
using Newtonsoft.Json;
using Redage.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Localization;
using NeptuneEvo.Quests;

namespace NeptuneEvo.Core
{
    class Animations : Script
    {
        internal class Animation
        {
            public string Dictionary { get; }
            public string Name { get; }
            public int Flag { get; }

            public Animation(string dict, string name, int flag)
            {
                Dictionary = dict;
                Name = name;
                Flag = flag;
            }
        }
        private static readonly nLog Log = new nLog("Core.Animations");
        private static IReadOnlyDictionary<string, Animation> AnimList = new Dictionary<string, Animation>()
        {
          {"1_1", new Animation("anim@amb@business@cfid@cfid_desk_no_work_bgen_chair_no_work@", "leg_smacking_lazyworker", 39)},
{"1_2", new Animation("mp_am_stripper", "lap_dance_player", 39)},
{"1_3", new Animation("anim@amb@business@cfm@cfm_machine_no_work@", "transition_sleep_v1_operator", 39)},
{"1_4", new Animation("missdrfriedlanderdrf_idles", "drf_idle_drf", 39)},
{"1_5", new Animation("anim@amb@business@cfid@cfid_desk_no_work_bgen_chair_no_work@", "look_around_v1_lazyworkerfemale", 39)},
{"1_6", new Animation("anim@amb@business@bgen@bgen_no_work@", "sit_phone_phoneputdown_idle-noworkfemale", 39)},
{"1_7", new Animation("anim@amb@business@bgen@bgen_no_work@", "sit_phone_phoneputdown_idle_nowork", 39)},
{"1_8", new Animation("amb@code_human_in_bus_passenger_idles@male@sit@idle_a", "idle_c", 39)},
{"1_9", new Animation("rcm_barry3", "barry_3_sit_loop", 39)},
{"1_10", new Animation("anim@amb@business@bgen@bgen_no_work@", "sit_phone_phoneputdown_fallasleep_nowork", 39)},
{"1_11", new Animation("switch@trevor@on_toilet", "trev_on_toilet_loop", 39)},
{"1_12", new Animation("anim@amb@business@bgen@bgen_no_work@", "sit_phone_phoneputdown_wakeup_nowork", 39)},
{"1_13", new Animation("anim@heists@fleeca_bank@ig_7_jetski_owner", "owner_idle", 39)},
{"1_14", new Animation("amb@lo_res_idles@", "world_human_picnic_female_lo_res_base", 39)},
{"1_15", new Animation("anim@amb@business@bgen@bgen_no_work@", "sit_phone_idle_01_nowork", 39)},
{"1_16", new Animation("anim@amb@business@bgen@bgen_no_work@", "sit_phone_idle_01-noworkfemale", 39)},
{"1_17", new Animation("missheist_jewel", "jh_2b_endloop_male2", 39)},
{"1_18", new Animation("switch@trevor@annoys_sunbathers", "trev_annoys_sunbathers_loop_girl", 39)},
{"1_19", new Animation("switch@trevor@annoys_sunbathers", "trev_annoys_sunbathers_loop_guy", 39)},
{"1_20", new Animation("missheistdockssetup1ig_10@base", "talk_pipe_base_worker1", 39)},
{"1_21", new Animation("anim@amb@office@boardroom@boss@male@", "idle_c", 39)},
{"1_22", new Animation("anim@amb@range@assemble_guns@", "expel_cartridge_01_amy_skater_01", 39)},
{"1_23", new Animation("amb@prop_human_seat_bar@male@hands_on_bar@base", "base", 39)},
{"1_24", new Animation("anim@special_peds@casino@vince@steps@cas_vince_ig2", "cas_vince_ig2_id_go_home", 39)},
{"1_25", new Animation("switch@michael@sitting", "idle", 39)},
{"1_26", new Animation("timetable@michael@on_chairbase", "on_chair_base", 39)},
{"1_27", new Animation("random@robbery", "sit_down_idle_01", 39)},
{"1_28", new Animation("anim@amb@casino@out_of_money@ped_male@02b@idles", "idle_a", 39)},
{"1_29", new Animation("anim@heists@prison_heistunfinished_biz@popov_react", "popov_cower", 39)},
{"1_30", new Animation("amb@code_human_cower@male@react_cowering", "base_front", 39)},
{"1_31", new Animation("timetable@trevor@smoking_meth@base", "base", 39)},
{"1_32", new Animation("switch@trevor@mocks_lapdance", "001443_01_trvs_28_idle_man", 39)},
{"1_33", new Animation("switch@michael@sunlounger", "sunlounger_idle", 39)},
{"1_34", new Animation("saveveniceb@", "t_sleep_r_loop_veniceb", 39)},
{"1_35", new Animation("timetable@michael@on_clubchairidle_a", "on_clubchair_b", 39)},
{"1_36", new Animation("switch@michael@lounge_chairs", "001523_01_mics3_9_lounge_chairs_idle_ama", 39)},
{"1_37", new Animation("amb@prop_human_seat_sunlounger@male@idle_a", "idle_a", 39)},
{"1_38", new Animation("amb@prop_human_seat_sunlounger@female@idle_a", "idle_a", 39)},
{"1_39", new Animation("amb@prop_human_seat_sunlounger@female@base", "base", 39)},
{"1_40", new Animation("anim@amb@business@cfm@cfm_machine_no_work@", "transition_sleep_v1_operator", 39)},
{"1_41", new Animation("anim@amb@business@bgen@bgen_no_work@", "sit_phone_idle_02_nowork", 39)},
{"1_42", new Animation("amb@lo_res_idles@", "world_human_picnic_female_lo_res_base", 39)},
{"1_43", new Animation("anim@amb@business@bgen@bgen_no_work@", "sit_phone_phoneputdown_fallasleep_nowork", 39)},
{"1_44", new Animation("misstrevor1", "ortega_outro_loop_ort", 39)},
{"1_45", new Animation("rcmjosh3", "sit_stairs_idle", 39)},
{"1_46", new Animation("anim@heists@fleeca_bank@ig_7_jetski_owner", "owner_idle", 39)},
{"1_47", new Animation("amb@medic@standing@tendtodead@enter", "enter", 39)},
{"1_48", new Animation("amb@medic@standing@kneel@base", "base", 39)},
{"1_49", new Animation("amb@prop_human_seat_bar@female@left_elbow_on_bar@idle_a", "idle_a", 39)},
{"1_50", new Animation("anim@amb@business@bgen@bgen_no_work@", "sit_phone_phoneputdown_sleeping-noworkfemale", 39)},
{"1_51", new Animation("anim@amb@yacht@jacuzzi@seated@female@variation_01@", "base", 39)},
{"1_52", new Animation("anim@amb@yacht@jacuzzi@seated@female@variation_02@", "base", 39)},
{"1_53", new Animation("anim@amb@yacht@jacuzzi@seated@female@variation_05@", "base", 39)},
{"1_54", new Animation("anim@amb@yacht@jacuzzi@seated@male@variation_03@", "base", 39)},
{"1_55", new Animation("anim@amb@casino@out_of_money@ped_female@01a@idles", "idle_d", 39)},
{"1_56", new Animation("rcmnigel1a_band_groupies", "base_m1", 39)},
{"1_57", new Animation("rcmtmom_1leadinout", "tmom_1_rcm_p3_leadout_loop", 39)},
{"1_58", new Animation("timetable@reunited@ig_10", "base_jimmy", 39)},
{"1_59", new Animation("timetable@reunited@ig_10", "isthisthebest_amanda", 39)},
{"1_60", new Animation("timetable@reunited@ig_10", "watching_this_amanda", 39)},
{"1_61", new Animation("safe@franklin@ig_13", "tv_idle_a", 39)},
{"1_62", new Animation("switch@trevor@floyd_crying", "console_end_loop_floyd", 39)},
{"1_63", new Animation("switch@trevor@mocks_lapdance", "001443_01_trvs_28_exit_man", 39)},
{"1_64", new Animation("anim@amb@nightclub@lazlow@lo_alone@", "lowalone_base_laz", 39)},
{"1_65", new Animation("timetable@tracy@ig_14@", "ig_14_idle_b_reallyloveudad_tracy", 39)},
{"1_66", new Animation("anim@amb@clubhouse@boss@female@", "idle_a", 39)},
{"1_67", new Animation("anim@amb@clubhouse@boss@female@", "idle_b", 39)},
{"1_68", new Animation("anim@amb@clubhouse@boss@female@", "idle_c", 39)},


{"2_1", new Animation("random@arrests@busted", "idle_c", 49)},
{"2_2", new Animation("misschinese1leadinoutchinese_1_int", "russ_leadin_loop", 39)},
{"2_3", new Animation("special_ped@baygor@monologue_6@monologue_6k", "salvation_comes_at_a_price_10", 39)},
{"2_4", new Animation("oddjobs@towingangryidle_a", "idle_c", 49)},
{"2_5", new Animation("amb@code_human_police_crowd_control@idle_a", "idle_c", 39)},
{"2_6", new Animation("random@bus_tour_guide@idle_a", "idle_b", 48)},
{"2_7", new Animation("anim@mp_player_intcelebrationfemale@bro_love", "bro_love", 0)},
{"2_8", new Animation("gestures@f@standing@casual", "gesture_hand_down", 48)},
{"2_9", new Animation("anim@mp_celebration@draw@male", "draw_react_male_a", 48)},
{"2_10", new Animation("gestures@f@standing@casual", "getsure_its_mine", 48)},
{"2_11", new Animation("anim@heists@ornate_bank@chat_manager", "fail", 48)},
{"2_12", new Animation("gestures@f@standing@casual", "gesture_damn", 48)},
{"2_13", new Animation("gestures@f@standing@casual", "gesture_bye_hard", 48)},
{"2_14", new Animation("gestures@f@standing@casual", "gesture_displeased", 48)},
{"2_15", new Animation("amb@code_human_in_car_mp_actions@nod@bodhi@ds@base", "nod_no_fp", 39)},
{"2_16", new Animation("gestures@f@standing@casual", "gesture_me", 48)},
{"2_17", new Animation("gestures@f@standing@casual", "gesture_you_hard", 48)},
{"2_18", new Animation("gestures@f@standing@casual", "gesture_point", 49)},
{"2_19", new Animation("gestures@f@standing@casual", "gesture_no_way", 48)},
{"2_20", new Animation("gestures@f@standing@casual", "gesture_nod_yes_soft", 48)},
{"2_21", new Animation("gestures@f@standing@casual", "gesture_come_here_soft", 48)},
{"2_22", new Animation("gestures@m@standing@fat", "gesture_bye_soft", 0)},
{"2_23", new Animation("special_ped@jerome@monologue_6@monologue_6g", "youthinkyourhappy_6", 48)},
{"2_24", new Animation("gestures@f@standing@casual", "gesture_bye_soft", 48)},
{"2_25", new Animation("anim@mp_player_intincarthumbs_upbodhi@ds@", "enter_fp", 48)},
{"2_26", new Animation("gestures@m@standing@casual", "gesture_no_way", 0)},
{"2_27", new Animation("mini@dartsoutro", "darts_outro_03_guy1", 0)},
{"2_28", new Animation("amb@prop_human_seat_computer@male@react_cowering", "cover_enter", 48)},
{"2_29", new Animation("anim@mp_player_intcelebrationfemale@face_palm", "face_palm", 0)},
{"2_30", new Animation("anim@mp_player_intcelebrationfemale@you_loco", "you_loco", 0)},
{"2_31", new Animation("anim@mp_parachute_outro@female@lose", "lose_loop", 0)},
{"2_32", new Animation("missarmenian2", "lamar_impatient_a", 48)},
{"2_33", new Animation("anim@mp_player_intcelebrationfemale@face_palm", "face_palm", 48)},
{"2_34", new Animation("random@hitch_lift", "come_here_idle_c", 48)},
{"2_35", new Animation("taxi_hail", "fp_hail_taxi", 0)},
{"2_36", new Animation("anim@arena@celeb@podium@no_prop@", "regal_a_1st", 0)},
{"2_37", new Animation("switch@franklin@cleaning_apt", "001918_01_fras_v2_1_cleaning_apt_idle", 1)},
{"2_38", new Animation("gestures@m@standing@casual", "gesture_i_will", 0)},
{"2_39", new Animation("anim@heists@ornate_bank@chat_manager", "charm", 48)},
{"2_40", new Animation("gestures@f@standing@casual", "gesture_hello", 48)},
{"2_41", new Animation("random@arrests", "thanks_male_05", 49)},
{"2_42", new Animation("random@arrests", "thanks_male_05", 49)},
{"2_43", new Animation("gestures@f@standing@casual", "gesture_easy_now", 48)},
{"2_44", new Animation("gestures@f@standing@casual", "gesture_i_will", 48)},
{"2_45", new Animation("amb@code_human_wander_idles@male@idle_a", "idle_a_wristwatch", 49)}, 
{"2_46", new Animation("missheist_jewel@hacking", "hack_loop", 49)},
{"2_47", new Animation("timetable@jimmy@doorknock@", "knockdoor_idle", 49)},
{"2_48", new Animation("switch@trevor@mocks_lapdance", "001443_01_trvs_28_idle_stripper", 1)},
{"2_49", new Animation("anim@heists@team_respawn@respawn_02", "heist_spawn_02_ped_d", 0)},
{"2_50", new Animation("amb@world_human_cop_idles@female@idle_b", "idle_d", 48)},
{"2_51", new Animation("move_crawl", "onfront_bwd", 1)},
{"2_52", new Animation("amb@world_human_bum_wash@male@high@idle_a", "idle_a", 0)},
{"2_53", new Animation("missfbi5misc", "plyr_roll_left", 0)},
{"2_54", new Animation("anim@mp_player_intcelebrationpaired@m_m_sarcastic", "sarcastic_left", 39)},
{"2_55", new Animation("anim@arena@celeb@podium@no_prop@", "crowd_point_a_1st", 1)},
{"2_56", new Animation("timetable@floyd@clean_kitchen@idle_a", "idle_b", 49)},
{"2_57", new Animation("mp_player_int_uppersalute", "mp_player_int_salute", 49)},
{"2_58", new Animation("amb@code_human_wander_mobile@female@base", "base", 49)},
{"2_59", new Animation("anim@heists@box_carry@", "idle", 49)},
{"2_60", new Animation("mp_intro_seq@", "mp_mech_fix", 49)},
{"2_61", new Animation("amb@medic@standing@kneel@enter", "enter", 0)},
{"2_62", new Animation("anim@mp_player_intincarslow_clapbodhi@ds@", "idle_a", 49)},
{"2_63", new Animation("anim@arena@celeb@flat@solo@no_props@", "jump_d_player_a", 0)},
{"2_64", new Animation("amb@code_human_police_investigate@idle_a", "idle_a", 0)},
{"2_65", new Animation("avoidsavoids_b_v05", "frback_tofront", 0)},
{"2_66", new Animation("gestures@m@standing@casual", "gesture_what_soft", 0)},

{"3_1", new Animation("amb@lo_res_idles@", "world_human_lean_male_hands_together_lo_res_base", 39)},
{"3_2", new Animation("amb@lo_res_idles@", "world_human_lean_female_holding_elbow_lo_res_base", 39)},
{"3_3", new Animation("random@hitch_lift", "idle_a", 39)},
{"3_4", new Animation("rcmnigel1aig_1", "base_02_willie", 39)},
{"3_5", new Animation("missfbi4leadinoutfbi_4_int", "fbi_4_int_trv_idle_dave", 39)},
{"3_6", new Animation("switch@franklin@gang_taunt_p5", "fras_ig_6_p5_loop_g2", 39)},
{"3_7", new Animation("amb@world_human_hang_out_street@female_arms_crossed@base", "base", 39)},
{"3_8", new Animation("amb@world_human_leaning@male@wall@back@foot_up@idle_a", "idle_a", 39)},
{"3_9", new Animation("anim@amb@clubhouse@bar@bartender@", "base_bartender", 39)},
{"3_10", new Animation("anim@amb@business@bgen@bgen_no_work@", "stand_phone_phoneputdown_idle_nowork", 39)},
{"3_11", new Animation("low_fun_mcs1-3", "mp_m_g_vagfun_01^6_dual-3", 39)},
{"3_12", new Animation("rcmme_amanda1", "stand_loop_cop", 39)},
{"3_13", new Animation("amb@world_human_cop_idles@female@idle_b", "idle_e", 39)},
{"3_14", new Animation("random@street_race", "_car_b_lookout", 39)},
{"3_15", new Animation("anim@amb@nightclub@peds@", "rcmme_amanda1_stand_loop_cop", 39)},
{"3_16", new Animation("amb@world_human_hang_out_street@female_arms_crossed@base", "base", 39)},
{"3_17", new Animation("random@arrests", "kneeling_arrest_idle", 39)},
{"3_18", new Animation("anim@heists@fleeca_bank@hostages@ped_d@", "flinch_loop", 39)},
{"3_19", new Animation("anim@heists@ornate_bank@hostages@ped_g@", "flinch_loop", 39)},
{"3_20", new Animation("random@hitch_lift", "f_distressed_loop", 39)},
{"3_21", new Animation("rcmpaparazzo_3big_1", "_action_guard_a", 39)},
{"3_22", new Animation("anim@move_hostages@male", "male_idle", 39)},
{"3_23", new Animation("anim@miss@low@fin@vagos@", "idle_ped06", 39)},
{"3_24", new Animation("amb@world_human_stand_guard@male@base", "base", 39)},
{"3_25", new Animation("missfbi4mcs_2", "loop_sec_b", 39)},
{"3_26", new Animation("anim@mp_freemode_return@f@idle", "idle_c", 39)},
{"3_27", new Animation("missprologueig_2", "idle_on_floor_malehostage02", 39)},
{"3_28", new Animation("missheistdockssetup1ig_13@kick_idle", "guard_beatup_kickidle_dockworker", 39)},
{"3_29", new Animation("timetable@tracy@sleep@", "idle_c", 39)},
{"3_30", new Animation("anim@gangops@morgue@table@", "body_search", 39)},
{"3_31", new Animation("missarmenian2", "corpse_search_exit_ped", 39)},
{"3_32", new Animation("missfbi1", "cpr_pumpchest_idle", 39)},
{"3_33", new Animation("misslamar1dead_body", "dead_idle", 39)},
{"3_34", new Animation("missarmenian2", "drunk_loop", 39)},
{"3_35", new Animation("dead", "dead_d", 39)},
{"3_36", new Animation("amb@world_human_sunbathe@male@front@base", "base", 39)},
{"3_37", new Animation("amb@world_human_bum_slumped@male@laying_on_right_side@base", "base", 39)},
{"3_38", new Animation("anim@arena@celeb@podium@no_prop@", "regal_c_3rd", 39)},
{"3_39", new Animation("anim@arena@celeb@podium@no_prop@", "dance_b_3rd", 39)},
{"3_40", new Animation("anim@arena@celeb@podium@no_prop@", "crowd_point_a_2nd", 39)},
{"3_41", new Animation("missheistdockssetup1ig_10@idle_d", "talk_pipe_d_worker2", 39)},
{"3_42", new Animation("anim@amb@business@bgen@bgen_no_work@", "stand_phone_phoneputdown_stretching-noworkfemale", 39)},
{"3_43", new Animation("friends@frt@ig_1", "trevor_impatient_wait_4", 39)},
{"3_44", new Animation("anim@amb@business@bgen@bgen_no_work@", "stand_phone_phoneputdown_sleeping_nowork", 39)},
{"3_45", new Animation("anim@miss@low@fin@vagos@", "idle_ped05", 39)},
{"3_46", new Animation("low_fun_mcs1-3", "mp_m_g_vagfun_01^12_dual-3", 39)},
{"3_47", new Animation("anim@amb@casino@hangout@ped_female@stand@01a@idles", "idle_d", 39)},
{"3_48", new Animation("anim@amb@business@bgen@bgen_no_work@", "stand_phone_lookaround_nowork", 39)},
{"3_49", new Animation("anim@amb@casino@hangout@ped_female@stand@02a@base", "base", 39)},
{"3_50", new Animation("amb@world_human_leaning@female@wall@back@holding_elbow@base", "base", 39)},
{"3_51", new Animation("switch@michael@parkbench_smoke_ranger", "ranger_nervous_loop", 39)},
{"3_52", new Animation("martin_1_int-0", "cs_martinmadrazo_dual-0", 39)},
{"3_53", new Animation("amb@code_human_police_investigate@base", "base", 39)},
{"3_54", new Animation("fbi_3_int-2", "cs_stevehains_dual-2", 39)},
{"3_55", new Animation("timetable@amanda@ig_9", "ig_9_base_amanda", 39)},
{"3_56", new Animation("anim@amb@yacht@rail@standing@male@variant_02@", "base", 39)},
{"3_57", new Animation("anim@amb@yacht@rail@standing@female@variant_01@", "base", 39)},
{"3_58", new Animation("low_fun_mcs1-3", "mp_m_g_vagfun_01^5_dual-3", 39)},
{"3_59", new Animation("low_fun_mcs1-3", "mp_m_g_vagfun_01^13_dual-3", 39)},
{"3_60", new Animation("missmurder", "idle", 39)},
{"3_61", new Animation("missfbi5ig_0", "lyinginpain_loop_steve", 39)},
{"3_62", new Animation("random@burial", "b_burial", 39)},
{"3_63", new Animation("armenian_1_int-0", "a_f_y_bevhills_01-0", 39)},
{"3_64", new Animation("missfbi3_sniping", "prone_dave", 39)},
{"3_65", new Animation("random@street_race", "_car_a_flirt_girl", 39)},
{"3_66", new Animation("rcmme_tracey1", "nervous_loop", 39)},
{"3_67", new Animation("anim@amb@casino@hangout@ped_male@stand@02b@idles", "idle_a", 39)},
{"3_68", new Animation("misscarsteal2pimpsex", "pimpsex_pimp", 39)},
{"3_69", new Animation("anim@mp_player_intselfieblow_kiss", "idle_a", 39)},
{"3_70", new Animation("amb@prop_human_seat_bar@male@hands_on_bar@react_cower", "enter_left", 39)},
{"3_71", new Animation("mini@hookers_spcokehead", "idle_wait", 39)},
{"3_72", new Animation("amb@world_human_sunbathe@female@back@idle_a", "idle_a", 39)},
{"3_73", new Animation("anim@amb@nightclub@dancers@club_ambientpeds@low-med_intensity", "li-mi_amb_club_10_v1_male^3", 39)},
{"3_74", new Animation("amb@world_human_sunbathe@female@front@idle_a", "idle_a", 39)},
{"3_75", new Animation("amb@world_human_sunbathe@female@front@idle_a", "idle_b", 39)},
{"3_76", new Animation("anim@amb@casino@shop@ped_female@01a@idles", "idle_b", 39)},
{"3_77", new Animation("anim@amb@casino@shop@ped_male@01a@idles", "idle_a", 39)},
{"3_78", new Animation("mini@strip_club@idles@bouncer@idle_a", "idle_a", 39)},
{"3_79", new Animation("mini@hookers_spvanilla", "idle_wait", 39)},
{"3_80", new Animation("mini@hookers_spcrackhead", "idle_b", 39)},
{"3_81", new Animation("mini@hookers_spcrackhead", "idle_c", 39)},

{"4_1", new Animation("anim@mp_player_intselfiethe_bird", "idle_a", 49)},
{"4_2", new Animation("anim@mp_player_intincardockstd@ps@", "idle_a", 49)},
{"4_3", new Animation("anim@mp_player_intuppernose_pick", "idle_a", 49)},
{"4_4", new Animation("anim@mp_player_intupperfinger", "idle_a", 49)},
{"4_5", new Animation("anim@mp_player_intcelebrationfemale@blow_kiss", "blow_kiss", 39)},
{"4_6", new Animation("anim@mp_player_intcelebrationfemale@chin_brush", "chin_brush", 39)},
{"4_7", new Animation("anim@mp_player_intcelebrationfemale@finger_kiss", "finger_kiss", 39)},
{"4_8", new Animation("amb@code_human_in_car_mp_actions@tit_squeeze@low@ps@base", "idle_a", 49)},
{"4_9", new Animation("anim@mp_player_intcelebrationfemale@air_shagging", "air_shagging", 39)},
{"4_10", new Animation("anim@mp_player_intcelebrationfemale@dock", "dock", 39)},
{"4_11", new Animation("rcmpaparazzo_2", "shag_loop_a", 39)},
{"4_12", new Animation("rcmpaparazzo_2", "shag_loop_poppy", 39)},
{"4_13", new Animation("timetable@trevor@skull_loving_bear", "skull_loving_bear", 39)},
{"4_14", new Animation("mp_ped_interaction", "kisses_guy_b", 39)},
{"4_15", new Animation("misscarsteal2pimpsex", "shagloop_hooker", 39)},
{"4_16", new Animation("misscarsteal2pimpsex", "shagloop_pimp", 39)},
{"4_17", new Animation("mini@hookers_spvanilla", "idle_a", 39)},
{"4_18", new Animation("mini@strip_club@private_dance@idle", "priv_dance_idle", 39)},
{"4_19", new Animation("mp_am_stripper", "lap_dance_girl", 39)},
{"4_20", new Animation("mini@strip_club@private_dance@part3", "priv_dance_p3", 39)},
{"4_21", new Animation("mini@strip_club@private_dance@part2", "priv_dance_p2", 39)},
{"4_22", new Animation("mini@strip_club@idles@stripper", "stripper_idle_04", 39)},
{"4_23", new Animation("mini@strip_club@lap_dance@ld_girl_a_song_a_p1", "ld_girl_a_song_a_p1_f", 39)},
{"4_24", new Animation("mini@strip_club@lap_dance@ld_girl_a_song_a_p2", "ld_girl_a_song_a_p2_f", 39)},
{"4_25", new Animation("mini@strip_club@lap_dance@ld_girl_a_song_a_p3", "ld_girl_a_song_a_p3_f", 39)},
{"4_26", new Animation("mini@strip_club@pole_dance@pole_dance3", "pd_dance_03", 39)},

{"5_1", new Animation("amb@world_human_push_ups@male@base", "base", 39)},
{"5_2", new Animation("amb@world_human_sit_ups@male@base", "base", 39)},
{"5_3", new Animation("anim@mp_player_intcelebrationmale@peace", "peace", 39)},
{"5_4", new Animation("missfam1_yachtbattleonyacht02_", "onboom_twohand_hang_idle", 39)},
{"5_5", new Animation("mini@triathlon", "idle_a", 39)},
{"5_6", new Animation("mini@triathlon", "idle_b", 39)},
{"5_7", new Animation("mini@triathlon", "idle_d", 39)},
{"5_8", new Animation("mini@triathlon", "idle_f", 39)},
{"5_9", new Animation("amb@world_human_muscle_flex@arms_at_side@idle_a", "idle_a", 39)},
{"5_10", new Animation("amb@world_human_muscle_flex@arms_at_side@idle_a", "idle_c", 39)},
{"5_11", new Animation("amb@world_human_muscle_flex@arms_in_front@base", "base", 39)},
{"5_12", new Animation("rcmcollect_paperleadinout@", "meditiate_idle", 39)},
{"5_13", new Animation("missfam5_yoga", "f_yogapose_a", 39)},
{"5_14", new Animation("missfam5_yoga", "c8_pose", 39)},
{"5_15", new Animation("missfam5_yoga", "b4_fail_to_start", 39)},
{"5_16", new Animation("missfam5_yoga", "start_to_c1", 39)},
{"5_17", new Animation("missfam5_yoga", "start_to_a1", 39)},
{"5_18", new Animation("missfam5_yoga", "a2_to_a3", 39)},
{"5_19", new Animation("missfam5_yoga", "a3_fail_to_start", 39)},
{"5_20", new Animation("amb@world_human_yoga@female@base", "base_a", 39)},
{"5_21", new Animation("mini@yoga", "outro_2", 39)},
{"5_22", new Animation("missfam5_yoga", "c5_pose", 39)},
{"5_23", new Animation("missfam5_yoga", "a2_pose", 39)},
{"5_24", new Animation("missfam5_yoga", "c6_pose", 39)},
{"5_25", new Animation("amb@world_human_yoga@female@base", "base_c", 39)},

{"6_1", new Animation("special_ped@mountain_dancer@monologue_1@monologue_1a", "mtn_dnc_if_you_want_to_get_to_heaven", 39)},
{"6_2", new Animation("anim@amb@nightclub@mini@dance@dance_solo@male@var_a@", "high_center_up", 39)},
{"6_3", new Animation("anim@amb@nightclub@mini@dance@dance_solo@female@var_a@", "high_center_down", 39)},
{"6_4", new Animation("anim@amb@nightclub@mini@dance@dance_solo@female@var_a@", "low_center_up", 39)},
{"6_5", new Animation("anim@amb@nightclub@dancers@crowddance_facedj@low_intesnsity", "li_dance_facedj_09_v1_male^2", 39)},
{"6_6", new Animation("anim@amb@nightclub@dancers@crowddance_facedj@low_intesnsity", "li_dance_facedj_09_v1_male^1", 39)},
{"6_7", new Animation("anim@amb@nightclub@lazlow@hi_railing@", "ambclub_12_mi_hi_bootyshake_laz", 39)},
{"6_8", new Animation("missfbi3_sniping", "dance_m_default", 39)},
{"6_9", new Animation("amb@world_human_partying@female@partying_beer@base", "base", 39)},
{"6_10", new Animation("amb@world_human_strip_watch_stand@male_a@idle_a", "idle_c", 39)},
{"6_11", new Animation("mini@strip_club@idles@dj@idle_04", "idle_04", 39)},
{"6_12", new Animation("amb@world_human_prostitute@cokehead@idle_a", "idle_a", 39)},
{"6_13", new Animation("amb@world_human_prostitute@cokehead@idle_a", "idle_c", 39)},
{"6_14", new Animation("mini@strip_club@lap_dance_2g@ld_2g_p2", "ld_2g_p2_s1", 39)},
{"6_15", new Animation("misschinese2_crystalmazemcs1_ig", "dance_loop_tao", 39)},
{"6_16", new Animation("missfbi3_sniping", "dance_m_default", 39)},
{"6_17", new Animation("amb@world_human_partying@female@partying_beer@idle_a", "idle_b", 39)},
{"6_18", new Animation("rcmnigel1bnmt_1b", "dance_intro_tyler", 39)},
{"6_19", new Animation("mini@strip_club@lap_dance_2g@ld_2g_p3", "ld_2g_p3_s2", 39)},
{"6_20", new Animation("anim@amb@casino@mini@dance@dance_solo@female@var_b@", "high_center_down", 39)},
{"6_21", new Animation("anim@amb@casino@mini@dance@dance_solo@female@var_b@", "high_center_up", 39)},
{"6_22", new Animation("anim@amb@casino@mini@dance@dance_solo@female@var_b@", "high_left", 39)},
{"6_23", new Animation("anim@amb@casino@mini@dance@dance_solo@female@var_b@", "high_left_down", 39)},
{"6_24", new Animation("anim@amb@casino@mini@dance@dance_solo@female@var_b@", "high_left_up", 39)},
{"6_25", new Animation("anim@amb@casino@mini@dance@dance_solo@female@var_b@", "high_right", 39)},
{"6_26", new Animation("anim@amb@casino@mini@dance@dance_solo@female@var_b@", "high_right_down", 39)},
{"6_27", new Animation("anim@amb@casino@mini@dance@dance_solo@female@var_b@", "high_right_up", 39)},
{"6_28", new Animation("anim@amb@casino@mini@dance@dance_solo@female@var_b@", "low_center", 39)},
{"6_29", new Animation("anim@amb@casino@mini@dance@dance_solo@female@var_b@", "low_center_down", 39)},
{"6_30", new Animation("anim@amb@casino@mini@dance@dance_solo@female@var_b@", "low_center_up", 39)},
{"6_31", new Animation("anim@amb@casino@mini@dance@dance_solo@female@var_b@", "low_left", 39)},
{"6_32", new Animation("anim@amb@casino@mini@dance@dance_solo@female@var_b@", "low_left_down", 39)},
{"6_33", new Animation("anim@amb@casino@mini@dance@dance_solo@female@var_b@", "low_left_up", 39)},
{"6_34", new Animation("anim@amb@casino@mini@dance@dance_solo@female@var_b@", "low_right", 39)},
{"6_35", new Animation("anim@amb@casino@mini@dance@dance_solo@female@var_b@", "low_right_down", 39)},
{"6_36", new Animation("anim@amb@casino@mini@dance@dance_solo@female@var_b@", "low_right_up", 39)},
{"6_37", new Animation("anim@amb@casino@mini@dance@dance_solo@female@var_b@", "med_center", 39)},
{"6_38", new Animation("anim@amb@casino@mini@dance@dance_solo@female@var_b@", "med_center_down", 39)},
{"6_39", new Animation("anim@amb@casino@mini@dance@dance_solo@female@var_b@", "med_center_up", 39)},
{"6_40", new Animation("anim@amb@casino@mini@dance@dance_solo@female@var_b@", "med_left", 39)},
{"6_41", new Animation("anim@amb@casino@mini@dance@dance_solo@female@var_b@", "med_left_down", 39)},
{"6_42", new Animation("anim@amb@casino@mini@dance@dance_solo@female@var_b@", "med_left_up", 39)},
{"6_43", new Animation("anim@amb@casino@mini@dance@dance_solo@female@var_b@", "med_right", 39)},
{"6_44", new Animation("anim@amb@casino@mini@dance@dance_solo@female@var_b@", "med_right_down", 39)},
{"6_45", new Animation("anim@amb@casino@mini@dance@dance_solo@female@var_b@", "med_right_up", 39)},
{"6_46", new Animation("amb@code_human_in_car_mp_actions@dance@bodhi@rds@base", "idle_a", 39)},
{"6_47", new Animation("amb@code_human_in_car_mp_actions@dance@bodhi@rds@base", "idle_b", 39)},
{"6_48", new Animation("amb@code_human_in_car_music@generic@ps@idle_a", "idle_a", 39)},
{"6_49", new Animation("amb@code_human_in_car_music@low@ps@idle_a", "idle_b", 39)},
{"6_50", new Animation("anim@amb@nightclub@dancers@solomun_entourage@", "mi_dance_facedj_17_v1_female^1", 39)},
{"6_51", new Animation("anim@amb@nightclub@dancers@crowddance_facedj@hi_intensity", "hi_dance_facedj_09_v2_female^1", 39)},
{"6_52", new Animation("anim@amb@nightclub@dancers@crowddance_facedj@hi_intensity", "hi_dance_facedj_09_v2_female^3", 39)},
{"6_53", new Animation("anim@amb@nightclub@mini@dance@dance_solo@female@var_b@", "high_center", 39)},
{"6_54", new Animation("anim@amb@nightclub@dancers@club_ambientpeds@", "mi-hi_amb_club_13_v1_female^1", 39)},
{"6_55", new Animation("anim@amb@nightclub@dancers@club_ambientpeds@", "mi-hi_amb_club_11_v1_female^1", 39)},
{"6_56", new Animation("anim@amb@nightclub@dancers@club_ambientpeds@med-hi_intensity", "mi-hi_amb_club_11_v1_female^6", 39)},
{"6_57", new Animation("anim@amb@nightclub@dancers@club_ambientpeds@med-hi_intensity", "mi-hi_amb_club_09_v1_male^3", 39)},
{"6_58", new Animation("anim@amb@nightclub@lazlow@hi_podium@", "danceidle_hi_11_turnaround_laz", 39)},
{"6_59", new Animation("anim@amb@nightclub@lazlow@hi_podium@", "danceidle_hi_15_crazyrobot_laz", 39)},
{"6_60", new Animation("anim@amb@nightclub@djs@black_madonna@", "dance_b_idle_a_blamadon", 39)},
{"6_61", new Animation("anim@amb@nightclub@dancers@crowddance_facedj@", "hi_dance_facedj_13_v2_female^6", 39)},
{"6_62", new Animation("anim@amb@nightclub@dancers@crowddance_facedj@", "hi_dance_facedj_09_v1_female^6", 39)},
{"6_63", new Animation("anim@amb@nightclub@dancers@crowddance_facedj@", "hi_dance_facedj_09_v2_female^1", 39)},
{"6_64", new Animation("anim@amb@nightclub@djs@dixon@", "dixn_dance_a_dixon", 39)},
{"6_65", new Animation("anim@amb@nightclub@dancers@crowddance_facedj_transitions@", "trans_dance_facedj_hi_to_li_09_v1_female^1", 39)},
{"6_66", new Animation("anim@amb@nightclub@dancers@crowddance_facedj_transitions@", "trans_dance_facedj_hi_to_li_09_v1_female^6", 39)},
{"6_67", new Animation("anim@amb@nightclub@dancers@crowddance_facedj_transitions@from_med_intensity", "trans_dance_facedj_mi_to_hi_08_v1_female^3", 39)},
{"6_68", new Animation("anim@amb@nightclub@dancers@crowddance_facedj_transitions@from_med_intensity", "trans_dance_facedj_mi_to_hi_08_v1_male^1", 39)},
{"6_69", new Animation("anim@amb@nightclub@dancers@crowddance_facedj_transitions@from_med_intensity", "trans_dance_facedj_mi_to_hi_08_v1_male^3", 39)},
{"6_70", new Animation("anim@amb@nightclub@dancers@crowddance_groups@", "hi_dance_crowd_09_v1_female^1", 39)},
{"6_71", new Animation("anim@amb@nightclub@dancers@crowddance_groups@", "hi_dance_crowd_09_v2_female^3", 39)},
{"6_72", new Animation("anim@amb@nightclub@dancers@crowddance_groups@", "hi_dance_crowd_09_v2_female^5", 39)},
{"6_73", new Animation("anim@amb@nightclub@dancers@crowddance_groups@", "hi_dance_crowd_09_v2_female^6", 39)},
{"6_74", new Animation("anim@amb@nightclub@dancers@crowddance_groups@", "hi_dance_crowd_09_v1_female^3", 39)},
{"6_75", new Animation("anim@amb@nightclub@dancers@crowddance_groups@", "hi_dance_crowd_09_v2_male^2", 39)},
{"6_76", new Animation("anim@amb@nightclub@dancers@crowddance_groups@", "hi_dance_crowd_09_v1_male^3", 39)},
{"6_77", new Animation("anim@amb@nightclub@dancers@crowddance_groups@", "hi_dance_crowd_09_v1_male^6", 39)},
{"6_78", new Animation("anim@amb@nightclub@dancers@crowddance_groups@", "hi_dance_crowd_17_v2_female^1", 39)},
{"6_79", new Animation("anim@amb@nightclub@dancers@crowddance_groups@", "hi_dance_crowd_17_v1_female^2", 39)},
{"6_80", new Animation("anim@amb@nightclub@dancers@crowddance_groups@", "hi_dance_crowd_17_v1_female^3", 39)},
{"6_81", new Animation("anim@amb@nightclub@dancers@crowddance_groups@", "hi_dance_crowd_17_v1_female^6", 39)},
{"6_82", new Animation("anim@amb@nightclub@dancers@crowddance_groups@", "hi_dance_crowd_15_v1_female^1", 39)},
{"6_83", new Animation("anim@amb@nightclub@dancers@crowddance_groups@", "hi_dance_crowd_15_v1_female^2", 39)},
{"6_84", new Animation("anim@amb@nightclub@dancers@crowddance_groups@", "hi_dance_crowd_15_v1_female^3", 39)},
{"6_85", new Animation("anim@amb@nightclub@dancers@crowddance_single_props@", "hi_dance_prop_09_v1_female^2", 39)},
        };
        public static void AnimationStop(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                if (sessionData.AnimationUse != null && AnimList.ContainsKey(sessionData.AnimationUse))
                {
                    var category = sessionData.AnimationUse;
                    Trigger.StopAnimation(player);
                    
                    if (AnimList[category].Flag == 39) 
                        NAPI.Entity.SetEntityPosition(player, player.Position + new Vector3(0, 0, 0.2));
                    
                    sessionData.HandsUp = false;
                    Trigger.ClientEvent(player, "client.animation.isPlayer", 0);
                }
                sessionData.AnimationUse = null;
            }
            catch (Exception e)
            {
                Log.Write($"AnimationStop Exception: {e.ToString()}");
            }
        }
        [RemoteEvent("server.animation.play")]
        public static void AnimationPlay(ExtPlayer player, string category, bool vehCheck = true)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                if (sessionData.AntiAnimDown || sessionData.Following != null || sessionData.SitPos != -1 || sessionData.InAirsoftLobby >= 0) return;
                if (vehCheck && player.IsInVehicle) return;
                if (sessionData.AnimationUse != null && sessionData.AnimationUse == category) return;
                if (!AnimList.ContainsKey(category))
                {
                    AnimationStop(player);
                    return;
                }
                var animData = AnimList[category];
                sessionData.AnimationUse = category;
                Trigger.PlayAnimation(player, animData.Dictionary, animData.Name, animData.Flag);
                Trigger.ClientEvent(player, "client.animation.isPlayer", 1);
                
                BattlePass.Repository.UpdateReward(player, 70);
                qMain.UpdateQuestsStage(player, Zdobich.QuestName, (int)zdobich_quests.Stage10, 1, isUpdateHud: true);
                qMain.UpdateQuestsComplete(player, Zdobich.QuestName, (int) zdobich_quests.Stage10, true);
                
                if (animData.Dictionary == "random@arrests@busted" && animData.Name == "idle_c") 
                    sessionData.HandsUp = true;
                else
                    sessionData.HandsUp = false;
            }
            catch (Exception e)
            {
                Log.Write($"animationSelected Exception: {e.ToString()}");
            }
        }
        public static void playerInteractionTarget(ExtPlayer player, ExtPlayer target)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                if (!target.IsCharacterData() || player.Position.DistanceTo(target.Position) > 3) return;

                var targetSessionData = target.GetSessionData();
                if (targetSessionData == null || targetSessionData.AnimationUse == null || !AnimList.ContainsKey(targetSessionData.AnimationUse))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlayerDontAnim), 3000);
                    return;
                }
                AnimationPlay(player, targetSessionData.AnimationUse);
                BattlePass.Repository.UpdateReward(player, 56);
            }
            catch (Exception e)
            {
                Log.Write($"playerInteractionTarget Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("server.animation.favorite")]
        public void AnimationDavorite(ExtPlayer player, string json)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;

                characterData.ConfigData.AnimFavorites = json;
            }
            catch (Exception e)
            {
                Log.Write($"AnimationDavorite Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("server.animation.bind")]
        public void AnimationBind(ExtPlayer player, string json)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;

                characterData.ConfigData.AnimBind = json;
            }
            catch (Exception e)
            {
                Log.Write($"AnimationDavorite Exception: {e.ToString()}");
            }
        }

        [Command("starta")]
        public static void CMD_starta(ExtPlayer player)
        {
            if (Main.ServerNumber != 0) return;
            Trigger.ClientEvent(player, "test.taskPlayAnim");
        }
    }
}
