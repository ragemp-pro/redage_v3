<script>
    import { translateText } from 'lang'
    import { charSim } from 'store/chars';
    import { executeClientToGroup, executeClientAsyncToGroup } from 'api/rage'
    import { selectNumber } from './../../stores'

    export let updateView;

    import AddContact from "./addContact.svelte";
    import SelectContact from "./selectContact.svelte";

    let searchText = "";

    let isPopup = false;

    let contactsData = [];
    let contactsSystemData = [];

    const filterCheckSystem = (data) => {

        let success = false;
        data.forEach(el => {
            success = el.IsSystem;
        });
        return success;
    }

    const ruCollator = new Intl.Collator('ru-RU');
    const updateListContacts = () => {
        isPopup = false;

        executeClientAsyncToGroup("getContacts").then((result) => {
            if (result && typeof result === "string") {
                result = JSON.parse(result);
                
                result = result.sort((a, b) => ruCollator.compare(a.Name, b.Name));

                contactsData = result.filter(el => !filterCheckSystem(el.List));
                contactsSystemData = result.filter(el => filterCheckSystem(el.List));
            }
        });
    }

    updateListContacts ();

    const onSelectContact = (number, isMessageDefault = false) => {
        selectNumber.set (number);

        if (isMessageDefault)
            executeClientToGroup("messageDefault", number);
    }

    const filterCheck = (data, text) => {
        if (!text || !text.length)
            return true;

        text = text.toUpperCase();
        let success = false;
        data.forEach(el => {
            if (el.Name.toString().toUpperCase().includes(text)) {
                success = true;
            }
        });
        return success;
    }
    import { fade } from 'svelte/transition'
    import { onInputFocus, onInputBlur } from "@/views/player/hudevo/phonenew/data";

    import { onDestroy } from 'svelte'
    onDestroy(() => {
        onInputBlur ();
    });
</script>
<div class="newphone__contacts" in:fade>
    {#if isPopup}
        <AddContact on:click={() => isPopup = false} numberValue={$selectNumber} {updateView} {updateListContacts} />
    {:else if $selectNumber !== null}
        <SelectContact {updateListContacts} on:click={() => isPopup = true} />
    {:else}
        <div class="box-center box-between w-1 newphone__project_padding16">
            <div class="newphone__maps_header">{translateText('player2', 'Контакты')}</div>
            <div class="newphone__contacts_circle" on:click={() => isPopup = true}>+</div>
        </div>
        <div class="newphone__contacts_inputblock newphone__project_padding16">
            <div class="phoneicons-loop"></div>
            <input type="text" class="newphone__contacts_input" placeholder="Поиск" bind:value={searchText} on:focus={onInputFocus} on:blur={onInputBlur}>
        </div>
        <div class="newphone__contacts_info newphone__project_padding16">
            <div class="newphone__contacts_avatar"></div>
            <div class="box-column">
                <div class="gray">{translateText('player2', 'Мой телефон')}</div>
                <div>{$charSim == -1 ? "Нет сим-карты" : $charSim}</div>
            </div>
        </div>
        <div class="newphone__contacts_list">
            <div class="newphone__contacts_categorie">{translateText('player2', 'Службы')}</div>
            {#each contactsSystemData as letterData}
                {#each letterData.List as contactData}
                    {#if !contactData.IsNotShow}
                    <div class="newphone__contacts_element"
                         on:click={()=> onSelectContact (contactData.Number, true)}>{contactData.Name}</div>
                    {/if}
                {/each}
            {/each}
            {#each contactsData.filter(el => filterCheck(el.List, searchText)) as letterData}
                <div class="newphone__contacts_categorie">{letterData.Name}</div>
                {#each letterData.List as contactData}
                    <div class="newphone__contacts_element"
                         on:click={()=> onSelectContact (contactData.Number)}>{contactData.Name}</div>
                {/each}
            {/each}
        </div>
    {/if}
</div>