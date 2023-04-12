<script>
    import { translateText } from 'lang'
    import { onInputFocus, onInputBlur } from "@/views/player/menu/elements/fractions/data.js";

    import { executeClientToGroup } from "api/rage";
    import moment from 'moment';
    import {setPopup} from "../../data";

    export let departmentId;
    export let members;
    export let onSelectMember;
    export let onlineData;
    export let ranks;

    let searchText = ""
    const filterCheck = (elText, text) => {
        if (!text || !text.length)
            return true;

        text = text.toUpperCase();

        if (elText.toString().toUpperCase().includes(text))
            return true;

        return false;
    }

    let deletePlayerUUid = 0;
    const onPlayerDeleteCallback = () => {
        executeClientToGroup("deletePlayerDepartment", departmentId, deletePlayerUUid)
    }

    const onDepartmentDelete = (item) => {
        deletePlayerUUid = item.uuid;
        setPopup ("Confirm", {
            headerTitle: "Убрать из отряда",
            headerIcon: "fractionsicon-members",
            text: `Вы действительно хотите убрать '${item.name}' из отряда?`,
            button: 'Убрать',
            callback: onPlayerDeleteCallback
        })
    }

</script>
<div class="fractions__main_box large">
    <div class="fractions__main_head box-between mb-0">
        <div class="box-flex">
            <span class="fractionsicon-person"></span>
            <div class="fractions__main_title">{translateText('player1', 'Участники')}</div>
        </div>
        <div class="box-between w-506">
            <div class="fractions__stats_element mb-0">
                <div class="fractions__stats_circle online">
                    <div class="fractions__stats_smallcircle"></div>
                </div>
                <div class="whitecolor">{onlineData.online} {translateText('player1', 'человек онлайн')}</div>
            </div>
            <div class="fractions__stats_element mb-0">
                <div class="fractions__stats_circle offline">
                    <div class="fractions__stats_smallcircle"></div>
                </div>
                <div class="whitecolor">{onlineData.offline} {translateText('player1', 'человек оффлайн')}</div>
            </div>
            <div class="fractions__stats_element mb-0">
                <div class="fractions__stats_circle neutral">
                    <div class="fractions__stats_smallcircle"></div>
                </div>
                <div class="whitecolor">{onlineData.all} {translateText('player1', 'человек всего')}</div>
            </div>
        </div>
        <div class="fractions__input small">
            <div class="fractionsicon-loop"></div>
            <input on:focus={onInputFocus} on:blur={onInputBlur} type="text" placeholder="Поиск.." bind:value={searchText}>
        </div>
    </div>
    <div class="box-between pr-22 pl-8 pr-135">
        <div class="fractions_stats_title fs-14 name">{translateText('player1', 'Имя Фамилия')}</div>
        <div class="fractions_stats_title fs-14 rank">{translateText('player1', 'Ранг')}</div>
        <div class="fractions_stats_title fs-14 rank">{translateText('player1', 'Отряд')}</div>
        <div class="fractions_stats_title fs-14 rank">{translateText('player1', 'Очки')}</div>
    </div>
    <div class="fractions__main_scroll big">
        {#each members.filter(el => filterCheck(el.name, searchText)) as item}
            <div class="fractions__scroll_element small hover">
                <div class="fractions_stats_title whitecolor name fs-16 box-flex m-0">
                    <div class="fractions__stats_circle" class:online={item.isOnline} class:offline={!item.isOnline}>
                        <div class="fractions__stats_smallcircle"></div>
                    </div>
                    <div class="longtextparams">{item.name}</div>
                </div>
                <div class="fractions_stats_title whitecolor rank fs-16 m-0 longtextparams">{item.rankName}</div>
                <div class="fractions_stats_title rank fs-16 m-0 longtextparams">{ranks[item.departmentRank].name}</div>
                <div class="fractions_stats_title rank fs-16 m-0 longtextparams">{item.score}</div>
                <div class="box-flex hidden">
                    <div class="fractionsicon-fired" on:click={() => onDepartmentDelete (item)}></div>
                </div>
            </div>
        {/each}
    </div>
</div>