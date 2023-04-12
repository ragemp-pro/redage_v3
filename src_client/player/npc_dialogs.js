global.questNameToPeds = {};

let pedsLightData = {};

const PedAnimList = {
    /*"s_m_y_dockwork_01": {
        animDictionary: "amb@world_human_bum_slumped@male@laying_on_left_side@base",
        animationName: "base",
    },
    "s_m_y_grip_01": {
        animDictionary: "amb@world_human_stand_mobile@female@standing@call@idle_a",
        animationName: "idle_a",
    },
    "s_m_m_ccrew_01": {
        animDictionary: "missheistdockssetup1ig_5@exit",
        animationName: "workers_talking_exit_dockworker1",
    },
    "a_f_m_bevhills_01": {
        animDictionary: "special_ped@jerome@monologue_4@monologue_4i",
        animationName: "listentome_8",
    },
    "csb_chin_goon": {
        animDictionary: "amb@world_human_smoking_fat@male@male_a@idle_a",
        animationName: "idle_c",
    },
    "s_m_m_gaffer_01": {
        animDictionary: "mini@repair",
        animationName: "fixing_a_ped",
    },
    "a_m_m_hasjew_01": {
        animDictionary: "amb@world_human_smoking_fat@male@male_a@idle_a",
        animationName: "idle_c",
    },
    "s_m_y_armymech_01": {
        animDictionary: "amb@world_human_gardener_plant@male@base",
        animationName: "base",
    },
    "a_m_m_golfer_01": {
        animDictionary: "missheistdockssetup1ig_5@base",
        animationName: "workers_talking_base_dockworker1",
    },
    "a_m_y_golfer_01": {
        animDictionary: "special_ped@jerome@monologue_4@monologue_4f",
        animationName: "listentome_5",
    },
    "cs_martinmadrazo": {
        animDictionary: "amb@world_human_stand_mobile@female@standing@call@idle_a",
        animationName: "idle_a",
    },
    "ig_andreas": {
        animDictionary: "amb@world_human_smoking_fat@male@male_a@idle_a",
        animationName: "idle_c",
    },
    "s_m_y_garbage": {
        animDictionary: "amb@world_human_stand_mobile@female@standing@call@idle_a",
        animationName: "idle_c",
    },
    "s_m_y_airworker": {
        animDictionary: "missmic4premiere",
        animationName: "interview_short_anton",
    },
    "u_m_m_edtoh": {
        animDictionary: "special_ped@jerome@monologue_4@monologue_4c",
        animationName: "listentome_2",
    },
    "mp_s_m_armoured_01": {
        animDictionary: "anim@heists@chicken_heist@ig_5_guard_wave_in",
        animationName: "guard_reaction",
    },
    "u_f_y_princess": {
        animDictionary: "amb@world_human_smoking_fat@male@male_a@idle_a",
        animationName: "idle_c",
    },
    "a_m_m_bevhills_01": {
        animDictionary: "amb@world_human_smoking_fat@male@male_a@idle_a",
        animationName: "idle_c",
    },
    "a_f_y_eastsa_03": {
        animDictionary: "amb@world_human_stand_mobile@female@standing@call@idle_a",
        animationName: "idle_a",
    },
    "a_m_m_paparazzi_01": {
        animDictionary: "amb@world_human_smoking_fat@male@male_a@idle_a",
        animationName: "idle_c",
    },
    "a_m_m_prolhost_01": {
        animDictionary: "amb@world_human_stand_mobile@female@standing@call@idle_a",
        animationName: "idle_a",
    },
    "a_m_m_skater_01": {
        animDictionary: "switch@franklin@lamar_tagging_wall",
        animationName: "lamar_tagging_exit_loop_lamar",
    },
    "ig_wade": {
        animDictionary: "switch@franklin@lamar_tagging_wall",
        animationName: "lamar_tagging_exit_loop_lamar",
    },
    "ig_denise": {
        animDictionary: "amb@world_human_stand_mobile@female@standing@call@idle_a",
        animationName: "idle_a",
    },
    "u_m_y_babyd": {
        animDictionary: "missheistdockssetup1ig_13@kick_idle",
        animationName: "guard_beatup_kickidle_dockworker",
    },
    "g_m_y_ballaorig_01": {
        animDictionary: "missheistdockssetup1ig_13@kick_idle",
        animationName: "guard_beatup_kickidle_guard1",
    },
    "g_m_y_ballaeast_01": {
        animDictionary: "misstrevor3_beatup",
        animationName: "guard_beatup_kickidle_guard1",
    },
    "g_m_y_armgoon_02": {
        animDictionary: "amb@world_human_stand_mobile@female@standing@call@idle_a",
        animationName: "idle_a",
    },
    "s_m_y_fireman_01": {
        animDictionary: "special_ped@jerome@monologue_4@monologue_4i",
        animationName: "listentome_8",
    },
    "ig_hao": {
        animDictionary: "random@arrests@busted",
        animationName: "idle_b",
    },
    "s_m_y_cop_01": {
        animDictionary: "combat@aim_variations@arrest",
        animationName: "cop_med_arrest_01",
    },
    "a_f_o_salton_01": {
        animDictionary: "cellphone@self@franklin@",
        animationName: "peace"
    },
    "mp_f_deadhooker": {
        animDictionary: "amb@world_human_prostitute@french@idle_a",
        animationName: "idle_a",
    },
    "s_f_y_hooker_02": {
        animDictionary: "mp_move@prostitute@f@cokehead",
        animationName: "idle",
    },
    "s_f_y_hooker_01": {
        animDictionary: "amb@world_human_prostitute@french@idle_a",
        animationName: "idle_c",
    },
    "csb_hugh": {
        animDictionary: "friends@frt@ig_1",
        animationName: "trevor_impatient_wait_1",
    },
    "a_m_m_indian_01 ": {
        animDictionary: "friends@frt@ig_1",
        animationName: "trevor_impatient_wait_4",
    },
    "cs_janet": {
        animDictionary: "amb@world_human_smoking_fat@male@male_a@idle_a",
        animationName: "idle_c",
    },
    "ig_jimmyboston": {
        animDictionary: "amb@world_human_stand_mobile@female@standing@call@idle_a",
        animationName: "idle_a",
    },
    "a_m_y_stwhi_01": {
        animDictionary: "amb@world_human_smoking_fat@male@male_a@idle_a",
        animationName: "idle_c",
    },
    "a_m_m_salton_03": {
        animDictionary: "amb@world_human_smoking_fat@male@male_a@idle_a",
        animationName: "idle_c",
    },
    "s_m_y_dealer_01": {
        animDictionary: "Anim friends@",
        animationName: "pickupwait",
    },
    "mp_s_m_armoured_01": {
        animDictionary: "anim@heists@chicken_heist@ig_5_guard_wave_in",
        animationName: "guard_reaction",
    },
    "cs_solomon": {
        animDictionary: "amb@prop_human_atm@female@idle_a",
        animationName: "idle_a",
    },
    "ig_popov": {
        animDictionary: "anim@amb@prop_human_atm@interior@male@idle_a",
        animationName: "idle_d",
    },
    "ig_tanisha": {
        animDictionary: "anim@amb@prop_human_atm@interior@male@idle_a",
        animationName: "idle_d",
    },
    "cs_tom": {
        animDictionary: "amb@prop_human_atm@female@idle_a",
        animationName: "idle_a",
    },
    "ig_taocheng": {
        animDictionary: "anim@amb@prop_human_atm@interior@male@idle_a",
        animationName: "idle_d",
    },
    "s_f_y_migrant_01": {
        animDictionary: "amb@world_human_gardener_plant@male@base",
        animationName: "base",
    },
    "s_m_m_migrant_01": {
        animDictionary: "amb@world_human_gardener_plant@male@base",
        animationName: "base",
    },
    "ig_omega": {
        animDictionary: "amb@world_human_gardener_plant@male@base",
        animationName: "base",
    },
    "a_f_y_hipster_02": {
        animDictionary: "special_ped@jerome@monologue_4@monologue_4i",
        animationName: "listentome_8",
    },
    "cs_gurk": {
        animDictionary: "special_ped@jerome@monologue_4@monologue_4c",
        animationName: "listentome_2",
    },
    "a_m_y_hipster_01": {
        animDictionary: "missheistdockssetup1ig_13@kick_idle",
        animationName: "guard_beatup_kickidle_dockworker",
    },
    "g_m_y_korean_01": {
        animDictionary: "missheistdockssetup1ig_13@kick_idle",
        animationName: "guard_beatup_kickidle_guard1",
    },
    "ig_chengsr": {
        animDictionary: "misstrevor3_beatup",
        animationName: "guard_beatup_kickidle_guard1",
    },
    "ig_jimmydisanto": {
        animDictionary: "random@bicycle_thief@ask_help",
        animationName: "please_man_you_gotta_help_me",
    },
    "ig_vagspeak": {
        animDictionary: "missmic4premiere",
        animationName: "interview_short_anton",
    },
    "ig_terry": {
        animDictionary: "amb@world_human_drinking@beer@male@idle_a",
        animationName: "idle_a"
    },
    "u_m_o_taphillbilly": {
        animDictionary: "amb@world_human_drinking@beer@male@idle_a",
        animationName: "idle_a"
    },
    "u_m_y_tattoo_01": {
        animDictionary: "amb@world_human_drinking@beer@male@idle_a",
        animationName: "idle_a"
    },
    "s_m_m_strperf_01": {
        animDictionary: "timetable@mime@01_gc",
        animationName: "base",
    },
    "s_m_y_mime": {
        animDictionary: "timetable@mime@ig_1",
        animationName: "driving",
    },
    "ig_maryann": {
        animDictionary: "anim@mp_player_intupperslow_clap",
        animationName: "idle_a",
    },
    "csb_ramp_hipster": {
        animDictionary: "cellphone@",
        animationName: "cellphone_photo_idle"
    },
    "a_m_y_runner_01": {
        animDictionary: "anim@cellphone@in_car@ds",
        animationName: "cellphone_text_read_base"
    },
    "cs_chrisformage": {
        animDictionary: "amb@world_human_smoking_fat@male@male_a@idle_a",
        animationName: "idle_c",
    },
    "u_m_m_jesus_01": {
        animDictionary: "missheistdockssetup1showoffcar@idle_a",
        animationName: "idle_a_5",
    },
    "s_f_y_baywatch_01": {
        animDictionary: "amb@world_human_sunbathe@female@front@idle_a",
        animationName: "idle_c",
    },
    "a_f_y_beach_02": {
        animDictionary: "amb@world_human_sunbathe@female@back@idle_a",
        animationName: "idle_a",
    },
    "a_f_m_beach_01": {
        animDictionary: "amb@world_human_sunbathe@male@back@idle_a",
        animationName: "idle_a",
    },
    "a_f_y_beach_01": {
        animDictionary: "amb@world_human_sunbathe@male@front@idle_a",
        animationName: "idle_a",
    },
    "a_m_m_beach_01": {
        animDictionary: "amb@world_human_sunbathe@female@back@idle_a",
        animationName: "idle_c",
    },
    "a_m_m_beach_02": {
        animDictionary: "amb@world_human_sunbathe@male@back@idle_a",
        animationName: "idle_c",
    },
    "a_f_m_fatcult_01": {
        animDictionary: "amb@world_human_sunbathe@male@front@idle_a ",
        animationName: "idle_c",
    },
    "g_m_y_famdnf_01": {
        animDictionary: "anim@amb@machinery@vertical_mill@",
        animationName: "scratch_amy_skater_01",
    },
    "hc_hacker": {
        animDictionary: "amb@world_human_smoking_fat@male@male_a@idle_a",
        animationName: "idle_c",
    },
    "a_m_m_stlat_02": {
        animDictionary: "Anim friends@",
        animationName: "pickupwait ",
    },
    "u_f_m_miranda": {
        animDictionary: "amb@world_human_bum_standing@twitchy@idle_a",
        animationName: "idle_a",
    },
    "u_m_m_jewelsec_01": {
        animDictionary: "amb@world_human_smoking_fat@male@male_a@idle_a",
        animationName: "idle_c",
    },
    "a_f_y_genhot_01": {
        animDictionary: "amb@world_human_smoking_fat@male@male_a@idle_a",
        animationName: "idle_c",
    },
    "g_m_y_mexgang_01": {
        animDictionary: "anim@amb@machinery@vertical_mill@",
        animationName: "reachout_amy_skater_01",
    },
    "a_m_y_beachvesp_02": {
        animDictionary: "amb@world_human_stand_mobile@female@standing@call@idle_a",
        animationName: "idle_a",
    },
    "ig_lamardavis": {
        animDictionary: "missheistdockssetup1ig_10@base",
        animationName: "talk_pipe_base_worker2",
    },
    "ig_lazlow": {
        animDictionary: "amb@world_human_smoking_fat@male@male_a@idle_a",
        animationName: "idle_c",
    },
    "u_m_y_party_01": {
        animDictionary: "missfbi3_party_b",
        animationName: "talk_balcony_loop_male1",
    },
    "mp_f_execpa_02": {
        animDictionary: "rcmnigel1aig_1",
        animationName: "this_is_awkward_willie",
    },
    "s_m_m_security_01": {
        animDictionary: "mini@strip_club@idles@bouncer@idle_a",
        animationName: "idle_a",
    },
    "cs_debra": {
        animDictionary: "friends@frt@ig_1",
        animationName: "trevor_base",
    },
    "a_m_y_vinewood_02": {
        animDictionary: "anim@amb@machinery@vertical_mill@",
        animationName: "look_high_amy_skater_01",
    },
    "ig_gustavo": {
        animDictionary: "mini@prostitutestalk",
        animationName: "street_argue_f_a",
    },
    "ig_patricia_02": {
        animDictionary: "missfbi3_party_d",
        animationName: "stand_talk_loop_a_male1",
    },
    "a_f_m_soucentmc_01": {
        animDictionary: "anim@mp_player_intupperphotography",
        animationName: "idle_a_fp",
    },
    "a_m_y_clubcust_04": {
        animDictionary: "amb@world_human_bum_standing@twitchy@idle_a",
        animationName: "idle_a",
    },
    "s_m_o_busker_01": {
        animDictionary: "anim@mp_player_intupperair_guitar",
        animationName: "idle_a",
    },
    "mp_m_famdd_01": {
        animDictionary: "combat@aim_variations@arrest",
        animationName: "cop_med_arrest_01",
    },
    "ig_bankman": {
        animDictionary: "anim@heists@fleeca_bank@hostages@ped_d@cower",
        animationName: "cower",
    },
    "a_f_y_soucent_01": {
        animDictionary: "missprologueig_2",
        animationName: "idle_on_floor_malehostage01",
    },
    "ig_patricia_02 ": {
        animDictionary: "mini@prostitutestalk",
        animationName: "street_argue_f_a",
    },
    "s_m_y_barman_01": {
        animDictionary: "anim@amb@machinery@vertical_mill@",
        animationName: "clean_surface_02_amy_skater_01",
    },
    "s_m_y_waiter_01": {
        animDictionary: "missheistdockssetup1showoffcar@idle_a",
        animationName: "idle_a_5",
    },
    "csb_money": {
        animDictionary: "amb@world_human_smoking_fat@male@male_a@idle_a",
        animationName: "idle_c",
    },
    "ig_miguelmadrazo": {
        animDictionary: "amb@world_human_drinking@beer@male@idle_a",
        animationName: "idle_a",
    },
    "ig_car3guy2": {
        animDictionary: "anim@mp_player_intupperphotography",
        animationName: "idle_a_fp",
    },
    "mp_m_exarmy_01": {
        animDictionary: "amb@world_human_stand_fishing@idle_a",
        animationName: "idle_b",
    },
    "u_m_m_filmdirector": {
        animDictionary: "amb@world_human_stand_fishing@idle_a",
        animationName: "idle_b",
    },
    "ig_floyd": {
        animDictionary: "amb@world_human_hammering@male@base",
        animationName: "base",
    },
    "s_m_m_gardener_01": {
        animDictionary: "amb@world_human_smoking_fat@male@male_a@idle_a",
        animationName: "idle_c",
    },
    "g_m_y_salvagoon_03": {
        animDictionary: "missfbi3_party_b",
        animationName: "talk_balcony_loop_male1",
    },
    "ig_screen_writer": {
        animDictionary: "missfbi3_party_b",
        animationName: "talk_balcony_loop_male2",
    },
    "s_m_y_ammucity_01": {
        animDictionary: "missheistdockssetup1ig_13@kick_idle",
        animationName: "guard_beatup_kickidle_guard1",
    },
    "ig_ramp_hic": {
        animDictionary: "missheistdockssetup1ig_13@kick_idle",
        animationName: "guard_beatup_kickidle_dockworker",
    },
    "s_f_y_migrant_01": {
        animDictionary: "amb@world_human_gardener_plant@male@base",
        animationName: "base",
    },
    "s_m_m_migrant_01": {
        animDictionary: "amb@world_human_gardener_plant@male@base",
        animationName: "base",
    },
    "csb_isldj_00": {
        animDictionary: "anim@amb@machinery@vertical_mill@",
        animationName: "look_high_amy_skater_01",
    },
    "a_m_y_stlat_01": {
        animDictionary: "amb@world_human_stand_mobile@female@standing@call@idle_a",
        animationName: "idle_a",
    },
    "s_m_m_strvend_01": {
        animDictionary: "missheistdockssetup1ig_13@kick_idle",
        animationName: "guard_beatup_kickidle_dockworker",
    },
    "a_m_m_tramp_01": {
        animDictionary: "amb@world_human_drinking@beer@male@idle_a",
        animationName: "idle_a",
    },
    "a_m_o_tramp_01": {
        animDictionary: "amb@world_human_drinking@beer@male@idle_a",
        animationName: "idle_a",
    },
    "u_m_o_tramp_01": {
        animDictionary: "amb@world_human_smoking_fat@male@male_a@idle_a",
        animationName: "idle_c",
    },
    "a_m_y_beach_04": {
        animDictionary: "special_ped@jerome@monologue_4@monologue_4c",
        animationName: "listentome_2",
    },
    "csb_jio": {
        animDictionary: "special_ped@jerome@monologue_4@monologue_4i",
        animationName: "listentome_8",
    },
    "csb_mjo": {
        animDictionary: "amb@world_human_stand_mobile@female@standing@call@idle_a",
        animationName: "idle_a",
    },
    "a_m_y_eastsa_02": {

        animDictionary: "anim@heists@ornate_bank@chat_manager",
        animationName: "average_car",
    },
    "a_f_m_fatwhite_01": {

            animDictionary: "anim@heists@ornate_bank@chat_manager",
            animationName: "on_foot",
    },
    "s_f_m_fembarber": {

            animDictionary: "anim@heists@ornate_bank@chat_manager",
            animationName: "no_speak",
        },
    "csb_fos_rep": {

    animDictionary: "anim@heists@ornate_bank@chat_manager",
    animationName: "poor_clothes",
    },
    
    "a_f_y_hipster_03": {
    
    animDictionary: "special_ped@jerome@monologue_4@monologue_4c",
    animationName: "listentome_2",
    },
    
    "a_f_y_hipster_04": {
    
    animDictionary: "anim@heists@ornate_bank@chat_manager",
    animationName: "average_car",
    },
    
    "csb_hugh": {
    
    animDictionary: "anim@heists@ornate_bank@chat_manager",
    animationName: "no_speak 1",
    },
    
    "ig_jay_norris": {
    
    animDictionary: "special_ped@jerome@monologue_4@monologue_4c",
    animationName: "listentome_2", 
    },
    "cs_jimmyboston": {

    animDictionary: "friends@frt@ig_1",
        animationName: "trevor_base", 
    },
    
    
    "a_m_o_ktown_01": {
    
    animDictionary: "amb@world_human_drug_dealer_hard@male@base",
        animationName: "base", 
    },
    
    
    "ig_lazlow": {
    
    animDictionary: "cellphone@",
        animationName: "cellphone_call_listen_base", 
    },
    
    
    "cs_mrs_thornhill": {
    
    animDictionary: "cellphone@",
        animationName: "cellphone_call_listen_base", 
    },
    
    
    "s_f_y_movprem_01": {
    
    animDictionary: "friends@frt@ig_1",
        animationName: "trevor_base", 
    },
    
    
    "u_m_m_filmdirector": {
    
    animDictionary: "special_ped@baygor@monologue_3@monologue_3f",
        animationName: "trees_can_talk_5", 
    },
    
    "s_f_y_hooker_01": {

        animDictionary: "mini@hookers_sp",
            animationName: "idle_reject_loop_a", 
        },    

    "a_f_y_indian_01": {

        animDictionary: "clothingtie",
         animationName: "check_out_a", 
        },
        
        
        "ig_janet": {
        
        animDictionary: "amb@world_human_cheering@female_d",
         animationName: "base", 
        },
        
        
        "a_m_y_ktown_01": {
        
        animDictionary: "amb@world_human_picnic@female@base",
         animationName: "base", 
        },
        
        
        "a_f_m_ktown_01": {
        
        animDictionary: "special_ped@jerome@monologue_4@monologue_4i",
         animationName: "listentome_8", 
        },
        
        
        "a_m_y_ktown_02": {
        
        animDictionary: "amb@world_human_hang_out_street@female_arm_side@idle_a",
         animationName: "idle_a", 
        },
        
        
        "ig_lifeinvad_01": {
        
        animDictionary: "mini@hookers_sp",
         animationName: "idle_reject_loop_a", 
        },
        
        
        "g_m_y_lost_03": {
        
        animDictionary: "cellphone@",
         animationName: "cellphone_call_listen_base", 
        },

        "s_m_m_marine_02": {

        animDictionary: "anim@heists@ornate_bank@chat_manager",
            animationName: "no_speak", 
        },
        
        
        
        "cs_martinmadrazo": {
        
        animDictionary: "amb@world_human_hang_out_street@female_arm_side@idle_a",
            animationName: "idle_a", 
        },
        
        
        
        "ig_marnie": {
        
        animDictionary: "mini@hookers_sp",
            animationName: "idle_reject_loop_a", 
        },
        
        
        
        "ig_milton": {
        
        animDictionary: "anim@heists@ornate_bank@chat_manager",
            animationName: "no_speak", 
        },
        
        
        
        "player_zero": {
        
        animDictionary: "anim@heists@ornate_bank@chat_manager",
            animationName: "no_speak", 
        },
        
        
        
        "cs_paper": {
        
        animDictionary: "special_ped@jerome@monologue_4@monologue_4i",
            animationName: "listentome_8", 
        },
        
        
        
        "cs_solomon": {
        
        animDictionary: "special_ped@jerome@monologue_4@monologue_4c",
            animationName: "listentome_2", 
        },
        
        
        
        "mp_f_execpa_01": {
        
        animDictionary: "anim@heists@ornate_bank@chat_manager",
            animationName: "no_speak", 
        },
            
            
            
            "s_m_y_airworker": {
            
            animDictionary: "special_ped@jerome@monologue_4@monologue_4c",
             animationName: "listentome_2", 
            },
            
            
            
            "s_f_y_airhostess_01": {
            
            animDictionary: "special_ped@jerome@monologue_4@monologue_4i",
             animationName: "listentome_8", 
            },
            
            
            
            "u_m_m_aldinapoli": {
            
            animDictionary: "amb@world_human_smoking_fat@male@male_a@idle_a",
             animationName: "listentome_8", 
            },
            
            "s_m_m_lathandy_01": {

                animDictionary: "mini@repair",
                 animationName: "fixing_a_player", 
                },
                
                
                
                "s_m_m_gardener_01": {
                
                animDictionary: "friends@",
                 animationName: "pickupwait", 
                },
                
                
                
                "a_m_m_eastsa_02": {
                
                animDictionary: "anim@heists@ornate_bank@chat_manager",
                 animationName: "no_speak", 
                },
                
                
                
                "mp_m_exarmy_01": {
                
                animDictionary: "mini@hookers_sp",
                 animationName: "idle_reject_loop_a", 
                },
                    "s_m_m_lathandy_01": {

                animDictionary: "mini@repair",
                animationName: "fixing_a_player", 
                },



                "s_m_m_gardener_01": {

                animDictionary: "friends@",
                animationName: "pickupwait", 
                },



                "a_m_m_eastsa_02": {

                animDictionary: "anim@heists@ornate_bank@chat_manager",
                animationName: "no_speak", 
                },



                "mp_m_exarmy_01": {

                animDictionary: "mini@hookers_sp",
                animationName: "idle_reject_loop_a", 
                },

                "ig_hao": {

                    animDictionary: "anim@heists@ornate_bank@chat_manager",
                     animationName: "no_speak", 
                    },
                    
                    
                    
                    "cs_gurk": {
                    
                    animDictionary: "anim@heists@ornate_bank@chat_manager",
                     animationName: "no_speak", 
                    },
                    
                    
                    
                    "a_m_m_hasjew_01": {
                    
                    animDictionary: "anim@heists@ornate_bank@chat_manager",
                     animationName: "no_speak", 
                    },
                    
                    
                    
                    "a_m_y_hasjew_01": {
                    
                    animDictionary: "anim@heists@ornate_bank@chat_manager",
                     animationName: "no_speak", 
                    },
                    
                    
                    
                    "u_f_y_jewelass_01": {
                    
                    animDictionary: "anim@heists@ornate_bank@chat_manager",
                     animationName: "no_speak", 
                    },

                    "a_m_y_jetski_01": {

                        animDictionary: "anim@amb@nightclub@dancers@club_ambientpeds@",
                         animationName: "li-mi_amb_club_09_v1_female^5", 
                        },
                        
                        
                        
                        "s_f_y_baywatch_01": {
                        
                        animDictionary: "anim@amb@nightclub@dancers@club_ambientpeds@",
                         animationName: "li-mi_amb_club_09_v1_female^5", 
                        },
                        
                        
                        
                        "s_m_y_baywatch_01": {
                        
                        animDictionary: "anim@amb@nightclub@dancers@club_ambientpeds@",
                         animationName: "li-mi_amb_club_09_v1_female^5", 
                        },
     
                        
                        "a_m_o_beach_01": {
                        
                        animDictionary: "anim@amb@nightclub@dancers@club_ambientpeds@",
                         animationName: "li-mi_amb_club_09_v1_female^5", 
                        },
                        
                        
                        
                        "a_m_y_bevhills_01": {
                        
                        animDictionary: "anim@heists@ornate_bank@chat_manager",
                         animationName: "no_speak", 
                        },
                        
                        
                        
                        "a_m_y_beachvesp_01": {
                        
                        animDictionary: "anim@heists@ornate_bank@chat_manager",
                         animationName: "no_speak", 
                        },
                        
                        
                        
                        "a_m_y_beachvesp_02": {
                        
                        animDictionary: "anim@heists@ornate_bank@chat_manager",
                         animationName: "no_speak", 
                        },                                                              
                        
                        
                        "a_m_y_beach_01": {
                        
                        animDictionary: "amb@lo_res_idles@",
                         animationName: "lying_face_down_lo_res_base", 
                        },
                        
                        
                        
                        "a_f_m_beach_01": {
                        
                        animDictionary: "anim@amb@business@bgen@bgen_no_work@",
                         animationName: "sit_phone_phoneputdown_sleeping_nowork", 
                        },
                        
                        
                        
                        "a_f_y_beach_01": {
                        
                        animDictionary: "anim@amb@business@bgen@bgen_no_work@",
                         animationName: "sit_phone_phoneputdown_sleeping_nowork", 
                        },
                        
                        
                        
                        "a_m_m_beach_02": {
                        
                        animDictionary: "amb@lo_res_idles@",
                         animationName: "lying_face_down_lo_res_base", 
                        },

                        "csb_car3guy1": {

                            animDictionary: "timetable@mime@ig_3",
                             animationName: "normally_dont_speak", 
                            },
                            
                            
                            
                            "s_m_y_ranger_01": {
                            
                            animDictionary: "anim@heists@ornate_bank@chat_manager",
                             animationName: "no_speak", 
                            },
                            
                            
                            
                            "g_m_m_chicold_01": {
                            
                            animDictionary: "melee@small_wpn@streamed_core",
                             animationName: "kick_close_a", 
                            },
                            
                            
                            
                            "csb_chin_goon": {
                            
                            animDictionary: "missheistdockssetup1ig_13@kick_idle",
                             animationName: "guard_beatup_kickidle_dockworker", 
                            },
                            
                            
                            
                            "s_m_y_sheriff_01": {
                            
                            animDictionary: "amb@world_human_drug_dealer_hard@male@base",
                             animationName: "base", 
                            },
                            
                            
                            
                            "a_m_y_skater_02": {
                            
                            animDictionary: "anim@heists@ornate_bank@chat_manager",
                             animationName: "no_speak", 
                            },
                            
                            
                            
                            "s_f_y_shop_low": {
                            
                            animDictionary: "anim@heists@ornate_bank@chat_manager",
                             animationName: "no_speak", 
                            },
                            
                            
                            
                            "s_f_y_sheriff_01": {
                            
                            animDictionary: "anim@amb@casino@valet_scenario@pose_b@",
                             animationName: "kick_floor_a_m_y_vinewood_01", 
                            },
                            
                            
                            
                            "s_m_y_robber_01": {
                            
                            animDictionary: "mini@repair",
                            animationName: "fixing_a_player", 
                            },
                            
                            
                            "ig_stevehains": {
                            
                            animDictionary: "mini@repair",
                             animationName: "fixing_a_player", 
                            },
                            
                            
                            
                            "a_m_m_salton_03": {
                            
                            animDictionary: "amb@world_human_bum_slumped@male@laying_on_left_side@base",
                             animationName: "base", 
                            },
                            
                            
                            
                            "a_m_y_runner_02": {
                            
                            animDictionary: "mini@repair",
                             animationName: "fixing_a_player", 
                            },
                            
                            
                            
                            "a_f_y_soucent_03": {
                            
                            animDictionary: "timetable@mime@ig_3",
                             animationName: "normally_dont_speak", 
                            },
                            
                            
                            "a_m_m_socenlat_01": {
                            
                            animDictionary: "anim@heists@ornate_bank@chat_manager",
                             animationName: "no_speak", 
                            },

"g_m_y_strpunk_01": {
animDictionary: "misstrevor3_beatup",
 animationName: "guard_beatup_kickidle_guard1", 
},


"ig_taocheng": {
animDictionary: "missheistdockssetup1ig_13@kick_idle",
 animationName: "guard_beatup_kickidle_dockworker", 
},


"a_m_y_stwhi_01": {
animDictionary: "timetable@mime@ig_3",
 animationName: "normally_dont_speak", 
},


"s_m_y_strvend_01": {

animDictionary: "anim@heists@ornate_bank@chat_manager",
 animationName: "no_speak", 
},

"ig_stretch": {

animDictionary: "anim@amb@business@bgen@bgen_no_work@",
 animationName: "sit_phone_phoneputdown_sleeping_nowork", 
},

"a_f_m_tourist_01": {

    animDictionary: "anim@heists@ornate_bank@chat_manager",
     animationName: "no_speak", 
    },
    
    
    "a_f_y_vinewood_02": {
    
    animDictionary: "anim@heists@ornate_bank@chat_manager",
     animationName: "no_speak",
    },
    
    
    "a_m_y_vinewood_03": {
    
    animDictionary: "anim@amb@machinery@vertical_mill@",
     animationName: "look_high_amy_skater_01",
    },
                                            
"csb_isldj_03": {

animDictionary: "anim@amb@nightclub@dancers@club_ambientpeds@",
 animationName: "li-mi_amb_club_09_v1_female^5",
},


"a_m_y_beach_04": {

animDictionary: "anim@amb@nightclub@dancers@club_ambientpeds@",
 animationName: "li-mi_amb_club_09_v1_female^5",
},


"a_m_y_yoga_01": {

animDictionary: "anim@amb@nightclub@dancers@club_ambientpeds@",
 animationName: "li-mi_amb_club_09_v1_female^5",
},



"csb_undercover": {

animDictionary: "anim@heists@ornate_bank@chat_manager",
 animationName: "no_speak",
},


"a_m_m_soucent_03": {

animDictionary: "anim@heists@ornate_bank@chat_manager",
 animationName: "no_speak",
},


"a_f_y_soucent_01": {

animDictionary: "anim@amb@casino@valet_scenario@pose_b@",
 animationName: "kick_floor_a_m_y_vinewood_01",
},


"a_f_o_soucent_02": {

animDictionary: "anim@amb@nightclub@dancers@club_ambientpeds@",
 animationName: "li-mi_amb_club_09_v1_female^5",
},


"ig_ramp_hic": {

animDictionary: "anim@amb@business@bgen@bgen_no_work@",
 animationName: "sit_phone_phoneputdown_sleeping_nowork",
},
"a_m_m_farmer_01": {

    animDictionary: "amb@world_human_gardener_plant@male@base",
     animationName: "base",
    },
    
    
    "s_m_m_cntrybar_01": {
    
    animDictionary: "amb@world_human_gardener_plant@male@base",
     animationName: "base",
    },
    
    
    
    "a_m_y_cyclist_01": {
    
    animDictionary: "amb@world_human_gardener_plant@male@base",
     animationName: "base",
    },
    
    
    "s_f_y_migrant_01": {
    
    animDictionary: "amb@world_human_gardener_plant@male@base",
     animationName: "base",
    },
    
    
    "ig_old_man2": {
    
    animDictionary: "amb@world_human_gardener_plant@male@base",
     animationName: "base",
    },
    
    
    "a_f_y_runner_01": {
    
    animDictionary: "amb@world_human_gardener_plant@male@base",
     animationName: "base",
    },
    
    
    "a_m_m_salton_02": {
    
    animDictionary: "amb@world_human_gardener_plant@male@base",
     animationName: "base",
    },
    
    
    "a_m_y_sunbathe_01": {
    
    animDictionary: "amb@world_human_gardener_plant@male@base",
     animationName: "base",
    },
    
    
    "s_m_m_trucker_01": {
    
    animDictionary: "amb@world_human_bum_slumped@male@laying_on_left_side@base",
     animationName: "base",
    },
    
    
    "u_m_o_taphillbilly": {
    
    animDictionary: "mini@repair",
     animationName: "fixing_a_player",
    },
    
    
    "a_m_o_soucent_02": {
    
    animDictionary: "anim@amb@nightclub@dancers@club_ambientpeds@",
     animationName: "li-mi_amb_club_09_v1_female^5",
    },
    
    
    "a_m_y_stwhi_02": {
    
    animDictionary: "anim@amb@business@bgen@bgen_no_work@",
     animationName: "sit_phone_phoneputdown_sleeping_nowork",
    },

    "cs_tom": {

        animDictionary: "cellphone@",
         animationName: "cellphone_call_listen_base", 
        },
        
        
        
        "s_m_m_ups_01": {
        
        animDictionary: "mini@repair",
         animationName: "fixing_a_player", 
        },
        
        
        
        "a_m_m_hillbilly_01": {
        
        animDictionary: "mini@repair",
         animationName: "fixing_a_player", 
        },
    
        "a_f_y_eastsa_02": {

            animDictionary: "anim@amb@nightclub@dancers@club_ambientpeds@",
             animationName: "li-mi_amb_club_09_v1_female^5", 
            },
            
            
            
            "cs_taocheng": {
            
            animDictionary: "mini@repair",
             animationName: "fixing_a_player", 
            },
            
            
            
            "a_m_y_hipster_03": {
            
            animDictionary: "anim@amb@business@bgen@bgen_no_work@",
             animationName: "sit_phone_phoneputdown_sleeping_nowork", 
            },

            "g_m_m_chigoon_02": {

                animDictionary: "anim@amb@nightclub@dancers@club_ambientpeds@",
                 animationName: "li-mi_amb_club_09_v1_female^5", 
                },
                
                
                "a_f_y_bevhills_04": {
                
                animDictionary: "timetable@mime@ig_3",
                 animationName: "normally_dont_speak", 
                },
                
                
                "s_m_o_busker_01": {
                
                animDictionary: "anim@heists@ornate_bank@chat_manager",
                 animationName: "no_speak", 
                },
                
                "csb_car3guy2": {

                    animDictionary: "rcmnigel1aig_1",
                     animationName: "this_is_awkward_willie", 
                    },
                    
                    
                    
                    "csb_ballasog": {
                    animDictionary: "amb@world_human_drug_dealer_hard@male@base",
                     animationName: "base", 
                    },
                    
                    
                    
                    "mp_m_claude_01": {
                    animDictionary: "anim@heists@ornate_bank@chat_manager",
                     animationName: "no_speak", 
                    },
                    
                    
                    
                    "u_f_y_comjane": {
                    animDictionary: "anim@heists@ornate_bank@chat_manager",
                     animationName: "no_speak", 
                    },
                    
                    
                    
                    "a_f_y_tourist_02": {
                    animDictionary: "anim@heists@ornate_bank@chat_manager",
                     animationName: "average_car", 
                    },
                    
                    
                    
                    "ig_tracydisanto": {
                    animDictionary: "anim@heists@ornate_bank@chat_manager",
                     animationName: "no_speak", 
                    },
                    
                    
                    "s_m_y_garbage": {
                    animDictionary: "mini@repair",
                     animationName: "fixing_a_player", 
                    },
                    
                    
                    
                    "a_m_y_genstreet_01": {
                    animDictionary: "mini@repair",
                     animationName: "fixing_a_player", 
                    },
                    
                    
                    
                    "csb_grove_str_dlr": {
                    animDictionary: "anim@amb@nightclub@dancers@club_ambientpeds@",
                     animationName: "li-mi_amb_club_09_v1_female^5", 
                    },
                    
                    
                    
                    
                    "a_m_o_genstreet_01": {
                    animDictionary: "anim@heists@ornate_bank@chat_manager",
                     animationName: "no_speak", 
                    },
                    
                    
                    
                    "csb_groom": {
                    animDictionary: "anim@heists@ornate_bank@chat_manager",
                     animationName: "no_speak", 
                    },
                    
                    
                    "a_f_y_genhot_01": {
                    animDictionary: "anim@heists@ornate_bank@chat_manager",
                     animationName: "no_speak", 
                    },
                    
                    
                    "u_m_y_hippie_01": {
                    animDictionary: "anim@heists@ornate_bank@chat_manager",
                     animationName: "no_speak",     
                    },       
                    
                "ig_talina": {

                animDictionary: "anim@amb@business@bgen@bgen_no_work@",
                    animationName: "sit_phone_phoneputdown_sleeping_nowork", 
                },
                    

                "ig_josh": {
                animDictionary: "special_ped@jerome@monologue_4@monologue_4c",
                animationName:  "listentome_2"
                }, 

                
                "a_f_m_ktown_02": {
                animDictionary: "special_ped@jerome@monologue_4@monologue_4i",
                animationName:  "listentome_8"
                }, 
                

                
                "cs_movpremf_01": {
                animDictionary: "anim@heists@ornate_bank@chat_manager",
                animationName:  "no_speak"
                }, 
                
                
                "ig_nigel": {
                animDictionary: "special_ped@jerome@monologue_4@monologue_4i",
                animationName:  "listentome_8"
                }, 
                

                
                "a_m_m_paparazzi_01": {
                animDictionary: "special_ped@jerome@monologue_4@monologue_4c",
                animationName:  "listentome_2"
                }, 
                

                
                "csb_reporter": {
                animDictionary: "special_ped@jerome@monologue_4@monologue_4c",
                animationName:  "listentome_2"
                }, 
                

                
                "ig_englishdave_02": {
                animDictionary: "special_ped@jerome@monologue_4@monologue_4i",
                animationName:  "listentome_8"
                }, 
                

                
                "g_m_m_armgoon_01": {
                animDictionary: "special_ped@jerome@monologue_4@monologue_4c",
                animationName:  "listentome_2"
                }, 
                

                
                "ig_amandatownley": {
                animDictionary: "special_ped@jerome@monologue_4@monologue_4i",
                animationName:  "listentome_8"
                }, 
                
                
                "mp_m_forgery_01": {
                animDictionary: "rcmnigel1aig_1",
                animationName:  "this_is_awkward_willie"
                }, 
                
                
                "u_m_o_tramp_01": {
                animDictionary: "amb@world_human_smoking_fat@male@male_a@idle_a",
                animationName:  "idle_c"
                }, 
                
                
                "a_m_y_runner_01": {
                animDictionary: "amb@world_human_push_ups@male@base",
                animationName:  "base"
                }, 
                
                
                "a_m_y_musclbeac_02": {
                animDictionary: "amb@world_human_push_ups@male@base",
                animationName:  "base"
                }, 
                
                
                "ig_chef": {
                animDictionary: "amb@world_human_smoking_fat@male@male_a@idle_a",
                animationName:  "idle_c"
                }, 
                

                
                "a_m_m_fatlatin_01": {
                animDictionary: "special_ped@jerome@monologue_4@monologue_4i",
                animationName:  "listentome_8"
                        }, 

                
                "a_f_m_fatbla_01": {
                animDictionary: "amb@world_human_smoking_fat@male@male_a@idle_a",
                animationName:  "idle_c"
                    }, 
                        
                
                "g_f_y_families_01": {
                animDictionary: "anim@amb@machinery@vertical_mill@",
                animationName:  "look_high_amy_skater_01"
                }, 

                "csb_anton": {
                animDictionary: "special_ped@jerome@monologue_4@monologue_4i",
                animationName:  "listentome_8"
                }, 
                
                
                "cs_ashley": {
                animDictionary: "special_ped@jerome@monologue_4@monologue_4c",
                animationName:  "listentome_2"
                }, 
                
                
                "g_m_y_ballaeast_01": {
                animDictionary: "special_ped@jerome@monologue_4@monologue_4i",
                animationName:  "listentome_8"
                }, 
                
                
                "s_f_y_bartender_01": {
                animDictionary: "special_ped@jerome@monologue_4@monologue_4c",
                animationName:  "listentome_2"
                }, 
                
                
                "ig_beverly": {
                animDictionary: "special_ped@jerome@monologue_4@monologue_4c",
                animationName:  "listentome_2"
                }, 
                
                
                "a_f_y_bevhills_01": {
                animDictionary: "special_ped@jerome@monologue_4@monologue_4i",
                animationName:  "listentome_8"
                }, 
                
                
                "mp_m_boatstaff_01": {
                animDictionary: "amb@world_human_smoking_fat@male@male_a@idle_a",
                animationName:  "idle_c"
                }, 
                
                
                "a_f_y_business_01": {
                animDictionary: "anim@heists@ornate_bank@chat_manager",
                animationName:  "no_speak"
                }, 

                
                "a_m_m_business_01": {
                animDictionary: "anim@heists@ornate_bank@chat_manager",
                animationName:  "no_speak"
                }, 

                
                "u_m_y_chip": {
                animDictionary: "special_ped@jerome@monologue_4@monologue_4c",
                animationName:  "listentome_2"
                }, 

                
                "cs_dale": {
                animDictionary: "amb@world_human_smoking_fat@male@male_a@idle_a",
                animationName:  "idle_c"
                }, 

                
                "s_m_m_paramedic_01": {
                animDictionary: "amb@world_human_smoking_fat@male@male_a@idle_a",
                animationName:  "idle_c"
                }, 

                
                "s_f_y_scrubs_01": {
                animDictionary: "special_ped@jerome@monologue_4@monologue_4c",
                animationName:  "listentome_2"
                }, 
                
                
                "ig_jackie": {
                animDictionary: "special_ped@jerome@monologue_4@monologue_4c",
                animationName:  "listentome_2"
                }, 

                
                "ig_miguelmadrazo": {
                animDictionary: "special_ped@jerome@monologue_4@monologue_4i",
                animationName:  "listentome_8"
                }, 
                

                "s_m_m_highsec_04": {
                animDictionary: "mini@strip_club@idles@bouncer@base",
                animationName:  "base"
                }, 
                

                
                "csb_isldj_00": {
                animDictionary: "mini@hookers_sp",
                animationName:  "idle_reject_loop_a"
                }, 
                
                
                "ig_mjo": {
                animDictionary: "mini@strip_club@idles@bouncer@idle_a",
                animationName:  "idle_a"
                }, 
                
                
                "cs_tenniscoach": {
                animDictionary: "anim@amb@machinery@vertical_mill@",
                animationName:  "look_high_amy_skater_01"
                }, 
                
                
                "ig_vagspeak": {
                animDictionary: "combat@aim_variations@arrest",
                animationName:  "cop_med_arrest_01"
                }, 
                
                
                "mp_m_g_vagfun_01": {
                animDictionary: "mini@strip_club@idles@bouncer@base",
                animationName:  "base"
                }, 
                
                
                "mp_m_weapexp_01": {
                animDictionary: "random@arrests@busted",
                animationName:  "idle_b"
                }, 
                
                
                "s_m_y_devinsec_01": {
                animDictionary: "special_ped@jerome@monologue_4@monologue_4c",
                animationName:  "listentome_2"
                }, 
                
                
                "a_m_y_downtown_01": {
                animDictionary: "special_ped@jerome@monologue_4@monologue_4i",
                animationName:  "listentome_8"
                }, 
                
                
                "cs_milton": {
                animDictionary: "amb@world_human_drug_dealer_hard@male@base",
                animationName:  "base"
                }, 
                
                
                "u_f_o_moviestar": {
                animDictionary: "mini@hookers_sp",
                animationName:  "idle_reject_loop_a"
                }, 
                
                
                "s_m_m_highsec_02": {
                animDictionary: "mini@strip_club@idles@bouncer@base",
                animationName:  "base"
                }, 
                
                
                "s_m_m_fieldworker_01": {
                animDictionary: "special_ped@jerome@monologue_4@monologue_4i",
                animationName:  "listentome_8"
                }, 
                

                "ig_isldj_04": {
                animDictionary: "special_ped@jerome@monologue_4@monologue_4c",
                animationName:  "listentome_2"
                }, 
                
                
                "a_f_y_clubcust_04": {
                animDictionary: "cellphone@self@franklin@",
                animationName:  "peace"
                }, 
                
                "a_m_m_skater_01": {
                animDictionary: "rcmnigel1aig_1",
                animationName:  "this_is_awkward_willie"
                }, 
                
                
                "a_m_m_soucent_01": {
                animDictionary: "amb@world_human_smoking_fat@male@male_a@idle_a",
                animationName:  "idle_c"
                }, 
                
                
                "u_f_y_poppymich": {
                animDictionary: "amb@world_human_smoking_fat@male@male_a@idle_a",
                animationName:  "idle_c"
                }, 
                
                
                "g_m_y_mexgoon_02": {
                animDictionary: "special_ped@jerome@monologue_4@monologue_4c",
                animationName:  "listentome_2"
                }, 
                
                "csb_porndudes": {
                animDictionary: "misschinese2_crystalmazemcs1_cs",
                animationName:  "dance_loop_tao"
                }, 
                
                
                "mp_f_deadhooker": {
                animDictionary: "misschinese2_crystalmazemcs1_ig",
                animationName:  "bar_peds_action_janet"
                }, 
                
                
                "csb_isldj_04": {
                animDictionary: "anim@mp_player_intcelebrationfemale@dj",
                animationName:  "dj"
                    
                }, 

                "g_m_y_lost_01": {
                animDictionary: "oddjobs@assassinate@multi@windowwasher",
                animationName:  "_wash_loop"
                }, 
                
                
                "g_m_y_mexgoon_03": {
                animDictionary: "oddjobs@assassinate@multi@windowwasher",
                animationName:  "_wash_loop"
                }, 
                
                
                "a_m_m_mexlabor_01": {
                animDictionary: "anim@heists@chicken_heist@ig_5_guard_wave_in",
                animationName:  "guard_reaction_in_booth"
                }, 
                
                
                "a_m_y_salton_01": {
                animDictionary: "oddjobs@assassinate@multi@windowwasher",
                animationName:  "_wash_loop"
                }, 
                
                "cs_floyd": {
                animDictionary: "special_ped@jerome@monologue_4@monologue_4c",
                animationName:  "listentome_2"
                }, 
                
                
                "cs_stevehains": {
                animDictionary: "special_ped@jerome@monologue_4@monologue_4i",
                animationName:  "listentome_8"
                }, 
                        
                "csb_stripper_01": {
                animDictionary: "misschinese2_crystalmazemcs1_ig",
                animationName:  "bar_peds_action_janet"
                }, 
                
                
                "ig_tonya": {
                animDictionary: "anim@amb@nightclub@dancers@club_ambientpeds@",
                animationName:  "li-mi_amb_club_09_v1_female^5"
                }, 
                
                
                "s_f_y_hooker_02": {
                animDictionary: "anim@amb@casino@mini@dance@dance_solo@female@var_a@",
                animationName:  "med_left_up"
                }, 
                
                
                "cs_lifeinvad_01": {
                animDictionary: "anim@amb@casino@mini@dance@dance_solo@female@var_a@",
                animationName:  "med_right_up"
                }, 
                
                
                "ig_maryann": {
                animDictionary: "anim@amb@casino@mini@dance@dance_solo@female@var_a@",
                animationName:  "high_center"
                }, 
                    
                "ig_michelle": {
                animDictionary: "anim@amb@nightclub@dancers@club_ambientpeds@",
                animationName:  "li-mi_amb_club_09_v1_female^5"
                }, 
                
                "a_m_y_mexthug_01": {
                animDictionary: "mini@strip_club@idles@bouncer@idle_a",
                animationName:  "idle_a "
                },

                
                "a_f_y_vinewood_03": {
                animDictionary: "anim@amb@nightclub@dancers@club_ambientpeds@",
                animationName:  "li-mi_amb_club_09_v1_female^5"
                }, 
                
                
                "g_f_y_vagos_01": {
                animDictionary: "anim@heists@ornate_bank@chat_manager",
                animationName:  "no_speak"
                }, 
                
                
                "u_m_m_willyfist": {
                animDictionary: "anim@heists@ornate_bank@chat_manager",
                animationName:  "no_speak"
                }, 
                                                    
                
                "s_m_y_xmech_02": {
                animDictionary: "timetable@mime@ig_3",
                animationName:  "normally_dont_speak"
                }, 
                
                
                "s_m_y_pilot_01": {
                animDictionary: "anim@heists@ornate_bank@chat_manager",
                animationName:  "no_speak"
                }, 

                "s_m_y_valet_01": {
                animDictionary: "anim@heists@ornate_bank@chat_manager",
                animationName:  "no_speak"
                }, 
                
                "cs_zimbor": {
                animDictionary: "anim@heists@ornate_bank@chat_manager",
                animationName:  "no_speak"
                }, 
                
                "mp_m_execpa_01": {
                animDictionary: "cellphone@",
                animationName:  "cellphone_call_listen_base"
                }, 
                                        
                "ig_agent": {
                animDictionary: "special_ped@jerome@monologue_4@monologue_4c",
                animationName:  "listentome_2"
                }, 
            
                
                "mp_f_execpa_02": {
                animDictionary: "special_ped@jerome@monologue_4@monologue_4i",
                animationName:  "listentome_8"
                }, 
                
                
                "a_m_o_beach_02": {
                animDictionary: "anim@heists@ornate_bank@chat_manager",
                animationName:  "no_speak"
                }, 
                
                
                "csb_jio": {
                animDictionary: "anim@heists@ornate_bank@chat_manager",
                animationName:  "no_speak"
                }, 
                
                
                "ig_patricia_02": {
                animDictionary: "timetable@mime@ig_3",
                animationName:  "normally_dont_speak"
                }, 
                
                
                
                "g_m_m_cartelguards_02": {
                animDictionary: "anim@heists@ornate_bank@chat_manager",
                animationName:  "no_speak"
                }, 
                
                
                "a_m_y_vinewood_01": {
                animDictionary: "timetable@mime@ig_3",
                animationName:  "normally_dont_speak"
                }, 
                
                
                "a_f_m_tramp_01": {
                animDictionary: "special_ped@jerome@monologue_4@monologue_4i",
                animationName:  "listentome_8"
                }, 
                
                
                "a_m_y_soucent_01": {
                animDictionary: "anim@heists@ornate_bank@chat_manager",
                animationName:  "no_speak"
                }, 
                
                
                "a_m_o_soucent_01": {
                animDictionary: "amb@world_human_smoking_fat@male@male_a@idle_a",
                animationName:  "idle_c"
                }, 

                
                "a_f_y_soucent_02": {
                animDictionary: "special_ped@jerome@monologue_4@monologue_4i",
                animationName:  "listentome_8"
                }, 
                
                
                "g_m_y_armgoon_02": {
                animDictionary: "timetable@mime@ig_3",
                animationName:  "normally_dont_speak"
                }, 
                
                
                "a_m_y_busicas_01": {
                animDictionary: "special_ped@jerome@monologue_4@monologue_4i",
                animationName:  "listentome_8"
                }, 
                
                
                "g_m_m_chiboss_01": {
                animDictionary: "amb@world_human_smoking_fat@male@male_a@idle_a",
                animationName:  "idle_c"
                }, 
                
                
                "csb_chef2": {
                animDictionary: "amb@world_human_smoking_fat@male@male_a@idle_a",
                animationName:  "idle_c"
                }, 
                
                "ig_tylerdix": {
                animDictionary: "amb@world_human_push_ups@male@base",
                animationName:  "base"
                }, 
                
                
                "a_f_y_tourist_01": {
                animDictionary: "anim@heists@ornate_bank@chat_manager",
                animationName:  "no_speak"
                }, 
                
                
                "a_f_y_tennis_01": {
                animDictionary: "special_ped@jerome@monologue_4@monologue_4c",
                animationName:  "listentome_2"
                }, 
                
                
                "cs_wade": {
                animDictionary: "rcmcollect_paperleadinout@",
                animationName:  "meditiate_idle"
                }, 
                
                
                "a_m_y_epsilon_01": {
                animDictionary: "amb@world_human_bum_wash@male@low@idle_a",
                animationName:  "idle_a"
                }, 
                            
                
                "u_f_o_prolhost_01": {
                animDictionary: "amb@world_human_smoking_fat@male@male_a@idle_a",
                animationName:  "idle_c"
                }, 
                
                
                "ig_beverly": {
                animDictionary: "amb@world_human_clipboard@male@idle_a",
                animationName:  "idle_a"
                }, 
                
                
                "csb_bride": {
                animDictionary: "special_ped@jerome@monologue_4@monologue_4c",
                animationName:  "listentome_2"
                }, 
                
                
                "mp_f_forgery_01": {
                animDictionary: "special_ped@jerome@monologue_4@monologue_4c",
                animationName:  "listentome_2"
                }, 

                
                "ig_oldrichguy": {
                animDictionary: "special_ped@jerome@monologue_4@monologue_4i",
                animationName:  "listentome_8"
                }, 
                
                
                "csb_juanstrickler": {
                animDictionary: "amb@world_human_smoking_fat@male@male_a@idle_a",
                animationName:  "idle_c"
                }, 
                
                
                "a_m_y_vinewood_02": {
                animDictionary: "anim@amb@casino@valet_scenario@pose_b@",
                animationName:  "kick_floor_a_m_y_vinewood_01"
                }, 
                
                
                "cs_amandatownley": {
                animDictionary: "timetable@mime@ig_3",
                animationName:  "normally_dont_speak"
                }, 
                
                
                "ig_bestmen": {
                animDictionary: "anim@heists@ornate_bank@chat_manager",
                animationName:  "no_speak"
                }, 
                
                "cs_debra": {
                animDictionary: "amb@world_human_smoking_fat@male@male_a@idle_a",
                animationName:  "idle_c"
                }, 
                
                
                "cs_davenorton": {
                animDictionary: "cellphone@",
                animationName:  "cellphone_call_listen_base"
                }, 
                
                
                "a_f_m_eastsa_01": {
                animDictionary: "special_ped@jerome@monologue_4@monologue_4i",
                animationName:  "listentome_8"
                }, 
                
                
                "cs_dom": {
                animDictionary: "anim@heists@ornate_bank@chat_manager",
                animationName:  "no_speak"
                }, 
                
                
                "cs_drfriedlander": {
                animDictionary: "anim@amb@casino@valet_scenario@pose_b@",
                animationName:  "kick_floor_a_m_y_vinewood_01"
                }, 

                "s_f_y_shop_mid": {
                animDictionary: "misschinese2_crystalmazemcs1_ig",
                animationName:  "bar_peds_action_janet"
                }, 

                
                "s_m_y_shop_mask": {
                animDictionary: "anim@amb@nightclub@dancers@club_ambientpeds@",
                animationName:  "li-mi_amb_club_09_v1_female^5"
                }, 

                
                "csb_iSLdj_02": {
                animDictionary: "misslester1aig_2main",
                animationName:  "hr_greet_gamer"
                }, 
                
                
                "ig_gustavo": {
                animDictionary: "anim@amb@casino@mini@dance@dance_solo@female@var_a@",
                animationName:  "high_center"
                }, 
                
                
                "ig_kaylee": {
                animDictionary: "anim@amb@casino@mini@dance@dance_solo@female@var_a@",
                animationName:  "med_right_up"
                }, 
                    
                "s_f_y_hooker_03": {
                animDictionary: "misschinese2_crystalmazemcs1_ig",
                animationName:  "bar_peds_action_janet"
                }, 
                

                "a_m_y_hippy_01": {
                animDictionary: "anim@amb@nightclub@dancers@club_ambientpeds@",
                animationName:  "li-mi_amb_club_09_v1_female^5"
                }, 
                
                
                "g_f_y_lost_01": {
                animDictionary: "anim@amb@nightclub@dancers@club_ambientpeds@",
                animationName:  "li-mi_amb_club_09_v1_female^5"
                }, 
                
                
                "a_f_y_beach_02": {
                animDictionary: "anim@amb@nightclub@dancers@club_ambientpeds@",
                animationName:  "li-mi_amb_club_09_v1_female^5"
                }, 
                                        
                
                "a_f_y_vinewood_01": {
                animDictionary: "anim@amb@casino@mini@dance@dance_solo@female@var_a@",
                animationName:  "high_center"
                }, 
                    
                "u_m_y_burgerdrug_01": {
                animDictionary: "misschinese2_crystalmazemcs1_ig",
                animationName:  "bar_peds_action_janet"
                }, 
                
                
                "s_f_m_maid_01": {
                animDictionary: "anim@amb@nightclub@dancers@club_ambientpeds@",
                animationName:  "li-mi_amb_club_09_v1_female^5"
                }, 
                
                
                "u_f_y_hotposh_01": {
                animDictionary: "anim@amb@casino@mini@dance@dance_solo@female@var_a@",
                animationName:  "med_left_up"
                }, 
                
                
                "ig_fabien": {
                animDictionary: "anim@amb@casino@mini@dance@dance_solo@female@var_a@",
                animationName:  "med_right_up"
                }, 
                    
                    
                "a_m_y_gay_01": {
                animDictionary: "misschinese2_crystalmazemcs1_cs",
                animationName:  "dance_loop_tao"
                }, 

                "ig_chengsr": {
                animDictionary: "timetable@mime@ig_3",
                animationName:  "normally_dont_speak"
                }, 

                
                "a_f_m_trampbeac_01": {
                animDictionary: "special_ped@jerome@monologue_4@monologue_4i",
                animationName:  "listentome_8"
                }, 

                
                "s_m_y_cop_01": {
                animDictionary: "amb@world_human_push_ups@male@base",
                animationName:  "base"
                }, 

                
                "s_m_m_fibsec_01": {
                animDictionary: "amb@world_human_bum_wash@male@low@idle_a",
                animationName:  "idle_a"
                }, 

                
                 "s_f_y_beachbarstaff_01": {
                animDictionary: "special_ped@jerome@monologue_4@monologue_4i",
                animationName:  "listentome_8"
                }, 

                "mp_f_chbar_01": {
                animDictionary: "special_ped@jerome@monologue_4@monologue_4c",
                animationName:  "listentome_2"
                }, 

                
                "mp_m_securoguard_01": {
                animDictionary: "amb@world_human_cop_idles@female@base",
                animationName:  "base"
                }, 

                
                "a_m_m_prolhost_01": {
                animDictionary: "cellphone@",
                animationName:  "cellphone_call_listen_base"
                }, 

                "csb_chef": {
                animDictionary: "amb@world_human_smoking_fat@male@male_a@idle_a",
                animationName:  "idle_c"
                }, 

                
                "ig_clay": {
                animDictionary: "special_ped@jerome@monologue_4@monologue_4c",
                animationName:  "listentome_2"
                }, 

                
                "ig_cletus": {
                animDictionary: "special_ped@jerome@monologue_4@monologue_4i",
                animationName:  "listentome_8"
                }, 
                
                
                "csb_denise_friend": {
                animDictionary: "rcmnigel1aig_1",
                animationName:  "this_is_awkward_willie"
                }, 

                "u_f_y_mistress": {
                animDictionary: "anim@heists@ornate_bank@chat_manager",
                animationName:  "no_speak"
                }, 
                
                
                "a_m_y_methhead_01": {
                animDictionary: "anim@heists@ornate_bank@chat_manager",
                animationName:  "no_speak"
                }, 
                
                
                "a_m_y_latino_01": {
                animDictionary: "anim@amb@machinery@vertical_mill@",
                animationName:  "look_high_amy_skater_01"
                }, 

                "csb_prologuedriver": {
                animDictionary: "melee@small_wpn@streamed_core",
                animationName:  "kick_close_a"
                }, 

                
                "cs_nervousron": {
                animDictionary: "missheistdockssetup1ig_13@kick_idle",
                animationName:  "guard_beatup_kickidle_dockworker"
                }, 

                
                "ig_tenniscoach": {
                animDictionary: "amb@world_human_smoking_fat@male@male_a@idle_a",
                animationName:  "idle_c"
                }, 

                
                "a_m_m_tennis_01": {
                animDictionary: "special_ped@jerome@monologue_4@monologue_4i",
                animationName:  "listentome_8"
                }, 
                
                
                "cs_tanisha": {
                animDictionary: "anim@heists@ornate_bank@chat_manager",
                animationName:  "no_speak"
                }, 
                
                
                "mp_m_meth_01": {
                animDictionary: "special_ped@jerome@monologue_4@monologue_4c",
                animationName:  "listentome_2"
                }, 

                "s_m_m_security_01": {
                animDictionary: "combat@aim_variations@arrest",
                animationName:  "cop_med_arrest_01"
                }, 
                
                
                "mp_m_shopkeep_01": {
                animDictionary: "random@arrests@busted",
                animationName:  "idle_b"
                }, 
                
                "cs_chengsr": {
                animDictionary: "amb@world_human_prostitute@french@idle_a",
                animationName:  "idle_a"
                }, 
                
                "g_m_importexport_01": {
                animDictionary: "special_ped@jerome@monologue_4@monologue_4c",
                animationName:  "listentome_2"
                }, 
                
                
                "csb_ary": {
                animDictionary: "anim@heists@ornate_bank@chat_manager",
                animationName:  "no_speak"
                }, 

                
                "a_m_y_clubcust_04": {
                animDictionary: "special_ped@jerome@monologue_4@monologue_4i",
                animationName:  "listentome_8"
                }, 
                
                
                "a_f_y_femaleagent": {
                animDictionary: "amb@world_human_bum_wash@male@low@idle_a",
                animationName:  "idle_a"
                }, 
                
                "cs_mrk": {
                animDictionary: "anim@amb@nightclub@dancers@club_ambientpeds@",
                animationName:  "li-mi_amb_club_09_v1_female^5"
                },   */
                 
                "s_f_y_cop_01": {
                    animDictionary: "mb@code_human_wander_idles_cop@female@static",
                    animationName:  "static"
                    }, 

                "s_m_m_dockwork_01": {
                    animDictionary: "amb@code_human_wander_mobile@male@base",
                    animationName:  "static"
                    }, 

                "s_m_m_dockwork_01": {
                    animDictionary: "amb@code_human_wander_idles_cop@female@static",
                    animationName:  "static"
                    }, 

                "csb_cop": {
                    animDictionary: "amb@code_human_wander_idles_cop@female@static",
                    animationName:  "static"
                    }, 

                "s_m_m_autoshop_02": {
                    animDictionary: "amb@world_human_smoking@male@male_b@base",
                    animationName:  "base"
                    }, 

                "s_m_y_construct_01": {
                    animDictionary: "amb@medic@standing@kneel@base",
                    animationName:  "base"
                    }, 

                "csb_trafficwarden": {
                    animDictionary: "amb@incar@male@smoking@base",
                    animationName:  "base"
                    }, 

                "s_m_m_ccrew_01": {
                    animDictionary: "amb@lo_res_idles@",
                    animationName:  "world_human_lean_female_holding_elbow_lo_res_base"
                    }, 

                "s_m_y_garbage": {
                    animDictionary: "amb@lo_res_idles@",
                    animationName:  "world_human_lean_female_holding_elbow_lo_res_base"
                    }, 

                "s_m_m_gaffer_01": {
                    animDictionary: "amb@code_human_wander_mobile@male@base",
                    animationName:  "static"
                    }, 

                "cs_floyd": {
                    animDictionary: "amb@world_human_smoking@male@male_b@enter",
                    animationName:  "enter"
                    }, 

                "s_m_m_lathandy_01": {
                    animDictionary: "amb@lo_res_idles@",
                    animationName:  "world_human_lean_female_holding_elbow_lo_res_base"
                    }, 

                "s_m_y_dwservice_02": {
                    animDictionary: "amb@code_human_wander_mobile@male@base",
                    animationName:  "static"
                    }, 

                "cs_prolsec_02": {
                    animDictionary: "amb@world_human_smoking@male@male_b@base",
                    animationName:  "base"
                    }, 

                "PrologueSec01C": {
                    animDictionary: "amb@code_human_wander_idles_cop@female@static",
                    animationName:  "static"
                    }, 

                "s_m_y_hwaycop_01": {
                    animDictionary: "amb@incar@male@smoking@base",
                    animationName:  "base"
                    }, 

                "s_f_y_ranger_01": {
                    animDictionary: "amb@code_human_wander_idles_cop@female@static",
                    animationName:  "static"
                    }, 

                "s_m_m_fibsec_01": {
                    animDictionary: "amb@world_human_smoking@male@male_b@base",
                    animationName:  "base"
                    }, 

                    
                "s_m_m_fibsec_01": {
                    animDictionary: "amb@world_human_smoking@male@male_b@base",
                    animationName:  "base"
                    }, 
                    
                "s_m_m_ciasec_01": {
                    animDictionary: "amb@world_human_smoking@male@male_b@base",
                    animationName:  "base"
                    }, 

    }

const startActorName = "npc_zdobich";

const selectStartQuests = (entity) => {
    if (global.isNewChar && questNameToPeds [startActorName] && questNameToPeds [startActorName].includes(entity)) {

        mp.events.call('notify', 3, 9, translateText("Не забудь подойти к NPC Виталий Дебич, чтобы начать стартовую линейку квестов, которая познакомит тебя с нашим чудесным штатом!"), 3000);
        global.createCamera("peds", entity);
        mp.events.call('createWaypoint', entity.position.x, entity.position.y);
        global.localplayer.freezePosition(true);

        setTimeout(function () {
            global.isNewChar = false;

            mp.gui.chat.push(translateText("Не забудь подойти к !{#ff3333}NPC Виталий Дебич!{#FFF}, чтобы начать стартовую линейку квестов, которая познакомит тебя с нашим чудесным штатом!"));

            global.cameraManager.deleteCamera('peds', true, 500);
            global.localplayer.freezePosition(false);

            isOpenEveryDayAward = true;
            global.binderFunctions.o_help();
        }, 5000);
    }
}


gm.events.add("pedStreamIn", (entity) => {
    const questPed = entity.getVariable('questName');
    if (questPed != null) {
        if (PedAnimList[questPed]) {
            global.requestAnimDict(PedAnimList[questPed].animDictionary).then(() => {
                entity.taskPlayAnim (PedAnimList[questPed].animDictionary, PedAnimList[questPed].animationName, 8, 8, -1, 1, 0, false, false, false);
            });
        }

        const hours = Natives.GET_CLOCK_HOURS ();
        if ([22, 23, 24, 0, 1, 2, 3, 4, 5, 6, 7].includes(hours)) {
            pedsLightData[entity.remoteId] = entity.position;
        }
    }
    selectStartQuests(entity);
});

gm.events.add("pedStreamOut", (entity) => {
    if (pedsLightData[entity.remoteId]) {
        delete pedsLightData[entity.remoteId];
    }
});

/*gm.events.add("render", () => {
    if (!global.loggedin) return;
    Object.values(pedsLightData).forEach((position) => {
        mp.game.graphics.drawSpotLight(position.x, position.y, position.z + 8, 0, 0, -1, 255, 255, 255, 35, 0.3, 5, 4.5, 5);
    })    
});*/

let StartQuest = null;
gm.events.add('client.questStore.init', (json) => {
    mp.gui.emmit(`window.questStore.init('${json}')`);
    
    mp.peds.forEach((ped) => {
        if (ped && ped.type === "ped") {  
            const questName = ped.getVariable('questName');
            if (typeof questName === "string" && questName.length > 3) {
                if (typeof questNameToPeds [questName] !== "object")
                    questNameToPeds [questName] = [];

                questNameToPeds [questName].push(ped);

                if (ped.handle !== 0)
                    selectStartQuests(ped);
            }     
        }
    });
    /*StartQuest = JSON.parse (json);
    if (!StartQuest) return;
    else if (!StartQuest.ActorName) return;
    else if (StartQuest.ActorName !== "npc_fd_zak") return;
    setTimeout(() => {
        mp.gui.emmit(`window.router.setView("QuestsDialog", { aName: 'npc_fd_zak', qId: 0, status: 0, compility: 0 })`);
        isQuestOpen = true;
        global.menuOpen();
    }, 25000);*/
});

let isQuestOpen = false;
let conversationPed = false;

const actorData = {
    npc_tracy:translateText("Трейси"),
    npc_doctor:translateText("Доктором Шульцем"),
    npc_granny:translateText("Бабушкой Granny"),
    npc_fd_dada:translateText("Дядюшкой"),
    npc_fd_edward:translateText("Эдвардом"),
    npc_fd_zak:translateText("Заком Цукербергом"),
    npc_airdrop:translateText("Хуаном Де Картелем"),
    npc_oressale:"Марком",
    npc_fracpolic:translateText("Работником полиции"),
    npc_fracsheriff:translateText("Шерифом"),
    npc_fracnews:translateText("Дженнифер"),
    npc_fracems:translateText("Эммануэлем"),
    npc_premium:translateText("Вовчиком"),
    npc_stock:translateText("Александром"),
    npc_huntingshop:translateText("Беаром Гриллсом"),
    npc_treessell:"Дмитрием",
    npc_donateautoroom:translateText("Донатой Редбаксовной"),
    npc_cityhall:translateText("Эльнарой Каримовой"),
    npc_wedding:translateText("Отцом Никитой"),
    npc_pet:translateText("Михаилом"),
    npc_petshop:translateText("Продавцом питомцев"),
    npc_zdobich:translateText("Виталием Дебичем"),
    npc_rieltor:translateText("Илоном Таском"),
    npc_furniture:"Иваном",
    npc_org:"Полли",
    npc_carevac:translateText("Робертом"),
    npc_airshop:translateText("Продавцом воздушного транспорта"),
    npc_eliteroom: translateText("Продавцом элитного транспорта")
}

gm.events.add('client.quest.open', (pedId, questName, qId, status, compility, speed) => {
    if (global.menuCheck()) return;
    else if (isQuestOpen)
        return;
    
    const pedData = mp.peds.atRemoteId(pedId);
    if (pedData && pedData.handle !== 0) {
        pedData.questName = questName;
        conversationPed = pedData;
        global.localplayer.setAlpha(0);
        global.speedCamera = speed;
        global.createCamera ("peds", pedData);
        mp.events.call('client.quest.startSpeech');
        if (actorData[pedData.questName]) 
            gm.discord(translateText("Болтает с {0}", actorData[pedData.questName]));
        mp.gui.emmit(`window.router.setView("QuestsDialog", { aName: '${questName}', qId: ${qId}, status: ${status}, compility: ${compility} })`);
        isQuestOpen = true;
        global.menuOpen();
    }
});

const npcExceptions = ["npc_eliteroom", "npc_airdrop", "npc_wedding", "npc_carevac", "npc_airshop", "npc_huntingshop", "npc_treessell", "npc_fd_dada"];
gm.events.add("sounds.playQuest", (sound, volume) => {
    if (conversationPed && mp.peds.exists (conversationPed)) {
        let volumeValue = global.VolumeQuest;
        if (npcExceptions.indexOf(conversationPed.questName) !== -1) {
            volumeValue *= 3.5;
        }

        new soundApi.Sounds3DPos ("ped_" + conversationPed.remoteId, sound, conversationPed.getBoneCoords(12844, 0, 0, 0), {
            volume: soundApi.getVolume (volume, volumeValue),
            looped: false,
            maxDistance: 5,
            rolloffFactor: 1,
            refDistance: 1,
            startOffsetPercent: 0,
            fade: 1000,
            syncAudio: 0
        });
    }
});


gm.events.add("sounds.stopQuest", () => {
    if (conversationPed && mp.peds.exists (conversationPed)) {
        mp.events.call("sounds.trigger", "onEnd", "ped_" + conversationPed.remoteId);
    }
});

gm.events.add('client.quest.startSpeech', () => {
    if (conversationPed && mp.peds.exists (conversationPed)) {
        conversationPed.playFacialAnim("mic_chatter", "mp_facial");
    }
});

gm.events.add('client.quest.close', (isRemote = true) => {
    if (!isQuestOpen)
        return;
    global.localplayer.setAlpha(255);
    // /global.cameraManager.stopCamera (true, 500);
    global.cameraManager.deleteCamera ('peds', true, 500);
    isQuestOpen = false;
    global.menuClose();
    mp.gui.emmit('window.router.setHud();');
    mp.events.call('sounds.stopQuest');
    if (conversationPed && mp.peds.exists (conversationPed)) {
        conversationPed.playFacialAnim("mood_normal_1", "facials@gen_male@variations@normal");
    }
    conversationPed = false;
    if (isRemote)
        mp.events.callRemote('server.quest.clear');
});

gm.events.add('client.quest.perform', (isClose) => {
    if (!isQuestOpen)
        return;

    mp.events.callRemote('server.quest.perform', isClose);

    if (isClose)
        mp.events.call('client.quest.close', false);
});

gm.events.add('client.quest.action', (isClose) => {
    if (!isQuestOpen)
        return;

    mp.events.callRemote('server.quest.action', isClose);

    if (isClose)
        mp.events.call('client.quest.close', false);
});

gm.events.add('client.quest.take', (index = 0) => {
    if (!isQuestOpen)
        return;
        
    mp.events.callRemote('server.quest.take', index);
    
    mp.events.call('client.quest.close', false);
});

const getNearestPed = (ActorName) => {
    let returnPed = null;

    if (typeof questNameToPeds [ActorName] === "object") {
        const playerPos = global.localplayer.position;

        questNameToPeds [ActorName].forEach((ped) => {
            if (mp.peds.exists (ped)) {
                if (returnPed === null)
                    returnPed = ped;
                else if (global.vdist2(ped.position, playerPos, true) < global.vdist2(returnPed, playerPos, true))
                    returnPed = ped;
            }
        });
    }
    return returnPed;
}

gm.events.add('client.quest.router', (ActorName) => {
    try {
        const ped = getNearestPed (ActorName);
        if (ped != null && mp.peds.exists (ped)) {
            mp.events.call('notify', 2, 9, translateText("Вы поставили метку на карте!"), 3000);
            mp.events.call('createWaypoint', ped.position.x, ped.position.y);
        }
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "quests", "client.quest.router", e.toString());
    }
});

let UsePedPosition = {}

gm.events.add('client.quest.selectQuest.Add', (ActorName, isEnd) => {
    try {
        if (ActorName && questNameToPeds [ActorName]) {
            UsePedPosition [ActorName] = {
                peds: questNameToPeds [ActorName],
                isEnd: isEnd
            }
        }
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "quests", "client.quest.selectQuest.Add", e.toString());
    }
});

gm.events.add('client.quest.selectQuest.Clear', () => {

    UsePedPosition = {}
});

gm.events.add('client.quest.selectQuest', (actorName) => {
    mp.events.callRemote('server.quest.selectQuest', actorName);
});

gm.events.add('client.quest.selectedQuest', (actorName) => {
    mp.gui.emmit(`window.questStore.selectQuest('${actorName}')`);
});

const colors = [255, 255, 255, 255];

gm.events.add("render", () => {
    if (!global.loggedin) return;

    const questsData = Object.values (UsePedPosition);
    if (questsData.length > 0) {
        const localPos = global.localplayer.position;
        questsData.forEach((questData) => {
            questData.peds.forEach((ped) => {
                if (mp.peds.exists (ped)) {

                    const dist = mp.game.system.vdist(ped.position.x, ped.position.y, ped.position.z, localPos.x, localPos.y, localPos.z);
                    if (dist < 10.0) {
                        const pos = mp.game.graphics.world3dToScreen2d(ped.position.x, ped.position.y, ped.position.z + 1);

                        if (!pos)
                            return false;

                        if (questData.isEnd)
                            global.DrawSprite("redage_textures_001", "quest_take", colors, pos.x, pos.y);
                        else
                            global.DrawSprite("redage_textures_001", "quest_perform", colors, pos.x, pos.y);
                    }
                }
            })
        });
    }
});

///////////////////////////////////////////////////////////////

const items_positions = [
    { x: -869.7544, y: -1494.4075, z: 5.1708245},
    { x: -1139.287, y: -525.0403, z: 32.953224 }
];

const collecting_items = {
    blip: undefined,
    area_shape: undefined,
    item_shape: undefined,
    object: undefined,
    collected: 0
};

gm.events.add('client.start.collecting_items', (index) => {
    collecting_items.collected = index;

    clearCollectingItemsInfo();
    if (items_positions[index]) {
        let item_pos = items_positions[index];
        collecting_items.blip = mp.blips.new(489, new mp.Vector3(item_pos.x, item_pos.y), { alpha: 255, color: 75, name: translateText("Мишка") });
        collecting_items.item_shape = mp.colshapes.newSphere(item_pos.x, item_pos.y, item_pos.z, 1);
        collecting_items.area_shape = mp.colshapes.newSphere(item_pos.x, item_pos.y, item_pos.z, 25);
        collecting_items.object = mp.objects.new(mp.game.joaat("v_ilev_mr_rasberryclean"), new mp.Vector3(item_pos.x, item_pos.y, item_pos.z - 1.05), { rotation: new mp.Vector3(0, 0, 30), dimension: -1 });
    }
});
/*
gm.events.add('client.take_quest_item', () => {
    // проверки мб нужные

    // таймаут и анимацию сбора мб

    clearCollectingItemsInfo();

    if ((collecting_items.collected + 1) >= 5) {
        mp.gui.chat.push(translateText("Вы собрали всех Мишек, отдайте их Бабушке Granny!"));
        mp.events.call('notify', 2, 9, translateText("Вы собрали всех Мишек, отдайте их Бабушке Granny!"), 3000);
        // необходимый запрос на сервер
    } else {
        collecting_items.collected += 1;

        if (collecting_items.collected == 4) {
            mp.gui.chat.push(translateText("Вы собрали 4/5 Мишек. Остался последний Мишка. На карте отмечено следующее примерное расположение последнего Мишки!"));
            mp.events.call('notify', 2, 9, translateText("Вы собрали 4/5 Мишек. Остался последний Мишка. На карте отмечено следующее примерное расположение последнего Мишки!"), 3000);
        } else {
            mp.gui.chat.push(translateText("Вы собрали {0}/5 Мишек, на карте отмечено следующее примерное расположение Мишки!"));
            mp.events.call('notify', 2, 9, translateText("Вы собрали {0}/5 Мишек, на карте отмечено следующее примерное расположение Мишки!"), 3000);
        }

        let item_pos = items_positions[collecting_items.collected];
        collecting_items.blip = mp.blips.new(489, new mp.Vector3(item_pos.x, item_pos.y), { alpha: 255, color: 75, name: translateText("Мишка") });
        collecting_items.item_shape = mp.colshapes.newSphere(item_pos.x, item_pos.y, item_pos.z, 1);
        collecting_items.area_shape = mp.colshapes.newSphere(item_pos.x, item_pos.y, item_pos.z, 25);
        collecting_items.object = mp.objects.new(mp.game.joaat('v_ilev_mr_rasberryclean'), new mp.Vector3(item_pos.x, item_pos.y, item_pos.z - 1.05), { rotation: new mp.Vector3(0, 0, 30), dimension: -1 });
    }
});*/

function clearCollectingItemsInfo() {
    mp.events.call('hud.cEnter');
    global.selectBear = -1;

    if (collecting_items.blip !== undefined) {
        collecting_items.blip.destroy();
        collecting_items.blip = undefined;
    }

    if (collecting_items.area_shape !== undefined) {
        collecting_items.area_shape.destroy();
        collecting_items.area_shape = undefined;
    }

    if (collecting_items.item_shape !== undefined) {
        collecting_items.item_shape.destroy();
        collecting_items.item_shape = undefined;
    }

    if (collecting_items.object !== undefined) {
        collecting_items.object.destroy();
        collecting_items.object = undefined;
    }
}

global.selectBear = -1;

gm.events.add('playerEnterColshape', (shape) => {
	try
	{
        if (shape && shape === collecting_items.area_shape) {
            mp.gui.chat.push(translateText("Где то в этой местности есть Плюшевый Мишка, найди и забери его."));
            mp.events.call('notify', 2, 9, translateText("Где то в этой местности есть Плюшевый Мишка, найди и забери его."), 3000);
        }

        if (shape && shape === collecting_items.item_shape) {
            mp.events.call('hud.oEnter', "Bear");
            global.selectBear = collecting_items.collected;
        }

        if (shape && shape === npc_dfday_mission.shape) {
            if (npc_dfday_mission.type == 1) {
                mp.events.call('hud.oEnter', "BoatFix");
                global.dfdayMissionCanPress = true;
            }
            else if (npc_dfday_mission.type == 2) {
                mp.events.call('client.update.npc_dfday_mission');
            }
        }
	}
	catch (e) 
    {
        mp.events.callRemote("client_trycatch", "events/halloween", "playerEnterColshape2", e.toString());
    }
});

gm.events.add('playerExitColshape', (shape) => {
	try
	{
		if (shape && shape === collecting_items.item_shape) {
			mp.events.call('hud.cEnter');
			global.selectBear = -1;
		}

        if (shape && shape === npc_dfday_mission.shape) {
            mp.events.call('hud.oEnter');
            global.dfdayMissionCanPress = false;
        }
	}
	catch (e) 
    {
        mp.events.callRemote("client_trycatch", "events/halloween", "playerExitColshape2", e.toString());
    }
});

// k 23 fevralya:

const npcEdward_doc_positions = [
    { x: -869.7544, y: -1494.4075, z: 4.8708245},
    { x: -1139.287, y: -525.0403, z: 32.653224 }
];

const npc_dfday_mission = {
    blip: undefined,
    shape: undefined,
    marker: undefined,
    type: 0,
    collected: 0
};

gm.events.add('client.create.npc_dfday_mission', (mission_type) => {
    clearDFDayMissionInfo();

    npc_dfday_mission.type = mission_type;

    if (mission_type == 1) {
        npc_dfday_mission.shape = mp.colshapes.newSphere(477.62192, -578.17017, 28.498318, 1);
        npc_dfday_mission.marker = mp.markers.new(1, new mp.Vector3(477.62192, -578.17017, 28.498318 - 1.25), 1, {
            visible: true,
            dimension: 0,
            color: [255, 255, 255, 220] 
        });
    }
    else if (mission_type == 2) {
        let item_pos = npcEdward_doc_positions[npc_dfday_mission.collected];
        npc_dfday_mission.blip = mp.blips.new(764, new mp.Vector3(item_pos.x, item_pos.y), { alpha: 255, color: 17, name: translateText("Документы") });
        npc_dfday_mission.shape = mp.colshapes.newSphere(item_pos.x, item_pos.y, item_pos.z, 3);
        npc_dfday_mission.marker = mp.markers.new(1, new mp.Vector3(item_pos.x, item_pos.y, item_pos.z - 1.25), 3, {
            visible: true,
            dimension: 0,
            color: [255, 255, 255, 220] 
        });
        mp.events.call('notify', 1, 9, translateText("На карте отмечены документы, которые ты должен собрать."), 3000);
    }
});

gm.events.add('client.update.npc_dfday_mission', () => {
    try {
        if (npc_dfday_mission.type == 1) {
            if (npc_dfday_mission.collected > 0) {
                mp.events.call('notify', 0, 9, translateText("Вы уже начали чинить машину."), 3000);
                return;
            }
    
            npc_dfday_mission.collected = 1;
    
            mp.events.call('freeze', true);
            global.requestAnimDict('amb@world_human_hammering@male@base').then(() => {
                global.localplayer.taskPlayAnim('amb@world_human_hammering@male@base', 'base', 8.0, -8.0, -1, 1, 0, false, false, false);
            });
    
            setTimeout(() => {
                clearDFDayMissionInfo();
                global.localplayer.clearTasksImmediately();
                mp.events.call('freeze', false);
                mp.events.call('notify', 2, 9, translateText("Ты починил машину, поговори с Дядюшкой."), 3000);
                mp.events.callRemote('server.update.npc_dfday_mission');
            }, 10000);
        }
        else if (npc_dfday_mission.type == 2) {
            if ((npc_dfday_mission.collected + 1) >= 5) {
                clearDFDayMissionInfo();
                mp.events.call('notify', 2, 9, translateText("Вы развезли все документы, поговорите с Эдвардом снова."), 3000);
                mp.events.call('createWaypoint', 2.710338, -708.8864);
                mp.events.callRemote('server.update.npc_fd_edward');
                return;
            }
    
            npc_dfday_mission.collected += 1;
    
            if (npc_dfday_mission.blip !== undefined) {
                npc_dfday_mission.blip.destroy();
                npc_dfday_mission.blip = undefined;
            }
    
            if (npc_dfday_mission.shape !== undefined) {
                npc_dfday_mission.shape.destroy();
                npc_dfday_mission.shape = undefined;
            }
    
            if (npc_dfday_mission.marker !== undefined) {
                npc_dfday_mission.marker.destroy();
                npc_dfday_mission.marker = undefined;
            }
    
            let item_pos = npcEdward_doc_positions[npc_dfday_mission.collected];
            npc_dfday_mission.blip = mp.blips.new(764, new mp.Vector3(item_pos.x, item_pos.y), { alpha: 255, color: 17, name: translateText("Документы") });
            npc_dfday_mission.shape = mp.colshapes.newSphere(item_pos.x, item_pos.y, item_pos.z, 3);
            npc_dfday_mission.marker = mp.markers.new(1, new mp.Vector3(item_pos.x, item_pos.y, item_pos.z - 1.25), 3, {
                visible: true,
                dimension: 0,
                color: [255, 255, 255, 220] 
            });
    
            mp.events.call('notify', 2, 9, translateText("Вы привезли документы. Двигайтесь к следующему месту!"), 3000);
        }
    }
    catch (e) 
	{
		mp.events.callRemote("client_trycatch", "player/npc_dialogs", "client.update.npc_dfday_mission", e.toString());
	}
});

function clearDFDayMissionInfo() {
    try
	{
        mp.events.call('hud.oEnter');
        global.dfdayMissionCanPress = false;

        npc_dfday_mission.type = 0;
        npc_dfday_mission.collected = 0;

        if (npc_dfday_mission.blip !== undefined) {
            npc_dfday_mission.blip.destroy();
            npc_dfday_mission.blip = undefined;
        }

        if (npc_dfday_mission.shape !== undefined) {
            npc_dfday_mission.shape.destroy();
            npc_dfday_mission.shape = undefined;
        }

        if (npc_dfday_mission.marker !== undefined) {
            npc_dfday_mission.marker.destroy();
            npc_dfday_mission.marker = undefined;
        }
	}
	catch (e) 
    {
        mp.events.callRemote("client_trycatch", "player/npc_dialogs", "clearDFDayMissionInfo", e.toString());
    }
}