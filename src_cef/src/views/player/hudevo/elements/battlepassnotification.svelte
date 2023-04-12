<script>
    import { fly } from 'svelte/transition';
    export let SafeSone;

    import { addListernEvent } from "api/functions";

    let visible = false;
    addListernEvent ("hud.bp.info", (_visible) => {
        visible = _visible;
    });
</script>
{#if visible}
<div class="hudevo__battlepass" style="margin-top: -{SafeSone.y}px" in:fly={{ y: -50, duration: 500 }} out:fly={{ y: -50, duration: 250 }}>
    <div class="hudevo__battlepass_image"></div>
    <div class="hudevo__notification_subtitle battlepass">ОГО!</div>
    <div class="hudevo__battlepass_background">
        ЗИМНИЙ ФЕСТ УЖЕ В ИГРЕ!
    </div>
    <div class="hudevo__notification_text large">
        На проекте стал доступен новый зимний фест, в котором можно получить редкие подарки, и обменять их на одежду, или же на уникальный снегоход!
    </div>
</div>
{/if}