<script>
    import { fly } from 'svelte/transition';
    import './main.sass';

    export let title;
    export let subtitle;
    export let notifictiontype;
</script>
<div class="notification-form" transition:fly="{{ y: -200, duration: 800 }}">
    <div class="notification-form__square box-center" class:notification-form__square_accept={notifictiontype == true} class:notification-form__square_error={notifictiontype == false}>
        <span class="auth-key"/>
    </div>
    <div class="notification-form__text-box">
        <div class="notification-form__title">{title}</div>
        <div class="notification-form__subtitle">{subtitle}</div>
    </div>
</div>