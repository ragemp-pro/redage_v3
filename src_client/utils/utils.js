global.formatIntZero = (num, length) => { 
    return ("0" + num).slice(0 - length); 
}

global.utils = new class {
    fixHash(model) {
        return this.toInt64(model);
    }
    toInt32(model) {
        return model >= 2147483647 ? model - 4294967295 - 1 : model;
    }
    toInt64(model) {
        return model < 0 ? model + 4294967295 + 1 : model;
    }
    isFloatEqual(a, b, tol = 0.00001) {
        return Math.abs(a - b) < tol;
    }
    getCoordsInFront(fX, fY, fA, fDistance) {
        const degreesToRadians = this.degreesToRadians(fA);
        return { x: fX - fDistance * Math.sin(degreesToRadians), y: fY + fDistance * Math.cos(degreesToRadians) };
    }
    getFrontVector(matrix, r, v) {
        const { x: x, y: y } = this.getCoordsInFront(matrix.x, matrix.y, r, v);
        return new mp.Vector3(x, y, matrix.z);
    }
    radiansToDegrees(degrees) {
        return degrees * (180 / Math.PI);
    }
    degreesToRadians(degrees) {
        return degrees * (Math.PI / 180);
    }
    getDistance(pos1, pos2) {
        return Math.hypot(pos1.x - pos2.x, pos1.y - pos2.y, pos1.z - pos2.z);
    }
}