<script>
    import { fly } from 'svelte/transition';
    import { charLVL, charEXP } from 'store/chars'
    let visible = false;

    let level = -1;
    let maxExp = 0;
    let progress = 0;

    const GetMaxExp = (lvl) => {
        maxExp = 3 + lvl * 3;
    }

    const GetProgress = (exp) => {
        progress = exp * 100 / maxExp;
        
        if (progress > 100) progress = 100;
        else if (level != $charLVL) progress = 100;
    }

    window.updateLevel = async () => {
        onInit ();
        visible = true;
        await window.wait(500);
        GetProgress ($charEXP);
        await window.wait(5000);
        visible = false;
        if (level != $charLVL) {
            level = $charLVL;
            progress = 0;
            GetMaxExp (level);
            await window.wait(250);
            window.NewLvl ()
        }
    }

	const onInit = () => {
        if (level === -1) {
            level = $charLVL;
            GetMaxExp (level);
            GetProgress ($charEXP);
        }
    }
</script>
{#if visible}
    <div class="hudevo__level" in:fly={{ y: -50, duration: 500 }} out:fly={{ y: -50, duration: 250 }}>
        <div class="hudevo__level_image">
            <div class="level">{level}</div>
            <div class="newlevel">{level + 1}</div>
        </div>
        <div class="hudevo__notification_subtitle ">Ура! Вы стали опытнее</div>
        <div class="hudevo__notification_title fs-14">
            До следующего уровня осталось {(3 + level * 3) - $charEXP} EXP
        </div>
    </div>
{/if}