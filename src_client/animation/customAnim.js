gm.animationFlags = {
    // Normal
    // Обычная
    Normal: 0,

    // Repeat/loop
    // Повтор/цикл
    Loop: 1,

    // Stop animation on last frame
    // Остановить анимацию на последнем кадре
    StopOnLastFrame: 2,

    // Animate upper body only
    // Анимировать только верхнюю часть тела
    UpperBodyOnly: 16,

    // Enable player control (animation is played as a secondary task - can be used to mix animations)
    // Включить управление плеером (анимация воспроизводится как второстепенная задача - может использоваться для смешивания анимаций)
    EnablePlayerControl: 32,

    // Cancellable
    // Подлежащий отмене
    Cancellable: 128,

    // Additive animation
    // Аддитивная анимация
    AdditiveAnimation: 256,

    // Disables collision and physics on ped
    // Отключает колизию и физику на ped
    NoCollisionAndPhysics: 512,
}


gm.requestAnimDict = (animDictionary) => new Promise(async (resolve, reject) => {
    if (mp.game.streaming.hasAnimDictLoaded(animDictionary))
        return resolve(true);

    mp.game.streaming.requestAnimDict(animDictionary);

    let time = 0;
    while (!mp.game.streaming.hasAnimDictLoaded(animDictionary)) {
        if (time > 500)
            return resolve(translateText("Ошибка requestAnimDict. Dict: ") + animDictionary);

        time++;

        await global.wait (10);
    }

    return resolve(true);
});

gm.playAnimation = (entity, animDictionary, animName, speed, animFlag) => {

    const lockx = arguments.length > 5 && arguments[5] !== undefined ? arguments[5] : false;
    const locky = arguments.length > 6 && arguments[6] !== undefined ? arguments[6] : false;
    const lockz = arguments.length > 7 && arguments[7] !== undefined ? arguments[7] : false;

    try {
        gm.requestAnimDict(animDictionary).then(async () => {
            if (entity) {
                entity.taskPlayAnim(animDictionary, animName, speed, 0.0, -1, animFlag, 0.0, lockx, locky, lockz);
            }
        });
    } catch (e) {
        crushLog("playAnimation", animDictionary, e.stack);
    }
}

gm.stopAnimation = (entity, animDictionary, animName) => {
    try {
        if (entity) {
            entity.stopAnimTask(animDictionary, animName, 1);
            if (entity.isPlayingAnim(animDictionary, animName, 3) == 0) {
                entity.clearTasksImmediately();
                entity.clearTasks();
            }
        }
    } catch (e) {
        crushLog("stopAnimation", animDictionary, e.stack);
    }
};