const isPointInArea2D = (point, area) => {
    let x = point[0],
        y = point[1];

    let inside = false;

    for (let i = 0, j = area.length - 1; i < area.length; j = i++) {
        let xi = area[i][0],
            yi = area[i][1];
        let xj = area[j][0],
            yj = area[j][1];

        let intersect = yi > y != yj > y && x < ((xj - xi) * (y - yi)) / (yj - yi) + xi;

        if (intersect) inside = !inside;
    }

    return inside;
};

const TWOPI = 6.283185307179586476925287;
const EPSILON = 0.0000001;

const modulus = (p) => Math.sqrt(p.x * p.x + p.y * p.y + p.z * p.z);

const getAngleSumBetweenPositionAndVertices = (position, vertices) => {
    let i;
    let m1, m2;
    let anglesum = 0,
        costheta;

    for (i = 0; i < vertices.length; i++) {
        const p1 = new mp.Vector3(vertices[i].x - position.x, vertices[i].y - position.y, vertices[i].z - position.z);
        const p2 = new mp.Vector3(vertices[(i + 1) % vertices.length].x - position.x, vertices[(i + 1) % vertices.length].y - position.y, vertices[(i + 1) % vertices.length].z - position.z);

        m1 = modulus(p1);
        m2 = modulus(p2);

        if (m1 * m2 <= EPSILON) return TWOPI;
        else costheta = (p1.x * p2.x + p1.y * p2.y + p1.z * p2.z) / (m1 * m2);

        anglesum += Math.acos(costheta);
    }
    return anglesum;
};

mp.polygons = {
    pool: [],
    add: (id, vertices, height, options = { lineColorRGBA: [255, 255, 255, 255], dimension: 0 }) => {
        const polygon = {
            id: id,
            vertices,
            height,
            lineColorRGBA: options.lineColorRGBA,
            dimension: options.dimension,
            colliding: false,
        };

        mp.polygons.pool.push(polygon);

        return polygon;
    },

    addGreenZone: (id, vertices, height, options = { lineColorRGBA: [0, 230, 0, 255], dimension: 0 }) => {

        let positions = [];

        vertices.forEach((position) => {
            positions.push(new mp.Vector3(position.x, position.y, position.z-1.12))
        })


        const polygon = {
            id: id,
            vertices: positions,
            height,
            lineColorRGBA: options.lineColorRGBA,
            dimension: options.dimension,
            colliding: false,
            isGreenZone: true
        };

        mp.polygons.pool.push(polygon);

        return polygon;
    },
    remove: (polygon) => {
        const index = mp.polygons.pool.findIndex((p) => p.id === polygon.id);

        if (index !== -1) mp.polygons.pool.splice(index, 1);
    },
    exists: (polygon) => {
        return mp.polygons.pool.some((p) => p.id === polygon.id);
    },
    isPositionWithinPolygon: (position, polygon, dimension) => {
        if (dimension && polygon.dimension !== dimension && polygon.dimension !== -1) return false;

        const { vertices } = polygon;

        const polygonPoints2D = [];

        for (let i in vertices) {
            if ((position.z >= vertices[i].z && position.z <= vertices[i].z + polygon.height) || getAngleSumBetweenPositionAndVertices(position, vertices) >= 5.8) polygonPoints2D.push([vertices[i].x, vertices[i].y]);
            else return false;
        }

        return isPointInArea2D([position.x, position.y], polygonPoints2D);
    },
}

require("./add");
