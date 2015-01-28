KFManager = {
    //版本
    version: 'v1.0',
    //命名空间
    namespace: function (str) {
        var arr = str.split("."), w = window;
        for (i = 0; i < arr.length; i++) {
            w[arr[i]] = w[arr[i]] || {};
            w = w[arr[i]];
        }
    },
    //继承
    extend: function () {
        // inline overrides  
        var io = function (o) {
            for (var m in o) {
                this[m] = o[m];
            }
        };
        var oc = Object.prototype.constructor;

        return function (sb, sp, overrides) {
            if (typeof sp == 'object') {
                overrides = sp;
                sp = sb;
                sb = overrides.constructor != oc ? overrides.constructor : function () { sp.apply(this, arguments); };
            }
            var F = function () { }, sbp, spp = sp.prototype;
            F.prototype = spp;
            sbp = sb.prototype = new F();
            sbp.constructor = sb;
            sb.superclass = spp;
            if (spp.constructor == oc) {
                spp.constructor = sp;
            }
            sb.override = function (o) {
                SmartO2O.override(sb, o);
            };
            sbp.override = io;
            SmartO2O.override(sb, overrides);
            sb.extend = function (o) { SmartO2O.extend(sb, o); };
            return sb;
        };
    } (),
    //重写
    override: function (origclass, overrides) {
        if (overrides) {
            var p = origclass.prototype;
            for (var method in overrides) {
                p[method] = overrides[method];
            }
        }
    },
    apply: function (o, c, defaults) {
        if (defaults) {
            // no "this" reference for friendly out of scope calls  
            SmartO2O.apply(o, defaults);
        }
        if (o && c && typeof c == 'object') {
            for (var p in c) {
                o[p] = c[p];
            }
        }
        return o;
    }
}

/*
* 1.类创建，可继承
* Parent，父类
*newparams，当前新创建类的参数集合，包括属性和方法
*/
window.Class = {
    Create: function (parent, newparams) {
        var NewFun = function () {
            if (typeof this.initialize != 'undefined') {
                this.initialize.apply(this, arguments);
            }
        }
        if (parent != null) {
            if (typeof parent == 'function') {
                NewFun.prototype = new parent(Array.prototype.slice.apply(arguments, [1]));
                NewFun.prototype.constructor = NewFun;
                NewFun._parent = parent;
                /*
                * 实现调用父类函数的方法
                * <code>
                * this.Parent('callFunction');
                * </code>
                * @param string name    类名
                * @return function
                */
                NewFun.AddMethod('Parent', function (name) {
                    var func = NewFun.constructor.prototype[name];
                    if (func == NewFun[name]) {
                        func = parent.prototype[name];
                    }
                    return func.apply(NewFun, Array.prototype.slice.apply(arguments, [1]));
                });
            }
            else {
                /*继承出错*/
            }
        }
        /* 合并属性和方法 */
        if (typeof newparams == 'object') {
            for (var param in newparams) {
                NewFun.AddMethod(param, newparams[param]);
            }
        }
        return NewFun;
    }
}

/*
* 2.向类中添加方法或属性
* @param string name    函数名
* @param function func        函数主体
* @return function
*/
Function.prototype.AddMethod = function (name, func) {
    this.prototype[name] = func;
    return this;
}

/*3.克隆
*clone:克隆方法，未实现
*cloneObject:克隆对象 o:需要克隆的对象  */
var CCloneable = Class.Create(null, {
    /*
    * 复制类
    * @return object
    */
    clone: function () {
        return null;
    },
    cloneObject: function (o) {
        if (o instanceof CCloneable) {
            // 如果继承自CCloneable则执行自身的考虑操作
            return o.clone();
        }
        var type = typeof (o);
        switch (type) {
            case 'undefined':
            case 'boolean':
            case 'string':
            case 'number': return o;
            case 'function': return o;
            case 'object':
                if (o == null) {
                    return null;
                }
                if (o instanceof Date) {
                    return new Date(o.getTime());
                }
                if (o instanceof Number) {
                    return new Number(o.valueOf());
                }
                if (o instanceof Boolean) {
                    return new Boolean(o.valueOf());
                }
                if (o instanceof Error) {
                    return new Error(o.number, o.message);
                }
                if (o instanceof Array) {
                    return o.concat([]);
                }
                if (o instanceof Object) {
                    var oo = {};
                    for (var k in o) {
                        oo[k] = o[k];
                    }
                    return oo;
                }
            default: break;
        }
        return null;
    }
});

/*4.数组操作，包括：
*1.初始化 initialize()
*2.输出数组，toArray()
*3.获得元素所处的位置，indexOf(o)
*4.获得元素在数组中最后的位置，lastIndexOf(o)
*5.添加元素,add
*6.添加多个元素,addAll
*7.移除元素，removeAt(i),i：需要移除的元素索引值
*8.移除元素，remove(o),o:需要移除的元素值
*9.判断元素是否存在，contains(o)
*10.清除链表,clear
*11.当前数组元素个数，size
*12.根据索引获得相应元素，get(i)
*13.对相应索引位置的元素进行赋值，set(i,o)
*14.复制当前数组对象并返回，clone
* */
var CArrayList = Class.Create(CCloneable, {
    /* 内置数组 */
    array: [],

    /*
    * 构造函数
    */
    initialize: function () {
        this.array = [];
    },

    /*
    * 输出数组
    * @return Array
    */
    toArray: function () {
        return this.array;
    },

    /*
    * 从前面获得对象所在数组位置
    * @param object o    要寻找的对象
    * @return int
    */
    indexOf: function (o) {
        var len = this.array.length;
        for (var i = 0; i < len; i++) {
            if (this.array[i].Key == o.Key) {
                return i;
            }
        }
        return -1;
    },

    /*
    * 从后面获得对象所在数组位置
    * @param object o 要寻找的对象
    * @rerturn int
    */
    lastIndexOf: function (o) {
        var len = this.array.length;
        for (var i = len; i >= 0; i--) {
            if (this.array[i].Key == o.Key) {
                return i;
            }
        }
        return -1;
    },

    /*
    * 添加元素
    * @param object o 被插入的对象
    */
    add: function (o) {
        var len = this.array.length;
        this.array[len] = o;
    },

    /*
    * 添加多个元素
    * @param Array a    元素数组
    */
    addAll: function (a) {
        if (a instanceof Array) {
            // 添加的元素是数组
            this.array = this.array.concat(a);
        } else if (typeof (a.toArray) == 'function'
            && ((a = a.toArray()) instanceof Array)) {
            // 添加的元素是链表
            this.array = this.array.concat(a);
        } else {
            throw new CException('参数错误', '添加链表的时候参数出错');
        }
    },

    /*
    * 移除元素
    * @param int i    索引值
    */
    removeAt: function (i) {
        var len = this.array.length;
        if (i < 0 || i >= len) {
            return null;
        }
        var o = this.array[i];
        this.array = this.array.slice(0, i).concat(this.array.slice(i + 1, len));
        return o;
    },

    /*
    * 移除元素
    * @param object o    元素
    */
    remove: function (o) {
        var i = this.indexOf(o);
        if (i == -1) {
            return this;
        }
        return this.removeAt(i);
    },

    /*
    * 验证元素是否存在
    * @return boolean
    */
    contains: function (o) {
        return this.indexOf(o) != -1;
    },

    /*
    * 清除链表
    */
    clear: function () {
        this.array.length = 0;
    },

    /*
    * 获得链表大小
    * @return int
    */
    size: function () {
        return this.array.length;
    },

    /*
    * 获得元素
    * @param int i        索引值
    * @return object
    */
    get: function (i) {
        var size = this.size();
        if (i >= 0 && i < size) {
            return this.array[i];
        } else {
            return null;
        }
    },

    /*
    * 设置元素
    * @param int i     索引值
    * @param ojbect    元素
    */
    set: function (i, o) {
        var size = this.size();
        if (i >= 0 && i < size) {
            this.array[i] = o;
        }
    },

    /*
    * 复制链表(重构)
    */
    clone: function () {
        var o = new CArrayList();
        o.addAll(this.array);
        return o;
    }
});

function getDate() {
    return (new Date()).format();
}

//时间格式化
Date.prototype.format = function (format) {
    if (!format) {
        format = "yyyy-MM-dd hh:mm:ss";
    }

    var o = {
        "M+": this.getMonth() + 1, // month
        "d+": this.getDate(), // day
        "h+": this.getHours(), // hour
        "m+": this.getMinutes(), // minute
        "s+": this.getSeconds(), // second
        "q+": Math.floor((this.getMonth() + 3) / 3), // quarter
        "S": this.getMilliseconds() // millisecond
    };

    if (/(y+)/.test(format)) {
        format = format.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    }

    for (var k in o) {
        if (new RegExp("(" + k + ")").test(format)) {
            format = format.replace(RegExp.$1, RegExp.$1.length == 1 ? o[k] : ("00" + o[k]).substr(("" + o[k]).length));
        }
    }

    return format;
};

function stopBubble(e) {
    if (e && e.stopPropagation)
        e.stopPropagation()
    else
        window.event.cancelBubble = true
} 

//// 显示快捷键列表
//function showPostKey() {
//    if (true) {
//        if ($("#pressdiv")[0].style.display == "none") {
//            $("#pressdiv").show();
//        }
//        else {
//            $("#pressdiv").hide();
//        }
//    }
//    else {
//        if ($("#pressdiv1")[0].style.display == "none") {
//            $("#pressdiv1").show();
//        }
//        else {
//            $("#pressdiv1").hide();
//        }
//    }
//}