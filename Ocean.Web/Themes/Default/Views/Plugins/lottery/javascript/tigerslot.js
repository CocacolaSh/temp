(function (document, $) {
    'use strict';

    /**
    * Flag to determine if touch is supported
    *
    * @type {boolean}
    * @constant
    */
    var TOUCH = $.support.touch = !!(('ontouchstart' in window) || window.DocumentTouch && document instanceof DocumentTouch);

    /**
    * Event namespace
    *
    * @type {string}
    * @constant
    */
    var HELPER_NAMESPACE = '._tap';

    /**
    * Event name
    *
    * @type {string}
    * @constant
    */
    var EVENT_NAME = 'tap';

    /**
    * Max distance between touchstart and touchend to be considered a tap
    *
    * @type {number}
    * @constant
    */
    var MAX_TAP_DELTA = 40;

    /**
    * Max duration between touchstart and touchend to be considered a tap
    *
    * @type {number}
    * @constant
    */
    var MAX_TAP_TIME = 400;

    /**
    * Event variables to copy to touches
    *
    * @type {Array}
    * @constant
    */
    var EVENT_VARIABLES = 'clientX clientY screenX screenY pageX pageY'.split(' ');

    /**
    * Fetch index of value from array
    *
    * @param {Array} array
    * @param {*} value
    * @returns {number}
    * @private
    */
    var _indexOf = function (array, value) {
        var index;
        if (Array.prototype.indexOf) {
            index = array.indexOf(value);
        } else {
            index = $.inArray(value, array);
        }

        return index;
    };

    /**
    * Object for tracking current touch settings (x, y, target, canceled, etc)
    *
    * @type {object}
    * @static
    */
    var TOUCH_VALUES = {

        /**
        * target element of touchstart event
        *
        * @type {jQuery}
        */
        $el: null,

        /**
        * pageX position of touch on touchstart
        *
        * @type {number}
        */
        x: 0,

        /**
        * pageY position of touch on touchstart
        *
        * @type {number}
        */
        y: 0,

        /**
        * Number of touches currently active on touchstart
        *
        * @type {number}
        */
        count: 0,

        /**
        * Has the current tap event been canceled?
        *
        * @type {boolean}
        */
        cancel: false,

        /**
        * Start time
        *
        * @type {number}
        */
        start: 0

    };

    /**
    * Create a new event from the original event
    * Copy over EVENT_VARIABLES from the first changedTouches
    *
    * @param {string} type
    * @param {jQuery.Event} e
    * @return {jQuery.Event}
    * @private
    */
    var _createEvent = function (type, e) {
        var originalEvent = e.originalEvent;
        var event = $.Event(originalEvent);
        var touch = originalEvent.changedTouches ? originalEvent.changedTouches[0] : originalEvent;

        event.type = type;

        var i = 0;
        var length = EVENT_VARIABLES.length;

        for (; i < length; i++) {
            event[EVENT_VARIABLES[i]] = touch[EVENT_VARIABLES[i]];
        }

        return event;
    };

    /**
    * Determine if a valid tap event
    *
    * @param {jQuery.Event} e
    * @returns {boolean}
    * @private
    */
    var _isTap = function (e) {
        var originalEvent = e.originalEvent;
        var touch = e.changedTouches ? e.changedTouches[0] : originalEvent.changedTouches[0];
        var xDelta = Math.abs(touch.pageX - TOUCH_VALUES.x);
        var yDelta = Math.abs(touch.pageY - TOUCH_VALUES.y);
        var delta = Math.max(xDelta, yDelta);

        return (
            Date.now() - TOUCH_VALUES.start < MAX_TAP_TIME &&
            delta < MAX_TAP_DELTA &&
            !TOUCH_VALUES.cancel &&
            TOUCH_VALUES.count === 1 &&
            Tap.isTracking
        );
    };

    /**
    * Tap object that will track touch events and
    * trigger the tap event when necessary
    *
    * @name Tap
    * @type {object}
    */
    var Tap = {

        /**
        * Flag to determine if touch events are currently enabled
        *
        * @type {boolean}
        */
        isEnabled: false,

        /**
        * Are we currently tracking a tap event?
        *
        * @type {boolean}
        */
        isTracking: false,

        /**
        * Enable touch event listeners
        *
        * @return {Tap}
        */
        enable: function () {
            if (this.isEnabled) {
                return this;
            }

            this.isEnabled = true;

            $(document.body)
                .on('touchstart' + HELPER_NAMESPACE, this.onTouchStart)
                .on('touchend' + HELPER_NAMESPACE, this.onTouchEnd)
                .on('touchcancel' + HELPER_NAMESPACE, this.onTouchCancel);

            return this;
        },

        /**
        * Disable touch event listeners
        *
        * @return {boolean}
        */
        disable: function () {
            if (!this.isEnabled) {
                return this;
            }

            this.isEnabled = false;

            $(document.body)
                .off('touchstart' + HELPER_NAMESPACE, this.onTouchStart)
                .off('touchend' + HELPER_NAMESPACE, this.onTouchEnd)
                .off('touchcancel' + HELPER_NAMESPACE, this.onTouchCancel);

            return this;
        },

        /**
        * Store touch start values and target
        * @param {jQuery.Event} e
        */
        onTouchStart: function (e) {
            var touches = e.originalEvent.touches;
            TOUCH_VALUES.count = touches.length;

            if (Tap.isTracking) {
                return;
            }

            Tap.isTracking = true;
            var touch = touches[0];

            TOUCH_VALUES.cancel = false;
            TOUCH_VALUES.start = Date.now();
            TOUCH_VALUES.$el = $(e.target);
            TOUCH_VALUES.x = touch.pageX;
            TOUCH_VALUES.y = touch.pageY;
        },

        /**
        * If touch has not been canceled, create a
        * tap event and trigger it on the target element
        *
        * @param {jQuery.Event} e
        */
        onTouchEnd: function (e) {
            if (_isTap(e)) {
                TOUCH_VALUES.$el.trigger(_createEvent(EVENT_NAME, e));
            }
            // Cancel tap
            Tap.onTouchCancel(e);
        },

        /**
        * Cancel tap by setting TOUCH_VALUES.cancel to true
        *
        * @param {jQuery.Event} e
        */
        onTouchCancel: function (e) {
            Tap.isTracking = false;
            TOUCH_VALUES.cancel = true;
        }

    };

    // Setup special event and enable
    // tap only if a tap event is bound
    $.event.special[EVENT_NAME] = {
        setup: function () {
            Tap.enable();
        }
    };

    // If we are not in a touch compatible browser, map tap event to the click event
    if (!TOUCH) {

        /**
        * Click event ID's that have already been converted to a tap
        *
        * @type {object}
        * @private
        */
        var _converted = [];

        /**
        * Convert click events into tap events
        *
        * @param {jQuery.Event} e
        * @private
        */
        var _onClick = function (e) {
            var originalEvent = e.originalEvent;
            if (e.isTrigger || _indexOf(_converted, originalEvent) >= 0) {
                return;
            }

            // limit size of _converted array
            if (_converted.length > 3) {
                _converted.splice(0, _converted.length - 3);
            }

            _converted.push(originalEvent);

            var event = _createEvent(EVENT_NAME, e);
            $(e.target).trigger(event);
        };

        // Bind click events that will be converted to a tap event
        //
        // Would have liked to use the bindType and delegateType properties
        // to map the tap event to click events, but this does not allow us to prevent the
        // tap event from triggering when a click event is manually triggered via .trigger().
        // Tap should only trigger if the user physically clicks.
        $.event.special[EVENT_NAME] = {
            setup: function () {
                $(this).on('click' + HELPER_NAMESPACE, _onClick);
            },
            teardown: function () {
                $(this).off('click' + HELPER_NAMESPACE, _onClick);
            }
        };
    }

} (document, jQuery));

/// <reference path="jquery-1.10.0.js" />
/// <reference path="jquery.mobile-1.3.1.js" />
(function () {
    var lastTime = 0;
    var vendors = ['ms', 'moz', 'webkit', 'o'];
    for (var x = 0; x < vendors.length && !window.requestAnimationFrame; ++x) {
        window.requestAnimationFrame = window[vendors[x] + 'RequestAnimationFrame'];
        window.cancelAnimationFrame = window[vendors[x] + 'CancelAnimationFrame']
                                   || window[vendors[x] + 'CancelRequestAnimationFrame'];
    }

    if (!window.requestAnimationFrame)
        window.requestAnimationFrame = function (callback, element) {
            var currTime = new Date().getTime();
            var timeToCall = Math.max(0, 16 - (currTime - lastTime));
            var id = window.setTimeout(function () { callback(currTime + timeToCall); },
              timeToCall);
            lastTime = currTime + timeToCall;
            return id;
        };

    if (!window.cancelAnimationFrame)
        window.cancelAnimationFrame = function (id) {
            clearTimeout(id);
        };
}());

(function () {
    window.GameTimer = function (fn, timeout) {
        this.__fn = fn;
        this.__timeout = timeout;
        this.__running = false;
        this.__lastTime = Date.now();
        this.__stopcallback = null;
    };

    window.GameTimer.prototype.__runer = function () {
        if (Date.now() - this.__lastTime >= this.__timeout) {
            this.__lastTime = Date.now();
            this.__fn.call(this);
        }
        if (this.__running) {
            window.requestAnimationFrame(this.__runer.bind(this));
        }
        else {
            if (typeof this.__stopcallback === 'function') {
                window.setTimeout(this.__stopcallback,100);
            }
        }
    };

    window.GameTimer.prototype.start = function () {
        this.__running = true;
        this.__runer();
    };
    window.GameTimer.prototype.stop = function (callback) {
        this.__running = false;
        this.__stopcallback = callback;
    };

})();

$(function () {

    var url_rndprize = '/plugins/lottery'; //'抽奖的地址';
    var url_getprize = '兑奖的地址';
    var itemPositions = [
        0, //奖项一
        100, //奖项二
        200, //奖项三
        300, //奖项四
        400, //奖项五
        500, //奖项六
        600, //奖项七
        700, //奖项八
        800, //奖项九
        900,
        1000,
        1100
    ];

    //游戏开始
    var gameStart = function (pluginId) {
        $(".zj_result").hide();
        $("#boxcontent").hide();
        lightFlicker.stop();
        lightRandom.stop();
        lightCycle.start();

        //游戏开始，指定用户的结果，从左到右，水果编码，从0开始。
        //随机给个用于测试
        //boxCycle.start(Math.round(Math.random() * 8), Math.round(Math.random() * 8), Math.round(Math.random() * 8));


        //先后台抽奖，生成获奖纪录，然后调用老虎机
        var marketing_id = $('.tigerslot').attr('data-marketing-id');
        var token = $('.tigerslot').attr('data-token');
        $.ajax({
            type: "POST",
            url: url_rndprize + "?Id=" + pluginId + "&_=" + new Date().getTime(),
            dataType: "json",
            data: {marketing_id: marketing_id,token: token},
            success: function (result) {
                if (result.message) {
                    alert(result.message);
                    return;
                }
                boxCycle.start(result);
            },
            error:function(){
                alert('抽奖插件异常，请联系客服！');
            }
        });
    };

    //游戏结束
    var gameOver = function (resultData) {
        lightFlicker.start();
        lightRandom.stop();
        lightCycle.stop();

        //alert('你获得的水果编码从左到右：' + left + ',' + middle + ',' + right);
        if (!resultData.Has_Gift) {
            $(".zj_result .result").html("很抱歉，您" + resultData.Name + ",继续加油!");
            $(".zj_result").show();
            $('.machine .gamebutton').removeClass('disabled');
        } else {
            if (resultData.Leavings_Quantity > 0) {
                $(".zj_result .result").html("恭喜您，中了“<span>" + resultData.Name + "</span>”,请填写在下面领奖信息！");
                $(".zj_result").show();
                $("#resultId").val(resultData.ResultID);
                $("#boxcontent").show();
            }
            else {
                $(".zj_result .result").html("恭喜您，中了“<span>" + resultData.Name + "</span>”,但是您的运气来得太晚了些，奖品已被拿光！");
                $(".zj_result").show();
            }
            $('.machine .gamebutton').removeClass('disabled');
            //$("#sncode").text(resultData.sn);
            //$("#prize").text(resultData.prize);
            //$("#result").slideDown(500);
        }
        var rest_chance = parseInt($('#rest_chance').text()) - 1;
        rest_chance = rest_chance < 0 ? 0 : rest_chance;
        $('#rest_chance').text(rest_chance);
    };

    //    //准备兑奖
    //    var getprize = function (listid, prizeid, code) {
    //        var tel = prompt('获奖纪录id:' + listid + ' ,奖品ID:' + prizeid + ' ,兑奖编码：' + code + '\n请输入手机号码兑奖：');
    //        if ($.trim(tel)) {
    //            /*
    //            $.post(url_getprize, {
    //            listid: listid, prizeid: prizeid, code: code
    //            }, function (result) {
    //            //操作成功,
    //            //setPrizeList(listid);
    //            });
    //            */
    //            setPrizeList(listid); //这句是为了演示，应该删除，放到操作成功的回调里
    //        }
    //        else {
    //            return false;
    //        }
    //    };

    //    //兑奖成功后更改兑奖纪录,用于提交兑奖后，更改获奖纪录的显示
    //    var setPrizeList = function (listid) {
    //        console.log($prizelist);
    //        var p = $prizelist.find('li[prizelist_id="' + listid + '"]');
    //        p.addClass('hasGetPrize');
    //    };

    var $machine = $('.machine');
    var $slotBox = $('.tigerslot .box');
    var light_html = '';
    for (var i = 0; i < 21; i++) {
        light_html += '<div class="light l' + i + '"></div>';
    }
    var $lights = $(light_html).appendTo($machine);
    var $result = $('#result').on('click', '.close-btn', function () {
        $result.slideUp();

        var submitData = {
            marketing_id: $('.tigerslot').attr('data-marketing-id'),
            sn: $.trim($("#sncode").text())
        };
        $.post('/marketing_fruit/cancel',
        		submitData,
        		function (data) {
        		    if (data.error == 1) {
        		        alert(data.msg);
        		        return;
        		    }
        		    if (data.success == 1) {
        		        //window.location.reload();
        		        $('#result #prize').empty();
        		        $('#result #sncode').empty();
        		        $('.machine .gamebutton').removeClass('disabled');
        		        return;
        		    } else {

        		    }
        		});
    });
    var $request_reward = $('#request-reward').on('click', '.close-btn', function () {
        $request_reward.slideUp();
    })

    var $gameButton = $('.machine .gamebutton').on("tap", function () {
        var $this = $(this);
        var pluginid = $this.attr("pluginId");
        if (!$this.hasClass('disabled')) {
            $this.addClass('disabled');
            $this.toggleClass(function (index, classname) {
                if (classname.indexOf('stop') > -1) {
                    boxCycle.stop(function (resultData) {
                        gameOver(resultData);
                        //$this.removeClass('disabled');
                    });
                } else {
                    gameStart(pluginid);
                    window.setTimeout(function () {
                        $this.removeClass('disabled');
                    }, 1500);
                }
                return 'stop';
            });
        }
    });

    var $prizelist = $('.part.prizelist').on('tap', '.getprize', function () {
        var $this = $(this), $parent = $this.parent();
        var code = $parent.find('.code').html();
        $('#sn').val(code);
        $("#request-reward").slideToggle(500);
        //        var prizeid = $parent.parent().attr('prize_id');
        //        var prizelist_id = $parent.parent().attr('prizelist_id');
        //        if (code) {
        //            window.setTimeout(getprize.bind(this, prizelist_id, prizeid, code),100);
        //        }
        return false;
    });

    //提交手机号码
    $('.part').on('tap', '#submit-btn', function () {
        var tel = $("#tel").val();
        var telreg = '/^1[3|4|5|8][0-9]\d{4,8}$/';
        if (tel == '') {
            alert("请输入手机号");
            return
        }

        if (!istel(tel)) {
            alert("手机号格式不正确");
            return
        }

        var submitData = {
            marketing_id: $('.tigerslot').attr('data-marketing-id'),
            sn: $("#sncode").text(),
            telephone: tel
        };
        $.post('/marketing_fruit/submit', submitData,
        		function (data) {
        		    if (data.error == 1) {
        		        alert(data.msg);
        		        return;
        		    }
        		    if (data.success == 1) {
        		        alert('恭喜您，提交成功!');
        		        setTimeout(function () {
        		            window.location.reload();
        		        }, 2000);
        		        return;
        		    }
        		})
        return false;
    });

    //提交验证码    
    $('.part').on('tap', '#ver-btn', function () {
        var ver_code = $("#ver-code").val();
        var sn = $('#sn').val();
        if (ver_code == '') {
            alert("请输入验证码");
            return;
        }

        var submitData = {
            marketing_id: $('.tigerslot').attr('data-marketing-id'),
            ver_code: ver_code,
            sn: sn
        };
        $.post('/marketing_fruit/verfication', submitData,
        		function (data) {
        		    if (data.error == 1) {
        		        alert(data.msg);
        		    } else if (data.success == 1) {
        		        alert('恭喜您，验证成功!');
        		        setTimeout(function () {
        		            window.location.reload();
        		        }, 2000);
        		    } else {
        		        alert('验证码无效，请重新输入!');
        		    }
        		}
        )
    });

    var lightCycle = new function () {
        var currIndex = 0, maxIndex = $lights.length - 1;
        $('.l0').addClass('on');
        var tmr = new GameTimer(function () {
            $lights.each(function () {
                var $this = $(this);
                if ($this.hasClass('on')) {
                    currIndex++;
                    if (currIndex > maxIndex) {
                        currIndex = 0;
                    }
                    $this.removeClass('on');
                    $('.l' + currIndex).addClass('on');
                    return false;
                }
            });
        }, 100);
        this.start = function () {
            tmr.start();
        };
        this.stop = function () {
            tmr.stop();
        };
    };
    var lightRandom = new function () {
        var tmr = new GameTimer(function () {
            $lights.each(function () {
                var r = Math.random() * 1000;
                if (r < 400) {
                    $(this).addClass('on');
                } else {
                    $(this).removeClass('on');
                }
            });
        }, 100);
        this.start = function () {
            tmr.start();
        };
        this.stop = function () {
            tmr.stop();
        };
    };

    var lightFlicker = new function () {
        $lights.each(function (index) {
            if ((index >> 1) == index / 2) {
                $(this).addClass('on');
            } else {
                $(this).removeClass('on');
            }
        });
        var tmr = new GameTimer(function () {
            $lights.toggleClass('on');
        }, 100);
        this.start = function () {
            tmr.start();
        };
        this.stop = function () {
            tmr.stop();
        };
    };


    var boxCycle = new function () {

        var speed_left = 0, speed_middle = 0, speed_right = 0, maxSpeed = 25;
        var running = false, toStop = false, toStopCount = 0;
        var boxPos_left = 0, boxPos_middle = 0, boxPos_right = 0;
        var toLeftIndex = 0, toMiddleIndex = 0, toRightIndex = 0;
        var resultData;

        var $box = $('.tigerslot .box'), $box_left = $('.tigerslot .strip.left .box'), $box_middle = $('.tigerslot .strip.middle .box'), $box_right = $('.tigerslot .strip.right .box');

        var fn_stop_callback = null;

        var tmr = new GameTimer(function () {
            if (toStop) {
                toStopCount--;
                speed_left = 0;
                boxPos_left = -itemPositions[toLeftIndex];
                if (toStopCount < 25) {
                    speed_middle = 0;
                    boxPos_middle = -itemPositions[toMiddleIndex];
                }
                if (toStopCount < 0) {
                    speed_right = 0;
                    boxPos_right = -itemPositions[toRightIndex];
                }


            } else {
                speed_left += 1;
                speed_middle += 1;
                speed_right += 1;
                if (speed_left > maxSpeed) {
                    speed_left = maxSpeed;
                }
                if (speed_middle > maxSpeed) {
                    speed_middle = maxSpeed;
                }
                if (speed_right > maxSpeed) {
                    speed_right = maxSpeed;
                }
            }

            boxPos_left += speed_left;
            boxPos_middle += speed_middle;
            boxPos_right += speed_right;

            $box_left.css('background-position', '0 ' + boxPos_left + 'px')
            $box_middle.css('background-position', '0 ' + boxPos_middle + 'px')
            $box_right.css('background-position', '0 ' + boxPos_right + 'px')

            if (speed_left == 0 && speed_middle == 0 && speed_right == 0) {
                tmr.stop(fn_stop_callback.bind(this, resultData));
            }

        }, 33);

        this.start = function (data) {
            var toIndex = data.Rate_Array.split(',');
            toLeftIndex = parseInt(toIndex[0], 10); toMiddleIndex = parseInt(toIndex[1], 10); toRightIndex = parseInt(toIndex[2], 10);
            running = true; toStop = false;
            resultData = data;
            tmr.start();
        };

        this.stop = function (fn) {
            fn_stop_callback = fn;
            toStop = true;
            toStopCount = 50;
        };


        this.reset = function () {
            $box_left.css('background-position', '0 ' + itemPositions[0] + 'px');
            $box_middle.css('background-position', '0 ' + itemPositions[0] + 'px');
            $box_right.css('background-position', '0 ' + itemPositions[0] + 'px');
        };
        this.reset();
    };

    //顶部滚动中奖信息
    AutoScrollHeader = (function (obj) {
        $(obj).find("ul:first").animate({
            marginTop: "-15px"
        }, 500, function () {
            $(this).css({ marginTop: "0px" }).find("li:first").appendTo(this);
        });
    });
    if ($('.scroll-reward-info li').length > 1) {
        setInterval('AutoScroll(".scroll-reward-info")', 4000);
    }

    //手机号码格式判断
    function istel(value) {
        //var regxEny = /^1[3|4|5|8][0-9]\d{4,8}$/;
        var regxEny = /^[0-9]*$/;
        return regxEny.test(value);
    }

    //初始给点欢乐
    lightFlicker.start();
    window.setTimeout(function () {
        lightFlicker.stop();
    }, 2000)

});