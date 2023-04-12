<script>
    import { translateText } from 'lang'
    import { executeClient } from 'api/rage'

    import './assets/sass/mats.sass'
    export let viewData;

    let isArmy = false,
        isMed = false;

    $: {
        if (viewData) {
            if (isArmy !== viewData.isArmy)
                isArmy = viewData.isArmy;
            if (isMed !== viewData.isMed)
                isMed = viewData.isMed;

        }
    }
    const load = (id) => {
        executeClient ('matsL', id);
    }

    const unload = (id) => {
        executeClient ('matsU', id);
    }

    const cancel = () => {
        executeClient ('matsU', 0)
    }

    const HandleKeyDown = (event) => {
        const { keyCode } = event;
        if (keyCode !== 27) return;

        executeClient ('matsU', 0);
    }
</script>

<svelte:window on:keyup={HandleKeyDown} />
<div id="mats">
    <div class="mats">
        {#if isMed}

        <div class="mod">
            <div class="module">
                <div class="ic-medic ics"></div>
                <div class="r">
                    <div class="title">{translateText('fractions', 'Аптечки')}</div>
                    <div class="desc">
                        <p>{translateText('fractions', 'Используются для оказания первой медицинской помощи тяжело раненому человеку')}.</p>
                        <p class="small"><span class="green">{translateText('fractions', 'Виды получения')}:</span> {translateText('fractions', 'Загрузка аптечек каретами EMS в лаборатории Humane Labs')}</p>
                    </div>
                    <div class="btns">
                        <div on:click={e => load(4)} class="btn green">{translateText('fractions', 'Загрузить')}</div>
                        <div on:click={e => unload(4)} class="btn red">{translateText('fractions', 'Выгрузить')}</div>
                    </div>
                </div>

            </div>
        </div>

        {:else}

            {#if isArmy}
                <div class="mod">
                    <div class="module">
                        <div class="ic-army ics"></div>
                        <div class="r">
                            <div class="title">{translateText('fractions', 'Материалы')}</div>
                            <div class="desc">
                                <p>{translateText('fractions', 'Используются для создания оружия, патронов, брони')}.</p>
                                <p></p>
                                <p class="small"><span class="green">{translateText('fractions', 'Виды получения')}:</span> {translateText('fractions', 'Поставки происходят из порта Лос Сантоса')}.</p>
                            </div>
                            <div class="btns">
                                <div on:click={e => load(1)} class="btn green">{translateText('fractions', 'Загрузить')}</div>
                                <div on:click={e => unload(1)} class="btn red">{translateText('fractions', 'Выгрузить')}</div>
                            </div>
                        </div>

                    </div>
                </div>

                {:else}

                    <div class="mod">
                        <div class="module">
                            <div class="ic-materials ics"></div>
                            <div class="r">
                                <div class="title">{translateText('fractions', 'Материалы')}</div>
                                <div class="desc">
                                    <p>{translateText('fractions', 'Используются для создания оружия и патронов')}.</p>
                                    <p class="small"><span class="green">{translateText('fractions', 'Виды получения')}:</span> {translateText('fractions', 'Угон Barracks с Форта-Занкудо')}</p>
                                </div>
                                <div class="btns">
                                    <div on:click={e => load(2)} class="btn green">{translateText('fractions', 'Загрузить')}</div>
                                    <div on:click={e => unload(2)} class="btn red">{translateText('fractions', 'Выгрузить')}</div>
                                </div>
                            </div>

                        </div>
                    </div>
                    <div class="mod">
                        <div class="module">
                            <div class="ic-drugs ics"></div>
                            <div class="r">
                                <div class="title">{translateText('fractions', 'Наркотики')}</div>
                                <div class="desc">
                                    <p>{translateText('fractions', 'Используются для пополнения здоровья с побочным эффектом')}.</p>
                                    <p class="small"><span class="green">{translateText('fractions', 'Виды получения')}:</span> {translateText('fractions', 'Покупаются бандами в особых точках')}.</p>
                                </div>
                                <div class="btns">
                                    <div on:click={e => load(3)} class="btn green">{translateText('fractions', 'Загрузить')}</div>
                                    <div on:click={e => unload(3)} class="btn red">{translateText('fractions', 'Выгрузить')}</div>
                                </div>
                            </div>

                        </div>
        
                    </div>
            {/if}

        {/if}
    </div>
</div>