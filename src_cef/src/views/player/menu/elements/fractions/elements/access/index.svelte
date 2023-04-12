<script>
    import { translateText } from 'lang'
    import { accessType } from "../../data";
    import { executeClientToGroup } from "api/rage";

    export let isSkip = false;
    export let executeName;
    export let itemId;
    export let title;
    export let mainScroll = "";
    export let clsScroll = "";
    export let isSelector = true;

    let selectItemId = itemId;
    let isWeaponAccess = false;
    let access = []

    const getAccess = (_access) => {
        if (_access && typeof _access === "string")
            access = JSON.parse(_access);
    }

    import { addListernEvent } from "api/functions";
    addListernEvent ("table.rankAccess", getAccess)

    const onUpdateAccess = (id, type) => {
        const index = access.findIndex(a => a.id === id);

        const item = access[index];

        if (item.type === type)
            return;

        if (typeof item !== "undefined")
            item.oldType = item.type;

        item.type = type;

        access[index] = item;
    }

    const executeAccess = () => {
        const updateAccess = {};

        access.forEach((item) => {
            if (typeof item.oldType !== "undefined" && item.oldType !== item.type) {
                updateAccess [item.id] = item.type;
            }
        });

        if (updateAccess && Object.keys(updateAccess).length)
            executeClientToGroup(executeName, selectItemId, JSON.stringify(updateAccess))
    }

    import { onDestroy } from 'svelte';
    onDestroy(executeAccess)


    $: if (itemId !== undefined) {
        executeAccess ();
        selectItemId = itemId;
    }
</script>

{#if isSelector}
    <div class="fractions__main_box {mainScroll}">
        <div class="fractions__main_head box-between">
            <div class="box-flex">
                <span class={title.icon}></span>
                <div class="fractions__main_title">{title.name}</div>
            </div>
        </div>
        <div class="box-flex">
            <div class="fractions__selector" class:active={!isWeaponAccess} on:click={() => isWeaponAccess = false}>
                {translateText('player1', 'Доступы')}
            </div>
            {#if access.findIndex(a => a.isWeaponAccess) !== -1}
                <div class="fractions__selector" class:active={isWeaponAccess} on:click={() => isWeaponAccess = true}>
                    Крафт
                </div>
            {/if}
        </div>
        <div class="fractions__main_scroll {clsScroll}">
            {#each access as item, index}
                <div class="fractions__scroll_element">
                    <div>{item.name}</div>
                    <div class="fractions__access_box">
                        <div class="fractions__access_element decline" class:active={item.type === accessType.Remove} on:click={() => onUpdateAccess (item.id, accessType.Remove)}><div class="fractionsicon-decline"></div></div>
                        {#if isSkip}
                            <div class="fractions__access_element neutral" class:active={item.type === accessType.Skip} on:click={() => onUpdateAccess (item.id, accessType.Skip)}><div class="fractionsicon-neutral"></div></div>
                        {/if}
                        <div class="fractions__access_element accept" class:active={item.type === accessType.Add} on:click={() => onUpdateAccess (item.id, accessType.Add)}><div class="fractionsicon-check"></div></div>
                    </div>
                </div>
            {/each}
        </div>
    </div>
{:else}
    <div class="fractions__main_scroll {clsScroll}">
        {#each access as item, index}
            <div class="fractions__scroll_element">
                <div>{item.name}</div>
                <div class="fractions__access_box">
                    <div class="fractions__access_element decline" class:active={item.type === accessType.Remove} on:click={() => onUpdateAccess (item.id, accessType.Remove)}><div class="fractionsicon-decline"></div></div>
                    {#if isSkip}
                        <div class="fractions__access_element neutral" class:active={item.type === accessType.Skip} on:click={() => onUpdateAccess (item.id, accessType.Skip)}><div class="fractionsicon-neutral"></div></div>
                    {/if}
                    <div class="fractions__access_element accept" class:active={item.type === accessType.Add} on:click={() => onUpdateAccess (item.id, accessType.Add)}><div class="fractionsicon-check"></div></div>
                </div>
            </div>
        {/each}
    </div>
{/if}