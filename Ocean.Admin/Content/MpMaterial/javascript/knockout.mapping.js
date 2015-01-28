﻿/// Knockout Mapping plugin v2.4.1
/// (c) 2013 Steven Sanderson, Roy Jacobs - http://knockoutjs.com/
/// License: MIT (http://www.opensource.org/licenses/mit-license.php)
(function (e) { "function" === typeof require && "object" === typeof exports && "object" === typeof module ? e(require("knockout"), exports) : "function" === typeof define && define.amd ? define(["knockout", "exports"], e) : e(ko, ko.mapping = {}) })(function (e, f) {
    function y(b, c) {
        var a, d; for (d in c) if (c.hasOwnProperty(d) && c[d]) if (a = f.getType(b[d]), d && b[d] && "array" !== a && "string" !== a) y(b[d], c[d]); else if ("array" === f.getType(b[d]) && "array" === f.getType(c[d])) {
            a = b; for (var e = d, l = b[d], n = c[d], t = {}, g = l.length - 1; 0 <= g; --g) t[l[g]] = l[g]; for (g =
            n.length - 1; 0 <= g; --g) t[n[g]] = n[g]; l = []; n = void 0; for (n in t) l.push(t[n]); a[e] = l
        } else b[d] = c[d]
    } function E(b, c) { var a = {}; y(a, b); y(a, c); return a } function z(b, c) {
        for (var a = E({}, b), e = L.length - 1; 0 <= e; e--) { var f = L[e]; a[f] && (a[""] instanceof Object || (a[""] = {}), a[""][f] = a[f], delete a[f]) } c && (a.ignore = h(c.ignore, a.ignore), a.include = h(c.include, a.include), a.copy = h(c.copy, a.copy), a.observe = h(c.observe, a.observe)); a.ignore = h(a.ignore, j.ignore); a.include = h(a.include, j.include); a.copy = h(a.copy, j.copy); a.observe = h(a.observe,
        j.observe); a.mappedProperties = a.mappedProperties || {}; a.copiedProperties = a.copiedProperties || {}; return a
    } function h(b, c) { "array" !== f.getType(b) && (b = "undefined" === f.getType(b) ? [] : [b]); "array" !== f.getType(c) && (c = "undefined" === f.getType(c) ? [] : [c]); return e.utils.arrayGetDistinctValues(b.concat(c)) } function F(b, c, a, d, k, l, n) {
        var t = "array" === f.getType(e.utils.unwrapObservable(c)); l = l || ""; if (f.isMapped(b)) { var g = e.utils.unwrapObservable(b)[p]; a = E(g, a) } var j = n || k, h = function () {
            return a[d] && a[d].create instanceof
            Function
        }, x = function (b) {
            var f = G, g = e.dependentObservable; e.dependentObservable = function (a, b, c) { c = c || {}; a && "object" == typeof a && (c = a); var d = c.deferEvaluation, M = !1; c.deferEvaluation = !0; a = new H(a, b, c); if (!d) { var g = a, d = e.dependentObservable; e.dependentObservable = H; a = e.isWriteableObservable(g); e.dependentObservable = d; d = H({ read: function () { M || (e.utils.arrayRemoveItem(f, g), M = !0); return g.apply(g, arguments) }, write: a && function (a) { return g(a) }, deferEvaluation: !0 }); d.__DO = g; a = d; f.push(a) } return a }; e.dependentObservable.fn =
            H.fn; e.computed = e.dependentObservable; b = e.utils.unwrapObservable(k) instanceof Array ? a[d].create({ data: b || c, parent: j, skip: N }) : a[d].create({ data: b || c, parent: j }); e.dependentObservable = g; e.computed = e.dependentObservable; return b
        }, u = function () { return a[d] && a[d].update instanceof Function }, v = function (b, f) { var g = { data: f || c, parent: j, target: e.utils.unwrapObservable(b) }; e.isWriteableObservable(b) && (g.observable = b); return a[d].update(g) }; if (n = I.get(c)) return n; d = d || ""; if (t) {
            var t = [], s = !1, m = function (a) { return a };
            a[d] && a[d].key && (m = a[d].key, s = !0); e.isObservable(b) || (b = e.observableArray([]), b.mappedRemove = function (a) { var c = "function" == typeof a ? a : function (b) { return b === m(a) }; return b.remove(function (a) { return c(m(a)) }) }, b.mappedRemoveAll = function (a) { var c = C(a, m); return b.remove(function (a) { return -1 != e.utils.arrayIndexOf(c, m(a)) }) }, b.mappedDestroy = function (a) { var c = "function" == typeof a ? a : function (b) { return b === m(a) }; return b.destroy(function (a) { return c(m(a)) }) }, b.mappedDestroyAll = function (a) {
                var c = C(a, m); return b.destroy(function (a) {
                    return -1 !=
                    e.utils.arrayIndexOf(c, m(a))
                })
            }, b.mappedIndexOf = function (a) { var c = C(b(), m); a = m(a); return e.utils.arrayIndexOf(c, a) }, b.mappedGet = function (a) { return b()[b.mappedIndexOf(a)] }, b.mappedCreate = function (a) { if (-1 !== b.mappedIndexOf(a)) throw Error("There already is an object with the key that you specified."); var c = h() ? x(a) : a; u() && (a = v(c, a), e.isWriteableObservable(c) ? c(a) : c = a); b.push(c); return c }); n = C(e.utils.unwrapObservable(b), m).sort(); g = C(c, m); s && g.sort(); s = e.utils.compareArrays(n, g); n = {}; var J, A = e.utils.unwrapObservable(c),
            y = {}, z = !0, g = 0; for (J = A.length; g < J; g++) { var r = m(A[g]); if (void 0 === r || r instanceof Object) { z = !1; break } y[r] = A[g] } var A = [], B = 0, g = 0; for (J = s.length; g < J; g++) {
                var r = s[g], q, w = l + "[" + g + "]"; switch (r.status) {
                    case "added": var D = z ? y[r.value] : K(e.utils.unwrapObservable(c), r.value, m); q = F(void 0, D, a, d, b, w, k); h() || (q = e.utils.unwrapObservable(q)); w = O(e.utils.unwrapObservable(c), D, n); q === N ? B++ : A[w - B] = q; n[w] = !0; break; case "retained": D = z ? y[r.value] : K(e.utils.unwrapObservable(c), r.value, m); q = K(b, r.value, m); F(q, D, a, d, b, w,
                    k); w = O(e.utils.unwrapObservable(c), D, n); A[w] = q; n[w] = !0; break; case "deleted": q = K(b, r.value, m)
                } t.push({ event: r.status, item: q })
            } b(A); a[d] && a[d].arrayChanged && e.utils.arrayForEach(t, function (b) { a[d].arrayChanged(b.event, b.item) })
        } else if (P(c)) {
            b = e.utils.unwrapObservable(b); if (!b) { if (h()) return s = x(), u() && (s = v(s)), s; if (u()) return v(s); b = {} } u() && (b = v(b)); I.save(c, b); if (u()) return b; Q(c, function (d) {
                var f = l.length ? l + "." + d : d; if (-1 == e.utils.arrayIndexOf(a.ignore, f)) if (-1 != e.utils.arrayIndexOf(a.copy, f)) b[d] =
                c[d]; else if ("object" != typeof c[d] && "array" != typeof c[d] && 0 < a.observe.length && -1 == e.utils.arrayIndexOf(a.observe, f)) b[d] = c[d], a.copiedProperties[f] = !0; else { var g = I.get(c[d]), k = F(b[d], c[d], a, d, b, f, b), g = g || k; if (0 < a.observe.length && -1 == e.utils.arrayIndexOf(a.observe, f)) b[d] = g(), a.copiedProperties[f] = !0; else { if (e.isWriteableObservable(b[d])) { if (g = e.utils.unwrapObservable(g), b[d]() !== g) b[d](g) } else g = void 0 === b[d] ? g : e.utils.unwrapObservable(g), b[d] = g; a.mappedProperties[f] = !0 } }
            })
        } else switch (f.getType(c)) {
            case "function": u() ?
            e.isWriteableObservable(c) ? (c(v(c)), b = c) : b = v(c) : b = c; break; default: if (e.isWriteableObservable(b)) return q = u() ? v(b) : e.utils.unwrapObservable(c), b(q), q; h() || u(); b = h() ? x() : e.observable(e.utils.unwrapObservable(c)); u() && b(v(b))
        } return b
    } function O(b, c, a) { for (var d = 0, e = b.length; d < e; d++) if (!0 !== a[d] && b[d] === c) return d; return null } function R(b, c) { var a; c && (a = c(b)); "undefined" === f.getType(a) && (a = b); return e.utils.unwrapObservable(a) } function K(b, c, a) {
        b = e.utils.unwrapObservable(b); for (var d = 0, f = b.length; d <
        f; d++) { var l = b[d]; if (R(l, a) === c) return l } throw Error("When calling ko.update*, the key '" + c + "' was not found!");
    } function C(b, c) { return e.utils.arrayMap(e.utils.unwrapObservable(b), function (a) { return c ? R(a, c) : a }) } function Q(b, c) { if ("array" === f.getType(b)) for (var a = 0; a < b.length; a++) c(a); else for (a in b) c(a) } function P(b) { var c = f.getType(b); return ("object" === c || "array" === c) && null !== b } function T() {
        var b = [], c = []; this.save = function (a, d) { var f = e.utils.arrayIndexOf(b, a); 0 <= f ? c[f] = d : (b.push(a), c.push(d)) };
        this.get = function (a) { a = e.utils.arrayIndexOf(b, a); return 0 <= a ? c[a] : void 0 }
    } function S() { var b = {}, c = function (a) { var c; try { c = a } catch (e) { c = "$$$" } a = b[c]; void 0 === a && (a = new T, b[c] = a); return a }; this.save = function (a, b) { c(a).save(a, b) }; this.get = function (a) { return c(a).get(a) } } var p = "__ko_mapping__", H = e.dependentObservable, B = 0, G, I, L = ["create", "update", "key", "arrayChanged"], N = {}, x = { include: ["_destroy"], ignore: [], copy: [], observe: [] }, j = x; f.isMapped = function (b) { return (b = e.utils.unwrapObservable(b)) && b[p] }; f.fromJS =
    function (b) { if (0 == arguments.length) throw Error("When calling ko.fromJS, pass the object you want to convert."); try { B++ || (G = [], I = new S); var c, a; 2 == arguments.length && (arguments[1][p] ? a = arguments[1] : c = arguments[1]); 3 == arguments.length && (c = arguments[1], a = arguments[2]); a && (c = E(c, a[p])); c = z(c); var d = F(a, b, c); a && (d = a); if (!--B) for (; G.length;) { var e = G.pop(); e && (e(), e.__DO.throttleEvaluation = e.throttleEvaluation) } d[p] = E(d[p], c); return d } catch (f) { throw B = 0, f; } }; f.fromJSON = function (b) {
        var c = e.utils.parseJson(b);
        arguments[0] = c; return f.fromJS.apply(this, arguments)
    }; f.updateFromJS = function () { throw Error("ko.mapping.updateFromJS, use ko.mapping.fromJS instead. Please note that the order of parameters is different!"); }; f.updateFromJSON = function () { throw Error("ko.mapping.updateFromJSON, use ko.mapping.fromJSON instead. Please note that the order of parameters is different!"); }; f.toJS = function (b, c) {
        j || f.resetDefaultOptions(); if (0 == arguments.length) throw Error("When calling ko.mapping.toJS, pass the object you want to convert.");
        if ("array" !== f.getType(j.ignore)) throw Error("ko.mapping.defaultOptions().ignore should be an array."); if ("array" !== f.getType(j.include)) throw Error("ko.mapping.defaultOptions().include should be an array."); if ("array" !== f.getType(j.copy)) throw Error("ko.mapping.defaultOptions().copy should be an array."); c = z(c, b[p]); return f.visitModel(b, function (a) { return e.utils.unwrapObservable(a) }, c)
    }; f.toJSON = function (b, c) { var a = f.toJS(b, c); return e.utils.stringifyJson(a) }; f.defaultOptions = function () {
        if (0 < arguments.length) j =
        arguments[0]; else return j
    }; f.resetDefaultOptions = function () { j = { include: x.include.slice(0), ignore: x.ignore.slice(0), copy: x.copy.slice(0) } }; f.getType = function (b) { if (b && "object" === typeof b) { if (b.constructor === Date) return "date"; if (b.constructor === Array) return "array" } return typeof b }; f.visitModel = function (b, c, a) {
        a = a || {}; a.visitedObjects = a.visitedObjects || new S; var d, k = e.utils.unwrapObservable(b); if (P(k)) a = z(a, k[p]), c(b, a.parentName), d = "array" === f.getType(k) ? [] : {}; else return c(b, a.parentName); a.visitedObjects.save(b,
        d); var l = a.parentName; Q(k, function (b) {
            if (!(a.ignore && -1 != e.utils.arrayIndexOf(a.ignore, b))) {
                var j = k[b], g = a, h = l || ""; "array" === f.getType(k) ? l && (h += "[" + b + "]") : (l && (h += "."), h += b); g.parentName = h; if (!(-1 === e.utils.arrayIndexOf(a.copy, b) && -1 === e.utils.arrayIndexOf(a.include, b) && k[p] && k[p].mappedProperties && !k[p].mappedProperties[b] && k[p].copiedProperties && !k[p].copiedProperties[b] && "array" !== f.getType(k))) switch (f.getType(e.utils.unwrapObservable(j))) {
                    case "object": case "array": case "undefined": g = a.visitedObjects.get(j);
                        d[b] = "undefined" !== f.getType(g) ? g : f.visitModel(j, c, a); break; default: d[b] = c(j, a.parentName)
                }
            }
        }); return d
    }
});
Komapping = { EncodeType: "entity", isEmpty: function (val) { if (val) { return ((val === null) || val.length == 0 || /^\s+$/.test(val)) } else { return true } }, arr1: ['&nbsp;', '&iexcl;', '&cent;', '&pound;', '&curren;', '&yen;', '&brvbar;', '&sect;', '&uml;', '&copy;', '&ordf;', '&laquo;', '&not;', '&shy;', '&reg;', '&macr;', '&deg;', '&plusmn;', '&sup2;', '&sup3;', '&acute;', '&micro;', '&para;', '&middot;', '&cedil;', '&sup1;', '&ordm;', '&raquo;', '&frac14;', '&frac12;', '&frac34;', '&iquest;', '&Agrave;', '&Aacute;', '&Acirc;', '&Atilde;', '&Auml;', '&Aring;', '&AElig;', '&Ccedil;', '&Egrave;', '&Eacute;', '&Ecirc;', '&Euml;', '&Igrave;', '&Iacute;', '&Icirc;', '&Iuml;', '&ETH;', '&Ntilde;', '&Ograve;', '&Oacute;', '&Ocirc;', '&Otilde;', '&Ouml;', '&times;', '&Oslash;', '&Ugrave;', '&Uacute;', '&Ucirc;', '&Uuml;', '&Yacute;', '&THORN;', '&szlig;', '&agrave;', '&aacute;', '&acirc;', '&atilde;', '&auml;', '&aring;', '&aelig;', '&ccedil;', '&egrave;', '&eacute;', '&ecirc;', '&euml;', '&igrave;', '&iacute;', '&icirc;', '&iuml;', '&eth;', '&ntilde;', '&ograve;', '&oacute;', '&ocirc;', '&otilde;', '&ouml;', '&divide;', '&oslash;', '&ugrave;', '&uacute;', '&ucirc;', '&uuml;', '&yacute;', '&thorn;', '&yuml;', '&quot;', '&amp;', '&lt;', '&gt;', '&OElig;', '&oelig;', '&Scaron;', '&scaron;', '&Yuml;', '&circ;', '&tilde;', '&ensp;', '&emsp;', '&thinsp;', '&zwnj;', '&zwj;', '&lrm;', '&rlm;', '&ndash;', '&mdash;', '&lsquo;', '&rsquo;', '&sbquo;', '&ldquo;', '&rdquo;', '&bdquo;', '&dagger;', '&Dagger;', '&permil;', '&lsaquo;', '&rsaquo;', '&euro;', '&fnof;', '&Alpha;', '&Beta;', '&Gamma;', '&Delta;', '&Epsilon;', '&Zeta;', '&Eta;', '&Theta;', '&Iota;', '&Kappa;', '&Lambda;', '&Mu;', '&Nu;', '&Xi;', '&Omicron;', '&Pi;', '&Rho;', '&Sigma;', '&Tau;', '&Upsilon;', '&Phi;', '&Chi;', '&Psi;', '&Omega;', '&alpha;', '&beta;', '&gamma;', '&delta;', '&epsilon;', '&zeta;', '&eta;', '&theta;', '&iota;', '&kappa;', '&lambda;', '&mu;', '&nu;', '&xi;', '&omicron;', '&pi;', '&rho;', '&sigmaf;', '&sigma;', '&tau;', '&upsilon;', '&phi;', '&chi;', '&psi;', '&omega;', '&thetasym;', '&upsih;', '&piv;', '&bull;', '&hellip;', '&prime;', '&Prime;', '&oline;', '&frasl;', '&weierp;', '&image;', '&real;', '&trade;', '&alefsym;', '&larr;', '&uarr;', '&rarr;', '&darr;', '&harr;', '&crarr;', '&lArr;', '&uArr;', '&rArr;', '&dArr;', '&hArr;', '&forall;', '&part;', '&exist;', '&empty;', '&nabla;', '&isin;', '&notin;', '&ni;', '&prod;', '&sum;', '&minus;', '&lowast;', '&radic;', '&prop;', '&infin;', '&ang;', '&and;', '&or;', '&cap;', '&cup;', '&int;', '&there4;', '&sim;', '&cong;', '&asymp;', '&ne;', '&equiv;', '&le;', '&ge;', '&sub;', '&sup;', '&nsub;', '&sube;', '&supe;', '&oplus;', '&otimes;', '&perp;', '&sdot;', '&lceil;', '&rceil;', '&lfloor;', '&rfloor;', '&lang;', '&rang;', '&loz;', '&spades;', '&clubs;', '&hearts;', '&diams;'], arr2: ['&#160;', '&#161;', '&#162;', '&#163;', '&#164;', '&#165;', '&#166;', '&#167;', '&#168;', '&#169;', '&#170;', '&#171;', '&#172;', '&#173;', '&#174;', '&#175;', '&#176;', '&#177;', '&#178;', '&#179;', '&#180;', '&#181;', '&#182;', '&#183;', '&#184;', '&#185;', '&#186;', '&#187;', '&#188;', '&#189;', '&#190;', '&#191;', '&#192;', '&#193;', '&#194;', '&#195;', '&#196;', '&#197;', '&#198;', '&#199;', '&#200;', '&#201;', '&#202;', '&#203;', '&#204;', '&#205;', '&#206;', '&#207;', '&#208;', '&#209;', '&#210;', '&#211;', '&#212;', '&#213;', '&#214;', '&#215;', '&#216;', '&#217;', '&#218;', '&#219;', '&#220;', '&#221;', '&#222;', '&#223;', '&#224;', '&#225;', '&#226;', '&#227;', '&#228;', '&#229;', '&#230;', '&#231;', '&#232;', '&#233;', '&#234;', '&#235;', '&#236;', '&#237;', '&#238;', '&#239;', '&#240;', '&#241;', '&#242;', '&#243;', '&#244;', '&#245;', '&#246;', '&#247;', '&#248;', '&#249;', '&#250;', '&#251;', '&#252;', '&#253;', '&#254;', '&#255;', '&#34;', '&#38;', '&#60;', '&#62;', '&#338;', '&#339;', '&#352;', '&#353;', '&#376;', '&#710;', '&#732;', '&#8194;', '&#8195;', '&#8201;', '&#8204;', '&#8205;', '&#8206;', '&#8207;', '&#8211;', '&#8212;', '&#8216;', '&#8217;', '&#8218;', '&#8220;', '&#8221;', '&#8222;', '&#8224;', '&#8225;', '&#8240;', '&#8249;', '&#8250;', '&#8364;', '&#402;', '&#913;', '&#914;', '&#915;', '&#916;', '&#917;', '&#918;', '&#919;', '&#920;', '&#921;', '&#922;', '&#923;', '&#924;', '&#925;', '&#926;', '&#927;', '&#928;', '&#929;', '&#931;', '&#932;', '&#933;', '&#934;', '&#935;', '&#936;', '&#937;', '&#945;', '&#946;', '&#947;', '&#948;', '&#949;', '&#950;', '&#951;', '&#952;', '&#953;', '&#954;', '&#955;', '&#956;', '&#957;', '&#958;', '&#959;', '&#960;', '&#961;', '&#962;', '&#963;', '&#964;', '&#965;', '&#966;', '&#967;', '&#968;', '&#969;', '&#977;', '&#978;', '&#982;', '&#8226;', '&#8230;', '&#8242;', '&#8243;', '&#8254;', '&#8260;', '&#8472;', '&#8465;', '&#8476;', '&#8482;', '&#8501;', '&#8592;', '&#8593;', '&#8594;', '&#8595;', '&#8596;', '&#8629;', '&#8656;', '&#8657;', '&#8658;', '&#8659;', '&#8660;', '&#8704;', '&#8706;', '&#8707;', '&#8709;', '&#8711;', '&#8712;', '&#8713;', '&#8715;', '&#8719;', '&#8721;', '&#8722;', '&#8727;', '&#8730;', '&#8733;', '&#8734;', '&#8736;', '&#8743;', '&#8744;', '&#8745;', '&#8746;', '&#8747;', '&#8756;', '&#8764;', '&#8773;', '&#8776;', '&#8800;', '&#8801;', '&#8804;', '&#8805;', '&#8834;', '&#8835;', '&#8836;', '&#8838;', '&#8839;', '&#8853;', '&#8855;', '&#8869;', '&#8901;', '&#8968;', '&#8969;', '&#8970;', '&#8971;', '&#9001;', '&#9002;', '&#9674;', '&#9824;', '&#9827;', '&#9829;', '&#9830;'], HTML2Numerical: function (s) { return this.swapArrayVals(s, this.arr1, this.arr2) }, NumericalToHTML: function (s) { return this.swapArrayVals(s, this.arr2, this.arr1) }, numEncode: function (s) { if (this.isEmpty(s)) return ""; var a = [], l = s.length; for (var i = 0; i < l; i++) { var c = s.charAt(i); if (c < " " || c > "~") { a.push("&#"); a.push(c.charCodeAt()); a.push(";") } else { a.push(c) } }; return a.join("") }, hmDe: function (s) { var c, m, d = s; if (this.isEmpty(d)) return ""; d = this.HTML2Numerical(d); arr = d.match(/&#[0-9]{1,5};/g); if (arr != null) { for (var x = 0; x < arr.length; x++) { m = arr[x]; c = m.substring(2, m.length - 1); if (c >= -32768 && c <= 65535) { d = d.replace(m, String.fromCharCode(c)) } else { d = d.replace(m, ""); } } }; return d }, hmEn: function (s, dbl) { if (this.isEmpty(s)) return ""; dbl = dbl || false; if (dbl) { if (this.EncodeType == "numerical") { s = s.replace(/&/g, "&#38;") } else { s = s.replace(/&/g, "&amp;") } }; s = this.XSSEncode(s, false); if (this.EncodeType == "numerical" || !dbl) { s = this.HTML2Numerical(s) }; s = this.numEncode(s); if (!dbl) { s = s.replace(/&#/g, "##AMPHASH##"); if (this.EncodeType == "numerical") { s = s.replace(/&/g, "&#38;") } else { s = s.replace(/&/g, "&amp;") }; s = s.replace(/##AMPHASH##/g, "&#") }; s = s.replace(/&#\d*([^\d;]|$)/g, "$1"); if (!dbl) { s = this.correctEncoding(s) }; if (this.EncodeType == "entity") { s = this.NumericalToHTML(s) }; return s }, XSSEncode: function (s, en) { if (!this.isEmpty(s)) { en = en || true; if (en) { s = s.replace(/\'/g, "&#39;"); s = s.replace(/\"/g, "&quot;"); s = s.replace(/</g, "&lt;"); s = s.replace(/>/g, "&gt;") } else { s = s.replace(/\'/g, "&#39;"); s = s.replace(/\"/g, "&#34;"); s = s.replace(/</g, "&#60;"); s = s.replace(/>/g, "&#62;") }; return s } else { return "" } }, hasEncoded: function (s) { if (/&#[0-9]{1,5};/g.test(s)) { return true } else if (/&[A-Z]{2,6};/gi.test(s)) { return true } else { return false } }, stripUnicode: function (s) { return s.replace(/[^\x20-\x7E]/g, "") }, correctEncoding: function (s) { return s.replace(/(&amp;)(amp;)+/, "$1") }, swapArrayVals: function (s, arr1, arr2) { if (this.isEmpty(s)) return ""; var re; if (arr1 && arr2) { if (arr1.length == arr2.length) { for (var x = 0, i = arr1.length; x < i; x++) { re = new RegExp(arr1[x], 'g'); s = s.replace(re, arr2[x]); } } }; return s }, inArray: function (item, arr) { for (var i = 0, x = arr.length; i < x; i++) { if (arr[i] === item) { return i } }; return -1 } }
