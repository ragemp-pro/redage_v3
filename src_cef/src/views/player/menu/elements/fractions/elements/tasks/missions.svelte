<script>
    import { executeClient, executeClientToGroup } from 'api/rage'
    import { format } from "api/formatter";
    executeClientToGroup('missionsLoad')

    import pngMission0 from './images/0.png'
    import pngMission1 from './images/1.png'
    import pngMission2 from './images/2.png'
    import pngMission3 from './images/3.png'
    import pngMission4 from './images/4.png'
    import pngMission5 from './images/5.png'
    import pngMission6 from './images/6.png'

    let missions = [
        {
            id: 0,
            title: "Наземный патруль",
            desc: "Наземный патруль - это задание для небольшого вооружённого отряда, задачей является осмотр местности, поддержание порядка и охрана штата. Патруль выполняется на наземном транспорте.",
            bonus: "",
            gps: [],
            colorId: 0,
            isTake: false,
            isShow: false,
            img: pngMission0
        },
        {
            id: 1,
            title: "Воздушный патруль",
            desc: "Воздушный патруль - это наблюдение за ранее назначенным районом с высоты птичьего полёта. Следуйте назначенному маршруту и наблюдайте за порядком свысока! Выполняется на вертолёте.",
            bonus: "",
            gps: [],
            colorId: 0,
            isTake: false,
            isShow: false,
            img: pngMission1
        },
        {
            id: 2,
            title: "Перевозка запрещённого груза",
            desc: "Перевозка запрещённого груза - это задание для мафий, в котором сотруднику придётся перевезти запрещённый груз со склада мафии на другой конец острова. Полиция будет на хвосте, так что нужно быть начеку.",
            bonus: "",
            gps: [],
            colorId: 0,
            isTake: false,
            isShow: false,
            img: pngMission2
        },
        {
            id: 3,
            title: "Перевозка медикаментов",
            desc: "Перевозка медикаментов - это задание для медиков, в котором сотрудникам придётся перевезти медикаменты к себе на склад. Бандиты будут на хвосте, так что нужно быть начеку.",
            bonus: "",
            gps: [],
            colorId: 0,
            isTake: false,
            isShow: false,
            img: pngMission3
        },
        {
            id: 4,
            title: "Перевозка материалов",
            desc: "Перевозка материалов - это задание для сотрудников армии, в котором сотрудникам придётся перевезти материалы. Бандиты будут на хвосте, так что нужно быть начеку.",
            bonus: "",
            gps: [],
            colorId: 0,
            isTake: false,
            isShow: false,
            img: pngMission4
        },
        {
            id: 5,
            title: "AIRDROP",
            desc: "AirDrop — это отличная возможность не только заработать, но и посоревноваться с другими игроками и определить кто достоин его забрать.",
            bonus: "",
            gps: [],
            colorId: 0,
            isTake: false,
            isShow: false,
            img: pngMission5
        },
        {
            id: 6,
            title: "Heli Crash",
            desc: "HeliCrash — это отличная возможность посоревноваться с другими игроками за ящики с ценным грузом, которые выпадают при крушении вертолета.",
            bonus: "",
            gps: [],
            colorId: 0,
            isTake: false,
            isShow: false,
            img: pngMission6
        },
    ]
    let selectMission = false;

    const onSelectMission = (item) => {
        selectMission = item;
    }

    const colors = [
        "green",
        "blue",
        "orange",
        "red",
    ]

    export let onSetLoad;

    onSetLoad (true);
    const updateLoad = (json) => {
        onSetLoad (false);

        json = JSON.parse(json)

        json.forEach((item) => {
            const index = missions.findIndex(m => m.id === item.id);

            if (missions [index]) {
                missions [index] = {
                    ...missions [index],
                    ...item
                }

                if (!selectMission && missions [index].isShow) {
                    selectMission = missions [index];
                }
                else if (selectMission && selectMission.id === missions [index].id) {
                    selectMission = missions [index];
                }
            }
        })
    }

    import { addListernEvent } from 'api/functions';
    addListernEvent("table.missions", updateLoad);

    const onUse = () => {
        
        executeClientToGroup('missionUse', selectMission.id)
    }

    const onGps = () => {
        executeClient('createWaypoint', selectMission.gps.x, selectMission.gps.y)
        executeClient('client.inventory.Close')
        window.notificationAdd(4, 9, `Место было отмечено на карте`, 3000);
    }
</script>

<div class="box-between mt-56 align-start">
    <div class="box-column">
        {#if selectMission}
        <div class="fractions__tasks_box">
            <div class="fractions__tasks_image" style="background-image: url({selectMission.img});"></div>
            <div class="fractions__tasks_title">{selectMission.title}</div>
            <div class="box-between">
                <div class="box-column">
                    <div class="fractions__tasks_subtitle">Награды:</div>
                    <div class="fractions__tasks_text">${format("money", selectMission.bonus)}</div>
                </div>
                <div class="box-column">
                    {#if selectMission.gps}
                        <div class="fractions__tasks_subtitle">Место проведения:</div>
                        <div class="fractions__tasks_text">
                            <div class="box-flex" on:click={onGps}>
                                <div class="fractionsicon-map"></div>
                                <div>Отметить в GPS</div>
                            </div>
                        </div>
                    {/if}
                </div>
            </div>
            <div class="fractions__tasks_subtitle mt-16">Описание:</div>
            <div class="fractions__tasks_text">
                {selectMission.desc}
            </div>
            {#if selectMission.isTake}
                <div class="fractions__tasks_button" on:click={onUse}>Принять участие</div>
            {/if}
        </div>
        {/if}
    </div>
    <div class="fractions__tasks_grid">
        {#each missions as mission}
            {#if mission.isShow}
                <div class="fractions__tasks_box" on:click={() => onSelectMission (mission)}>
                    <div class="fractions__tasks_image {colors[mission.colorId]}" style="background-image: url({mission.img});"></div>
                    <div class="fractions__tasks_text center">{mission.title}</div>
                </div>
            {/if}
        {/each}
    </div>
</div>