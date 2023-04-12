<script>
    import { translateText } from 'lang'
    let isLive = false; 
    let isCallList = false; 
</script>
<div class="weazelnews__title">{translateText('fractions', 'Прямой эфир')}</div>
{#if !isLive}
    <div class="weazelnews__live_block">
        <div class="weazelnews__live_title">{translateText('fractions', 'Старт прямого эфира')}</div>
        <div class="weazelnews__live_subtitle">{translateText('fractions', 'Внимание, у Вас есть возможность проводить прямые эфиры, будьте грамотны и внимательны, a еще - не пишите чушь.')}</div>
        <div class="weazelnews__button" on:click={() => isLive = true}>{translateText('fractions', 'Начать прямой эфир')}</div>
    </div>
{/if}
{#if isLive && !isCallList}
    <div class="weazelnews__live_block">
        <div class="weazelnews__live_title">{translateText('fractions', 'Прямой эфир с')} Vitaliy Zdobich</div>
        <div class="weazelnews__live_subtitle">{translateText('fractions', 'Внимание, у Вас есть возможность проводить прямые эфиры, будьте грамотны и внимательны, a еще - не пишите чушь.')}</div>
        <div class="box-flex">
            <div class="weazelnews__button mr-14" on:click={() => isLive = false}>{translateText('fractions', 'Завершить эфир')}</div>
            <div class="weazelnews__button mr-14">{translateText('fractions', 'Выгнать участника')}</div>
            <div class="weazelnews__button mr-14" on:click={() => isCallList = true}>{translateText('fractions', 'Список дозвонившихся')}</div>
        </div>
    </div>
    <div class="weazelnews__chatbox">
        <div class="weazelnews__message">
            <div class="weazelnews__message_name">Vitaliy Zdobich [358]</div>
            <div class="weazelnews__message_type">{translateText('fractions', 'Участник эфира')}</div>
            <div class="weazelnews__message_text">Всем привет, с вами участник прямого эфира Витос Здобос.</div>
        </div>
        <div class="weazelnews__message me">
            <div class="weazelnews__message_name">Vitaliy Zdobich [358]</div>
            <div class="weazelnews__message_type">{translateText('fractions', 'Ведущий эфира')}</div>
            <div class="weazelnews__message_text">Всем привет, с вами участник прямого эфира Витос Здобос.</div>
        </div>
    </div>
    <div class="box-flex">
        <input class="weazelnews__chat_input">
        <div class="weazelnews__chat_button">➤</div>
    </div>
{/if}
{#if isLive && isCallList}
    <div class="weazelnews__live_block">
        <div class="weazelnews__live_title">{translateText('fractions', 'Прямой эфир с')} Vitaliy Zdobich</div>
        <div class="weazelnews__live_subtitle">{translateText('fractions', 'Внимание, у Вас есть возможность проводить прямые эфиры, будьте грамотны и внимательны, a еще - не пишите чушь.')}</div>
        <div class="box-flex">
            <div class="weazelnews__button mr-14" on:click={() => isLive = false}>{translateText('fractions', 'Завершить эфир')}</div>
            <div class="weazelnews__button mr-14" on:click={() => isCallList = false}>{translateText('fractions', 'Вернуться к эфиру')}</div>
        </div>
    </div>
   <div class="weazelnews__live_calllist">
       <div class="weazelnews__live_element">
           <div class="weazelnews__element_block">Vitaliy Zdobich [123] <span class="gray">Тел.</span> 1337228</div>
           <div class="weazelnews__element_button">Допустить до эфира</div>
       </div>
       <div class="weazelnews__live_element">
        <div class="weazelnews__element_block">Vitaliy Zdobich [123] <span class="gray">Тел.</span> 1337228</div>
        <div class="weazelnews__element_button live">Перейти к эфиру</div>
        <div class="weazelnews__element_button kick">Выгнать из эфира</div>
    </div>
   </div>
{/if}