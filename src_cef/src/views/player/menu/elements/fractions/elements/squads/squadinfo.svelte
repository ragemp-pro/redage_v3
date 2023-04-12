<script>
    import { translateText } from 'lang'
    import Access from '../access/index.svelte'
    import { executeClientToGroup } from "api/rage";
    import { format } from 'api/formatter'
    import { onInputFocus, onInputBlur } from "@/views/player/menu/elements/fractions/data.js";

    export let departmentId;
    export let selectRank;
    export let onSelectRank;

    executeClientToGroup('rankAccessInit', JSON.stringify(selectRank.access), JSON.stringify(selectRank.lock))

    let rankName = "";
    const onUpdateName = () => {
        let check = format("rank", rankName);
        if (!check.valid) {
            window.notificationAdd(4, 9, check.text, 3000);
            return;
        }

        executeClientToGroup('updateDepartmentRankName', departmentId, selectRank.id, rankName)
        rankName = "";
    }
</script>
<div class="fractions__stats_subtitle gray hover mt-20 large" on:click={() => onSelectRank()}>&#x2190; {translateText('player1', 'Вернуться к списку рангов')}</div>
<div class="box-between mt-20 not-center">
    <div class="box-column">
        <Access title={{
                    icon: 'fractionsicon-rank',
                    name: selectRank.name
                }}
                itemId={`${departmentId}|${selectRank.id}`}
                executeName="updateDepartmentRankAccess"
                isSelector={true}
                isSkip={true}
                mainScroll="h-568"
                clsScroll="big" />
    </div>
    <div class="fractions__main_box w-317">
        <div class="fractions__main_head">
            <span class="fractionsicon-rank"></span>
            <div class="fractions__main_title">{translateText('player1', 'Сменить название')}</div>
        </div>
        <div class="fractions__input w-269">
            <input on:focus={onInputFocus} on:blur={onInputBlur} bind:value={rankName} type="text" placeholder={translateText('player1', 'Введите название..')}>
        </div>
        <div class="fractions__main_button h-40" on:click={onUpdateName}>{translateText('player1', 'Изменить')}</div>
    </div>
</div>