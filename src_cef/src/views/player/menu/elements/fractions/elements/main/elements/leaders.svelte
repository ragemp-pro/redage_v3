<script>
    import { translateText } from 'lang'
    import {executeClientAsyncToGroup} from "api/rage";
    import { TimeFormat } from 'api/moment'

    let leader = {}

    const getStats = () => {
        executeClientAsyncToGroup("getLeader").then((result) => {
            if (result && typeof result === "string")
                leader = JSON.parse(result);
        });
    }
    getStats ();

    import Avatar from '../../avatar/index.svelte'

    const setAvatar = (png) => {
        leader.avatar = png;
    }
</script>

<div class="box-flex">
    <Avatar url={leader.avatar} uuid={leader.uuid} {setAvatar} />
    <div class="box-column">
        <div class="fractions__main_element mb-24">
            <div class="fractions_stats_title">{translateText('player1', 'Имя Фамилия')}:</div>
            <div class="fractions__stats_subtitle">{leader.name}</div>
        </div>
        <div class="fractions__main_element">
            <div class="fractions_stats_title">{translateText('player1', 'Дата вступления')}:</div>
            <div class="fractions__stats_subtitle">{TimeFormat (leader.date, "HH:mm DD.MM.YY")}</div>
        </div>
    </div>
</div>