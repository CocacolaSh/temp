Ocean = {
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
                Ocean.override(sb, o);
            };
            sbp.override = io;
            Ocean.override(sb, overrides);
            sb.extend = function (o) { Ocean.extend(sb, o); };
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
            Ocean.apply(o, defaults);
        }
        if (o && c && typeof c == 'object') {
            for (var p in c) {
                o[p] = c[p];
            }
        }
        return o;
    }
}

//表单操作相关
//if (typeof (DYCore) == "undefined") DYCore = {};
Ocean.namespace("Ocean");
(function (Ocean) {
    var forms = {};
    Ocean.Forms = forms;
    //判断列表中的复选框是否都没被选中
    forms.IsSelectChecked = function (chkbox) {
        var box = chkbox;
        elem = box.form.elements;
        var isSelect = false;
        for (i = 0; i < elem.length; i++) {
            if (isSelect) break;
            if (elem[i].type == "checkbox" && elem[i].id != box.id) {
                if (elem[i].checked) isSelect = true;
            }
        }

        if (!isSelect) {
            alert("请选择要删除的项！");
            return false;
        } else {
            if (confirm('确定要删除所选记录？'))
                return true;
            else
                return false;
        }
    };
    // 复选框全选
    forms.SelectAllCheckbox = function (chkbox) {
        var box = chkbox;
        oState = box.checked;
        elem = box.form.elements;
        for (i = 0; i < elem.length; i++) {
            if (elem[i].type == "checkbox" && elem[i].id != box.id) {
                if (elem[i].checked != oState) {
                    elem[i].click();
                }
            }
        }
    };
    //判断单选框是否有被选中
    forms.CheckRadio = function (radio) {
        var result = false;
        for (var i = 0; i < radio.length; i++) {
            if (radio[i].checked) {
                result = true;
                break;
            }
        }
        return result;
    };
    //判断复选框是否有被选中
    forms.CheckSelect = function (select) {
        var result = false;
        for (var i = 0; i < select.length; i++) {
            if (select[i].selected && select[i].value != '' && select[i].value != 0) {
                result = true;
                break;
            }
        }
        return result;
    };
    //取得表单元素
    forms.$ = function (id) {
        return document.getElementById(id);
    };
    //其他待补充
})(Ocean);