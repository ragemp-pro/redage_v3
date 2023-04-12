<script>
    import { translateText } from 'lang'
    import './sass/games.sass';
    import { executeClient } from 'api/rage';

    let
        active = false,
        lists = [
            {title: 'Мафия', class: 'mafia', desc: translateText('games', 'Примерь на себя одну из ролей разбойного города и прими участие в легендарной игре!')},
            {title: 'Танки', class: 'tanks', desc: translateText('games', 'Останься последним выжившим в хардкорном сражении на настоящих танках!')},
            {title: 'Страйкбол', class: 'airsoft', desc: translateText('games', 'Прояви мастерство владения оружием в одном из увлекательных режимов на разных картах!')},
        ];
        
	const activeClass = (bool) => {
        active = bool
	}

    const selectEvent = (index) => {
        executeClient ('selectEventClient', index);
    };

    const handleKeyDown = (event) => {
        const { keyCode } = event;
        if (keyCode !== 27) return;

        executeClient ('eventsMenuHide');
    }
</script>

<svelte:window on:keyup={handleKeyDown} />

<div id="games">
    <div class="games-title">{translateText('games', 'Выберите игру')}</div>

    <ul class="games-list">

        {#each lists as g, index}
            <li class={g.class} class:active on:mouseenter={() => activeClass(true)} on:mouseleave={() => activeClass(false)} on:click={() => selectEvent(index)}>
                <div class="games-list-icon"/>
                <div class="games-list-title">{g.title}</div>
                <p class="games-list-desc">{g.desc}</p>
            </li>
        {/each}

    </ul>
    <div class="games-help-info">
            <div class="games-help-info-subtitle">
                <div class="games-help-info-icon"/>
                <div class="games-help-info-text">ESС</div>
            </div>
            <p>{translateText('games', 'Нажмите чтобы закрыть выбор игр')}</p>
    </div>
</div>