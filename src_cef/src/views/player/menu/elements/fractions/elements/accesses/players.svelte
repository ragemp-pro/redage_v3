<script>
    import { translateText } from 'lang'
    import { executeClientToGroup } from 'api/rage'
    import Access from '../access/index.svelte'
    import { charUUID } from 'store/chars';

    executeClientToGroup('membersLoad')

    let members = []

    const getMembers = (_member, _members, _onlineData) => {

        if (_members && typeof _members === "string")
            members = JSON.parse(_members);

        if (members.length > 0)
            onSelectUUId (members[0])
    }

    import { addListernEvent } from "api/functions";
    addListernEvent ("table.members", getMembers)

    const UpdateMember = (_member) => {
        _member = JSON.parse(_member);

        let index = members.findIndex((m) => m.uuid === _member.uuid);

        if (members [index]) {
            members [index] = _member;
        }
    }

    addListernEvent ("table.updateMember", UpdateMember)

    let selectUUId = -1;
    const onSelectUUId = (item) => {
        executeClientToGroup('rankAccessInit', JSON.stringify(item.access), JSON.stringify(item.lock))
        selectUUId = item.uuid;
    }
</script>
<div class="box-column mr-20 align-startflex">
    <div class="fractions_stats_title mt-20">{translateText('player1', 'Имя')}:</div>
    <div class="fractions__main_scroll w-482 h-480">
        {#each members as item}
            {#if $charUUID !== item.uuid}
                <div class="fractions__scroll_element hover p-20 fw-bold" on:click={() => onSelectUUId(item)}>
                    <div class="box-flex">
                        <div class="mr-40">{item.uuid}</div>
                        <div>{item.name}</div>
                    </div>
                    {#if selectUUId === item.uuid}
                        <div class="fs-36">&#8250;</div>
                    {/if}
                </div>
            {/if}
        {/each}
    </div>
</div>

<div class="box-column">
    <div class="fractions_stats_title mt-20">{translateText('player1', 'Доступы')}:</div>
    {#if selectUUId >= 0}
        <Access
            itemId={selectUUId}
            executeName="updateMemberRankAccess"
            isSelector={false}
            clsScroll="w-399 h-480"
            isSkip={true} />
    {:else}
        <div class="fractions__main_scroll w-399 h-480" />
    {/if}
</div>
<div class="box-column">
</div>