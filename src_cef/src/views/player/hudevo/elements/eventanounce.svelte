<script>
    import { fly } from 'svelte/transition';

    import { addListernEvent } from "api/functions";
    
    let 
        visible = false,
        subTitle,
        title, 
        desc, 
        image;

    addListernEvent ("hud.event", (_visible, _subTitle, _title, _desc, _image) => {
        if (_visible) {
            subTitle = _subTitle;
            title = _title;
            desc = _desc;
            image = _image;
        }
        visible = _visible;
    });

</script>
{#if visible}
    <div class="hudevo__event" in:fly={{ y: -50, duration: 500 }} out:fly={{ y: -50, duration: 250 }}>
        <div class="hudevo__event_image" style="background-image: url({document.cloud}events/{image})" /> 
        <div class="hudevo__notification_subtitle mt-51">{subTitle}</div>
        <div class="hudevo__notification_title">
            {title}
        </div>
        <div class="hudevo__notification_text medium">
            {desc}
        </div>
    </div>
{/if}