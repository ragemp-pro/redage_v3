<script>
    import { translateText } from 'lang'
    import { executeClientToGroup, executeClientAsyncToGroup } from 'api/rage'
    import { TimeFormat } from 'api/moment'
    import { currentPage, selectNumber } from './../../stores'
    import { validate } from 'api/validation';

    export let updateView;

    let recents = [];

    executeClientAsyncToGroup("getRecents").then((result) => {
        if (result && typeof result === "string")
            recents = JSON.parse(result);
    });

    const onCall = (number) => {

        number = Number (number);

        let check;

        check = validate("phonenumber", number);
        if(!check.valid) {
            window.notificationAdd(4, 9, check.text, 3000);
            return;
        }

        executeClientToGroup ("call", number)
        currentPage.set ("callView");
    }

    const onClear = () => {
        executeClientAsyncToGroup("recentsClear").then((result) => {
            if (result)
                recents = [];
        });
    }

    const onInfo = (event, number) => {
        event.stopPropagation();
        selectNumber.set (number);

        updateView ("contacts");
    }
    import { fade } from 'svelte/transition'    

</script>
<div class="newphone__contacts" in:fade>
    <div class="box-center box-between w-1 newphone__project_padding16">
        <div class="newphone__maps_header">{translateText('player2', 'Недавние')}</div>
        <div class="blue" on:click={onClear}>{translateText('player2', 'Очистить')}</div>
    </div>
    {#if recents && recents.length > 0}
        <div class="newphone__contacts_list big">
            {#each recents as recent}
                <div class="newphone__recent_element" on:click={() => onCall (recent.Number)} >
                    <div class="box-flex">
                        <div class="phoneicons-call" style="opacity: {recent.isCall ? 1 : 0}"></div>
                        <div class="newphone__recent_name">{recent.Name}</div>
                    </div>
                    <div class="box-flex">
                        <div class="gray">{TimeFormat (recent.time, "H:mm")}</div>
                        <div class="newphone__recent_info" on:click={(e) => onInfo (e, recent.Number)} >
                            <div class="phoneicons-info"></div>
                        </div>
                    </div>
                </div>
            {/each}
        </div>
    {:else}
        <div class="contactgray newphone__project_padding16">{translateText('player2', 'История звонков была очищена или с данной сим-карты ещё не было звонков. Пора совершить первый!')}</div>
    {/if}
</div>