/**
* author Ajso
* date 2014.3.9
* 通信
* func:通信回调函数
* obj:参数对象
*/
(function (wnd, $, communicationFunc, communicationAddress) {
    var communication = function () {
        $.ajax({
            type: "GET",
            url: communicationAddress + "&rand=" + Math.random().toString().replace(".", ""),
            //dataType: "json",
            timeout: 60000,
            error: function (data, textSataus, error) {
                if (!wnd.globalLock) {
                    communication()
                }
            }, //超时重新 Comet
            success: function (msg) {
                if (!msg) {
                    communication();
                    return void (0);
                }
                var json = eval("(" + msg + ")");
                if (json.dataMessage || json.dataNotice) {
                    communicationFunc(json.dataMessage, json.dataNotice);
                }
                if (!wnd.globalLock) {
                    communication()
                }
            }
        });
    };
    if (!wnd.globalLock) {
        communication()
    }
})(window, $, communicationFunc, communicationAddress);