<script>
    import { translateText } from 'lang'
    import moment from 'moment';
    import { executeClient, invokeMethod } from 'api/rage'
    import Commands from 'json/commands.js'
    import { format } from 'api/formatter'
    import CustomKey from './Key.svelte'
    import { isInputToggled, isHelp } from 'store/hud'
    export let SafeSone;
    //
    let selectChat = 0;
    const ChatTegs = [
        {
            name: "IC",
            chat: false,
            visible: true
        },
        {
            name: "OOC",
            chat: "b",
            visible: true
        },
        {
            name: "ME",
            chat: "me",
            visible: true
        },
        {
            name: "DO",
            chat: "do",
            visible: true
        },
        {
            name: "TRY",
            chat: "try",
            visible: true
        },
        {
            name: "REP",
            chat: "report",
            visible: true
        },
       /* {
            name: "R",
            chat: "r",
            visible: false
        },
        {
            name: "F",
            chat: "f",
            visible: false
        },
        {
            name: "DEP",
            chat: "dep",
            visible: false
        },
        {
            name: "GOV",
            chat: "gov",
            visible: false
        },*/
    ]

    // Элемент инпута
    let TextInput;

    // Пулл сообщений
    let Messages = [];

    // Текущее значение инпута
    let InputValue = "";

    // Сохранённое сообщение из инпута, если чат закрыт через Esc
    let savedInput = "";

    // Текущий индекс буфера
    let bufferState = -1;
    // Текущее значение буфера
    let bufferCurrent = "";
    // Буфер отправленных сообщений
    let buffer = [];

    // Размер чата
    let pagesize = 10;

    // Обновляем состояния рейджа, о том что у нас открыт инпут
    //$: invokeMethod("setTypingInChatState", $isInputToggled);

    // API чата
    let timerId = 0;
    let noActive = false;
    const AddChatMessage = (text) => {
        text = format("parse", text);

        if (Messages.length > 50) {
            Messages.pop();
        }
        let generatedMessage = getColorizedMessagePart(text);
        Messages = [{time: moment().format('HH:mm:ss'), parts: generatedMessage}, ...Messages];

        onActive ();
    };

    const onActive = () => {
        if ($isInputToggled)
            return;
        noActive = false;

        if (timerId)
            clearTimeout (timerId);

        timerId = setTimeout(ClearData, 15000);
    }

    const ClearData = () => {
        timerId = 0;
        noActive = true;
    }

    const parseMessage = (text) => {
        const re = new RegExp('!{((#[0-9a-f]{3})|(#[0-9a-f]{6})|([0-9a-f]{3})|([0-9a-f]{6})|(\\w+))}', 'i');
        const match = re.exec(text);

        if (!match) {
            return null;
        }

        let color = match[1];

        color = color ? `${color.startsWith('#')
            ? color
            : /(^[0-9A-Fa-f]{6}$)|(^[0-9A-Fa-f]{3}$)/i.test(color) ? '#' + color : color}` : null

        return {
            color,
            index: match.index,
            remainingText: text.slice(match.index + match[0].length)
        };
    }

    const getColorizedMessagePart = (text) => {
        const parts = [];
        let prevColor = null;
        let currentText = text;
        let message;
        let attempts = 0;

        for (; (message = parseMessage(currentText)) || !attempts;) {
            const prevText = currentText.slice(0, message ? message.index : currentText.length);

            if (prevText.length) {
                parts.push({
                    color: prevColor,
                    text: prevText.replace(/\\!{/g, '!{').replace(/\\}/g, '}')
                });
            }

            if (message) {
                prevColor = message.color;
                currentText = message.remainingText;
            } else {
                attempts++;
            }
        }
        return parts;
    }

    const ToggleChatInput = (state, clearchat = false, updatelastbuffer = false, event = false) => {
        if(state) {
            noActive = false;

            if (timerId)
                clearTimeout (timerId);

            window.hudStore.isInputToggled (true);
            timerId = 0;

            setTimeout(() => {
                if(TextInput) {
                    TextInput.focus();

                    if(bufferState === -1) {
                        InputValue = savedInput;
                        SetInputFocused();
                    } else {
                        if(buffer[bufferState] !== undefined) {
                            InputValue = buffer[bufferState];
                        } else {
                            InputValue = "";
                        }
                        SetInputFocused();
                    }
                    if (event) executeClient ("client:OnChatInputChanged", true);
                }
            }, 0);
        } else {
            bufferCurrent = clearchat ? "" : (updatelastbuffer ? InputValue : bufferCurrent),
            bufferState = clearchat ? -1 : bufferState,
            savedInput = clearchat ? "" : InputValue;
            window.hudStore.isInputToggled (false);
            //selectChat = 0;
            onActive ();
            InputValue = "";

            if (event) executeClient ("client:OnChatInputChanged", false);
        }
    };
    
    const OnSubmitMessage = () => {
        bufferState = -1;
        bufferCurrent = "";

        if(InputValue) {
            if(buffer.length > 50) buffer.shift();

             // Не сохраняем одинаковые друг за другом идущие сообщения
            if(buffer[buffer.length - 1] !== InputValue) buffer.push(InputValue);

            if(InputValue[0] === '/') {
                let commandText = InputValue;   
                commandText = commandText.trim().substr(1);             
                if (commandText.length > 0)
                {
                    let params = commandText.split(' ');
                    let command = params[0];
                    params.shift();
                    switch(command)
                    {
                        case "widthsize":
                        case "pagesize":
                        case "fontsize":
                        case "timestamp":
                        case "chatalpha":
                            window.notificationAdd(4, 9, translateText('player2', 'Данная функция была перенесена в настройки, найти их вы можете перейдя в инвентарь > вкладка настройки.'), 3000);
                            break;
                        case "weatherinfo":
                            executeClient ("weatherinfo");
                            break;
                        case "restartopen":
                            executeClient ("restart.open");
                            break;
                        case "greenzone":
                            executeClient ("greenzone");
                            break;
                        case "cltest":
                            executeClient ("test.test");
                            break;
                        default:
                            commandText = format("parseDell", commandText);

                            if (commandText.length > 0)
                                invokeMethod("command", commandText);
                            break;
                    }
                }
            } else {
                let message = format("stringify", InputValue);

                if (message.length > 0) {
                    if (selectChat === 0) {
                        invokeMethod("chatMessage", message);
                    } else {
                        const commandText = `${ChatTegs [selectChat].chat} ${message}`

                        if (commandText.length > 0)
                            invokeMethod("command", commandText);

                    }
                }
                    
            }
        }
        ToggleChatInput(false, true, true, true);
    };

    const SetInputFocused = () => {
        if(TextInput) {
            TextInput.focus();
            TextInput.selectionStart = InputValue.length;
        }
    }

    const handleKeyUp = (e) => {
        if($isInputToggled) {
            switch(e.keyCode) {
                case 13:
                    OnSubmitMessage();
                    break;
            }
        }
    }

    const handleKeyDown = (event) => {
        if($isInputToggled) {
            switch(event.keyCode) {
                case 9: // Отключаем TAB
                    event.preventDefault();

                    if (++selectChat >= ChatTegs.length)
                        selectChat = 0;
                    break;
                case 38: // Поднимаем буфер вверх
                    event.preventDefault();
                    if(bufferState === -1) {
                        bufferState = (buffer.length - 1);
                        bufferCurrent = InputValue;
                    }
                    else if(bufferState > 0) {
                        bufferState = bufferState - 1;
                        SetInputFocused();
                    } else {
                        SetInputFocused();
                        break;
                    }

                    if(buffer[bufferState]) {
                        InputValue = buffer[bufferState];
                        SetInputFocused();
                    }
                    break;
                case 40: // Опускаем буфер вниз
                    event.preventDefault();
                    if(bufferState === -1) break;
                    if(bufferState < buffer.length - 1) {
                        bufferState = bufferState + 1;
                        SetInputFocused();
                    }
                    else {
                        InputValue = bufferCurrent;
                        bufferState = -1;
                        bufferCurrent = "";
                        SetInputFocused();
                        break;
                    }

                    if(buffer[bufferState] !== undefined) {
                        InputValue = buffer[bufferState];
                        SetInputFocused();
                    }
                    break;
                default:
                    break;
            }
        }
    };

    let Alpha = 100;
    let Transition = 0;

    const SetChatAlpha = (alpha, transition) => {
        Alpha = alpha;
        Transition = transition;
    }

    const GetChatAlpha = () => {
        return Alpha;
    }


    let TimeStamp = true;

    const ToggleTimeStamp = (toggle) => {
        TimeStamp = toggle;
    }

    const GetTimeStamp = () => {
        return TimeStamp;
    }

    let PageSize = 10;
    const UpdatePageSize = (pageSize) => {
        PageSize = pageSize;
    }

    let WidthSize = 50;
    const UpdateWidthSize = (widthSize) => {
        WidthSize = widthSize;
    }

    let FontSize = 16;
    const UpdateFontSize = (fontSize) => {
        FontSize = fontSize;
        UpdateMessageHeight(fontSize);
    }

    let MessageHeight = 0;
    const UpdateMessageHeight = (messageHeight) => {
        MessageHeight = messageHeight * 1.25;
    }
    UpdateMessageHeight (FontSize);

    let chatShadow = false;
    window.chat = {
        updateConfig: (config) => {
            config = JSON.parse(config);

            ToggleTimeStamp (config["Timestamp"]);
            UpdatePageSize (config["Pagesize"]);
            SetChatAlpha (config["ChatOpacity"], config["Transition"]);
            UpdateFontSize (config["Fontsize"]);
            UpdateWidthSize (config["Widthsize"]);
            chatShadow = config["ChatShadow"];
        },
        toggleInput: (toggled, clearchat = false, updatelastbuffer = false, event = false) => {
            ToggleChatInput (toggled, clearchat, updatelastbuffer, event)
        },
        addMessage: (message, clearhtml = true) => {
            AddChatMessage (message);
        }
    }
    if (window.mp && window.mp.events) {

        window.mp.events.add("chat:push", (message) => {
            AddChatMessage(message);
        });
    }

    let heightChat = 0;


    let messageElement;

    /*$: if (messageElement) {
        console.log(messageElement)
    }*/
</script>

<svelte:window on:keyup={handleKeyUp} on:keydown={handleKeyDown}/>
<div class="hudevo__chat-block" style="height: {heightChat}px; margin-top: {SafeSone.y}px; margin-left: {SafeSone.x}px;">
    <div class="hudevo__chat" class:noactive={noActive} style="opacity: {!noActive ? 1 : 1 / 100 * Alpha}" bind:clientHeight={heightChat}>
        <div class="hudevo__chat_messages"
            style="width: {WidthSize}%;font-size: {FontSize}px;line-height: {FontSize * 1.25}px;height: {MessageHeight * PageSize}px;transition: opacity {1 / 100 * Transition}s;overflow-y: {$isInputToggled ? 'auto' : 'hidden'};" class:scroll={(Messages.length > pagesize ? (!$isInputToggled ? false : true) : false)}>
            {#each Messages as msgs, index}
                <p class:oldshadow={chatShadow}>
                    {#each msgs.parts as msgpart}
                        <span style="color: {msgpart.color}">
                            {@html TimeStamp ? ((msgpart.text.includes('color:')) ? '<' + msgpart.text.split('<')[1].split('>')[0] + '>' + `[${msgs.time}] ` + '</span>' : `[${msgs.time}] `) : ""}
                            {@html msgpart.text}
                        </span>
                    {/each}
                </p>
            {/each}
        </div>
        {#if $isInputToggled}
            <div class="hudevo__chat_input_block">
                <div class="hudevo__chat_input_box">
                    <div class="hudevo__chat_input_chat">{ChatTegs [selectChat].name}</div>
                    <input class="hudevo__chat_input" bind:this={TextInput} spellCheck={false} maxLength={144} placeholder="Введите сообщение" bind:value={InputValue} />
                    <div class="hudevo__chat_input_send" on:click={OnSubmitMessage}>
                        <span class="hud__icon-send hudevo__chat_input_send_icon" class:active={InputValue.length > 0} />
                    </div>
                </div>
            </div>
            <div class="hudevo__chat_buttons_block">
                <div class="hudevo__chat_buttons">
                    <div class="hudevo__chat_buttons_key">
                        <CustomKey keyCode={9}>TAB</CustomKey>
                    </div>
                    {#each ChatTegs as tag, index}
                        {#if tag.visible}
                        <div class="hudevo__chat_button" class:active={index === selectChat} on:click={() => selectChat = index }>{tag.name}</div>
                        {/if}
                    {/each}
                </div>
            </div>
            {#if $isHelp && InputValue && InputValue.length >= 1}
            <div class="hudevo__chat_buttons_helpcmd">
                {#each Commands as item, _}
                    {#if item.cmd.indexOf(InputValue) != -1 && item.cmd.indexOf("/") != -1}
                    <div class="hudevo__chat_buttons_helpcmd_text"><b>{item.cmd}</b> <span class="desc">{item.descr}</span></div>
                    {/if}
                {/each}
            </div>
            {/if}
        {/if}
    </div>
</div>