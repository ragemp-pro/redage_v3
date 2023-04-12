<script>

    import Key from './key.svelte'
</script>


<div class="keyboard-bg">

    <div class="key-group-left max-1 indent-1">
        <div class="key-line top">
            <div class="key-group max-4">
                <Key title="Esc" disabled={true} code={27} />
            </div>
            <div class="key-group max-3">
                <Key title="F1" disabled={true} code={112} />
                <Key title="F2" code={113} />
                <Key title="F3" code={114} />
                <Key title="F4" code={115} />
            </div>

            <div class="key-group max-3">
                <Key title="F5" code={116} />
                <Key title="F6" code={117} />
                <Key title="F7" code={118} />
                <Key title="F8" code={119} />
            </div> 

            <div class="key-group max-3">
                <Key title="F9" code={120} />
                <Key title="F10" code={121} />
                <Key title="F11" code={122} />
                <Key title="F12" code={123} />
            </div>
        </div>

        <div class="key-line bottom">
            <div class="key-group">
                <Key title="~ `" disabled={true} code={192} />
                <Key title="! 1" code={49} />
                <Key title="@ 2" code={50} />
                <Key title="# 3" code={51} />
                <Key title="$ 4" code={52} />
                <Key title="% 5" code={53} />
                <Key title="^ 6" code={54} />
                <Key title="&amp; 7" code={55} />
                <Key title="* 8" code={56} />
                <Key title="( 9" code={57} />
                <Key title="0 )" code={48} />
                <Key title="_ -" code={189} />
                <Key title="+ =" code={187} />
                <Key title="Backspace" style="backspace" code={8} />
            </div>
        </div>


        <div class="key-line bottom">
            <div class="key-group">
                <Key title="Tab" style="tab" code={9} />
                <Key title="Q" code={81} />
                <Key title="W" disabled={true} code={87} />
                <Key title="E" code={69} />
                <Key title="R" code={82} />
                <Key title="T" code={84} />
                <Key title="Y" code={89} />
                <Key title="U" code={85} />
                <Key title="I" code={73} />
                <Key title="O" code={79} />
                <Key title="P" disabled={true} code={80} />
                <Key title={`{ [`} code={219} />
                <Key title="} ]" code={221} />
                <Key title="| \" style="at" code={220} />
            </div>
        </div>

        <div class="key-line bottom">
            <div class="key-group">
                <Key title="Caps Lock" style="caps" code={20} />
                <Key title="A" disabled={true} code={65} />
                <Key title="S" disabled={true} code={83} />
                <Key title="D" disabled={true} code={68} />
                <Key title="F" disabled={true} code={70} />
                <Key title="G" code={71} />
                <Key title="H" code={72} />
                <Key title="J" code={74} />
                <Key title="K" code={75} />
                <Key title="L" code={76} />
                <Key title=": ;" code={186} />
                <Key title={`" '`} code={222} />
                <Key title="Enter" style="enter" code={13} />
            </div>
        </div>

        <div class="key-line bottom">
            <div class="key-group">
                <Key title="Shift" style="shift" code={16} />
                <Key title="Z" code={90} />
                <Key title="X" code={88} />
                <Key title="C" code={67} />
                <Key title="V" code={86} />
                <Key title="B" code={66} />
                <Key title="N" code={78} />
                <Key title="M" code={77} />
                <Key title="&lt; ," code={188} />
                <Key title="&gt; ." code={190} />
                <Key title="? /" code={191} />
                <Key title="Shift" style="shift-2" code={16} />
            </div>
        </div>

        <div class="key-line">
            <div class="key-group">
                <Key title="Ctrl" style="w" code={17} />
                <Key title="Super" disabled={true} style="w" code={91} />
                <Key title="Alt" style="w" code={18} />
                <Key title="" style="space" disabled={true} code={32} />
                <Key title="AltGr" style="w" code={67} />
                <Key title="Menu" style="w" code={93} />
                <Key title="Ctrl" style="w" code={17} />
                <Key title="" style="w" />    
            </div>
        </div>            
    </div>

    <div class="key-group-center max-2 indent-1">
        <div class="key-line top">
            <div class="key-group">
                <Key title="PrtSc" disabled={true} />
                <Key title="ScrLk" code={145} />
                <Key title="Pause" code={19} />
            </div>
        </div>
        <div class="key-line bottom">
            <div class="key-group">
                <Key title="Insert" code={45} />
                <Key title="Home" code={36} />
                <Key title="PgUp" code={33} />
            </div>
        </div>
        <div class="key-line bottom">
            <div class="key-group">
                <Key title="Delete" code={46} />
                <Key title="End" code={35} />
                <Key title="PgDn" code={34} />
            </div>
        </div>
        <div class="key-line-arrow">
            <div class="key-b">
                <div class="key-line bottom">
                    <div class="key-group center">                            
                        <Key title="↑" code={38} />
                    </div>
                </div>
                <div class="key-line">
                    <div class="key-group">
                        <Key title="←" code={37} />
                        <Key title="↓" code={40} />
                        <Key title="→" code={39} />
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div class="key-group-right max-3">
        <div class="key-line top">
            <div class="key-group">
                <div class="key empty DCS">
                    <div class=" keycap">
                    
                    </div>
                </div>
            </div>
        </div>

        <div class="key-line bottom">                
            <div class="key-group">
                <Key title="Num Lock" code={144} />
                <Key title="/" code={111} />
                <Key title="*" code={106} />
                <Key title="-" code={109} />
            </div>
        </div>

        <div class="numpad-x">
            <div class="numpad-a max-2 indent-2">
                <div class="key-line bottom">                    
                    <div class="key-group">
                        <Key title="7 Hme" code={36} />
                        <Key title="8 ↑" code={38} />
                        <Key title="9 PgU" code={33} />
                    </div>
                </div>

                <div class="key-line bottom max-2">
                    <div class="key-group">
                        <Key title="4 ←" code={37} />
                        <Key title="5" code={12} />
                        <Key title="6 →" code={39} />
                    </div>
                </div>
                <div class="key-line bottom">
                    <div class="key-group">
                        <Key title="1 End" code={35} />
                        <Key title="2 ↓" code={40} />
                        <Key title="3 PgD" code={34} />
                    </div>
                </div>
                <div class="key-line max-2">
                    <div class="key-group">
                        <Key title="0 Ins" style="numpad-null" code={45} />
                        <Key title=". Del" code={46} />
                    </div>
                </div>
            </div>

            <div class="numpad-b max-4">
                <div class="key-group">
                    <Key title="+" style="numpad-plus bottom" code={107} />
                    <Key title="Enter" style="numpad-enter" code={13} />
                </div>
            </div>
        </div>
    </div>
</div>