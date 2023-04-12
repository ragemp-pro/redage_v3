<script>
    import './main.sass';

    let Filter = 0.9;
    let Smoke = 1.0;
    let Shadow = 1.0;

    const UpdateFilter = (filter, smoke, shadow) => {
        Filter = filter;
        Smoke = smoke;
        Shadow = shadow;
    }

    window.events.addEvent("cef.filter.UpdateFilter", UpdateFilter);
</script>

<div class="filter">
    <span class="filter-img filter" style={`opacity: ${Filter};`}/>
    <span class="filter-img smoke"  style={`opacity: ${Smoke};`}/>
    <span class="filter-img shadow" style={`opacity: ${Shadow};`}/>
</div>