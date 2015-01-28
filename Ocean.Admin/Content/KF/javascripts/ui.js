/**
* author Ajso
* date 2014.3.9
*/
(function (wnd, $) {
    // 整个html、脚本初始化标志
    var 
   doc = document,
   docHeight = 0,
   docWidth = 0,

   BODY_OFFHEIGHT = 68,
   BULLETIN_OFFWIDTH = 500,
   STATE_OFFLEFT = 122,
   TITLE_OFFWIDTH = 167,
   TALK_OFFWIDTH = 344,
   TALK_OFFHEIGHT = 152,
   TRACK_OFFHEIGHT = 178,
   VISITOR_INPUT_OFFWIDTH = 458,
   LIST_OFFHEIGHT = 119,
   INFO_QUICK_OFFLEFT = 176,
   CYY_OFFHEIGHT = 305,
   TALK_SCROLL_OFFHEIGHT = 149,
   ITALK_SCROLL_OFFHEIGHT = 146,
   MSGPRE_OFFWIDTH = 349,
   CONTENT_OFFWIDTH = 356,
   CONTENT_LOW_OFFHEIGHT = 296,
   CONTENT_HIGH_OFFHEIGHT = 336,
   ICONTENT_OFFHEIGHT = 294,
   ICONTENT_OFFWIDTH = 357,
   ITOOLS_OFFTOP = 239,
   ITOOLS_OFFWIDTH = 344,
   IMPORT_OFFTOP = 210,
   IINPUT_OFFWIDTH = 455,
   TRACK_TITLE_OFFWIDTH = 12,
   BOTTOM_OFFWIDTH = 330,

   SEND_TIPS_OFFTOP = 210,
   SEND_TIPS_OFFLEFT = 430,
   PRESSDIV_OFFTOP = 188,
   PRESSDIV_OFFLEFT = 185,

   INNER_SEND_TIPS_OFFTOP = 210,
   INNER_SEND_TIPS_OFFLEFT = 430,
   PRESSDIV1_OFFTOP = 188,
   PRESSDIV1_OFFLEFT = 185,

   FACE_OFFTOP = 365,
   FILE_OFFTOP = 395,
   SWITCH_OFFLEFT = 202,

   CYY_CONTENT_OFFHEIGHT = 405,
   GUEST_CARD_OFFWIDTH = 460,
   GUEST_CARD_OFFHEIGHT = 310,

   LINK_MAN_OFFWIDTH = 460,
   LINK_MAN_OFFHEIGHT = 310,

   FREEZE_TOP = 120,
   FREEZE_LEFT = 162,
   FREEZE_HEIGHT = 28,

   PHONE_HEIGHT = 440,
   PHONE_WIDTH = 333,

   SELECT_TOP = 135,
   SELECT_IP_FROM = 444,
   SELECT_CURPAGE = 406,

   TALK_BOTTOM = 27,
   HEAD_MANAGE = 290,
   HEAD_STATE = 127,
   HEAD_NAV_RIGHT = 360;

   wnd.resize = function() {
        try {
            docHeight = document.documentElement.clientHeight;
            if (docHeight == 0) {
                docHeight = document.body.clientHeight;
            }

            docWidth = document.documentElement.clientWidth;
            if (docWidth == 0) {
                docWidth = document.body.clientWidth;
            }

            if (docWidth < 774) {
                docWidth = 774;
            }

            if (docHeight < 565) {
                docHeight = 565;
            }

            resizeHeadAndFoot();
            resizeVisitorTalk();
            resizeInnerTalk();
            resizeVisitorList();
            resizeGuestCard();
            resizeLinkMan();
            resizeFreeze();
            resizeInsertFreeze();
            resizeChatMonitor();
        }
        catch (e) { }
    }

    // Head and Foot
    wnd.resizeHeadAndFoot = function() {
        try {
            document.getElementById("body").style.height = (docHeight - BODY_OFFHEIGHT) + "px";
            document.getElementById("bulletin").style.width = (docWidth - BULLETIN_OFFWIDTH) + "px";
            document.getElementById("p_manage").style.left = (docWidth - HEAD_MANAGE) + 70 + "px";
            document.getElementById("manage_list").style.left = (docWidth - HEAD_MANAGE) + "px";
            document.getElementById("state").style.left = (docWidth - HEAD_STATE) + "px";
            document.getElementById("state_list").style.left = (docWidth - HEAD_STATE) + "px";
            document.getElementById("nav_right").style.left = (docWidth - HEAD_NAV_RIGHT) + "px";

            document.getElementById("company_bottom").style.width = Math.round((docWidth - BOTTOM_OFFWIDTH) / 4) + "px";
            document.getElementById("worker_bottom").style.width = Math.round((docWidth - BOTTOM_OFFWIDTH) / 2) + "px";
            document.getElementById("author_bottom").style.width = Math.round((docWidth - BOTTOM_OFFWIDTH) / 4) + "px";

            document.getElementById("chartDiv").style.top = (docHeight - PHONE_HEIGHT) + "px";
            document.getElementById("chartDiv").style.left = Math.round((docWidth - PHONE_WIDTH) / 2) + "px";
            document.getElementById("bottom").style.top = (docHeight - TALK_BOTTOM) + "px";
        } catch (e) { }
    }

    // 访客对话
    wnd.resizeVisitorTalk = function() {
        try {
            document.getElementById("talk_title").style.width = (docWidth - TITLE_OFFWIDTH) + "px";
            document.getElementById("talk").style.width = (docWidth - TALK_OFFWIDTH) + "px";
            document.getElementById("talk").style.height = (docHeight - TALK_OFFHEIGHT) + "px";
            document.getElementById("input1").style.width = (docWidth - VISITOR_INPUT_OFFWIDTH) + "px";
            document.getElementById("list").style.height = (docHeight - LIST_OFFHEIGHT) + "px";
            document.getElementById("quick").style.left = (docWidth - INFO_QUICK_OFFLEFT) + "px";
            document.getElementById("info").style.left = (docWidth - INFO_QUICK_OFFLEFT) + "px";
            document.getElementById("cyy_list").style.height = (docHeight - CYY_OFFHEIGHT - 30) + "px";
            document.getElementById("client_content").style.height = (docHeight - TALK_SCROLL_OFFHEIGHT) + "px";
            document.getElementById("premsg").style.width = (docWidth - MSGPRE_OFFWIDTH) + "px";

            document.getElementById("send_tips").style.top = (docHeight - SEND_TIPS_OFFTOP) + "px";
            document.getElementById("send_tips").style.left = (docWidth - SEND_TIPS_OFFLEFT) + "px";
            document.getElementById("pressdiv").style.top = (docHeight - PRESSDIV_OFFTOP) + "px";
            document.getElementById("pressdiv").style.left = (docWidth - PRESSDIV_OFFLEFT) + "px";

            resizeContents();
            resizeFace();
            resizeFile();

            document.getElementById("switch_div").style.left = (docWidth - SWITCH_OFFLEFT) + "px";
            document.getElementById("cyy_add").style.left = (docWidth - INFO_QUICK_OFFLEFT) + "px";
            document.getElementById("cyy_add").style.height = (docHeight - CYY_OFFHEIGHT + 2) + "px";
            document.getElementById("phrase_content").style.height = (docHeight - CYY_CONTENT_OFFHEIGHT) + "px";

            if (m_cyySearch != null) m_cyySearch.resize(docWidth, docHeight);
        }
        catch (e) { }
    }

    // 内部对话
    wnd.resizeInnerTalk = function() {
        try {
            document.getElementById("talk_title2").style.width = (docWidth - TITLE_OFFWIDTH) + "px";
            document.getElementById("kf_li2").style.height = (docHeight - LIST_OFFHEIGHT) + "px";
            document.getElementById("online_kf").style.height = (docHeight - ITALK_SCROLL_OFFHEIGHT) + "px";
            document.getElementById("kf_chat").style.width = (docWidth - TALK_OFFWIDTH) + "px";

            document.getElementById("kf_chat").style.height = (docHeight - ICONTENT_OFFHEIGHT + 18) + "px";
            document.getElementById("inside-talk-disp").style.height = (docHeight - ICONTENT_OFFHEIGHT + 18) + "px";

            resizeiContents();

            document.getElementById("kf_import").style.top = (docHeight - IMPORT_OFFTOP) + "px";
            document.getElementById("kf_import").style.width = (docWidth - TALK_OFFWIDTH) + "px";
            document.getElementById("iinput").style.width = (docWidth - IINPUT_OFFWIDTH) + "px";
            document.getElementById("inner_send_tips").style.top = (docHeight - INNER_SEND_TIPS_OFFTOP) + "px";
            document.getElementById("inner_send_tips").style.left = (docWidth - INNER_SEND_TIPS_OFFLEFT) + "px";
            document.getElementById("pressdiv1").style.top = (docHeight - PRESSDIV1_OFFTOP) + "px";
            document.getElementById("pressdiv1").style.left = (docWidth - PRESSDIV1_OFFLEFT) + "px";

            document.getElementById("inner_tools").style.top = (docHeight - ITOOLS_OFFTOP) + "px";
            document.getElementById("inner_tools").style.width = (docWidth - ITOOLS_OFFWIDTH) + "px";
            document.getElementById("affiche").style.left = (docWidth - INFO_QUICK_OFFLEFT) + "px";
            document.getElementById("iworker_list").style.left = (docWidth - INFO_QUICK_OFFLEFT) + "px";
            document.getElementById("iworker_list").style.height = (docHeight - CYY_OFFHEIGHT + 26) + "px";
            document.getElementById("iworkers").style.height = (docHeight - CYY_OFFHEIGHT) + "px";
        }
        catch (e) { }
    }

    // 网站访客
    wnd.resizeVisitorList = function() {
        try {
            document.getElementById("track_t").style.width = (docWidth - TRACK_TITLE_OFFWIDTH) + "px";
            document.getElementById("track").style.width = (docWidth - TRACK_TITLE_OFFWIDTH) + "px";
            document.getElementById("track").style.height = (docHeight - TRACK_OFFHEIGHT) + "px";
            document.getElementById("select_div").style.top = (docHeight - SELECT_TOP) + "px";
            document.getElementById("select_div").style.width = (docWidth - TRACK_TITLE_OFFWIDTH) + "px";

            document.getElementById("select_ip").style.width = Math.round((docWidth - SELECT_IP_FROM) / 3) + "px";
            document.getElementById("select_curpage").style.width = Math.round((docWidth - SELECT_CURPAGE) / 3) + "px";
            document.getElementById("select_from").style.width = Math.round((docWidth - SELECT_IP_FROM) / 3) + "px";
        }
        catch (e) { }
    }

    // 实时监控
    wnd.resizeChatMonitor = function() {
        try {
            document.getElementById("chat_monitor").style.width = (docWidth - 12) + "px";
            document.getElementById("chat_monitor").style.height = (docHeight - 120) + "px";
        }
        catch (e) { }
    }

    // 调整所有消息显示框
    wnd.resizeContents = function() {
        contentBymsgpre("0");
        for (var i = 0; i < m_khList.length; i++) {
            var gid = m_khList[i].gid;
            contentBymsgpre(gid);
        }
    }

    // 调整消息显示框宽高
    wnd.contentBymsgpre = function(gid) {
        try {
            document.getElementById("content_" + gid).style.width = (docWidth - CONTENT_OFFWIDTH) + "px";
            if (m_msgPreType == 1) {
                document.getElementById("content_" + gid).style.height = (docHeight - CONTENT_HIGH_OFFHEIGHT) + "px";
            }
            else {
                document.getElementById("content_" + gid).style.height = (docHeight - CONTENT_LOW_OFFHEIGHT) + "px";
            }
        }
        catch (e) { }
    }

    // 调整名片位置
    wnd.resizeGuestCard = function() {
        try {
            var div = document.getElementById("guest_card");
            div.style.left = (docWidth - GUEST_CARD_OFFWIDTH) / 2;
            div.style.top = (docHeight - GUEST_CARD_OFFHEIGHT) / 2;
        }
        catch (e) { }
    }

    // 调整联系人位置
    wnd.resizeLinkMan = function() {
        try {
            var div = document.getElementById("link_man");
            div.style.left = (docWidth - LINK_MAN_OFFWIDTH) / 2;
            div.style.top = (docHeight - LINK_MAN_OFFHEIGHT) / 2;
        }
        catch (e) { }
    }

    // 调整断线提示层位置
    wnd.resizeFreeze = function() {
        try {
            if (m_freezeDiv != null) {
                m_freezeDiv.resizeFreezeDiv(FREEZE_LEFT, FREEZE_TOP, (docWidth - TALK_OFFWIDTH), FREEZE_HEIGHT);
            }
        }
        catch (e) { }
    }

    // 调整表情框位置
    wnd.resizeFace = function() {
        try {
            if (m_tabID == 1) {
                document.getElementById("face_div").style.top = (docHeight - FACE_OFFTOP + 22) + "px";
            }
            else {
                document.getElementById("face_div").style.top = (docHeight - FACE_OFFTOP + 22) + "px";
            }
        }
        catch (e) { }
    }

    // 调整文件框位置
    wnd.resizeFile = function() {
        try {
            if (m_tabID == 1) {
                document.getElementById("file_div").style.top = (docHeight - FILE_OFFTOP + 22) + "px";
            }
            else {
                document.getElementById("file_div").style.top = (docHeight - FILE_OFFTOP + 22) + "px";
            }
        }
        catch (e) { }
    }

    // 调整插入冻结层位置
    wnd.resizeInsertFreeze = function() {
        try {
            document.getElementById("insert_freeze").style.height = docHeight + "px";
            document.getElementById("insert_freeze").style.width = docWidth + "px";
            document.getElementById("insert_div").style.top = Math.round((docHeight - 180) / 2) + "px";
            document.getElementById("insert_div").style.left = Math.round((docWidth - 360) / 2) + "px";
        } catch (e) { }
    }

    // 调整所有内部对话框
    wnd.resizeiContents = function() {
        try {
            document.getElementById("icontent_orign").style.width = (docWidth - ICONTENT_OFFWIDTH) + "px";
            document.getElementById("icontent_orign").style.height = (docHeight - ICONTENT_OFFHEIGHT) + "px";
            for (var i = 0; i < m_iTalkList.length; i++) {
                var id = m_iTalkList[i].id;
                document.getElementById("icontent_" + id).style.width = (docWidth - ICONTENT_OFFWIDTH) + "px";
                document.getElementById("icontent_" + id).style.height = (docHeight - ICONTENT_OFFHEIGHT) + "px";
            }
        } catch (e) { }
    }
    //注册onresize
    wnd.onresize = resize;
})(window, $);