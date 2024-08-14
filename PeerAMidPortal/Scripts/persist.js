const persist = {

    get: function (key) {
        var json = localStorage.getItem(key);
        if (json)
            return JSON.parse(json);
        return null;
    },

    set: function (key, value) {
        var json = JSON.stringify(value);
        localStorage.setItem(key, json);
    },

    remove: function (key) {
        localStorage.removeItem(key);
    }
}