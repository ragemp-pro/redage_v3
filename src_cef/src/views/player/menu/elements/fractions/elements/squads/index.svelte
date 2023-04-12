<script>
    import { translateText } from 'lang'
    import SquadsList from './squadslist.svelte'
    import SquadSettings, {id} from './squadsettings.svelte'
    import SquadRanks from './squadranks.svelte'
    import SquadInfo from './squadinfo.svelte'


    import { setGroup, executeClientToGroup, executeClientAsyncToGroup } from 'api/rage'
    executeClientToGroup('departmentsLoad')

    let isLoad = false;
    let departments = []

    const getDepartments = (_departments) => {
        departments = JSON.parse(_departments);

        if (selectDepartment) {
            const index = departments.findIndex(r => r.id === selectDepartment.id);

            if (departments [index]) {
                selectDepartment = departments [index];
            }
            else
                selectDepartment = null;
        }

        isLoad = true
    }
    import { addListernEvent } from "api/functions";
    addListernEvent ("table.departments", getDepartments)


    //
    let department = {}
    let members = [];

    let onlineData = {
        online: 0,
        offline: 0,
        all: 0
    };

    const getDepartment = (_department, _members, _onlineData) => {
        department = JSON.parse(_department);
        if (settings)
            onSettings (department)
        members = JSON.parse(_members);
        onlineData = JSON.parse(_onlineData);

        isLoad = true
    }
    addListernEvent ("table.department", getDepartment)

    //

    let selectDepartment = null;

    const onSelectDepartment = (item = null) => {
        if (item) {
            isLoad = false;
            executeClientToGroup('departmentLoad', item.id)
        }
        selectDepartment = item;
    }

    import { setPopup } from "../../data";

    let deleteId = 0;
    const onDepartmentDeleteCallback = () => {
        executeClientToGroup('removeDepartment', deleteId);
    }

    const onDepartmentDelete = (item) => {
        deleteId = item.id;
        setPopup ("Confirm", {
            headerTitle: "Распустить отряд",
            headerIcon: "fractionsicon-squads",
            text: `Вы действительно хотите распустить отряд '${item.id}. ${item.name}'?`,
            button: 'Удалить',
            callback: onDepartmentDeleteCallback
        })
    }

    //

    let settings = null;
    const onSettings = (item = null) => {
        settings = item;
    }

    //

    import Logo from '../other/logo.svelte'

</script>
<div class="fractions__header">
    <div class="box-flex">
        <Logo />
        <div class="box-column">
            <div class="box-flex">
                <span class="fractionsicon-squads"></span>
                <div class="fractions__header_title">{translateText('player1', 'Отряды')}</div>
            </div>
            <div class="fractions__header_subtitle">{translateText('player1', 'Меню просмотра, настройки и создания отрядов')}</div>
        </div>
    </div>
</div>
{#if isLoad}
    {#if selectDepartment === null}
        <SquadsList {departments} {onSelectDepartment} {onDepartmentDelete} />
    {:else if selectDepartment !== null && settings === null}
        <SquadSettings {onSelectDepartment} {onDepartmentDelete} {department} {members} {onlineData} {onSettings} />
    {:else if selectDepartment !== null && settings !== null}
        <SquadRanks {settings} {onSettings} />
    {/if}
{/if}