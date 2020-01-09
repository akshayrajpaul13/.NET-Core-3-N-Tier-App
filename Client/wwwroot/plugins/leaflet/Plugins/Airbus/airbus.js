L.TileLayer.OneAtlas = L.TileLayer.extend({
    defaultWmtsParams: {
        service: 'WMTS',
        request: 'GetTile',
        version: '1.1.1',
        layer: '',
        style: '',
        tilematrixSet: '',
        format: 'image/jpeg'
    },

    initialize: function (url, options) {
		    this.options = L.extend({apiKey: "", serviceMode: ""}, this.options);
        this._url = url;
        this._apiKey = options.apiKey;
        this._serviceMode = options.serviceMode || "KVP";
        if(this._apiKey.indexOf("Bearer ") === -1)  {
            this._apiKey = "Bearer " + this._apiKey;
        }
        var wmtsParams = L.extend({}, this.defaultWmtsParams);
        for (var i in options) {
            if(!options.hasOwnProperty(i)) continue;
            if (!this.options.hasOwnProperty(i) && i !== "matrixIds") {
                wmtsParams[i] = options[i];
            }
        }
        this.wmtsParams = wmtsParams;
        this.matrixIds = options.matrixIds ||this.getDefaultMatrix();
        L.setOptions(this, options);
    },

    createTile: function (coords, done) {
        var tile = document.createElement('img');
        L.DomEvent.on(tile, 'load', L.bind(this._tileOnLoad, this, done, tile));
        L.DomEvent.on(tile, 'error', L.bind(this._tileOnError, this, done, tile));
        if (this.options.crossOrigin) {
            tile.crossOrigin = '';
        }
        tile.alt = '';
        tile.setAttribute('role', 'presentation');
        var url = this.getTileUrl(coords);
        this._loadTile(tile, url);
        return tile;
    },

    _loadTile: function(tile, url) {
        var xhr = new XMLHttpRequest();
        xhr.onreadystatechange = function()  {
            if (xhr.readyState === XMLHttpRequest.DONE && xhr.status === 200) {
                tile.src = URL.createObjectURL(xhr.response);
            }
        };
        xhr.open('GET', url);
        xhr.responseType = 'blob';
        xhr.setRequestHeader('Authorization', this._apiKey);
        xhr.send();
    },

    getTileUrl: function (coords) {
        if(this._serviceMode === "KVP") {
            return this._url + L.Util.getParamString(this.wmtsParams, this._url) + "&tilematrix=" + coords.z + "&tilerow=" + coords.y + "&tilecol=" + coords.x;
        }
        else if(this._serviceMode === "RESTful")
        {
            return this._url.replace("{x}", coords.x).replace("{y}", coords.y).replace("{z}", coords.z);
        }
        else
        {
            throw new Error("Invalid serviceMode. Supported service modes are 'KVP' and 'RESTful'");
        }
    },

    getDefaultMatrix : function () {
        return Array.from(Array(22), function(item, index) {
            return {
                identifier: "" + index,
                topLeftCorner: new L.LatLng(20037508.3428, -20037508.3428)
            };
        });
    }
});

L.tileLayer.oneAtlas = function (url, options) {
    return new L.TileLayer.OneAtlas(url, options);
};

if (!Array.from) {
  Array.from = (function () {
    var toStr = Object.prototype.toString;
    var isCallable = function (fn) {
      return typeof fn === 'function' || toStr.call(fn) === '[object Function]';
    };
    var toInteger = function (value) {
      var number = Number(value);
      if (isNaN(number)) { return 0; }
      if (number === 0 || !isFinite(number)) { return number; }
      return (number > 0 ? 1 : -1) * Math.floor(Math.abs(number));
    };
    var maxSafeInteger = Math.pow(2, 53) - 1;
    var toLength = function (value) {
      var len = toInteger(value);
      return Math.min(Math.max(len, 0), maxSafeInteger);
    };

    // The length property of the from method is 1.
    return function from(arrayLike/*, mapFn, thisArg */) {
      // 1. Let C be the this value.
      var C = this;

      // 2. Let items be ToObject(arrayLike).
      var items = Object(arrayLike);

      // 3. ReturnIfAbrupt(items).
      if (arrayLike == null) {
        throw new TypeError("Array.from requires an array-like object - not null or undefined");
      }

      // 4. If mapfn is undefined, then var mapping be false.
      var mapFn = arguments.length > 1 ? arguments[1] : void undefined;
      var T;
      if (typeof mapFn !== 'undefined') {
        // 5. else
        // 5. a If IsCallable(mapfn) is false, throw a TypeError exception.
        if (!isCallable(mapFn)) {
          throw new TypeError('Array.from: when provided, the second argument must be a function');
        }

        // 5. b. If thisArg was supplied, var T be thisArg; else var T be undefined.
        if (arguments.length > 2) {
          T = arguments[2];
        }
      }

      // 10. Let lenValue be Get(items, "length").
      // 11. Let len be ToLength(lenValue).
      var len = toLength(items.length);

      // 13. If IsConstructor(C) is true, then
      // 13. a. Let A be the result of calling the [[Construct]] internal method of C with an argument list containing the single item len.
      // 14. a. Else, Let A be ArrayCreate(len).
      var A = isCallable(C) ? Object(new C(len)) : new Array(len);

      // 16. Let k be 0.
      var k = 0;
      // 17. Repeat, while k < len… (also steps a - h)
      var kValue;
      while (k < len) {
        kValue = items[k];
        if (mapFn) {
          A[k] = typeof T === 'undefined' ? mapFn(kValue, k) : mapFn.call(T, kValue, k);
        } else {
          A[k] = kValue;
        }
        k += 1;
      }
      // 18. Let putStatus be Put(A, "length", len, true).
      A.length = len;
      // 20. Return A.
      return A;
    };
  }());
}