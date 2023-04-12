<script>
    import { translateText } from 'lang'
    import ProgressBar from 'progressbar.js';

    import { setGroup, executeClientToGroup, executeClientAsyncToGroup } from 'api/rage'


    setGroup (".battlepass.");

    let missionsArray = [];

    let selectPage = 0;

    const getMissions = (index) => {

        executeClientAsyncToGroup("getMissions", index).then((result) => {
            if (result && typeof result === "string")
                missionsArray = JSON.parse(result)
        });
    }

    getMissions(selectPage);

    const onSelectPage = (index) => {
        selectPage = index;

        getMissions(index);
    }


    executeClientAsyncToGroup("getMissionsPage").then((result) => {
        onSelectPage (result);
    });

    //

    const statusData = {
        closed: 0,
        selected: 1,
        done: 2,
        active: 3
    }

    const lineData = [
        "battlepassicons-bottom-top-right1",
        "",
        "battlepassicons-top-bottom",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "battlepassicons-bottom-top-right1",
        "battlepassicons-top-bottom-right",
        "",
        "",
        "",
        "",
        "",
        ""
    ];

    let hoverMission = false;
    import { spring } from 'svelte/motion';

    let coords = spring({ x: 0, y: 0 }, {
        stiffness: 1.0,
        damping: 1.0
    });

    let offsetInElementX = 0;
    let offsetInElementY = 0;
    const handleFavoriteSlotMouseEnter = (event, index) => {
        const target = event.target.getBoundingClientRect();

        offsetInElementX = 15 * (target.height / 100);
        offsetInElementY = 125 * (target.height / 100);

        coords.set({ x: event.clientX, y: event.clientY });

        hoverMission = missionsArray [index];
        hoverMission.i = index;
    }

    const handleFavoriteSlotMouseLeave = () => {
        hoverMission = false;
    }

    // Глобальные эвенты
    const handleGlobalMouseMove = (event) => {
        if (hoverMission !== false) {
            coords.set({ x: event.clientX, y: event.clientY });
        }
    }

    //

    const isStatusActive = (status) => status === statusData.done;

    const isActiveLine = (index, missions) => {

        if ((index === 0 || index === 2) && isStatusActive (missions [0].status))
            return true;

        if ((index === 1) && isStatusActive (missions [1].status))
            return true;

        if ((index === 3) && isStatusActive (missions [3].status))
            return true;

        if ((index === 4) && isStatusActive (missions [4].status))
            return true;

        //


        if ((index === 5) && isStatusActive (missions [2].status))
            return true;

        if ((index === 6 || index === 9) && isStatusActive (missions [6].status))
            return true;

        if ((index === 7) && isStatusActive (missions [7].status))
            return true;

        if ((index === 8) && isStatusActive (missions [8].status))
            return true;

        if ((index === 10) && isStatusActive (missions [10].status))
            return true;

        if ((index === 11 || index === 12) && isStatusActive (missions [11].status))
            return true;

        if ((index === 13) && isStatusActive (missions [12].status))
            return true;

        if ((index === 14) && isStatusActive (missions [13].status))
            return true;

        if ((index === 15) && isStatusActive (missions [15].status))
            return true;

        if ((index === 16) && isStatusActive (missions [16].status))
            return true;

        if ((index === 17) && isStatusActive (missions [17].status))
            return true;

        if ((index === 18) && isStatusActive (missions [18].status))
            return true;

        return false;
    }


    const isActiveBox = (index) => {

        if ((index === 1 || index === 2) && isStatusActive (missionsArray [0].status))
            return true;

        if ((index === 3) && isStatusActive (missionsArray [1].status))
            return true;

        if ((index === 4) && isStatusActive (missionsArray [3].status))
            return true;

        if ((index === 5) && isStatusActive (missionsArray [4].status))
            return true;

        //

        if ((index === 6) && isStatusActive (missionsArray [2].status))
            return true;

        if ((index === 7 || index === 10) && isStatusActive (missionsArray [6].status))
            return true;

        if ((index === 8) && isStatusActive (missionsArray [7].status))
            return true;

        if ((index === 9) && isStatusActive (missionsArray [8].status))
            return true;

        if ((index === 11) && isStatusActive (missionsArray [10].status))
            return true;

        if ((index === 12 || index === 13) && isStatusActive (missionsArray [11].status))
            return true;

        if ((index === 14) && isStatusActive (missionsArray [12].status))
            return true;

        if ((index === 15) && isStatusActive (missionsArray [13].status))
            return true;

        if ((index === 16) && isStatusActive (missionsArray [15].status))
            return true;

        if ((index === 17) && isStatusActive (missionsArray [16].status))
            return true;

        if ((index === 18) && isStatusActive (missionsArray [17].status))
            return true;

        if ((index === 19) && isStatusActive (missionsArray [18].status))
            return true

        if ((index === 20) && isStatusActive (missionsArray [19].status))
            return true;

        return false;
    }

    const onPage = (number) => {

        number += selectPage;

        if (number >= 0 && number <= 3) {
            selectPage = number;
            onSelectPage (selectPage)
        }
    }

    let progressBarId = null;

    const CreateProgressBar = (index, id) => {
        if (!window.loaderData.delay ("battlePass.CreateProgressBar", 1))
            return;

        if (progressBarId !== null)
            progressBarId.destroy();

        progressBarId = new ProgressBar.Circle("#progressBarKey" + index, {
            color: '#E71D36',
            strokeWidth: 6,
            trailWidth: 8,
            easing: 'easeInOut',
            duration: 900,
            trailColor: 'rgba(0, 0, 117, 0)', //цвет фонового кружка
            text: {
                autoStyleContainer: false
            },
            from: { width: 4 },
            to: { width: 6 },
            // Set default step function for all animate calls
            step: function(state, circle) {
                circle.path.setAttribute('stroke-width', state.width);
            }
        });

        StartTimer (id);
    };

    let intervalId = null;

    const StartTimer = (index) => {
        let progress = 0;
        intervalId = setInterval (() => {
            progress += 10;

            if (progress < 1000) {
                progressBarId.set(progress / 1000);
            } else {
                Clear (index);
                //Конец
            }
        }, 8);
    }

    const Clear = (id = -1) => {

        if (typeof id === "number")
            executeClientToGroup ("setMissions", id)

        if (intervalId !== null)
            clearInterval (intervalId);

        intervalId = null;

        if (progressBarId !== null)
            progressBarId.destroy();

        progressBarId = null;
    };

    import { addListernEvent } from 'api/functions'
    addListernEvent ("updateMissions", () => {
        onSelectPage (selectPage)
    })

    let boxPopup;
    const fixOutToX = (coordsX, offset, element) => {
        if (!element) return coordsX;
        else if (document.querySelector('#battlepass')) {
            let mainWidth = document.querySelector('#battlepass').getBoundingClientRect().width;
            let elementWidth = element.getBoundingClientRect().width;
            if ((elementWidth + coordsX + offset) >= mainWidth) return coordsX - offset - elementWidth;
            return coordsX + offset;
        }
        return coordsX + offset;
    }
</script>

<svelte:window on:mousemove={handleGlobalMouseMove} on:mouseup={Clear} />

{#if hoverMission !== false}
    <div bind:this={boxPopup} class="battlepass__missions_popup" style={`top:${$coords.y - offsetInElementY}px;left:${fixOutToX ($coords.x, offsetInElementX, boxPopup)}px;`}>
        <div class="battlepass__missions_header">
            <div class="box-flex">
                <div class="battlepass__missions_romb"></div>
                <div class="battlepass__missions_name">{hoverMission.name}</div>
            </div>
            <div class="battlepass__missions_subname">{hoverMission.title}</div>
        </div>
        <div class="battlepass__missions_descr">{hoverMission.descr}</div>
        <div class="battlepass__missions_progress">
            <div class="battlepass__missions_currentprogress" style="width: {hoverMission.count / hoverMission.maxCount * 100}%">
            </div>
        </div>
        <div class="battlepass__missions_text">{hoverMission.count} / {hoverMission.maxCount}</div>
        <div class="battlepass__missions_prise">
            {hoverMission.money}$ <span class="red">{translateText('player', 'и')}</span> {hoverMission.exp} XP
        </div>
        <div class="battlepass__missions_gray">+ {translateText('player', 'открывает следующее задание')}</div>
        {#if hoverMission.status == statusData.selected && hoverMission.isDone}
            <div class="battlepass__missions_footer">
                <div class="battlepass__missions_mouse"></div>
                <div>{translateText('player', 'ЗАЖМИТЕ ЛКМ, ЧТОБЫ ЗАБРАТЬ НАГРАДЫ')}</div>
            </div>
        {:else if hoverMission.status == statusData.selected && !hoverMission.isDone}
            <div class="battlepass__missions_footer" class:trans={true}>
                <div>{translateText('player', 'В ПРОЦЕССЕ ВЫПОЛНЕНИЯ')}</div>
            </div>
        {:else if hoverMission.status == statusData.closed && isActiveBox (hoverMission.i)}
            <div class="battlepass__missions_footer">
                <div class="battlepass__missions_mouse"></div>
                <div>{translateText('player', 'ЗАЖМИТЕ ЛКМ, ЧТОБЫ ВЫБРАТЬ ЗАДАНИЕ')}</div>
            </div>
        {:else if hoverMission.status == statusData.closed && !isActiveBox (hoverMission.i)}
            <div class="battlepass__missions_footer" class:trans={true}>
                <div>{translateText('player', 'ПРОЙДИТЕ ПРОШЛЫЕ ЗАДАНИЯ, ЧТОБЫ ОТКРЫТЬ ЭТО')}</div>
            </div>
        {:else if hoverMission.status == statusData.done}
            <div class="battlepass__missions_footer" class:trans={true}>
                <div>{translateText('player', 'ВЫ ВЫПОЛНИЛИ ЭТО ЗАДАНИЕ И ЗАБРАЛИ НАГРАДУ')}</div>
            </div>
        {/if}
    </div>
{/if}

<div class="battlepass__missions">
    <div class="battlepass__missions_column">
        <div class="box-flex">
            <div class="battlepass__page_button left" on:click={() => onPage (-1)}></div>
            <div class="battlepass__column_page first" class:active={selectPage === 0} on:click={() => onSelectPage (0)}>1</div>
            <div class="battlepass__column_page" class:active={selectPage === 1} on:click={() => onSelectPage (1)}>2</div>
            <div class="battlepass__column_page" class:active={selectPage === 2} on:click={() => onSelectPage (2)}>3</div>
            <div class="battlepass__column_page" class:active={selectPage === 3} on:click={() => onSelectPage (3)}>4</div>
            <div class="battlepass__page_button right" on:click={() => onPage (1)}></div>
        </div>
        <div class="battlepass__missions_level">{selectPage + 1} {translateText('player', 'уровень')}</div>
    </div>

    {#each missionsArray as mission, index}
        <div class="battlepass__missions_element position{index+1}" class:closed={mission.status === statusData.closed && !isActiveBox (index)} class:active={mission.status === statusData.active || (mission.status === statusData.closed && isActiveBox (index))} class:done={mission.status === statusData.done} class:selected={mission.status === statusData.selected}>
            {#if mission.status === statusData.active || (mission.status === statusData.closed && isActiveBox (index)) || (hoverMission.status == statusData.selected && hoverMission.isDone)}
                <div class="battlepass__missions_element_progress" id="progressBarKey{index}" />
                <div class="battlepass__missions_element_info"
                     on:mousedown={() => CreateProgressBar (index, mission.id)}
                     on:mouseenter={(e) => handleFavoriteSlotMouseEnter (e, index)}
                     on:mouseleave={handleFavoriteSlotMouseLeave} />
            {:else}
                <div class="battlepass__missions_element_info"
                     on:mouseenter={(e) => handleFavoriteSlotMouseEnter (e, index)}
                     on:mouseleave={handleFavoriteSlotMouseLeave} />
            {/if}
        </div>
        <div class="{lineData [index]} battlepass__line_{index+1}" class:active={isActiveLine (index, missionsArray)} />
    {/each}
    <div class="battlepass__missions_bot">
        <div><span class="red">{translateText('player', 'Пройдено')}</span> {missionsArray.filter((mission) => isStatusActive (mission.status)).length}/{missionsArray.length}</div>
        <div><span class="red">{translateText('player', 'Награда за прохождение всех')}</span> 5000$ <span class="red">{translateText('player', 'и')}</span> 50 XP</div>
    </div>
</div>