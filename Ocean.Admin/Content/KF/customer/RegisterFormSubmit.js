var RegisterPageFormSubmit = function () {
    $(document).find("form").submit(function () {
        var url = $(this).attr("action");
        var msg = $("#submitMsg").val();
        var msg2 = msg.replace(/\r\n/g, '<br />').replace(/\n/g, '<br />');
        $("#contin").append("<li class=\"dialog fr\"><p>" + msg2 + "</p></li>");
        myscroll.refresh();
        myscroll.scrollTo(0, 100, 0, true);
        $(this).ajaxSubmit({
            type: "post",
            url: url,
            dataType: "html",
            error: function (XmlHttpRequest, textStatus, errorThrown) {
                alert("发生错误,请检查[" + url.split("?")[0] + "]页面代码是否正确");
            },
            success: function (result) {
                var json = eval("(" + result + ")");
                if (json.data.code == "error") {
                    alert(json.data.model);
                } else {
                    window.ajaxCallbackFunc();
                }
            }
        })
        $("#submitMsg").val("");
        return false;
    })
}

function sendAjaxMessage(url, message, e) {
    $.ajax({
        type: "POST",
        url: url,
        data: "messageText=" + escape(message),
        success: function (result) {
            var json = eval("(" + result + ")");
            if (json.data.code == "error") {
                alert(json.data.model);
            } else {
                if (url == window.globalSendTopicUrl) {
                    alert("发送成功");
                    $('#titleMsg').val("");
                    $('#sendTitle').hide();
                }
            }
        }
    });
    if (e) {
        alert("发送成功");
    }
}

function sendAjaxGetTopic(url, topicId) {
    $.ajax({
        type: "POST",
        url: url,
        data: "topicId=" + topicId,
        success: function (result) {
            var json = eval("(" + result + ")");
            if (json.data.code == "error") {
                alert(json.data.model);
            } else {
                var model = json.data.model[0];
                var msg = "";

                if (model.PicUrl) {
                    msg = "<p style='text-align:center;'><img style='border:solid 1px #e1e1e1;height:200px;max-width:97%' src='" + model.PicUrl + "'></p>";
                }

                msg += "<p style='font-size:12px;'>" + model.GroupTopic + "</p>";

                if (model.GoodsPrice) {
                    msg += "<p style='font-size:12px;'>￥" + model.GoodsPrice + "</p>";
                }
                var topicId = "###" + model.Id;
                var clickUrl = "javascript:void(0)";
                msg += "<p>";

                if (model.ClickUrl) {
                    clickUrl = model.ClickUrl;
                    msg += "<a href='" + clickUrl + "' target='_blank'>查看详情</a>&nbsp;&nbsp;&nbsp;";
                }

                msg += "<a href='javascript:void(0);' onclick='sendAjaxMessage(\"" + window.globalUrl + "\",\"" + topicId + "\",1);'>发给聊友看看</a></p>";

                $("#contin").append("<li class=\"dialog fl\"><p>" + msg + "</p></li>");
                myscroll.refresh();
                myscroll.scrollTo(0, 300, 0, true);
            }
        }
    });
}