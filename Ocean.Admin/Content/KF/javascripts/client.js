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
    //使用message对象封装消息  
    wnd.HtmlEncode = function(str){
        return str.replace(/&/g, '&amp;').replace(/"/g, '&quot;').replace(/</g, '&lt;').replace(/>/g, '&gt;');
        //return str.replace(/&/g, '&amp;').replace(/"/g, '&quot;').replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/\r\n/g,'<br />').replace(/\n/g,'<br />');
    }
    wnd.UrlEncode = function(str){
        return str;
        //return encodeURIComponent(str);
    }
    wnd.Message = {  
        time: 0,  
        title: document.title,  
        timer: null,  
        //显示新消息提示  
        show: function (message) {  
            var title = wnd.Message.title.replace("【　　　】", "").replace("【" + message + "】", "");  
            //定时器，设置消息切换频率闪烁效果就此产生  
            wnd.Message.timer = setTimeout(function () {  
                wnd.Message.time++;  
                wnd.Message.show(message);  
                if (wnd.Message.time % 2 == 0) {  
                    document.title = "【" + message + "】" + title  
                }  
                else {  
                    document.title = "【　　　】" + title  
                };  
            }, 600);  
            return [wnd.Message.timer, wnd.Message.title];  
        },  
        // 取消新消息提示  
        clear: function () {  
            clearTimeout(wnd.Message.timer);  
            document.title = wnd.Message.title;  
        }  
    };  
    function Client() { 
        //正在会话的Key
        this.TalkMeetingKey = "";
        //会话队列以及操作相关
        var arrMeeting = new CArrayList();
        this.MeetingOperate = function (operate, o) {
            if (operate == "insert") {
                var i = arrMeeting.indexOf(o);
                if (i == -1) {
                    arrMeeting.add(o);
                } else {
                    arrMeeting.set(i, o);
                }
            }
            if (operate == "delete") {
                arrMeeting.remove(o);
            }
            if (operate == "update") {
                var i = arrMeeting.indexOf(o);
                if (i == -1) {
                    arrMeeting.add(o);
                } else {
                    arrMeeting.set(i, o);
                }
            }
            if (operate == "get") {
                var i = arrMeeting.indexOf(o);
                if (i == -1) {
                    return null;
                } else {
                    return arrMeeting.get(i);
                }
            }
            if (operate == "getall") {
                return arrMeeting.toArray();
            }
        }
        //等待咨询队列
        var arrGuest = new CArrayList();
        this.GuestOperate = function (operate, o) {
            if (operate == "insert") {
                var i = arrGuest.indexOf(o);
                if (i == -1) {
                    arrGuest.add(o);
                } else {
                    arrGuest.set(i, o);
                }
            }
            if (operate == "delete") {
                arrGuest.remove(o);
            }
            if (operate == "update") {
                var i = arrGuest.indexOf(o);
                if (i == -1) {
                    arrGuest.add(o);
                } else {
                    arrGuest.set(i, o);
                }
            }
            if (operate == "get") {
                var i = arrGuest.indexOf(o);
                if (i == -1) {
                    return null;
                } else {
                    return arrGuest.get(i);
                }
            }
            if (operate == "getall") {
                return arrGuest.toArray();
            }
        }
        //发送消息
        this.SendMsg = function (msg) {
            try{
                var o =
                {
                    Key: Oclient.TalkMeetingKey
                }
                var o2 = Oclient.MeetingOperate("get", o);
                if (o2 == null) {
                    alert("请选择聊天对象");
                } else {
                    var msg2 = msg.replace(/\r\n/g,'<br />').replace(/\n/g,'<br />');
                    o2.Message = msg2;
                    o2.SendTimeOfService = getDate();
                    var len = o2.Text.length;
                    o2.Text[len] = { Role: 1, Message: msg2, SendTimeOfService: getDate() };
                    Oclient.MeetingOperate("update", o2);
                }
                Oclient.RenderTalkDialog();
                $.ajax({
                    type: "POST",
                    url: "/Service/SendPrivateMessage",
                    data: "ReceiveUserId=" + Oclient.TalkMeetingKey + "&messageText=" + escape(msg)+"&KfMeetingId=" + o2.MeetingId,
                    success: function (msg) {
                        var json = eval("(" + msg + ")");
                        if(json.data.code == "error"){
                            alert(json.data.model);
                        }
                    }
                });
            }catch(e){}
        }
        //客服离线
        this.OffLine = function () {

        }
        //切换访客会话
        this.ClickMeeting = function (key) {
            try{
                Oclient.TalkMeetingKey = key;
                var o =
                {
                    Key: Oclient.TalkMeetingKey
                }
                var o2 = Oclient.MeetingOperate("get", o);
                o2.Count = 0;
                Oclient.MeetingOperate("update", o2);
                //渲染相关窗体
                Oclient.RenderMeetingList();
                Oclient.RenderTalkDialog();
                //获取访客信息
                this.GetMpUserInfo(key);
                //处理摘要
                this.GetMeetingExplain(o2.MeetingId);
                $('#cyy_add').hide();
                $('#phrase_content').val($('#meetingExplain').html());
            }catch(e){}
        }
        //接入访客会话请求
        this.ReceptionKfMeeting = function (key,meetingId) {
            //接入会话
            $.ajax({
                type: "POST",
                url: "/Service/ReceptionKfMeeting",
                data: "KfMeetingId=" + meetingId,
                success: function (msg) {
                    var json = eval("(" + msg + ")");
                    if(json.data.code == "error"){
                        alert("当前会话已被对方拒绝，请执行拒绝操作");
                        return void(0);
                    }
                    if(json.data.code == "ok"){
                        try{
                            //判断是否为第一位访客的情况
                            var initCount = !Oclient.TalkMeetingKey ? 0 : 1;
                            if (!Oclient.TalkMeetingKey) {
                                Oclient.TalkMeetingKey = key;
                            }
                            var o =
                            {
                                Key: key
                            }
                            var o2 = Oclient.GuestOperate("get", o);
                            //初始化会话
                            var o3 =
                            {
                                Key: key,
                                SendNickName: o2.SendNickName,
                                Status: 1,
                                Message: "接入成功，您现在可以开始聊天了",
                                SendTimeOfService: getDate(),
                                Count: initCount,
                                MeetingId : meetingId,
                                Text: new Array({ Role: 2, Message: "接入成功，您现在可以开始聊天了", SendTimeOfService: getDate() })
                            }
                            Oclient.MeetingOperate("insert", o3);
                            //更新等待咨询
                            var o4 =
                            {
                                Key: key,
                            }
                            Oclient.GuestOperate("delete", o4);
                            //渲染相关窗体
                            Oclient.RenderMeetingList();
                            Oclient.RenderTalkDialog();
                            Oclient.RenderWaitGuestDialog();
                        }catch(e){}
                    }
                }
            });
        }
        //从数据库还原会话请求
        this.RestoreKfMeeting = function (key,meetingId,visitName) {
            try{
                //判断是否为第一位访客的情况
                var initCount = !Oclient.TalkMeetingKey ? 0 : 1;
                if (!Oclient.TalkMeetingKey) {
                    Oclient.TalkMeetingKey = key;
                }
                //初始化会话,"访客_"+ parseInt(Math.random()*(100-1+1)+1)
                var o1 =
                {
                    Key: key,
                    SendNickName: visitName,
                    Status: 1,
                    Message: "接入成功，您现在可以开始聊天了",
                    SendTimeOfService: getDate(),
                    Count: initCount,
                    MeetingId : meetingId,
                    Text: new Array({ Role: 2, Message: "接入成功，您现在可以开始聊天了", SendTimeOfService: getDate() })
                }
                Oclient.MeetingOperate("insert", o1);
                //渲染相关窗体
                Oclient.RenderMeetingList();
                Oclient.RenderTalkDialog();
            }catch(e){}
        }
        //拒绝访客会话请求
        this.RejectKfMeeting = function (key,meetingId) {
            try{
                //更新等待咨询
                if(confirm("确定要拒绝此接入请求吗?")){
                    var o =
                    {
                        Key: key,
                    }
                    Oclient.GuestOperate("delete", o);
                    //渲染相关窗体
                    Oclient.RenderWaitGuestDialog();
                    //结束会话
                    $.ajax({
                        type: "POST",
                        url: "/Service/RejectKfMeeting",
                        data: "KfMeetingId=" + meetingId,
                        success: function (msg) {

                        }
                    });
                }
            }catch(e){}
        }
        //结束访客会话
        this.EndKfMeeting = function(key,meetingId){
            try{
                if(confirm("确定要结束本次会话吗?")){
                    //删除本次会话
                    var o =
                    {
                        Key: key,
                    }
                    Oclient.MeetingOperate("delete", o);
                    //获取当前队列的第一个会话
                    var meetings = Oclient.MeetingOperate("getall");
                    if(meetings.length == 0){
                        Oclient.TalkMeetingKey = "";
                    }else{
                        var nextO = meetings[0];
                        Oclient.ClickMeeting(nextO.Key);
                    }
                    //渲染相关窗体
                    Oclient.RenderMeetingList();
                    Oclient.RenderTalkDialog();
                    stopBubble();
                    //结束会话
                    $.ajax({
                        type: "POST",
                        url: "/Service/EndKfMeeting",
                        data: "KfMeetingId=" + meetingId,
                        success: function (msg) {

                        }
                    });
                }
            }catch(e){}
        }
        //获取用户信息
        this.GetMpUserInfo = function(receiveUserId){
            try{
                $.ajax({
                        type: "POST",
                        url: "/Service/GetMpUserInfo",
                        data: "ReceiveUserId=" + receiveUserId,
                        success: function (msg) {
                            var json = eval("(" + msg + ")");
                            $("#visitor_info").empty();
                            if(json.data.model[0].IsAuth == 0){
                                //var info = "<p><img style=\"width:80px;border:solid 1px #e1e1e1;\" src=\"" + json.data.model[0].HeadImgUrl + "\"></p>";
                                var info = "<p>昵称:" + json.data.model[0].NickName + "</p>";
                                info += "<p>性别:" + (json.data.model[0].Sex == 0 ? "未知" : (json.data.model[0].Sex == 1 ? "男":"女")) + "</p>";
                                info += "<p>地区:" + json.data.model[0].Country + "&nbsp;" + json.data.model[0].Province + "&nbsp;" + json.data.model[0].City + "</p>";
                                info += "<p>关注日期:" + json.dataExt.split("|")[0] + "</p>";
                                info += "<p>最后访问日期:" + json.dataExt.split("|")[1] + "</p>";
                                $("#visitor_info").append(info);
                                $("#loan").hide();
                                $("#fnb").hide();
                            }else{
                                //var info = "<p><img style=\"width:80px;border:solid 1px #e1e1e1;\" src=\"" + json.data.model[0].HeadImgUrl + "\"></p>";
                                var info = "<p>昵称:" + json.data.model[0].NickName + "</p>";
                                info += "<p>性别:" + (json.data.model[0].Sex == 0 ? "未知" : (json.data.model[0].Sex == 1 ? "男":"女")) + "</p>";
                                info += "<p>地区:" + json.data.model[0].Country + "&nbsp;" + json.data.model[0].Province + "&nbsp;" + json.data.model[0].City + "</p>";
                                info += "<p>关注日期:" + json.dataExt.split("|")[0] + "</p>";
                                info += "<p>最后访问日期:" + json.dataExt.split("|")[1] + "</p>";
                                info += "<p>真实姓名:" + (json.data.model[0].Name == null ? "" : json.data.model[0].Name) + "</p>";
                                info += "<p>手机:" + (json.data.model[0].MobilePhone == null ? "" : json.data.model[0].MobilePhone) + "</p>";
                                info += "<p>是否福农宝客户:" + (json.dataExt.split("|")[2] == "0" ? "不是" : "是") + "</p>";
                                $("#visitor_info").append(info);
                                $("#loan").show();
                                $("#fnb").show();
                            }
                        }
                    });
                }catch(e){}
        }
        //获取摘要信息
        this.GetMeetingExplain = function(kfMeetingId){
            try{
                $.ajax({
                        type: "POST",
                        url: "/Service/GetMeetingExplain",
                        data: "kfMeetingId=" + kfMeetingId,
                        success: function (msg) {
                            var json = eval("(" + msg + ")");
                            $("#meetingExplain").empty();
                            $("#meetingExplain").append(json.data.model);
                        }
                    });
                }catch(e){}
        }
        //更新摘要信息
        this.UpdateMeetingExplain = function(){
            try{
                if(!Oclient.TalkMeetingKey){
                   alert("请先选择需要编辑的会话");
                   return void(0);
                }
                var o =
                {
                    Key: Oclient.TalkMeetingKey
                }
                var o2 = Oclient.MeetingOperate("get", o);
                $.ajax({
                        type: "POST",
                        url: "/Service/UpdateMeetingExplain",
                        data: "KfMeetingId=" + o2.MeetingId + "&Explain=" + $("#phrase_content").val(),
                        success: function (msg) {
                            var json = eval("(" + msg + ")");
                            if(json.data.code == "error"){
                                alert(json.data.model);
                            }else{
                                $("#phrase_content").val("");
                                $('#cyy_add').hide();
                                $("#meetingExplain").empty();
                                $("#meetingExplain").append(json.data.model);
                            }
                        }
                    });
                }catch(e){}
        }
        //渲染会话列表
        this.RenderMeetingList = function () {
            try{
                $("#client_content").empty();
                var meetings = Oclient.MeetingOperate("getall");
                for (var i = 0; i < meetings.length; i++) {
                    var template = "";
                    if (meetings[i].Key == Oclient.TalkMeetingKey && meetings[i].Status == 0) {
                        template = "<li style=\"cursor: pointer;background:url(/Content/KF/images/list_newbg.gif) 2px -92px no-repeat\" onclick=\"Oclient.ClickMeeting('" + meetings[i].Key + "');\"><div class=\"fklist_left\">" + meetings[i].SendNickName + "</div><div class=\"fklist_right\" onclick=\"Oclient.EndKfMeeting('" + meetings[i].Key + "','" + meetings[i].MeetingId + "');\"><img src=\"/Content/KF/images/delete.png\" alt=\"结束会话\"></div><div class=\"clear\"></div></li>"
                    }
                    else if (meetings[i].Key == Oclient.TalkMeetingKey) {
                        template = "<li style=\"cursor: pointer;background:url(/Content/KF/images/list_newbg.gif) 2px -0px no-repeat\" onclick=\"Oclient.ClickMeeting('" + meetings[i].Key + "');\"><div class=\"fklist_left\">" + meetings[i].SendNickName + "</div><div class=\"fklist_right\" onclick=\"Oclient.EndKfMeeting('" + meetings[i].Key + "','" + meetings[i].MeetingId + "');\"><img src=\"/Content/KF/images/delete.png\" alt=\"结束会话\"></div><div class=\"clear\"></div></li>"
                    }
                    else if (meetings[i].Status == 0){
                        template = "<li style=\"cursor: pointer;background:url(/Content/KF/images/list_newbg.gif) 2px -45px no-repeat\" onclick=\"Oclient.ClickMeeting('" + meetings[i].Key + "');\"><div class=\"fklist_left\">" + meetings[i].SendNickName + "</div><div class=\"fklist_right\" onclick=\"Oclient.EndKfMeeting('" + meetings[i].Key + "','" + meetings[i].MeetingId + "');\"><img src=\"/Content/KF/images/delete.png\" alt=\"结束会话\"></div><div class=\"clear\"></div></li>"
                    }
                    else if (meetings[i].Count == 0) {
                        template = "<li style=\"cursor: pointer;\" onclick=\"Oclient.ClickMeeting('" + meetings[i].Key + "');\"><div class=\"fklist_left\">" + meetings[i].SendNickName + "</div><div class=\"fklist_right\" onclick=\"Oclient.EndKfMeeting('" + meetings[i].Key + "','" + meetings[i].MeetingId + "');\"><img src=\"/Content/KF/images/delete.png\" alt=\"结束会话\"></div><div class=\"clear\"></div></li>"
                    }
                    else if (meetings[i].Count > 0) {
                        template = "<li style=\"cursor: pointer;background:url(/Content/KF/images/list_newbg.gif) 2px -25px no-repeat\" onclick=\"Oclient.ClickMeeting('" + meetings[i].Key + "');\"><div class=\"fklist_left\">" + "(" + meetings[i].Count + ")" + meetings[i].SendNickName + "</div><div class=\"fklist_right\" onclick=\"Oclient.EndKfMeeting('" + meetings[i].Key + "','" + meetings[i].MeetingId + "');\"><img src=\"/Content/KF/images/delete.png\" alt=\"结束会话\"></div><div class=\"clear\"></div></li>"
                    } else {
                        template = "<li style=\"cursor: pointer;\" onclick=\"Oclient.ClickMeeting('" + meetings[i].Key + "');\"><div class=\"fklist_left\">" + "(" + meetings[i].Count + ")" + meetings[i].SendNickName + "</div><div class=\"fklist_right\" onclick=\"Oclient.EndKfMeeting('" + meetings[i].Key + "','" + meetings[i].MeetingId + "');\"><img src=\"/Content/KF/images/delete.png\" alt=\"结束会话\"></div><div class=\"clear\"></div></li>"
                    }
                    $("#client_content").append(template);
                }
                $("#kh_count").html("当前共有" + meetings.length + "个对话");
                $("#talk_cnt").html("当前对话: <span style=\"color:blue;\">" + meetings.length + "</span>");
                $("#visi_cnt").html("在线访客: <span style=\"color:green;\">" +  (Oclient.MeetingOperate("getall").length + Oclient.GuestOperate("getall").length) + "</span>");
            }catch(e){}
        }
        //渲染聊天窗体
        this.RenderTalkDialog = function () {
            try{
                if ($("#talk_content")) {
                    $("#talk_content").remove();
                }
                if(Oclient.TalkMeetingKey){
                    var o =
                    {
                        Key: Oclient.TalkMeetingKey
                    }
                    var o2 = Oclient.MeetingOperate("get", o);
                    if (o2 != null) {
                        var content = "<div style=\"display: block;\" id=\"talk_content\"><div id=\"talk_content_2\" class=\"content\" style=\"width: 97%; height: 66%;\"><span>" + o2.SendNickName + "</span>与您开始对话";
                        for (var i = 0; i < o2.Text.length; i++) {
                            if (o2.Text[i].Role == 1) {
                                content += "<div class=\"send-msg-name\" style=\"margin-top: 0px;\">我:[" + o2.Text[i].SendTimeOfService + "]</div><p class=\"recv-msg-content\" style='color:#000000;'>" + o2.Text[i].Message + "</p>";
                            } else if (o2.Text[i].Role == 2) {
                                content += "<div class=\"send-msg-name\" style=\"margin-top: 0px;\">" + o2.SendNickName + ":[" + o2.Text[i].SendTimeOfService + "]</div><p class=\"recv-msg-content\" style='color:blue;'>" + o2.Text[i].Message + "</p>";
                            }
                        }
                        content += "</div></div>";
                        $("#talk").prepend(content);
                        $("#talk_content_2").scrollTop(99999);
                        $("#wait_conn").html(o2.SendNickName);
                    }
                }
                else{
                    $("#wait_conn").html("");
                }
            }catch(e){}
        }
        //渲染等待咨询
        this.RenderWaitGuestDialog = function () {
            try{
                $("#guest").empty();
                var guests = Oclient.GuestOperate("getall");
                for (var i = 0; i < guests.length; i++) {
                    var template = '<tr style="height: 30px"><td width="19" class="td">&nbsp;</td><td width="76"><a href="javascript:void(0);" onclick="Oclient.ReceptionKfMeeting(\'' + guests[i].Key + '\',\'' + guests[i].Args + '\')">接入</a>|<a href="javascript:void(0);" onclick="Oclient.RejectKfMeeting(\'' + guests[i].Key + '\',\'' + guests[i].Args + '\')">拒绝</a></td><td width="8"></td><td width="108">等待接入</td><td width="8"></td><td>' + guests[i].SendNickName + '</td><td width="8"></td><td width="103">&nbsp;</td><td width="5"></td><td width="60">&nbsp;</td><td width="8"></td><td width="79">&nbsp;</td></tr>';
                    $("#guest").append(template);
                }
                $("#wait_cnt").html("等待咨询: <span style=\"color:red;\">" + guests.length + "</span>");
                $("#visi_cnt").html("在线访客: <span style=\"color:green;\">" + (Oclient.MeetingOperate("getall").length + Oclient.GuestOperate("getall").length) + "</span>");
            }catch(e){}
        }
        //监听是否有多个客服端,并保持客服在线
        this.Monitor = function () {
            try{
                $.ajax({
                    type: "POST",
                    url: "/Service/Monitor",
                    data: "hash=" + wnd.communicationHash,
                    success: function (msg) {
                       if(msg == "0"){
                          window.globalLock = true;
				          $("#zDiv").append("<div style='top:20px;left:20px;background:black;padding:20px;color:red;font-size:15px;'>当前账号已在另一个地方登录使用，您被迫退出！</div>").height($(window).height());
                          $("#zDiv").show();
				          alert("当前账号已在另一个地方登录使用，您被迫退出！");
                          window.clearInterval(window.Monitor); 
                       }
                    }
                });
            }catch(e){}
        }
        //响应编辑框键盘输入
        this.onTextAreaKeydown = function(e){
            e = e || wnd.event; 
            var code = e.which ? e.which : e.keyCode;
            if (code == 13 || code == 108)   // 判断是否为回车键
                if (//(!e.ctrlKey && !e.altKey && !e.shiftKey) // 回车键发送
                 //|| 
                 (e.ctrlKey && !e.altKey && !e.shiftKey) // Ctrl + 回车键
                 ) {
                    try{
                        if ($.trim($("#input1").val())) {
                            var msg = $.trim($("#input1").val());
                            msg = window.HtmlEncode(msg);
                            msg = window.UrlEncode(msg);
                            window.Oclient.SendMsg(msg);
                            $("#input1").val("");
                        } else {
                            $("#send_tips").html("<p style='color:#000000;'>发送内容不能为空</p>");
		                    $("#send_tips").show();
                            $("#send_tips").hide(2500);
                        }
                        return false;
                    }catch(e){}
                }
        }
        //设置在线状态
        this.SetOnlineStatus = function(onlineStatus){
            try{
                $.ajax({
                    type: "POST",
                    url: "/Service/SetOnlineStatus",
                    data: "OnlineStatus=" + onlineStatus,
                    success: function (msg) {
                        var json = eval("(" + msg + ")");
                        var status = json.data.model;
                        if(status == "0"){
                            $("#state_on").show();
                            $("#state_off").hide();
                            $("#state_set").html("<a>在线</a>");
                        }else if(status == "1"){
                            $("#state_on").hide();
                            $("#state_off").show();
                            $("#state_set").html("<a>离开</a>");
                        }
                    }
                });
            }catch(e){}
        }
    }

    /**
    * 把客服对象暴露给全局
    */
    wnd.Oclient = new Client();
    /**
    * 注册通信地址和通信回调函数
    */
    wnd.communicationAddress = "/Service/MonitoringItem?author=ajso";
    wnd.communicationFunc = function (dataMessage, dataNotice) {
        try{
            if (dataMessage) {
                Message.show("新消息");  
                document.getElementById("sound").src="/Content/KF/images/msg.wav";
                try{
                    document.getElementById("sound").play();
                }catch(e){
            
                }
                try{
                    document.getElementById("sound").autostart=true;
                }catch(e){
            
                }
                $.each(dataMessage, function (i, item) {
                    var o =
                    {
                        Key: item.SendUserId
                    }
                    var o2 = Oclient.MeetingOperate("get", o);
                    if (o2 == null) {

                    } else {
                        o2.Message = item.Message;
                        o2.SendTimeOfService = item.SendTimeOfService;
                        if (o2.Key != Oclient.TalkMeetingKey) {
                            o2.Count++;
                        }
                        var len = o2.Text.length;
                        o2.Text[len] = { Role: 2, Message: item.Message, SendTimeOfService: item.SendTimeOfService };
                        Oclient.MeetingOperate("update", o2);
                    }
                    //渲染相关窗体
                    Oclient.RenderMeetingList();
                    Oclient.RenderTalkDialog();
                });
            }
            if (dataNotice) {
                $.each(dataNotice, function (i, item) {
                    //开始会话(等待客服接入)
                    if (item.NoticeType == 1) {
                        Message.show("新咨询");  
                        document.getElementById("sound").src="/Content/KF/images/msg.wav";
                        try{
                            document.getElementById("sound").play();
                        }catch(e){
            
                        }
                        try{
                            document.getElementById("sound").autostart=true;
                        }catch(e){
            
                        }
                        var o =
                        {
                            Key: item.SendUserId,
                            SendNickName: item.SendNickName,
                            NoticeType: item.NoticeType,
                            Status: item.Status,
                            Args: item.Args
                        }
                        var o2 = Oclient.GuestOperate("get", o);
                        if (o2 == null) {
                            Oclient.GuestOperate("insert", o);
                        }
                        //渲染相关窗体
                        Oclient.RenderWaitGuestDialog();
                    }else if(item.NoticeType == 2 || item.NoticeType == 4){
                        var o =
                        {
                            Key: item.Args
                        }
                        Oclient.GuestOperate("delete", o);
                        //渲染相关窗体
                        Oclient.RenderWaitGuestDialog();
                    }else if(item.NoticeType == 3){
                        var o =
                        {
                            Key: item.Args
                        }
                        o2 = Oclient.MeetingOperate("get", o);
                        o2.Status = 0;
                        Oclient.MeetingOperate("update", o2);
                        //渲染相关窗体
                        Oclient.RenderMeetingList();
                    }
                });
            }
        }catch(e){}
    }

    /**
    * 绑定消息发送按钮事件
    */
    $('#send_a').bind('click', function () {
        try{
            if ($.trim($("#input1").val())) {
                var msg = $.trim($("#input1").val());
                msg = window.HtmlEncode(msg);
                msg = window.UrlEncode(msg);
                window.Oclient.SendMsg(msg);
                $("#input1").val("");
            } else {
                $("#send_tips").html("<p style='color:#000000;'>发送内容不能为空</p>");
		        $("#send_tips").show();
                $("#send_tips").hide(2500);
            }
        }catch(e){}
    });
    $('#input1').bind('keydown',wnd.Oclient.onTextAreaKeydown);
    /**
    * 绑定切换按钮事件
    */
    $('#stab_1').bind('click', function () {
        $('#part_1').show();
        $('#part_3').hide();
        $('#stab_1').addClass("on");
        $('#stab_3').removeClass("on");
    });
    $('#stab_3').bind('click', function () {
        $('#part_3').show();
        $('#part_1').hide();
        $('#stab_3').addClass("on");
        $('#stab_1').removeClass("on");
    });
    /**
    * 定时发送请求，防止多个客服端
    */
    wnd.Monitor = setInterval("Oclient.Monitor()",7000);
    wnd.document.onclick = function(){
        wnd.Message.clear();
    }   
})(window, $);