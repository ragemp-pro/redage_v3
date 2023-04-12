<script>
    let furnitureData = [
        {
            name: "Сигнализация",
            price: "500000"
        },
        {
            name: "Столы",
        },
        {
            name: "Стулья",
        },
        {
            name: "Картины",
        },
    ]
</script>
{#if false}
    <div class="box-column">
        <div class="house__main_menu">
            {#each furnitureData as furniture, index}
                <div class="house__element">
                    {#if furniture.price}
                    <div class="box-between">
                        <div>{furniture.name}</div>
                        <div class="green">${furniture.price}</div>
                    </div>
                    {:else}
                        <div class="box-center" style="width:100%">
                            <div>{furniture.name}</div>
                        </div>
                    {/if}
            
                </div>
            {/each}
        </div>
        <div class="house__info small m-0 margin-none">
            Список доступных улучшений для дома
        </div>
    </div>
{/if}
{#if true}
    <div class="house__furniture">
        <div class="house__furniture_element">
            <div class="box-between">
                <div class="box-column">
                    <div class="house__furniture_title">Лампа</div>
                    <div class="house__furniture_text money">$20.000</div>
                    <div class="box-flex">
                        <div class="house__furniture_smallimage"></div>
                        <div class="house__furniture_text">10 сосны</div>
                    </div>
                    <div class="box-flex">
                        <div class="house__furniture_smallimage"></div>
                        <div class="house__furniture_text">10 сосны</div>
                    </div>
                    <div class="box-flex">
                        <div class="house__furniture_smallimage"></div>
                        <div class="house__furniture_text">10 сосны</div>
                    </div>
                </div>
                <div class="house__furniture_image"></div>
            </div>
            <div class="box-between">
                <div class="house__element_button">
                    <div class="houseicon-safe house__furniture_icon"></div>
                    Купить за наличные
                </div>
                <div class="house__element_button">
                    <div class="houseicon-garage house__furniture_icon"></div>
                    Скрафтить самому
                </div>
            </div>
        </div>
        <div class="house__furniture_element">
            <div class="box-between">
                <div class="box-column">
                    <div class="house__furniture_title">Лампа</div>
                    <div class="house__furniture_text money">$20.000</div>
                    <div class="box-flex">
                        <div class="house__furniture_smallimage"></div>
                        <div class="house__furniture_text">10 сосны</div>
                    </div>
                    <div class="box-flex">
                        <div class="house__furniture_smallimage"></div>
                        <div class="house__furniture_text">10 сосны</div>
                    </div>
                    <div class="box-flex">
                        <div class="house__furniture_smallimage"></div>
                        <div class="house__furniture_text">10 сосны</div>
                    </div>
                </div>
                <div class="house__furniture_image"></div>
            </div>
            <div class="box-between">
                <div class="house__element_button">
                    <div class="houseicon-safe house__furniture_icon"></div>
                    Купить за наличные
                </div>
                <div class="house__element_button">
                    <div class="houseicon-garage house__furniture_icon"></div>
                    Скрафтить самому
                </div>
            </div>
        </div>
        <div class="house__furniture_element">
            <div class="box-between">
                <div class="box-column">
                    <div class="house__furniture_title">Лампа</div>
                    <div class="house__furniture_text money">$20.000</div>
                    <div class="box-flex">
                        <div class="house__furniture_smallimage"></div>
                        <div class="house__furniture_text">10 сосны</div>
                    </div>
                    <div class="box-flex">
                        <div class="house__furniture_smallimage"></div>
                        <div class="house__furniture_text">10 сосны</div>
                    </div>
                    <div class="box-flex">
                        <div class="house__furniture_smallimage"></div>
                        <div class="house__furniture_text">10 сосны</div>
                    </div>
                </div>
                <div class="house__furniture_image"></div>
            </div>
            <div class="box-between">
                <div class="house__element_button">
                    <div class="houseicon-safe house__furniture_icon"></div>
                    Купить за наличные
                </div>
                <div class="house__element_button">
                    <div class="houseicon-garage house__furniture_icon"></div>
                    Скрафтить самому
                </div>
            </div>
        </div>
        <div class="house__furniture_element">
            <div class="box-between">
                <div class="box-column">
                    <div class="house__furniture_title">Лампа</div>
                    <div class="house__furniture_text money">$20.000</div>
                    <div class="box-flex">
                        <div class="house__furniture_smallimage"></div>
                        <div class="house__furniture_text">10 сосны</div>
                    </div>
                    <div class="box-flex">
                        <div class="house__furniture_smallimage"></div>
                        <div class="house__furniture_text">10 сосны</div>
                    </div>
                    <div class="box-flex">
                        <div class="house__furniture_smallimage"></div>
                        <div class="house__furniture_text">10 сосны</div>
                    </div>
                </div>
                <div class="house__furniture_image"></div>
            </div>
            <div class="box-between">
                <div class="house__element_button">
                    <div class="houseicon-safe house__furniture_icon"></div>
                    Купить за наличные
                </div>
                <div class="house__element_button">
                    <div class="houseicon-garage house__furniture_icon"></div>
                    Скрафтить самому
                </div>
            </div>
        </div>

    </div>
{/if}