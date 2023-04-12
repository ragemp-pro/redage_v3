<script>
    import { createHammer } from './tinder'

    import {executeClientAsyncToGroup, executeClientToGroup} from "api/rage";

    let status = false;
    const updateTinder = (type, action) => {
        if (type === "updateStatus") {
            status = action;
        } else if (type === "confirm") {
            onAction (action)
        }
    }

    let list = []

    const getList = () => {
        executeClientAsyncToGroup("tinder.getList").then((result) => {
            if (hasJsonStructure(result))
                list = JSON.parse(result);
        });
    }

    getList ();

    import { addListernEvent } from 'api/functions';
    addListernEvent ("phone.tinder.getList", getList)

    const onAction = (isLove) => {
		if (!list || list.length == 0) return;
        const tinder = list[0];
        if (tinder) {
            executeClientToGroup("tinder.action", tinder.uuid, isLove);
            list.splice(0, 1);
            list = list;
        }
    }

    import { fly } from 'svelte/transition';
    import {hasJsonStructure} from "api/functions";

    let isDestroy = false;
    import { onDestroy } from 'svelte';
    onDestroy(() => {
        isDestroy = true;
    });
</script>


{#if list.length}
    <div class="tinder" class:tinder_love={status === "love"} class:tinder_nope={status === "nope"}>
        <div class="tinder--status">
          <i class="tindericons-mark"></i>
          <i class="tindericons-heart"></i>
        </div>

        <div class="tinder--cards">
            <div class="tinder--card" use:createHammer={updateTinder}>
				<div class="tindimage" style="background-image: url({list[0].avatar})" />
				<div class="tinder__name newphone__project_padding16">{list[0].name}</div>
				<div class="tinder__line"></div>
				<div class="tinder__descr newphone__project_padding16">{list[0].text}</div>
			</div>
        </div>

        <div class="tinder--buttons">
          <button id="nope"><i class="tindericons-mark" on:click={() => onAction (false)}></i></button>
          <button id="love"><i class="tindericons-heart" on:click={() => onAction (true)}></i></button>
        </div>
    </div>
{:else}
    <div class="newphone__forbes_anon newphone__project_padding20">
        <div class="newphone__forbes_hidden"></div>
        <div class="gray">Кажется, тут никого нет.. Но скоро кто-то появится!</div>
    </div>
{/if}