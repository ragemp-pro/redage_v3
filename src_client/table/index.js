import { accessIdToKey, accessKeyToId, accessKeyToName, accessType } from "./name";
global.tableInFocus = false;

global.table = new class {
    constructor() {
        this.ranksName = null;
        this.ranksKeyName = null;
        this.defaultAccess = null;
        this.departmentsTag = null;
    }

    getRank(key, json) {
        if (typeof json === "string")
            json = JSON.parse(json);

        return {
            id: key,
            playerCount: json[0],
            name: json[1],
            salary: json[2],
            maxScore: json[3],
        }
    }

    getRanks(json) {
        if (typeof json === "string")
            json = JSON.parse(json);

        let ranks = []

        Object.keys(json).forEach((key) => {
            if (json [key])
                ranks.push(this.getRank(key, json [key]));
        });

        return ranks;
    }

    getRanksKey(json) {
        if (typeof json === "string")
            json = JSON.parse(json);

        let ranks = {}

        Object.keys(json).forEach((key) => {
            if (json [key])
                ranks[key] = this.getRank(key, json [key]);
        });

        return ranks;
    }

    setRanksName (json) {
        if (typeof json === "string")
            json = JSON.parse(json);

        const oldRanksName = this.ranksName;

        this.ranksName = this.getRanks (json);
        this.ranksKeyName = this.getRanksKey (json);

        if (oldRanksName !== null) {
            Object.keys(oldRanksName).forEach((key) => {
                if (oldRanksName [key] && this.ranksName [key])
                    this.ranksName [key].playerCount = oldRanksName [key].playerCount;
            });
        }
    }

    getRankName(rank) {

        if (this.ranksKeyName && this.ranksKeyName [rank])
            return this.ranksKeyName [rank].name;

        return "";
    }

    getMaxScore(rank) {

        if (this.ranksKeyName && this.ranksKeyName [rank])
            return this.ranksKeyName [rank].maxScore;

        return "";
    }

    getMember(json) {
        if (typeof json === "string")
            json = JSON.parse(json);

        return {
            uuid: json[0],
            name: json[1],
            rank: json[2],
            rankName: this.getRankName(json[2]),
            playerId: json[3],
            isOnline: json[3] !== -1,
            avatar: json[4],
            date: json[5],
            departmentId: json[6],
            departmentName: this.getDepartmentName(json[6]),
            departmentRank: json[7],
            access: json[8],
            lock: json[9],
            score: json[10],
            maxScore: this.getMaxScore(json[2]),
            lastLoginDate: json[11],
            todayTime: json[12],
            weekTime: json[13],
            monthTime: json[14],
            totalTime: json[15],
        }
    }
    getMembers(json) {
        if (typeof json === "string")
            json = JSON.parse(json);

        let members = []
        json.forEach((data) => {
            members.push(this.getMember(data));
        });

        members.sort(function(x1,x2) {
            if (x1.isOnline < x2.isOnline) return 1;
            if (x1.isOnline > x2.isOnline) return -1;
            // при равных score сортируем по time
            if (x1.rank < x2.rank) return 1;
            if (x1.rank > x2.rank) return -1;
            return 0;
        });

        return members;
    }

    getOnline(json) {
        if (typeof json === "string")
            json = JSON.parse(json);

        return {
            online: json[0],
            offline: json[1],
            all: json[2]
        }

    }

    getStats(json) {
        if (typeof json === "string")
            json = JSON.parse(json);

        return {
            drugs: json[0],
            mats: json[1],
            maxMats: json[2],
            medKits: json[3],
            money: json[4],
            warZonesCount: json[5],
            gangZonesCount: json[6],
            bizCount: json[7]
        }
    }

    getInfo(json) {
        if (typeof json === "string")
            json = JSON.parse(json);

        return {
            isLeader: json[0],
            id: json[1],
            name: json[2],
            discord: json[3],
            date: json[4],
            slogan: json[5],
            salary: json[6],
            color: json[7],
            none1: json[8],
            isStock: json[9],
            isAccessStock: json[10],
            orgUpgrate: json[11],
            isLog: json[12],
            setRank: json[13],
            setVehicleRank: json[14],
            invite: json[15],
            unInvite: json[16],
            tableWall: json[17],
            editAllTabletWall: json[18],
            createDepartment: json[19],
            deleteDepartment: json[20],
            reprimand: json[21],
            clothesEdit: json[22],
            isGunStock: json[23],
            isGunAccessStock: json[24],
            familyZone: json[25],
            crimeOptions: json[26]
        }
    }

    getBoard(json) {
        if (typeof json === "string")
            json = JSON.parse(json);

        return {
            count: json[0],
            title: json[1],
            text: json[2],
            time: json[3],
            uuid: json[4],
            name: json[5],
            rank: json[6],
            rankName: this.getRankName(json[6]),
            page: json[7]
        }
    }

    getLog(json) {
        if (typeof json === "string")
            json = JSON.parse(json);

        return {
            text: json[0],
            time: json[1],
            uuid: json[2],
            name: json[3],
            rank: json[4],
            rankName: this.getRankName(json[4]),
        }
    }

    getLogs(json) {
        if (typeof json === "string")
            json = JSON.parse(json);

        let logs = []
        json.forEach((data) => {
            logs.push(this.getLog(data));
        });

        return logs;
    }

    setDefaultAccess(_defaultAccess) {
        if (typeof _defaultAccess === "string")
            _defaultAccess = JSON.parse(_defaultAccess);

        this.defaultAccess = _defaultAccess;
    }

    getAccess(_access, _lock = null) {
        if (typeof _access === "string")
            _access = JSON.parse(_access);

        if (typeof _lock === "string")
            _lock = JSON.parse(_lock);

        let access = [];

        this.defaultAccess.forEach(id => {
            let type = accessType.Remove;

            if (_access && _access.includes(id))
                type = accessType.Add;
            else if (_lock && _lock.includes(id))
                type = accessType.Remove;
            else if (_lock && !_lock.includes(id))
                type = accessType.Skip;

            const accessKey = accessIdToKey [id];

            access.push({
                id: id,
                name: accessKeyToName [accessKey],
                type: type
            })
        });

        return access;
    }

    //

    getDepartment(key, json) {
        if (typeof json === "string")
            json = JSON.parse(json);

        return {
            id: key,
            playerCount: json[0],
            chiefName: json[1],
            name: json[2],
            tag: json[3],
        }
    }

    getDepartments(json) {
        if (typeof json === "string")
            json = JSON.parse(json);

        let departments = []
        let tags = {};

        Object.keys(json).forEach((key) => {
            if (json [key]) {
                const department = this.getDepartment(key, json [key]);
                tags[key] = this.getDepartmentsTag(key, department.name);
                departments.push(department);
            }
        });

        this.departmentsTag = tags;

        return departments;
    }



    getDepartmentRank(key, json) {
        if (typeof json === "string")
            json = JSON.parse(json);

        return {
            id: key,
            playerCount: json[0],
            name: json[1],
            access: json[2],
            lock: json[3],
        }
    }

    getDepartmentRanks(json) {
        if (typeof json === "string")
            json = JSON.parse(json);

        let ranks = [];

        Object.keys(json).forEach((key) => {
            if (json [key])
                ranks.push(this.getDepartmentRank(key, json [key]));
        });

        return ranks;
    }

    //

    getDepartmentsTag(key, name) {
        return {
            id: key,
            name: name
        }
    }

    setDepartmentsTag(json) {
        if (typeof json === "string")
            json = JSON.parse(json);

        let departmentsTag = {};

        Object.keys(json).forEach((key) => {
            if (json [key])
                departmentsTag[key] = this.getDepartmentsTag(key, json [key]);
        });

        this.departmentsTag = departmentsTag;
    }

    getDepartmentName(departmentId) {

        if (this.departmentsTag && this.departmentsTag [departmentId])
            return this.departmentsTag [departmentId].name;

        return "Нет";
    }

    //

    getVehicle(json) {
        if (typeof json === "string")
            json = JSON.parse(json);

        return {
            model: json[0],
            number: json[1],
            rank: json[2],
            rankName: this.getRankName(json[2]),
            isEvacuation: json[3],
            isGps: json[4],
        }
    }
    getVehicles(json) {
        if (typeof json === "string")
            json = JSON.parse(json);

        let vehicles = []
        json.forEach((data) => {
            vehicles.push(this.getVehicle(data));
        });

        return vehicles;
    }

    //

    getAwards(json) {
        if (typeof json === "string")
            json = JSON.parse(json);

        let awards = [];

        json.forEach((data) => {
            awards.push({
                type: data [0],
                itemId: data [1],
                count: data [2],
                data: data [3],
                gender: data [4],
            })
        })

        return awards;
    }

    getTask(json) {
        if (typeof json === "string")
            json = JSON.parse(json);

        return {
            name: json [0],
            desc: json [1],
            count: json [2],
            maxCount: json [3],
            status: json [2] >= json [3],
            success: json [4],
            exp: json [5],
            awards: this.getAwards (json [6]),
            time: json [7]
        }
    }

    getTasks(json) {
        if (typeof json === "string")
            json = JSON.parse(json);

        let tasks = []
        json.forEach((data) => {
            tasks.push(this.getTask(data));
        });

        return tasks;
    }

    //


    getMission(json) {
        if (typeof json === "string")
            json = JSON.parse(json);

        return {
            id: json [0],
            bonus: json [1],
            isShow: json [2],
            gps: json [3],
            isTake: json [4],
        }
    }

    getMissions(json) {
        if (typeof json === "string")
            json = JSON.parse(json);

        let missions = []
        json.forEach((data) => {
            missions.push(this.getMission(data));
        });

        return missions;
    }
}

require('./org')
require('./frac')
//require('./warPoints')
require('./war')
require('./familyZone')
require('./orgCreate')