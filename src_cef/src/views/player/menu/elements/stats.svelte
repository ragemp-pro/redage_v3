<script>
    import { translateText } from 'lang'    
    import { otherStatsData } from 'store/account'
    import { charData } from 'store/chars';
    import { format } from 'api/formatter'
    import moment from 'moment'
    import fraction from 'json/fraction.js'
    import jobs from 'json/jobs.js'
    import vipinfo from 'json/vipinfo.js'
    export let visible;

    let selectCharData = $charData;   

    let useVisible = -1;
    
    $: {
        if (useVisible != visible) {
            if (visible && $otherStatsData.Name/* && $otherStatsData.UUID !== selectCharData.UUID*/) {
                selectCharData = $otherStatsData;
            } else if (visible && !$otherStatsData.Name && selectCharData !== $charData) {
                selectCharData = $charData;
            } else if (!visible && $otherStatsData.Name) {
                selectCharData = $charData;
                window.accountStore.otherStatsData ('{}');
            }
            useVisible = visible;
        }
    }
    let LicId = 0;
    const LicData = [
        [translateText('player2', 'Лицензия на мотоцикл'), translateText('player2', 'Получить данную лицензию можно в здании автошколы. Полиция может оштрафовать за езду без лицензии.')],
        [translateText('player2', 'Лицензия на легковой транспорт'), translateText('player2', 'Получить данную лицензию можно в здании автошколы. Требуется для работы таксистом, почтальоном и механиком. Полиция может оштрафовать за езду без лицензии.')],
        [translateText('player2', 'Лицензия на грузовой транспорт'), translateText('player2', 'Получить данную лицензию можно в здании автошколы. Требуется для работы инкассатором, дальнобойщиком и водителем автобуса. Полиция может оштрафовать за езду без лицензии.')],
        [translateText('player2', 'Лицензия на водный транспорт'), translateText('player2', 'Получить можно в автошколе. Требуется для аренды водного транспорта.')],
        [translateText('player2', 'Лицензия на вертолёт'), translateText('player2', 'Получить данную лицензию можно в здании автошколы. Требуется для аренды вертолётов. Необходим 20 уровень для приобретения данного вида лицензии.')],
        [translateText('player2', 'Лицензия на самолёт'), translateText('player2', 'Получить данную лицензию можно в здании автошколы. Требуется для аренды самолётов. Необходим 20 уровень персонажа для приобретения данного вида лицензии.')],
        [translateText('player2', 'Лицензия на оружие'), translateText('player2', 'Оформить данную лицензию можно в LSPD, для этого требуется 1 уровень персонажа и мед. карта. Необходима для покупки оружия в специализированных магазинах и для их законного ношения. Подробную информацию об оформлении уточняйте в LSPD.')],
        [translateText('player2', 'Мед. карта'), translateText('player2', 'Оформить мед. карту можно у докторов в EMS, для оформления необходим 1 уровень персонажа. Мед. карта карта требуется для трудоустройства в гос. организации. Подробную информацию об оформлении и стоимости уточняйте в EMS.')],
        [translateText('player2', 'Лицензия парамедика'), translateText('player2', 'Оформить данную лицензию можно в EMS, для этого необходим 10 уровень персонажа. Лиц. парамедика значительно повышает шанс успешного оказания ПМП при использовании аптечки. Подробную информацию об оформлении и стоимости уточняйте в EMS.')],
    ]

    const Bool = (text) => {
        return String(text).toLowerCase() === "true";
    }
</script>
{#if selectCharData !== undefined && selectCharData.Warns !== undefined}
<div class="box-stats margin-top-70">
    <div class="box-column margin-right-20">
        <div class="box-bg box-width-352" style="align-items: center">
            <div class="box-icon stats__person_image"/>
            <div class="title">{selectCharData.Login}</div>
            <div class="font-size-24 margin-bottom-34" style="width: 100%">{translateText('player2', 'Информация об аккаунте')}</div>

            <div class="info-box">
                <span class="right">{translateText('player2', 'Премиум аккаунт')}</span>
                <div class="info-line" />
                <span class="white">{selectCharData.VipLvl > 0 ? `${vipinfo[selectCharData.VipLvl]} (До ${moment(selectCharData.VipDate).format('DD.MM.YYYY')})` : vipinfo[selectCharData.VipLvl]}</span>
            </div>
            <div class="info-box margin-top-18">
                <span class="right">{translateText('player2', 'Варны')}</span>
                <div class="info-line" />
                <span class="white">{selectCharData.Warns > 0 ? `${selectCharData.Warns} до ${moment(selectCharData.Unwarn).format('DD.MM.YYYY HH:mm')}` : 0}</span>
            </div>
            <div class="info-box margin-top-18">
                <span class="right">{translateText('player2', 'Онлайн сегодня')}</span>
                <div class="info-line" />
                <span class="white">{moment.duration(selectCharData.TodayTime, "minutes").format("h[ч.] m[м.]")}</span>
            </div>
            <div class="info-box margin-top-18">
                <span class="right">{translateText('player2', 'Онлайн за месяц')}</span>
                <div class="info-line" />
                <span class="white">{moment.duration(selectCharData.MonthTime, "minutes").format("w[нед.] d[д.] h[ч.] m[м.]")}</span>
            </div>
            <div class="info-box margin-top-18">
                <span class="right">{translateText('player2', 'Онлайн за год')}</span>
                <div class="info-line" />
                <span class="white">{moment.duration(selectCharData.YearTime, "minutes").format("M[мес.] w[нед.] d[д.] h[ч.] m[м.]")}</span>
            </div>
            <div class="info-box margin-top-18">
                <span class="right">{translateText('player2', 'Общий онлайн')}</span>
                <div class="info-line" />
                <span class="white">{moment.duration(selectCharData.TotalTime, "minutes").format("y[г.] M[мес.] w[нед.] d[д.] h[ч.] m[м.]")}</span>
            </div>
        </div>
        {#if selectCharData.jobSkillsInfo}
        <div class="box-bg box-width-352 margin-top-20 h-370" style="align-items: center">
            <div class="font-size-24 margin-bottom-34" style="width: 100%">{translateText('player2', 'Навыки на работах')}</div>
            {#each selectCharData.jobSkillsInfo as job, index}
                <div class="info-box margin-top-18">
                    <span class="right width-120">{job.name}</span>
                    <div class="stars-box">
                        <span class="tooltiptext">{job.current}/{job.nextLevel}</span>
                        <div class="stars-box-current" style="width: {job.currentLevel >= 5 ? 100 : (((job.currentLevel / 5) * 100) + ((job.current / job.nextLevel) * 20))}%">
                            <div class="star"></div>
                            <div class="star"></div>
                            <div class="star"></div>
                            <div class="star"></div>
                            <div class="star"></div>
                        </div>
                        <div class="star"></div>
                        <div class="star"></div>
                        <div class="star"></div>
                        <div class="star"></div>
                        <div class="star"></div>
                    </div>
                </div>
            {/each}
        </div>
        {/if}
        <!--
        <div class="box-bg box-height-466 box-width-352 margin-top-20" style="opacity: 0.5">
            <div class="font-size-24 margin-bottom-34">Нарушения</div>
            <div class="margin-bottom-34">
                <div class="font-size-18 margin-bottom-32">IC Нарушения</div>
                <div class="info-box margin-top-18">
                    <span class="right">Преступления</span>
                    <div class="info-line" />
                    <span class="white">Не обнаружено</span>
                </div>
                <div class="info-box margin-top-18">
                    <span class="right">Штрафов</span>
                    <div class="info-line" />
                    <span class="white">2</span>
                </div>
            </div>

            <div>
                <div class="font-size-18 margin-bottom-32">IC Нарушения</div>
                <div class="info-box margin-top-18">
                    <span class="right">Преступления</span>
                    <div class="info-line" />
                    <span class="white">Не обнаружено</span>
                </div>
                <div class="info-box margin-top-18">
                    <span class="right">Штрафов</span>
                    <div class="info-line" />
                    <span class="white">2</span>
                </div>

            </div>
            <div class="box-center"><div class="btn-select">Журнал Нарушений</div></div>
        </div>-->
    </div>
    <div class="box-column">
        <div class="box-flex">
            <div class="box-bg box-width-352 margin-right-20">
                <div class="font-size-24 margin-bottom-34">{translateText('player2', 'Информация о персонаже')}</div>
                <div class="info-box">
                    <span class="right">{translateText('player2', 'Имя Фамилия')}</span>
                    <div class="info-line" />
                    <span class="white">{selectCharData.Name}</span>
                </div>
                <div class="info-box margin-top-18">
                    <span class="right">{translateText('player2', 'Статус')}</span>
                    <div class="info-line" />
                    <span class="white">{selectCharData.isAdmin ? "Админ" : "Игрок"}</span>
                </div>
                <div class="info-box margin-top-18">
                    <span class="right">{translateText('player2', 'Брак')}</span>
                    <div class="info-line" />
                    <span class="white">{selectCharData.WeddingName}</span>
                </div>
                <div class="info-box margin-top-18">
                    <span class="right">{translateText('player2', 'Пол')}</span>
                    <div class="info-line" />
                    <span class="white">{Bool(selectCharData.Gender) ? "Мужской" : "Женский"}</span>
                </div>
                <div class="info-box margin-top-18">
                    <span class="right">{translateText('player2', 'Уровень')}</span>
                    <div class="info-line" />
                    <span class="white">{selectCharData.LVL} ({selectCharData.EXP} / {(3 + selectCharData.LVL * 3)})</span>
                </div>
                <div class="info-box margin-top-18">
                    <span class="right">{translateText('player2', 'Телефон')}</span>
                    <div class="info-line" />
                    <span class="white">{selectCharData.Sim == -1 ? "Нет сим-карты" : selectCharData.Sim}</span>
                </div>
                <div class="info-box margin-top-18">
                    <span class="right">{translateText('player2', 'Работа')}</span>
                    <div class="info-line" />
                    <span class="white">{jobs[selectCharData.WorkID]}</span>
                </div>
                {#if selectCharData.FractionID > 0}
                <div class="info-box margin-top-18">
                    <span class="right">{translateText('player2', 'Фракция')}</span>
                    <div class="info-line" />
                    <span class="white">{fraction[selectCharData.FractionID]}</span>
                </div>
                <div class="info-box margin-top-18">
                    <span class="right">{translateText('player2', 'Должность')}</span>
                    <div class="info-line" />
                    <span class="white">{selectCharData.FractionLVL}</span>
                </div>
                {/if}
                {#if selectCharData.OrganizationID > 0}
                <div class="info-box margin-top-18">
                    <span class="right">{translateText('player2', 'Семья')}</span>
                    <div class="info-line" />
                    <span class="white">#{selectCharData.OrganizationID}</span>
                </div>
                <div class="info-box margin-top-18">
                    <span class="right">{translateText('player2', 'Должность')}</span>
                    <div class="info-line" />
                    <span class="white">{selectCharData.OrganizationLVL}</span>
                </div>
                {/if}
                <div class="info-box margin-top-18">
                    <span class="right">{translateText('player2', 'Номер ID карты')}</span>
                    <div class="info-line" />
                    <span class="white">{selectCharData.UUID}</span>
                </div>
                <div class="info-box margin-top-18">
                    <span class="right">{translateText('player2', 'Номер счета в банке')}</span>
                    <div class="info-line" />
                    <span class="white">{selectCharData.Bank}</span>
                </div>
                <div class="info-box margin-top-18">
                    <span class="right">{translateText('player2', 'Денег в банке')}</span>
                    <div class="info-line" />
                    <span class="white">${format("money", selectCharData.BankMoney)}</span>
                </div>                        
                <div class="info-box margin-top-18">
                    <span class="right">{translateText('player2', 'Денег на руках')}</span>
                    <div class="info-line" />
                    <span class="white">${format("money", selectCharData.Money)}</span>
                </div>
                <div class="info-box margin-top-18">
                    <span class="right">{translateText('player2', 'Дата создания')}</span>
                    <div class="info-line" />
                    <span class="white">{moment(selectCharData.CreateDate).format('DD.MM.YYYY HH:mm')}</span>
                </div>
            </div>
            <div class="box-bg box-width-352">
                <!--<div class="font-size-24 margin-bottom-34">{translateText('player2', 'Имущество')}</div>-->
                <div class="font-size-18 margin-bottom-32">{translateText('player2', 'Недвижимость')}</div>
                {#if selectCharData.houseId}
                <div class="info-box">
                    <span class="right">{translateText('player2', 'Дом')}</span>
                    <div class="info-line" />
                    <span class="white">#{selectCharData.houseId}</span>
                </div>
                <div class="info-box margin-top-18">
                    <span class="right">{translateText('player2', 'На счету дома')}</span>
                    <div class="info-line" />
                    <span class="white">{selectCharData.houseCash}</span>
                </div>
                <div class="info-box margin-top-18">
                    <span class="right">{translateText('player2', 'Списывается в час')}</span>
                    <div class="info-line" />
                    <span class="white">{selectCharData.houseCopiesHour}</span>
                </div>
                <div class="info-box margin-top-18">
                    <span class="right">{translateText('player2', 'Оплачено на')}</span>
                    <div class="info-line" />
                    <span class="white">{selectCharData.housePaid}</span>
                </div>
                <div class="info-box margin-top-18">
                    <span class="right">{translateText('player2', 'Класс дома')}</span>
                    <div class="info-line" />
                    <span class="white">{selectCharData.houseType}</span>
                </div>
                <div class="info-box margin-top-18">
                    <span class="right">{translateText('player2', 'Вместительность гаража')}</span>
                    <div class="info-line" />
                    <span class="white">{selectCharData.maxcars}</span>
                </div>
                {/if}
                {#if selectCharData.BizId}
                <div class="info-box margin-top-18">
                    <span class="right">{translateText('player2', 'Бизнес')}</span>
                    <div class="info-line" />
                    <span class="white">#{selectCharData.BizId}</span>
                </div>
                <div class="info-box margin-top-18">
                    <span class="right">{translateText('player2', 'На счету бизнеса')}</span>
                    <div class="info-line" />
                    <span class="white">{selectCharData.BizCash}</span>
                </div>
                <div class="info-box margin-top-18">
                    <span class="right">{translateText('player2', 'Списывается в час')}</span>
                    <div class="info-line" />
                    <span class="white">{selectCharData.BizCopiesHour}</span>
                </div>
                <div class="info-box margin-top-18">
                    <span class="right">{translateText('player2', 'Оплачено на')}</span>
                    <div class="info-line" />
                    <span class="white">{selectCharData.BizPaid}</span>
                </div>
                {/if}
            </div>
        </div>

        <div class="box-bg box-width-724 box-height-321 margin-top-20">
            <div class="font-size-24 margin-bottom-34">{translateText('player2', 'Лицензии')}</div>
            <div class="box-flex margin-bottom-34">
                <div class="icon-box" class:active={Boolean(selectCharData.Licenses[0]) == true} on:mouseenter={() => LicId = 0}>
                    <span class="inv-lic-moto" />
                    <div class="circle-icon" />
                </div>
                <div class="icon-box" class:active={Boolean(selectCharData.Licenses[1]) == true} on:mouseenter={() => LicId = 1}>
                    <span class="inv-lic-car" />
                    <div class="circle-icon" />
                </div>
                <div class="icon-box" class:active={Boolean(selectCharData.Licenses[2]) == true} on:mouseenter={() => LicId = 2}>
                    <span class="inv-lic-truck" />
                    <div class="circle-icon" />
                </div>
                <div class="icon-box" class:active={Boolean(selectCharData.Licenses[3]) == true} on:mouseenter={() => LicId = 3}>
                    <span class="inv-lic-boat" />
                    <div class="circle-icon" />
                </div>
                <div class="icon-box" class:active={Boolean(selectCharData.Licenses[4]) == true} on:mouseenter={() => LicId = 4}>
                    <span class="inv-lic-helipad" />
                    <div class="circle-icon" />
                </div>
                <div class="icon-box" class:active={Boolean(selectCharData.Licenses[5]) == true} on:mouseenter={() => LicId = 5}>
                    <span class="inv-lic-drone" />
                    <div class="circle-icon" />
                </div>
                <div class="icon-box" class:active={Boolean(selectCharData.Licenses[6]) == true} on:mouseenter={() => LicId = 6}>
                    <span class="inv-lic-gun" />
                    <div class="circle-icon" />
                </div>
                <div class="icon-box" class:active={Boolean(selectCharData.Licenses[7]) == true} on:mouseenter={() => LicId = 7}>
                    <span class="inv-lic-medical" />
                    <div class="circle-icon" />
                </div>
                <div class="icon-box" class:active={Boolean(selectCharData.Licenses[8]) == true} on:mouseenter={() => LicId = 8}>
                    <span class="inv-lic-health" />
                    <div class="circle-icon" />
                </div>
            </div>
            <div class="font-size-18">{LicData [LicId][0]}</div>
            <div class="margin-top-20 box-desc" style="width: 100%;white-space: normal">{LicData [LicId][1]}</div>
        </div>
    </div>
</div>
{/if}