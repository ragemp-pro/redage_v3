<script>
    import { charUUID } from 'store/chars';
    import {translateText} from "lang";
    import {executeClient, executeClientToGroup} from "api/rage";

    export let url;
    export let uuid;
    export let cls;
    export let setAvatar;

    export let cameraLink;
    const updateCameraToggled = () => {
        if (cameraLink == true) {
            return window.notificationAdd(4, 9, translateText('fractions', 'Фотография загружается'), 3000);
        }
        //return window.notificationAdd(4, 9, "Временно не работает", 3000);
        window.router.setPopUp("PopupCamera", updateCameraLink)
        executeClient ("camera.open")
    }

    const updateCameraLink = (link) => {
        cameraLink = link;
        if (cameraLink === true)
            return;

        executeClientToGroup("avatar", cameraLink);

        if (typeof setAvatar === "function")
            setAvatar (link);
    }

    import { addListernEvent } from 'api/functions';
    addListernEvent ("cameraLink", updateCameraLink)
</script>

{#if url && /(?:jpg|jpeg|png)/g.test(url)}
    <div class="fractions__lead_photo {cls}" style="background-image: url({url})">
        {#if $charUUID === uuid}
            <div class="fractions__photo_absolute">
                <span class="fractionsicon-plus" on:click={updateCameraToggled}></span>
            </div>
        {/if}
    </div>
{:else}
    <div class="fractions__lead_photo {cls}">
        {#if $charUUID === uuid}
            <div class="fractions__photo_absolute">
                <span class="fractionsicon-plus" on:click={updateCameraToggled}></span>
            </div>
        {/if}
    </div>
{/if}