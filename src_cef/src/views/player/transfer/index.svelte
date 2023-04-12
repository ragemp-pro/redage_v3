<script>
    import './assets/main.sass';
    import copy from 'clipboard-copy'
    const onClick = () => {
        copy('51.77.42.5:22005')
    }

</script>
<div id ="player-transfer">
    <button class="player-transfer-container" on:click={onClick} />
</div>