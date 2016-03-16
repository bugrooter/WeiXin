$(document).ready(
    function () {
        $(".accordion-nav>li>a").click(function () {
            $(".curr").removeAttr("class");
            $(this).attr("class", "curr");
        });
});

function selectAll(obj) {
    $('input[name="dropcheck"]').each(function () {
        if ($("#setall").prop("checked")) {
            $(this).prop("checked", 'true');
        }
        else {
            $(this).removeAttr("checked");
        }
    });
}

function check_mobile(mobile) {
    var mobile = mobile.replace(/^\s*|\s*$/g, '');
    var length = mobile.length;
    var a = /^(1[3|4|5|8])[0-9]{9}$/;
    if (length == 0) {
        return false;
    }
    else {
        if (a.test(mobile)) {
            return true;
        }
        else {
            return false;
        }
    }


}


//检测邮箱
function check_email(email) {
    var email = email.replace(/^\s*|\s*$/g, '');
    var length = email.length;
    var a = /^([a-zA-Z0-9]+[_|\_|\.]?)*[a-zA-Z0-9]+@([a-zA-Z0-9]+[_|\_|\.]?)*[a-zA-Z0-9]+\.[a-zA-Z]{2,3}$/;
    if (length == 0)//用email为空的时候
    {
        return false;
    }
    else {
        if (a.test(email)) {
            return true;
        }
        else {
            return false;
        }

    }

}


//写cookies

function setCookie(name, value) {
    var Days = 30;
    var exp = new Date();
    exp.setTime(exp.getTime() + Days * 24 * 60 * 60 * 1000);
    document.cookie = name + "=" + escape(value) + ";expires=" + exp.toGMTString();
}

//读取cookies
function getCookie(name) {
    var arr, reg = new RegExp("(^| )" + name + "=([^;]*)(;|$)");

    if (arr = document.cookie.match(reg))

        return unescape(arr[2]);
    else
        return null;
}

//删除cookies
function delCookie(name) {
    var exp = new Date();
    exp.setTime(exp.getTime() - 1);
    var cval = getCookie(name);
    if (cval != null)
        document.cookie = name + "=" + cval + ";path=/;expires=" + exp.toGMTString();
}

function deleteAny(cssname, delmathod, listmethod, formid) {
    var id = "";
    $("input[name=" + cssname + "]").each(function () {
        if ($(this).prop("checked")) {
            id += $(this).val() + ",";
        }
    });
    if ($.trim(id) == "") {
        msg_alter("请选择要删除的数据");
        return;
    }
    var curpage = parseInt($("#curpage").text());
    HanlerListData({
        getUrl: "/public/action/DataHandler.ashx?action=DelDo",
        postData: "id=" + id + "&method=" + delmathod,
        formId: null,
        callBack: LoadListData,
        callParams: {
            getUrl: "/public/action/DataHandler.ashx?action=QueryList",
            postData: "method=" + listmethod + "&curpage=" + curpage,
            formId: formid
        }
    });
}

function deleteAll(delmathod, listmethod, formid) {
    var curpage = parseInt($("#curpage").text());
    HanlerListData({
        getUrl: "/public/action/DataHandler.ashx?action=DelDo",
        postData: "method=" + delmathod,
        formId: null,
        callBack: LoadListData,
        callParams: {
            getUrl: "/public/action/DataHandler.ashx?action=QueryList",
            postData: "method=" + listmethod + "&curpage=" + curpage,
            formId: formid
        }
    });
}

function count_textlength(val, cid) {
    var len = 300 - parseInt(val.length);
    $("#" + cid).text(len);
}

function change_filename(fval, fid) {
    var filepath = fval;
    var i = filepath.lastIndexOf("\\");
    if (i > -1) {
        filepath = filepath.substring(i + 1);
    }
    document.getElementById(fid).value = filepath;
}

function gopage(cue, method) {
    var curpage = parseInt($("#curpage").text());
    var maxpage = parseInt($("#totalpage").text());
    var newpage = 1;
    if (cue == -1) {
        newpage = curpage - 1;
    }
    else if (cue == 0) {
        newpage = 1;
    }
    else if (cue == 1) {
        newpage = curpage + 1;
    }
    else if (cue == 2) {
        newpage = maxpage;
    }
    else if (cue == 3) {
        newpage = parseInt($("#gogo").val());
        if (isNaN(newpage)) {
            $("#gogo").val("");
            return;
        }
    }
    newpage = newpage < 1 ? 1 : newpage;
    newpage = newpage > maxpage ? maxpage : newpage;
    LoadListData({ getUrl: "/public/action/DataHandler.ashx?action=QueryList", postData: "method=" + method + "&curpage=" + newpage, formId: "#list_search_form" });
}

//表单提交操作
//请求地址，提交数据，表单Id，回调函数，回调函数参数
//{getUrl,postData, formId,callBack,callParams }
function doSaveWithFile(checkFrom, params) {
    if (checkFrom != null && checkFrom() == false) return;
    var _postData = "";
    var _getData = "";
    var _getUrl = "";
    if (typeof params.formId != "undefined" && params.formId != null) {

    }
    else {
        msg_alter("没有要保存的数据");
    }
    if (typeof params.getUrl != "undefined" && params.getUrl != null) {
        if (new RegExp(/\?(\w+)=(\w){0,1}/ig).test(params.getUrl)) {
            _getUrl = params.getUrl + "&r=" + Math.random();
        }
        else {
            _getUrl = params.getUrl + "?r=" + Math.random();
        }
    }
    $(params.formId).attr("action", _getUrl);
    $(params.formId).attr("method", "post");
    $(params.formId).ajaxSubmit({
        dataType: 'text',
        beforeSend: function () { msg_loading('数据保存中...'); },
        success: function (msg) {  //操作成功后的处理
            var obj = jQuery.parseJSON(msg);
            if (obj.code == 100) {
                msg_alter("登陆超时，请重新登录");
                setTimeout(function () { window.top.location.href = "/admin/admin_login.aspx"; }, 500);
                return;
            }
            if (obj.success == false) {  //业务逻辑操作失败  
                msg_alter("数据保存失败，原因是:" + obj.info);
            } else {	//业务逻辑操作成功 
                dialog_close();
                msg_close();
                if (typeof params.callBack != "undefined" && params.callBack != null) {
                    params.callBack(params.callParams);
                }
            }
        },
        error: function (msg) { //系统错误后的处理  
            msg_alter("数据保存失败!");
        }
    });
}

//表单提交操作
//请求地址，提交数据，表单Id，回调函数，回调函数参数
//{getUrl,postData, formId,callBack,callParams }
function doSave(checkFrom, params) {
    if (checkFrom != null && checkFrom() == false) return;
    var _postData = "";
    var _getData = "";
    var _getUrl = "";
    if (typeof params.formId != "undefined" && params.formId != null) {
        _postData = $(params.formId).serialize();
        if (params.postData != null) {
            _postData += "&" + params.postData;
        }
    }
    else {
        _postData = params.postData;
    }
    if (typeof params.getUrl != "undefined" && params.getUrl != null) {
        if (new RegExp(/\?(\w+)=(\w){0,1}/ig).test(params.getUrl)) {
            _getUrl = params.getUrl + "&r=" + Math.random();
        }
        else {
            _getUrl = params.getUrl + "?r=" + Math.random();
        }
    }
    $.ajax({
        type: 'post',
        url: _getUrl,
        data: _postData,
        dataType: 'text',
        beforeSend: function () { msg_loading('数据保存中...'); },
        success: function (msg) {  //操作成功后的处理
            var obj = jQuery.parseJSON(msg);
            if (obj.code == 100) {
                msg_alter("登陆超时，请重新登录");
                setTimeout(function () { window.top.location.href = "/admin/admin_login.aspx"; }, 500);
                return;
            }
            if (obj.success == false) {  //业务逻辑操作失败  
                msg_alter("数据保存失败，原因是:" + obj.info);
            } else {	//业务逻辑操作成功 
                dialog_close();
                msg_close();
                if (typeof params.callBack != "undefined" && params.callBack != null) {
                    params.callBack(params.callParams);
                }
            }
        },
        error: function (msg) { //系统错误后的处理  
            msg_alter("数据保存失败!");
        }
    });
}

//列表中操作
//请求地址，提交数据，表单Id，回调函数，回调函数参数
//{getUrl,postData, formId,callBack,callParams }
function HanlerListData(params) {
    var _postData = "";
    var _getData = "";
    var _getUrl = ""; 
    if (typeof params.formId != "undefined" && params.formId != null) {
        _postData = $(params.formId).serialize();
        if (params.postData != null) {
            _postData += "&" + params.postData;
        }
    }
    else {
        _postData = params.postData;
    }
    if (typeof params.getUrl != "undefined" && params.getUrl != null) {
        if (new RegExp(/\?(\w+)=(\w){0,1}/ig).test(params.getUrl)) {
            _getUrl = params.getUrl + "&r=" + Math.random();
        }
        else {
            _getUrl = params.getUrl + "?r=" + Math.random();
        }
    }

    msg_confirm("是否确认该操作", function () {
        $.ajax({
            type: 'post',
            url: _getUrl,
            data: _postData,
            dataType: 'text',
            beforeSend: function () { msg_loading('操作执行中...'); },
            success: function (msg) {  //操作成功后的处理
                var obj = jQuery.parseJSON(msg);
                if (obj.code == 100) {
                    msg_alter("登陆超时，请重新登录");
                    setTimeout(function () { window.top.location.href = "/admin/admin_login.aspx"; }, 500);
                    return;
                }
                msg_close();
                if (obj.success == true) {
                    if (typeof params.callBack != "undefined" && params.callBack != null) {
                        params.callBack(params.callParams);
                    }
                }
                else {
                    msg_alter(obj.info);
                }
            },
            error: function (msg) { //系统错误后的处理  
                msg_alter("操作执行失败");
            }
        });
    });
}

//获取列表
//请求地址，提交数据，表单Id，回调函数，回调函数参数
//{getUrl,postData, formId,callBack,callParams }
function LoadListData(params) {
    var _postData = "";
    var _getData = "";
    var _getUrl = "";
    if (typeof params.formId != "undefined") {
        _postData = $(params.formId).serialize();
        if (params.postData != null) {
            _postData += "&" + params.postData;
        }
    }
    else {
        _postData = params.postData;
    }
    if (typeof params.getUrl != "undefined") {
        if (new RegExp(/\?(\w+)=(\w){0,1}/ig).test(params.getUrl)) {
            _getUrl = params.getUrl + "&r=" + Math.random();
        }
        else {
            _getUrl = params.getUrl + "?r=" + Math.random();
        }
    }
    $.ajax({
        type: 'post',
        url: _getUrl,
        data: _postData,
        dataType: 'text',
        beforeSend: function () { msg_loading('数据加载中...'); },
        success: function (msg) {  //操作成功后的处理
            var obj = jQuery.parseJSON(msg);
            if (obj.code == 100) {
                msg_alter("登陆超时，请重新登录");
                setTimeout(function () { window.top.location.href = "/admin/admin_login.aspx"; }, 500);
                return;
            }
            msg_close();
            $("#open").attr("id", "");
            $("#content").html(obj.info);
        },
        error: function (msg) { //系统错误后的处理  
            $("#open").attr("id", "");
            msg_alter("数据加载失败");
        }
    });
}

function GetJsonData(params) {
    var _postData = "";
    var _getData = "";
    var _getUrl = "";
    if (typeof params.formId != "undefined") {
        _postData = $(params.formId).serialize();
        if (params.postData != null) {
            _postData += "&" + params.postData;
        }
    }
    else {
        _postData = params.postData;
    }
    if (typeof params.getUrl != "undefined") {
        if (new RegExp(/\?(\w+)=(\w){0,1}/ig).test(params.getUrl)) {
            _getUrl = params.getUrl + "&r=" + Math.random();
        }
        else {
            _getUrl = params.getUrl + "?r=" + Math.random();
        }
    }
    $.ajax({
        type: 'post',
        url: _getUrl,
        data: _postData,
        dataType: 'text',
        beforeSend: function () { },
        success: function (msg) {  //操作成功后的处理
            var obj = jQuery.parseJSON(msg);
            if (obj.code == 100) {
                msg_alter("登陆超时，请重新登录");
                setTimeout(function () { window.top.location.href = "/admin/admin_login.aspx"; }, 500);
                return;
            }
            msg_close();
            if (typeof params.callBack != "undefined" && params.callBack != null) {
                params.callBack(obj.info);
            }
        },
        error: function (msg) { //系统错误后的处理  
            $("#open").attr("id", "");
        }
    });
}