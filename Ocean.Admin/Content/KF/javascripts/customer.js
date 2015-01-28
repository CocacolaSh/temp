/**
* author Ajso
* date 2014.3.9
*/
(function (wnd, $) {
    /**
    * 全局锁，true时则通信中断
    */
    wnd.globalLock = false;

    /**
    * 客服人员对象
    */
    function Ocustomer() {
        //发送消息
        this.SendMsg = function () {
            return void (0);
        }

        //客服离线
        this.OffLine = function () {
//            $.ajax({
//                type: "POST",
//                url: "/Client/OffLine",
//                data: "",
//                success: function (msg) {

//                }
//            });
        }
    }
    /**
    * 把客服类暴露给全局
    */
    wnd.Ocustomer = Ocustomer;
    /**
    * 注册通信地址和通信回调函数
    */
    wnd.communicationAddress = "/Client/MonitoringItem?SendUserId=" + window.SendUserId;
    wnd.communicationFunc = function (dataMessage, dataNotice) {
        $.each(dataMessage, function (i, item) {
            $("#contin").append("<li class=\"dialog fl\"><p>" + item.Message + "</p></li>");
            myscroll.refresh();
            myscroll.scrollTo(0, 100, 0, true);
        });
    }
})(window, $);