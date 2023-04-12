import Hammer from 'hammerjs';

let tinderContainer;
let allCards;
let nope;
let love;

function initCards(card, index) {
    tinderContainer.classList.add('loaded');
}


export const createHammer = (node, callback) => {
    tinderContainer = document.querySelector('.tinder');

    //let
    const hm = new Hammer(node);

    let status = false;

    hm.on('pan', function (event) {
        node.classList.add('moving');
    });

    hm.on('pan', function (event) {
        if (event.deltaX === 0) return;
        if (event.center.x === 0 && event.center.y === 0) return;

        if (event.deltaX > 0) {
            if (status !== "love") {
                status = "love";
                callback("updateStatus", status)
            }
        } else if (event.deltaX < 0) {
            if (status !== "nope") {
                status = "nope";
                callback("updateStatus", status)
            }
        } else if (status !== false) {
            status = false;
            callback ("updateStatus", status)
        }

        const xMulti = event.deltaX * 0.03;
        const yMulti = event.deltaY / 80;
        const rotate = xMulti * yMulti;

        node.style.transform = 'translate(' + event.deltaX + 'px, ' + event.deltaY + 'px) rotate(' + rotate + 'deg)';
    });

    hm.on('panend', function (event) {
        node.classList.remove('moving');

        const keep = Math.abs(event.deltaX) < 80 || Math.abs(event.velocityX) < 0.5;

        if (keep) {
            node.style.transform = '';
        } else {
            callback("confirm", status === "love")
        }
        status = false;
        callback("updateStatus", status)
    });
    callback ()
}