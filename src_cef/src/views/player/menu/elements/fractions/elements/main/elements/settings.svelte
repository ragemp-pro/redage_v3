<script>
    import { translateText } from 'lang'
    import { executeClientToGroup, executeClientAsyncToGroup } from "api/rage";
    import copy from 'copy-to-clipboard';
    import { isFraction } from "@/views/player/menu/elements/fractions/index.svelte";
    import {setPopup} from "../../../data";
    import { TimeFormat } from 'api/moment'
    import { format } from 'api/formatter'

    export let settings;

    const getUsersName = (count) => {
        if (count >= 2 && 4 <= count)
            return `${count} ${translateText('player1', 'человека')}`;

        return `${count} ${translateText('player1', 'человек')}`;
    }

    const onSaveSetting = (slogan, value, discord, color) => {
        let check;
        if (settings.discord != discord) {
            check = format("discord", discord);
            if (!check.valid) {
                window.notificationAdd(4, 9, check.text, 3000);
                return;
            }

            discord = discord.replace("https://discord.gg/", "")
        }

        if (settings.salary != value) {
            if (0 > value || value > 3) {
                window.notificationAdd(4, 9, "Сбор не может быть меньше 0% и не быть выше 3%", 5000);
                return;
            }
        }

        if (color && color.length)
            executeClientToGroup ("saveSetting", slogan, value, discord, color[0], color[1], color[2]);
        else
            executeClientToGroup ("saveSetting", slogan, value, discord, -1, -1, -1);
    }

    const onOpenPopupSetting = () => {
        setPopup ("Input", {
            headerTitle: "Настройки информации",
            headerIcon: "fractionsicon-stats",
            inputs: [
                {
                    title: "Слоган",
                    placeholder: "Слоган организации",
                    maxLength: 36,
                    value: settings.slogan
                },
                {
                    title: translateText('player1', 'Сборы в казну'),
                    placeholder: "Налог с игрока в казну (0-3%)",
                    maxLength: 1,
                    value: settings.salary,
                    type: "number",
                    min: 0,
                    max: 3
                },
                {
                    title: "Discord",
                    placeholder: "Инвайт-буквы (пример: JbcMC7Zb)",
                    maxLength: 26,
                    value: settings.discord
                },
                {
                    //title: "Discord",
                    //placeholder: "Ссылка на Discord:",
                    type: "color",
                    value: settings.color
                },
            ],
            button: translateText('popups', 'Подтвердить'),
            callback: onSaveSetting
        })
    }

    const onCopyDiscord = () => {
        copy(`https://discord.gg/${settings.discord}`)
        window.notificationAdd(4, 9, `Вы скопировали ссылку на Discord. Вставьте её в свой обычный браузер :)`, 7000);
    }
</script>

<div class="fractions__main_box w-520">
    <div class="fractions__main_head box-between">
        <div class="box-flex">
            <span class="fractionsicon-stats"></span>
            <div class="fractions__main_title">{translateText('player1', 'Информация')} {isFraction ? translateText('player1', 'о фракции') : translateText('player1', 'об организации')}</div>
        </div>
        {#if settings.isLeader}
            <div class="fractions__main_button mini" on:click={onOpenPopupSetting}>{translateText('player1', 'Настроить')}</div>
        {/if}
    </div>
    <div class="fractions__main_element flex">
        <div class="fractions_stats_title mr-12">{translateText('player1', 'Слоган')}:</div>
        <div class="fractions__stats_subtitle large">"{settings.slogan}"</div>
    </div>
    <div class="fractions__main_grid">
        <div class="fractions__main_element">
            <div class="fractions_stats_title">{translateText('player1', 'Путь')} {isFraction ? translateText('player1', 'фракции') : translateText('player1', 'организации')}:</div>
            <div class="fractions__stats_subtitle">{settings.crimeOptions ? translateText('player1', 'Криминал') : translateText('player1', 'Закон')}</div>
        </div>
        <div class="fractions__main_element">
            <div class="fractions_stats_title">{translateText('player1', 'Дата создания')}:</div>
            <div class="fractions__stats_subtitle">{TimeFormat (settings.date, "HH:mm DD.MM.YY")}</div>
        </div>
        <div class="fractions__main_element">
            <div class="fractions_stats_title">{translateText('player1', 'Сборы в казну')}:</div>
            <div class="fractions__stats_subtitle">{settings.salary}%</div>
        </div>
        <div class="fractions__main_element">
            <div class="fractions_stats_title">Discord:</div>
            <div class="fractions__stats_subtitle">
                <div class="box-flex">
                    {settings.discord}
                    <span class="fractionsicon-copy" on:click={onCopyDiscord}></span>
                </div>
            </div>
        </div>
    </div>
</div>