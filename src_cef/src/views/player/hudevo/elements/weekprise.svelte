<script>
    import { addListernEvent } from 'api/functions';
    import { fly } from 'svelte/transition';

    import { getPngUrl, getTimeFromMinsText } from "@/views/player/menu/elements/rewardslist/elements/data";

    let award = {}
    let visible = false;
    addListernEvent ("hud.award", (_visible, _award) => {
        if (_visible) {
            award = JSON.parse(_award)
        }
        visible = _visible;
    });


</script>
{#if visible}
    <div class="hudevo__weekprise" in:fly={{ y: 200, duration: 500 }} out:fly={{ y: -200, duration: 250 }}>
        <div class="hudevo__weekprise_prise">
            <div class="hudevo__weekprise_image" style="background-image: url({getPngUrl (award)})"></div>
        </div>
        <div class="hudevo__weekprise_sprite"></div>
        <div class="box-column">
            <div class="hudevo__notification_subtitle w-228">Отыграй 70 часов за неделю</div>
            <div class="hudevo__notification_title w-228" >
                И ПОЛУЧИ ПРИЗ
            </div>
            <div class="hudevo__notification_text w-228">
                {award.desc}
            </div>
            <div class="align-right">Ещё {getTimeFromMinsText (award.time)}</div>
        </div>
    </div>
{/if}