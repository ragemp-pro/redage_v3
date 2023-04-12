<script>
    import { translateText } from 'lang'
    import './main.sass'
    import { setPopup, getPopupData } from "../../data";

    const popupData = getPopupData();

    import { executeClientToGroup, executeClientAsyncToGroup } from 'api/rage'
    executeClientToGroup('membersLoad')

    let members = [];
    let selectRankId = 1;

    const getMembers = (_member, _members, _onlineData) => {
        if (_members && typeof _members === "string")
            members = JSON.parse(_members);
    }

    import { addListernEvent } from "api/functions";
    addListernEvent ("table.members", getMembers)

        
    const onSend = () => {

        if (typeof popupData.callback === "function" && Object.keys(ranksData) && Object.keys(ranksData).length)
            popupData.callback(JSON.stringify(ranksData));

        setPopup ()
    }

    const HandleKeyDown = (event) => {
        const { keyCode } = event;
        if (keyCode == 13)
            onSend ()
    }


    const onSelectRankId = (index) => {
        selectRankId = index;
    }

    let ranksData = {};
    const onSelectRank = (uuid) => {
        for(let key in ranksData) {
            if (ranksData[key] === uuid)
                delete ranksData[key];
        }

        ranksData[selectRankId] = uuid;
    }

    let searchText = ""
    const filterCheck = (elText, text) => {
        if (!text || !text.length)
            return true;

        text = text.toUpperCase();

        if (elText.toString().toUpperCase().includes(text))
            return true;

        return false;
    }

    let settings = {};
    const getSettings = () => {
        executeClientAsyncToGroup("getSettings").then((result) => {
            if (result && typeof result === "string")
                settings = JSON.parse(result);
        });
    }
    getSettings();
</script>

<svelte:window on:keyup={HandleKeyDown} />
<div id="fractions">
    <div class="popup__newhud_squadranks">
        <div class="popup__newhud_box w-782">
            <div class="fractions__main_head box-between">
                <div class="box-flex">
                    <span class="fractionsicon-complaints"></span>
                    <div class="fractions__main_title">{translateText('player1', 'Назначить руководство')}</div>
                </div>
                <div class="fractions__input medium">
                    <div class="fractionsicon-loop"></div>
                    <input type="text" placeholder={translateText('player1', 'Поиск..')} bind:value={searchText}>
                </div>
            </div>
            <div class="box-between align-startflex">
                <div class="box-between mr-20 w-1 align-startflex">
                    <div class="box-column">
                        <div class="fractions__main_scroll w-240 h-480">
                            {#each popupData.ranks as item, index}
                                {#if item.id > 0 && (settings.isLeader || popupData.myRank > item.id)}
                                    <div class="fractions__scroll_element hover p-20 fw-bold" class:activeselected={selectRankId === Number (item.id)} on:click={() => onSelectRankId (Number (item.id))}>
                                        <div>{item.name}</div>
                                        <div class="fs-36">&#8250;</div>
                                    </div>
                                {/if}
                            {/each}
                        </div>
                    </div>
                    <div class="box-column">
                        <div class="fractions__main_scroll w-500 h-480">
                            {#each members.filter(el => filterCheck(el.name, searchText)) as item}
                                {#if item.departmentId === 0 || item.departmentId === popupData.id}
                                    <div class="fractions__scroll_element hover p-20 fw-bold" on:click={() => onSelectRank (item.uuid)}>
                                        <div class="box-flex">
                                            <div>{item.name} - {item.rankName}</div>
                                        </div>
                                        {#if (!ranksData[selectRankId] && !Object.values(ranksData).includes(item.uuid) && item.departmentRank === Number (popupData.ranks[selectRankId].id)) || ranksData[selectRankId] === item.uuid}
                                            <div class="fs-36">&#10004;</div>
                                        {/if}
                                    </div>
                                {/if}
                            {/each}
                        </div>
                    </div>
                </div>
            </div>
            <div class="popup__newhud__buttons">
                <div class="popup__newhud_button" on:click={onSend}>Сохранить</div>
                <div class="popup__newhud_button" on:click={() => setPopup ()}>{translateText('popups', 'Закрыть')}</div>
            </div>
        </div>
    </div>
</div>