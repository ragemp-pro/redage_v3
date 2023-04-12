<script>
    import {executeClientAsyncToGroup} from "api/rage";
    import {hasJsonStructure} from "api/functions";
    import { onCall, onMessage } from "@/views/player/hudevo/phonenew/data";

    let list = [];
    const getList = () => {
        executeClientAsyncToGroup("tinder.getLikes").then((result) => {
            if (hasJsonStructure(result))
                list = JSON.parse(result);
        });
    }
    getList ();

    import { addListernEvent } from 'api/functions';
    addListernEvent ("phone.tinder.getLikes", getList)
</script>
<div class="newphone__maps_headertitleGPS newphone__project_padding16">Ваши совпадения:</div>
<div class="newphone__ads_list auction big newphone__project_padding16">
    {#each list as item, index}
        <div class="newphone__project_categorie">
            <div class="newphone__tinder_circle" style="background-image: url({item.avatar})"></div>
            <div class="newphone__maps_headertitleGPS">{item.name}</div>
            {#if item.number >= 0}
                <div class="newphone__tinder_circle">
                    <span class="phoneicons-chat" on:click={() => onMessage (item.number)}></span>
                </div>
            {:else}
                <div />
            {/if}
        </div>
    {/each}
</div>