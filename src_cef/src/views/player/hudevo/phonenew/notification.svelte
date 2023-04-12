<script>

    export let phoneNotification;

    import { formatMessage } from "@/views/player/hudevo/phonenew/components/messages/data";

</script>
<div class="newphone__notification">
    <div class="box-center box-between">
        <div class="box-center box-flex">
            <div class="newphone__maps_headerimage radio small" style={phoneNotification.Avatar ? `background-image: url(${phoneNotification.Avatar})` : ""}></div>
            <div>{phoneNotification.Name}</div>
        </div>
        <div class="gray">сейчас</div>
    </div>
    <div class="mt-5 fw-bold">{@html formatMessage (phoneNotification.Text)}</div>
</div>