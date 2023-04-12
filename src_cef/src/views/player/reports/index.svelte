<script>
    import { executeClient } from 'api/rage'
    import RALogo from './images/ra-logo.png'
    import './css/main.css'
    import { selected, reportsData, text } from './index'
    import { onDestroy } from 'svelte'
    
    let
        reports = [],
        answer = $text;

    onDestroy(() => {
        text.set(answer) 
    });

    reportsData.subscribe(value => {
		reports = value;
    });
    
    const func = (funcName) => {
        const report = $selected;
        if (!report) return;
        executeClient ("funcreport", report.id, funcName);
    }

    let antiSpam = new Date().getTime();
    const onSelectReport = (report, index) => {
        if (report.blocked) return;
        else if ($selected) return;
        else if (new Date().getTime() - antiSpam < 500) return;
        antiSpam = new Date().getTime();

        selected.set (report);
        //if (index != undefined && reports[index]) {
        //    reports[index].blocked = true;
        //    reports[index].blockedBy = $charName;
        //}
        executeClient ("takereport", report.id);
    }

    const onSendAnswer = (report) => {
        if (!report) return;
        else if (!answer) return;
        
        executeClient ("sendreport", report.id, answer);
        selected.set(false);
        answer = "";
    }

    const onReturnReport = (report) => {
        if (!report) return;

        //const index = reports.findIndex(r => r.id == report.id);
        //if (index != undefined && reports[index]) {
        //    reports[index].blocked = false;
        //    reports[index].blockedBy = "";
        //}
        executeClient ("takereport", report.id);
        selected.set(false);
        answer = "";
    }

    const exitReport = () => {
        executeClient ("exitreport");
    }

    const onKeyBan = (e) => {
        if (event.key === 'Enter') {
            event.preventDefault();
        }
    }
</script>

<div class="report-wrapper">
    <div class="ticketfull">
        <div class="ticketinside">
            <div class="report">
                <p class="reporttext">Репорты</p>
                <div class="boxes">
                    <div class="boxreports">
                        {#each reports as report, index}
                            <div class="onereport" on:click={e => onSelectReport(report, index)}>
                                <div class="reportinfo">
                                    <p class="task">Задача</p>
                                    <p class="tasknumber">№ {report.id}</p>
                                    <p class="taskuser">{report.author}</p>
                                </div>
                                <div class="reporttasks">
                                    <p>{report.text}</p>
                                </div>
                                {#if report.blocked}
                                <div class="reportsadmin">
                                    <div class="adminname">
                                        <p>{report.blockedBy}</p>
                                    </div>
                                    <div class="blockadmins">
                                        <svg width="9" height="100" viewBox="0 0 9 100" fill="none" xmlns="http://www.w3.org/2000/svg">
                                            <path d="M0 0H9V100H0V0Z" fill="#BCA73A" />
                                        </svg>
                                    </div>
                                </div>
                                {/if}
                            </div>
                        {/each}
                    </div>
                </div>
            </div>
            <div class="answer">
                <p class="reporttext">Ответы</p>
                <div class="boxes">
                    <div class="boxreports">
                        {#if $selected}
                        <div class="onereport">
                            <div class="reportinfo">
                                <p class="task">Задача</p>
                                <p class="tasknumber">№ {$selected.id}</p>
                                <p class="taskuser">{$selected.author}</p>
                            </div>
                            <div class="reporttasks">
                                <p>{$selected.text}</p>
                            </div>
                        </div>
                        {/if}
                    </div>
                </div>
                <div class="answertoquestion">
                    <div class="textquestion">
                        <form action="javascript:void(0);">
                            <textarea class="user" onKeyPress={e => onKeyBan(e)} ref="answerBox" bind:value={answer} placeholder="Введите ответ на вопрос..."  wrap="soft" cols="30" rows="5"></textarea>
                        </form>
                    </div>
                    <div class="buttonquestion">
                        <div class="box-flex">
                            <button class="buttonone" on:click={e => onSendAnswer($selected)}>Ответить</button>
                            <button class="buttononek" on:click={e => onReturnReport($selected)}>Вернуть</button>
                        </div>
                        <div class="box-flex" style="margin-top: 6px">
                            <button class="buttonone min" on:click={e => func("tp")}>tp</button>
                            <button class="buttonone min" on:click={e => func("metp")}>metp</button>
                            <button class="buttonone min" on:click={e => func("sp")}>sp</button>
                            <button class="buttonone min" on:click={e => func("stats")}>stats</button>
                            <button class="buttonone min" on:click={e => func("kill")}>kill</button>
                        </div>
                        <div class="box-flex" style="margin-top: 6px">
                            <button class="buttonone sr" on:click={e => func("ptime")}>ptime</button>
                            <button class="buttonone sr" on:click={e => func("checkdim")}>checkdim</button>
                            <button class="buttonone sr" on:click={e => func("nhistory")}>nhistory</button>
                        </div>
                    </div>
                </div>
            </div>
            <div class="template">
                <p class="reporttext">Шаблоны</p>
                <div class="boxestempl">
                    <div class="boxtemplates">
                    </div>
                </div>
                <div class="texttempl">
                    <form action="">
                        <p>
                            <textarea class="user"  placeholder="Напишите ваш текст для шаблона и нажмите на пустой" cols="30" rows="5"></textarea>
                        </p>
                    </form>
                </div>
            </div>
        </div>
        <div class="imglogo">
            <img src={RALogo} alt="RALogo" width="360" height="92"/>
        </div>
        <div class="swglogo">
            <form action="javascript:void(0);">
                <button on:click={exitReport} id="buttonswgclose">
                    <svg width="20" height="20" viewBox="0 0 20 20" fill="none" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink"><mask id="mask0" mask-type="alpha" maskUnits="userSpaceOnUse" x="0" y="0" width="20" height="20"><rect width="20" height="20" transform="matrix(1 0 0 -1 0 20)" fill="url(#pattern0)"></rect></mask><g mask="url(#mask0)"><path d="M-11.6667 -11.6666H38.3333V38.3334H-11.6667V-11.6666Z" fill="#A33C3C"></path></g><defs><pattern id="pattern0" patternContentUnits="objectBoundingBox" width="1" height="1"><use xlink:href="#image0" transform="scale(0.00195312)"></use></pattern><image id="image0" width="512" height="512" xlink:href="data:image/png;base64,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"></image></defs></svg>
                </button>
            </form>
        </div>
    </div>
</div>