const EVENT_NAME = "SRV::CL::OverlayMessage"
const POSITION_OFFSET = 1.4
const MESSAGE_COLORS = [
    // message
    [255,255,255,255],
    // me
    [224,102,255,255],
    // do
    [224,102,255,255],
    // try
    [224,102,255,255]
]
const MESSAGE_SCALE = 0.25
const MESSAGE_FONT = 4
const DISSAPEAR_TIME = 5

mp.events.add(EVENT_NAME, (data) => {
    let info = JSON.parse(data)
    let sender = mp.players.atRemoteId(info.sender)
    if (sender === undefined) return
    sender.overlayMessage = info
    setTimeout(() => {
        if (sender.overlayMessage !== info) return
        else sender.overlayMessage = null
    }, 1000 * DISSAPEAR_TIME)
})

mp.events.add('render', () => {
    mp.players.forEachInRange(mp.players.local.position, 10, (player) => {
        if (player.overlayMessage) {
            let info = player.overlayMessage
            let pos = player.position
            pos.z += POSITION_OFFSET

            let additionalText = info.type == 4 ? getAdditionalText(info.result) : ''
            let text = info.text + additionalText

            mp.game.graphics.drawText(text, [pos.x,pos.y,pos.z], {
                font: MESSAGE_FONT,
                color: MESSAGE_COLORS[info.type - 1],
                scale: [MESSAGE_SCALE, MESSAGE_SCALE],
                centre: true,
                outline: false,
            })
        }
    })
})


function getAdditionalText(result) {
    return result ? " | ~g~Удачно" : " | ~r~Неудачно"
}