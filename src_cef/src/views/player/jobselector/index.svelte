<script>
    import { translateText } from 'lang'
    import { executeClient } from 'api/rage'
    import { charLVL, charWorkID } from 'store/chars';



    import PadUnlock from './assets/images/padlock-unlock.svg';
    import PadLock from './assets/images/lock.svg';
    import './assets/css/main.css'
    import {hasJsonStructure} from "api/functions";


    export let viewData;

    if (hasJsonStructure (viewData))
        viewData = JSON.parse(viewData);

    let list = [
            {class: "electro", name: translateText('player', 'Электрик'), level: 0, jobid: 1},
            {class: "kosilka", name: translateText('player', 'Газонокосильщик'), level: 0, jobid: 5},
            {class: "pochta", name: translateText('player', 'Почтальон'), level: 0, jobid: 2},
            {class: "taxi", name: translateText('player', 'Таксист'), level: 0, jobid: 3},
            {class: "bus", name: translateText('player', 'Водитель автобуса'), level: 0, jobid: 4},
            {class: "mechanic", name: translateText('player', 'Автомеханик'), level: 0, jobid: 8},
            {class: "truck", name: translateText('player', 'Дальнобойщик'), level: 1, jobid: 6},
            {class: "inkos", name: translateText('player', 'Инкассатор'), level: 1, jobid: 7},
        ];

    const selectJob = (selectedjob) => {
        executeClient ("selectJob", selectedjob);
    }
    const closeJobMenu = () => {
        executeClient ("closeJobMenu");
    }
    
</script> 

<div class="joblist">
    <div class="adaptive">
        <div class="boxtop">
            <div class="box">
                <div class="header"> 
                    <div on:click={closeJobMenu} class="arrow"></div>
                    <p class="txtheader">{translateText('player', 'Устроиться на работу')}</p>
                </div>
                <div class="workbox">
                    {#each list as forjob, index}
                    <div key={index} class={"workboxes " + forjob.class}>
                        <div class="inkoslvl">
                            <p class="textlvl">LVL</p>
                            <p class="textlvl2">{viewData[forjob.jobid]}</p>
                        </div>
        
                        <div class="worktoptxt ">
                            <p class="textboxwork"></p> {forjob.name}
        
                            {#if viewData[forjob.jobid] <= $charLVL}
                            <div class="jobstatus">
                                <img class="workimg" src={PadUnlock} alt="PadUnlock"/>
                                <p class="worktextopen">{translateText('player', 'Открыто')}</p>
                            </div>   
                            {:else}
                            <div class="jobstatus">
                                <img class="workimg" src={PadLock} alt="PadLock"/>
                                <p class="worktextopen">{translateText('player', 'Закрыто')}</p>
                            </div>
                            {/if}
                        </div>

                        {#if (viewData[forjob.jobid] <= $charLVL)}
                            {#if (forjob.jobid == $charWorkID)}
                                <div>
                                    <div class="buttongetsettledred" on:click={() => selectJob(-1)}>
                                        <p сlass="buttontxtgetsettled">{translateText('player', 'Уволиться')}</p>
                                    </div>
                                </div>
                            {:else}
                                <div>
                                    <div class="buttongetsettled" on:click={() => selectJob(forjob.jobid)}>
                                        <div сlass="buttontxtgetsettled">{translateText('player', 'Устроиться')}</div>
                                    </div>
                                </div>
                            {/if}
                        {/if}
                        </div>
                    {/each}
                </div>
            </div>
        </div>
    </div>
</div>