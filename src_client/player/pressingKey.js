global.pressingKey = new class {
    create (name) {
        this[name] = new Date().getTime();
    }
    function (name, callback) {
        if (typeof (callback) !== "function") return;
        else if (typeof (this[name]) === "undefined") callback (-1);
        else if ((this[name] && new Date().getTime() - this[name]) < 1000) callback (true);
        else callback (false);
    }
}
/*
pressingKey.create("test")
pressingKey.create("test12")
pressingKey.create("test22")

setInterval(() => {
    pressingKey.function("test", (value) => {

        console.log(value)
    })
    pressingKey.function("test12", (value) => {

        console.log(value)
    })
    pressingKey.function("test122", (value) => {

        console.log(value)
    })
}, 5000)*/