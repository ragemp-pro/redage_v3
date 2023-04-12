<script>

    import { charFractionID } from 'store/chars'
    import { isFraction } from '../../data'

</script>

<div class="fractions__header_image" style="background-image: url({document.cloud + ($charFractionID > 0 && isFraction () ? `img/fractionLogo/Fraction_${$charFractionID}.png` : `img/fractionLogo/Org.png`)})" />