var ColorHex = new Array('00', '33', '66', '99', 'CC', 'FF');
var SpColorHex = new Array('FF0000', '00FF00', '0000FF', 'FFFF00', '00FFFF', 'FF00FF');
var current = null;
var elementId = null;

$(function () {
    $(document).click(function (e) {
        if (e.target.tagName != "TD" && e.target.tagName!="INPUT") {
            if ($(".open_color_panel").length==1) {
                $("#open_color_panel").remove();
            }
        }
    });
});

function getEvent() {
    if (document.all) {
        return window.event; //如果是ie
    }
    func = getEvent.caller;
    while (func != null) {
        var arg0 = func.arguments[0];
        if (arg0) {
            if ((arg0.constructor == Event || arg0.constructor == MouseEvent) || (typeof (arg0) == "object" && arg0.preventDefault && arg0.stopPropagation)) {
                return arg0;
            }
        }
        func = func.caller;
    }
    return null;
}

function intocolor(obj) {
    elementId = obj;
    var evt = getEvent();
    if ($(".open_color_panel").length == 1) {
        $("#open_color_panel").remove();
    }
    $("#" + obj).after(getTableHtml());
    $("#open_color_panel").css("left", $("#" + obj).offset().left);
    $("#open_color_panel").css("top", $("#" + obj).offset().top+30);
}

function doclick() {
    var evt = getEvent();
    var element = evt.srcElement || evt.target;
    if (element.tagName == "TD") {
        var bg = rgbToHex(element._background);
        $("#" + elementId).css("background-color", bg);
        $("#" + elementId).val(bg);
        $("#open_color_panel").remove();
    }
}

function getTableHtml() {
    var colorTable = ''
    for (i = 0; i < 2; i++) {
        for (j = 0; j < 6; j++) {
            colorTable = colorTable + '<tr height=12>'
//            colorTable = colorTable + '<td width=11 style="background-color:#000000">'

//            if (i == 0) {
//                colorTable = colorTable + '<td width=11 style="background-color:#' + ColorHex[j] + ColorHex[j] + ColorHex[j] + '">'
//            }
//            else {
//                colorTable = colorTable + '<td width=11 style="background-color:#' + SpColorHex[j] + '">'
//            }


//            colorTable = colorTable + '<td width=11 style="background-color:#000000">'
            for (k = 0; k < 3; k++) {
                for (l = 0; l < 6; l++) {
                    colorTable = colorTable + '<td width=11 style="background-color:#' + ColorHex[k + i * 3] + ColorHex[l] + ColorHex[j] + '">'
                }
            }
        }
    }
    colorTable = '<div id="open_color_panel" class="open_color_panel" style="position: absolute;z-index:10001;"><table width=217 border="0" cellspacing="0" cellpadding="0" style="border:1px #000000 solid;border-bottom:none;border-collapse: collapse" bordercolor="000000">'
           + '<tr height=30><td colspan=21 bgcolor=#cccccc>'
           + '<table cellpadding="0" cellspacing="1" border="0" style="border-collapse: collapse">'
           + '<tr><td width="3"><td><input type="text" name="DisColor" id="DisColor" size="6" disabled style="border:solid 1px #000000;background-color:#ffff00;"></td>'
           + '<td width="3"><td><input type="text" name="HexColor" id="HexColor" size="7" style="border:inset 1px;font-family:Arial;width:60px;" value="#000000"></td></tr></table></td></table>'
           + '<table border="1" cellspacing="0" cellpadding="0" style="border-collapse: collapse" bordercolor="000000" onmouseover="doOver()" onmouseout="doOut()" onclick="doclick()" style="cursor:hand;">'
           + colorTable + '</table></div>';
    return colorTable;
}

function doOver() {
    var evt = getEvent();
    var element = evt.srcElement || evt.target;
    var DisColor = document.getElementById("DisColor");
    var HexColor = document.getElementById("HexColor");
    if ((element.tagName == "TD") && (current != element)) {
        if (current != null) { current.style.backgroundColor = current._background }
        element._background = element.style.backgroundColor
        DisColor.style.backgroundColor = rgbToHex(element.style.backgroundColor)
        HexColor.value = rgbToHex(element.style.backgroundColor)
        element.style.backgroundColor = "white"
        current = element
    }
}

/**
* firefox 的颜色是以(RGB())出现，进行转换
*/
function rgbToHex(aa) {
    if (aa.indexOf("rgb") != -1) {
        aa = aa.replace("rgb(", "")
        aa = aa.replace(")", "")
        aa = aa.split(",")
        r = parseInt(aa[0]);
        g = parseInt(aa[1]);
        b = parseInt(aa[2]);
        r = r.toString(16);
        if (r.length == 1) { r = '0' + r; }
        g = g.toString(16);
        if (g.length == 1) { g = '0' + g; }
        b = b.toString(16);
        if (b.length == 1) { b = '0' + b; }
        return ("#" + r + g + b).toUpperCase();
    }
    else {
        return aa;
    }
}

function doOut() {

    if (current != null) current.style.backgroundColor = current._background;
}