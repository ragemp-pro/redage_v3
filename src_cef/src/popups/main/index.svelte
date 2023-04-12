<script>
    import { translateText } from 'lang'
    import './main.sass'    
    export let popupData;

    const statuses = [
        {
            code: "kick",
            status: "Вас кикнули",
            text: "Вас кикнул Администратор : Dmitriy Jaguar вы можете обжаловать кик на форуме проекта https://forum.redage.net/",
            button: "Обжаловать"
        },
        {
            code: "ban",
            status: "Вы забанены",
            text: "Вас забанил Администратор : Dmitrio Jaguarchik до 21.22.2019 причина: Вылил ягуар https://forum.redage.net/",
            button: "Обжаловать"
        },
        {
            code: "restart",
            status: "Сервер на рестарте",
            text: "В данный момент сервер на рестарте, ожадайте в течении 5 минут сервер запустится",
            button: "Встать в очередь"
        }
    ]

    let currentStatus = 0;
</script>

<div id="mainpopup" class="{popupData.Type}">
    <div class="mainpopup__back hud__icon-warning"></div>
    <div class="mainpopup__main {popupData.Type}">
        <div class="mainpopup__smallback hud__icon-warning"></div>
        <div class="popup__title {popupData.Type}">{popupData.Title}</div>
        <div class="popup__subtitle">{popupData.Text}</div>
        {#if false}
        <div class="box-between">
            <div class="popup__button big {popupData.Type}">{statuses[currentStatus].button}</div>
            <div class="popup__button red">{translateText('popups', 'Назад')}</div>
        </div>
        {/if}
    </div>
</div>