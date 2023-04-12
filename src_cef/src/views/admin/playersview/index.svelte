<script>
    import { executeClient } from 'api/rage'
    import { translateText } from 'lang'
    import './assets/css/playerlist.css';
    import SmallLogo from '../../assets/images/logo.png';

    let items = [],
        playerssort = 0,
        sortrevert = 0;

        window.playerlist = {
            addPlayer: (id, name, level, ping) =>
            {
                id = id,
                name = name,
                level = level,
                ping = ping;
            },
            updatePlayerList: () =>
            {
                //forceUpdate();
            }
        }

    const sortPlayers = (id) => {

    }
 
</script>

<div class="plist">
    <div class="head">
        <img src={SmallLogo} alt="SmallLogo"/>
        <h1>RedAge.net</h1>
        <span>Players: {items.length}</span>
    </div>
    <div class="list">
        <table>
            <thead>
            <tr>
                <th on:click={() => sortPlayers(0)} class={(playerssort == 0 ? "selectedSort" : "") + (sortrevert ? " reversedSort" : "")}>ID</th>
                <th on:click={() => sortPlayers(1)} class={(playerssort == 1 ? "selectedSort" : "") + (sortrevert ? " reversedSort" : "")}>{translateText('admin', 'Никнейм')}</th>
                <th on:click={() => sortPlayers(2)} class={(playerssort == 2 ? "selectedSort" : "") + (sortrevert ? " reversedSort" : "")}>{translateText('admin', 'Уровень')}</th>
                <th on:click={() => sortPlayers(3)} class={(playerssort == 3 ? "selectedSort" : "") + (sortrevert ? " reversedSort" : "")}>{translateText('admin', 'Пинг')}</th>
            </tr>
            </thead>
            <tbody>
                {#each items as value, index}
                    <tr>
                        <td>{value.id}</td>
                        <td>{value.name}</td>
                        <td>{value.level}</td>
                        <td>{value.ping}</td>
                    </tr>
                {/each}
            </tbody>
        </table>
    </div>
</div>