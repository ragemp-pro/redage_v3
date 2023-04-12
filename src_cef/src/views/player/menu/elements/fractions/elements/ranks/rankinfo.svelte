<script>
    import { translateText } from 'lang'
    import { executeClientToGroup } from "api/rage";
    import { format } from 'api/formatter'
    import { onInputFocus, onInputBlur } from "@/views/player/menu/elements/fractions/data.js";
    export let settings;

    import Access from '../access/index.svelte'

    executeClientToGroup('rankAccessLoad', settings.id)

    export let onSettings;

    let rankName = "";
    const onUpdateName = () => {
        let check = format("rank", rankName);
        if (!check.valid) {
            window.notificationAdd(4, 9, check.text, 3000);
            return;
        }

        executeClientToGroup('updateRankName', settings.id, rankName)
        rankName = "";
    }

    let score = settings.maxScore;
    const onUpdateScore = () => {
        score = Math.round(score.replace(/\D+/g, ""));
        if (score < 0) score = 0;
        else if (score > 9999999) score = 9999999;

        executeClientToGroup('updateRankScore', settings.id, score)
    }
</script>


<div class="fractions__stats_subtitle gray hover mt-20 large" on:click={() => onSettings()}>&#x2190; {translateText('player1', 'Вернуться к списку рангов')}</div>
<div class="box-between mt-20 not-center">
    <div class="box-column">
        <Access title={{
                    icon: 'fractionsicon-rank',
                    name: settings.name
                }}
                itemId={settings.id}
                executeName="updateRankAccess"
                isSelector={true}
                mainScroll="h-568"
                clsScroll="big" />
    </div>
    <div class="box-column">
        <div class="fractions__main_box w-317 mb-24">
            <div class="fractions__main_head">
                <span class="fractionsicon-rank"></span>
                <div class="fractions__main_title">{translateText('player1', 'Сменить название')}</div>
            </div>
            <div class="fractions__input w-269">
                <input on:focus={onInputFocus} on:blur={onInputBlur} bind:value={rankName} type="text" placeholder={translateText('player1', 'Введите название..')}>
            </div>
            <div class="fractions__main_button h-40" on:click={onUpdateName}>{translateText('player1', 'Изменить')}</div>
        </div>
        <div class="fractions__main_box w-317">
            <div class="fractions__main_head">
                <span class="fractionsicon-rank"></span>
                <div class="fractions__main_title">{translateText('player1', 'XP до получения')}</div>
            </div>
            <div class="fractions__input w-269">
                <input on:focus={onInputFocus} on:blur={onInputBlur} bind:value={score} type="number" placeholder={translateText('player1', 'Введите кол-во..')}>
            </div>
            <div class="fractions__main_button h-40" on:click={onUpdateScore}>{translateText('player1', 'Изменить')}</div>
        </div>
    </div>
</div>