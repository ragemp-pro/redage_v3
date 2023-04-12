<script>
    import { selectChar } from './../store.js';
    import { format } from 'api/formatter'
    import fraction from 'json/fraction.js'

    import Spawn from './spawn.svelte'


</script>

<Spawn {...$selectChar.Data} isBan={$selectChar.ban} />

<div class="auth__stats">
    <div class="auth__stat">
        <div class="gray">Денег на руках</div>
        <div>${format("money", $selectChar.Data.Money)}</div>
    </div>
    <div class="auth__stat">
        <div class="gray">Денег на счету</div>
        <div>${format("money", $selectChar.Data.BankMoney)}</div>
    </div>
    <div class="auth__stat">
        <div class="gray">Фракция</div>
        <div>{fraction[$selectChar.Data.FractionID]}</div>
    </div>
    {#if $selectChar.ban && $selectChar.ban.Reason}
        <div class="auth__stat">
            <div class="gray">Причина</div>
            <div>{$selectChar.ban.Reason}</div>
        </div>
        <div class="auth__stat">
            <div class="gray">Забанил</div>
            <div>{$selectChar.ban.Admin}</div>
        </div>
        <div class="auth__stat">
            <div class="gray">Дата бана</div>
            <div>{$selectChar.ban.Time}</div>
        </div>
        <div class="auth__stat">
            <div class="gray">Дата разбана</div>
            <div>{$selectChar.ban.Until}</div>
        </div>
    {/if}
</div>