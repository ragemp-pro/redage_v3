<script>
    import { translateText } from 'lang'
    import InputCustom from 'components/input/index.svelte'
    import { accountLogin } from 'store/account'
    
    import { executeClient } from 'api/rage'

    let regusername = "",
        regemail = "",
        regpass1 = "",
        regpass2 = "",
        regpromo = "";

    const onRegister = () => {
        if(
            regusername &&
            regemail &&
            regpass1 &&
            regpass2 && $accountLogin !== -99) {

            executeClient ('client:OnSignUpv2', regusername, regemail, regpromo, regpass1, regpass2);
        }
    }

    const onKeyUp = (event) => {        
        const { keyCode } = event;
        if (keyCode === 13)
            onRegister ();
    }
    let language = "?";
</script>
<svelte:window on:keyup={onKeyUp} />
<div class="main">
    <div class="main__box">
        <div class="main__forms">
            <InputCustom updateLang={(lang) => language = lang} setValue={(value) => regusername = value} value={regusername} placeholder={translateText('player2', 'Логин..')} type="text" icon="auth-user"/>
            <InputCustom updateLang={(lang) => language = lang} setValue={(value) => regemail = value} value={regemail} placeholder={translateText('player2', 'Почта..')} type="email" icon="auth-mail"/>
            <InputCustom updateLang={(lang) => language = lang} setValue={(value) => regpass1 = value} value={regpass1} placeholder={translateText('player2', 'Пароль..')} type="password" icon="auth-lock"/>
            <InputCustom updateLang={(lang) => language = lang} setValue={(value) => regpass2 = value} value={regpass2} placeholder={translateText('player2', 'Повторите пароль..')} type="password" icon="auth-pass-repeat"/>
            <InputCustom updateLang={(lang) => language = lang} setValue={(value) => regpromo = value} value={regpromo} placeholder={translateText('player2', 'Промокод..')} type="text" icon="auth-gift"/>
            <!--<div class="main__buttons_box main__buttons_registration">
                <InputCustom setValue={(value) => regpromo = value} value={regpromo} placeholder="Промокод.." type="text" icon="auth-gift"/>
                <div class="main__button main_button_size_small">
                    <div class="main__button_top box-center">
                        <span class="auth-lock"/>
                    </div>
                    <div class="main__button_bottom box-center">
                        <div class="main__button_rectangle box-center">ALT</div>
                    </div>
                </div>
            </div>-->
            <div class="box-flex">
                <div class="main__button main_button_size_large" on:click={onRegister}>
                    <div class="main__button_left box-center">{translateText('player2', 'Создать')}</div>
                    <div class="main__button_right box-center">
                        <div class="main__button_square box-center">
                            <span class="auth-arrow"/>
                        </div>
                    </div>
                </div>
                <div class="main__button main_button_size_small box-center">
                    <div class="main__button_bottom box-center">
                        <div class="main__button_rectangle box-center">{language}</div>
                    </div>
                </div>
            </div>
        </div>
        <div class="main__scroll"/>
    </div>
</div>