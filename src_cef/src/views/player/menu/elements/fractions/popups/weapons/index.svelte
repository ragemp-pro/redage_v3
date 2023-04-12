<script>
    import './main.sass'
    import { executeClient } from 'api/rage'
    import { fly } from 'svelte/transition';
    import { translateText } from 'lang'
</script>
<div class="popup__newhud">
    <div class="popup__newhud_box">
        <div class="popup__newhud_title">
            <span class="hud__icon-info popup__newhud_icon"/>Добавление оружия
        </div>
        <input class="popup__newhud_input" placeholder="Введите название оружия.."/>
        <div class="popup__newhud_list">
            <div class="popup__list_element">
                <div class="popup__element_weapon">
                    <div class="popup__weapon_img"></div>
                </div>
                <div class="box-column">
                    <div class="popup__element_title">Micro SMG</div>
                    <div class="popup__element_subtitle">Лёгкое оружие</div>
                </div>
                <div class="popup__element_title">Добавить</div>
            </div>
            <div class="popup__list_element added">
                <div class="popup__element_weapon">
                    <div class="popup__weapon_img"></div>
                </div>
                <div class="box-column">
                    <div class="popup__element_title">Micro SMG</div>
                    <div class="popup__element_subtitle">Лёгкое оружие</div>
                </div>
                <div class="popup__element_title green">Добавлено</div>
            </div>
            <div class="popup__list_element">
                <div class="popup__element_weapon">
                    <div class="popup__weapon_img"></div>
                </div>
                <div class="box-column">
                    <div class="popup__element_title">Micro SMG</div>
                    <div class="popup__element_subtitle">Лёгкое оружие</div>
                </div>
                <div class="popup__element_title">Добавить</div>
            </div>
            <div class="popup__list_element added">
                <div class="popup__element_weapon">
                    <div class="popup__weapon_img"></div>
                </div>
                <div class="box-column">
                    <div class="popup__element_title">Micro SMG</div>
                    <div class="popup__element_subtitle">Лёгкое оружие</div>
                </div>
                <div class="popup__element_title green">Добавлено</div>
            </div>
            <div class="popup__list_element">
                <div class="popup__element_weapon">
                    <div class="popup__weapon_img"></div>
                </div>
                <div class="box-column">
                    <div class="popup__element_title">Micro SMG</div>
                    <div class="popup__element_subtitle">Лёгкое оружие</div>
                </div>
                <div class="popup__element_title">Добавить</div>
            </div>
            <div class="popup__list_element added">
                <div class="popup__element_weapon">
                    <div class="popup__weapon_img"></div>
                </div>
                <div class="box-column">
                    <div class="popup__element_title">Micro SMG</div>
                    <div class="popup__element_subtitle">Лёгкое оружие</div>
                </div>
                <div class="popup__element_title green">Добавлено</div>
            </div>
            <div class="popup__list_element">
                <div class="popup__element_weapon">
                    <div class="popup__weapon_img"></div>
                </div>
                <div class="box-column">
                    <div class="popup__element_title">Micro SMG</div>
                    <div class="popup__element_subtitle">Лёгкое оружие</div>
                </div>
                <div class="popup__element_title">Добавить</div>
            </div>
            <div class="popup__list_element added">
                <div class="popup__element_weapon">
                    <div class="popup__weapon_img"></div>
                </div>
                <div class="box-column">
                    <div class="popup__element_title">Micro SMG</div>
                    <div class="popup__element_subtitle">Лёгкое оружие</div>
                </div>
                <div class="popup__element_title green">Добавлено</div>
            </div>
        </div>
        <div class="popup__newhud__buttons">
            <div class="popup__newhud_button">{translateText('popups', 'Подтвердить')}</div>
            <div class="popup__newhud_button">{translateText('popups', 'Отмена')}</div>
        </div>
    </div>
</div>