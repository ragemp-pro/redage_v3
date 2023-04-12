<script>
    import { translateText } from 'lang'
    import { executeClientToGroup } from "api/rage";
    executeClientToGroup ("business.loadStats");
    export let onSelectedViewBusiness;

    import LoaderSmall from './../../loadersmall.svelte'
    let isLoad = false;

    const initTime = (jsonStats, jsonProducts, jsonUsers) => {

        jsonStats = JSON.parse(jsonStats);

        jsonStats.forEach((item) => {
            options.series[0].data.unshift(Number (item[1]));
        });

        isLoad = true;
    }

    import { addListernEvent } from 'api/functions';
    addListernEvent ("phoneBusinessStatsInit", initTime);


    /*const create = (node) => {
        new frappe.Chart(node, {  // or a DOM element,
            // new Chart() in case of ES6 module with above usage
            title: "Статистика за месяц",
            data: data,
            type: 'axis-mixed', // or 'bar', 'line', 'scatter', 'pie', 'percentage'
            height: 250,
            colors: ['#7cd6fd'],
            axisOptions: {
                xIsSeries: true  // default: false
            },
        });
    }

    const createDonut = (node) => {
        new frappe.Chart(node, {  // or a DOM element,
            // new Chart() in case of ES6 module with above usage
            title: "Статистика по товарам",
            data: data,
            type: 'pie', // or 'bar', 'line', 'scatter', 'pie', 'percentage'
            height: 350,
            colors: ['#7cd6fd', '#743ee2', "#6B5B95", "#ECDB54", "#E94B3C", "#6F9FD8", "#944743", "#DBB1CD", "#EC9787", "#00A591", "#6B5B95", "#6C4F3D", "#EADEDB", "#BC70A4", "#BFD641", "#2E4A62", "#B4B7BA", "#C0AB8E", "#F0EDE5", "#92B558", "#DC4C46", "#672E3B","#F3D6E4", "#D2691E", "#578CA9", "#F2552C", "#95DEE3", "#5A7247", "#92B6D5", "#AF9483", "#006E51", "#F7CAC9", "#91A8D0", "#034F84", "#FAE03C"],
            maxSlices: 30
        });
    }*/

    import { format } from 'api/formatter'
    import { chart } from "svelte-apexcharts";
    let options = {
        labels: [],
        series: [
            {
                name: translateText('player2', 'Сумма'),
                data: [
                ]
            },
        ],
        chart: {
            type: "area",
            height: 250,
            zoom: {
                enabled: false,
            },
            toolbar: {
                tools: {
                    download: false,
                },
            },
        },
        tooltip: {
            y: {
                formatter: function (val) {
                    return "$" + format("money", val);
                }
            }
        },
        dataLabels: {
            enabled: false,
        },
        stroke: {
            curve: "straight",
        },

        title: {
            text: translateText('player2', 'Статистика за месяц'),
            align: "left",
        },
        yaxis: {
            show: false,
        },
        xaxis: {
            labels: {
                show: false,
            }
        },

    };

</script>

{#if !isLoad}
    <LoaderSmall />
{:else}
    <div class="box-100 over">
        <div use:chart={options} />
        <!--<div class="newphone__charts" use:createDonut></div>-->
    </div>
    <div class="violet box-center m-top10" on:click={() => onSelectedViewBusiness ("Menu")}>{translateText('player2', 'Назад')}</div>
{/if}