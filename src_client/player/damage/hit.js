let hitsData = [];

const addHit = (amount, remoteId, boneId, isHead, isDead) => {

    let netPlayer = global.getPlayerByRemoteId(parseInt(remoteId));

    if (netPlayer != null && 0 !== netPlayer.handle) {

        const getBoneCoords = netPlayer.getBoneCoords(boneId, 0, 0, 0);

        if (isDead) {
            hitsData.push({
                amount: amount,
                position: new mp.Vector3(getBoneCoords.x, getBoneCoords.y, getBoneCoords.z - 0.2),
                count: 0,
                head: isHead,
                isDead: true
            });
        }

        hitsData.push({
            amount: amount,
            position: new mp.Vector3(getBoneCoords.x, getBoneCoords.y, getBoneCoords.z),
            count: 0,
            head: isHead
        });
    }
}

gm.events.add('client.addHit', addHit);

gm.events.add("render", function () {
    hitsData.forEach((data) => {
        let color = [
            255,
            255,
            255
        ];

        let title = data.amount.toString();

        if (data.isDead) {
            title = 'DEAD';
            color = [
                255,
                180,
                0
            ];
        } else if (data.head)
            color = [
                255,
                0,
                0
            ];

        mp.game.graphics.drawText(title, [
            data.position.x,
            data.position.y,
            data.position.z + 1.4
        ], {
            font: 2,
            centre: true,
            color: [
                color[0],
                color[1],
                color[2],
                255 - data.count
            ],
            scale: [0.3, 0.3],
            outline: true
        });
        data.count += 2;
        data.position.z += 0.02;

        if (data.count > 155) {
            const index = hitsData.findIndex(d => d === data);

            hitsData.splice(index, 1);
        }
    })
});