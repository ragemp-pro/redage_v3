<script>
    import { onMount } from "svelte";
    import {executeClient, executeClientToGroup} from "api/rage";
    import Loader from './../loader.svelte'


    onMount(async () => {
        const phoneClass = document.querySelector('.newphone__image');
        phoneClass.classList.add('phone__camera');

        setTimeout(() => {

            window.router.setPopUp("PopupCamera", updateCameraLink);
            setTimeout(() => {
                executeClient ("camera.open");
            }, 25)
            phoneClass.classList.remove('phone__camera');
        }, 150)
    });

    import { currentPage } from "@/views/player/hudevo/phonenew/stores";

    const updateCameraLink = (link) => {
        if (link && typeof link === "string") {
            executeClientToGroup ("addGallery", link);
            currentPage.set("mainmenu")
        } else if (!link)
            currentPage.set("mainmenu")
    }

    import { addListernEvent } from 'api/functions';
    addListernEvent ("cameraLink", updateCameraLink)

</script>

<Loader />