<script>
    import { fly } from 'svelte/transition';
</script>
<div class="hudevo__lobby" in:fly={{ y: -50, duration: 500 }} out:fly={{ y: -50, duration: 250 }}>
    <div class="box-flex">
        <div class="hudevo__lobby_header">Лобби</div>
        <div class="hudevo__lobby_count">2/4</div>
    </div>
    <div class="hudevo__lobby_element hudevo__elementparams paramsright">
        Vitaliy_Zdobich <span class="hudevoicon-person"></span>
    </div>
    <div class="hudevo__lobby_health"><div class="currenthealth"></div></div>
</div>