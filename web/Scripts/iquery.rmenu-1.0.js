(function($) {
    $.fn.extend({
        rmenu: function(options) {
            var config = {
                width: "80px",
                /*菜单宽度*/
                items: [{
                    seltor: null,
                    prop: "id",
                    list: ["新增", "编辑", "删除", "预览"],
                    showtitle: true,
                    title: "操作菜单"
                }],
                /*弹出菜单项{选择器，要获取的属性数据，菜单显示}*/
                seltor: null,
                /*要屏蔽的右键菜单*/
                mousekey: 3
            };
            var tdata = null;
            var tid = "";
            var clickel = null;
            var itemsindex = -1;
            var ops = $.extend(config, options);
            if (config.seltor != null) {
                $(config.seltor).unbind("contextmenu");
                $(config.seltor).unbind("mousedown");
                $(config.seltor).bind("contextmenu",
                function(e) {
                    return false;
                });
                $(config.seltor).bind("mousedown",
                function(e) {
                    var el = e.srcElement || e.target;
                    if ($(el).prop("id") != "right_menu_dialog" && $(el).hasClass("rmenu_item") == false && $(el).hasClass("right_menu_title") == false) {
                        $("#right_menu_dialog").remove();
                    }
                });
            }

            $(this).unbind("contextmenu");
            $(this).unbind("mousedown");
            $(this).bind("contextmenu",
            function(e) {
                return false;
            });
            $(this).bind("mousedown",
            function(e) {
                var el = e.srcElement || e.target;
                if ($(el).prop("id") != "right_menu_dialog" && $(el).hasClass("rmenu_item") == false && $(el).hasClass("right_menu_title") == false) {
                    $("#right_menu_dialog").remove();
                }
            });

            $(this).bind("mousedown",
            function(e) {
                var el = e.srcElement || e.target;
                var x = $(el).offset().left;
                var w = parseInt($(el).css("width").replace("px", ""));
                var h = parseInt($(el).css("height").replace("px", ""));
                var y = $(el).offset().top + h;
                /*判断点击的元素ID*/
                for (var m = 0; m < config.items.length; m++) {
                    if ($(el).hasClass(config.items[m].seltor) && e.which == config.mousekey) {
                        tdata = null,
                        tid = "",
                        clickel = null,
                        itemsindex = -1;
                        /*初始化变量*/
                        if ($("#right_menu_dialog").size() >= 0) {
                            $("#right_menu_dialog").remove();
                        }
                        if (typeof config.items[m].prop != "undefined") tdata = $(el).attr(config.items[m].prop);
                        tid = $(el).attr("id");
                        clickel = config.items[m].seltor;
                        itemsindex = m;
                        /*菜单显示位置*/

                        var text = "<div id=\"right_menu_dialog\" class=\"right_menu\" style=\"overflow:hidden;width:" + config.width + ";height:auto;background-color:#B8E3CF;left:" + e.pageX + "px;top:" + e.pageY + "px;position:absolute;border:2px solid #8a83a1;z-index:10001;display:none;\">";
                        if (config.items[m].showtitle != false) {
                            text += "<div class=\"right_menu_title\" style=\"text-align:center;width:100%;padding:5px 0 5px 0;background-color:#25A2CD;color:#37398d;\">" + (config.items[m].title == undefined ? "操作菜单": config.items[m].title) + "</div>"
                        }
                        text += "<ul style=\"list-style:none;text-align:center;margin-left:0;width:100%;\">";
                        for (var i = 0; i < config.items[m].list.length; i++) {
                            text += "<li class=\"rmenu_item\" style=\"cursor:pointer;background-color:#5A5F5F;margin-top:1px;margin-bottom:1px;color:#fff;height:26px;line-height:26px;\">" + config.items[m].list[i] + "</li>";
                        }
                        text += "</ul></div>"
                        if ($("#right_menu_dialog").size() == 0) {
                            $(this).eq(0).append(text);
                            $("#right_menu_dialog").slideDown("fast");
                        }
                        return false;
                    }
                }
            });

            $(this).delegate(".rmenu_item", "mouseover",
            function() {
                $(this).css("background-color", "#58C2CF");
            }).delegate(".rmenu_item", "mouseout",
            function() {
                $(this).css("background-color", "#5A5F5F");
            }).delegate(".rmenu_item", "click",
            function(n) {
                /*先隐藏再删除*/
                if ($("#right_menu_dialog").size() >= 0) {
                    $("#right_menu_dialog").hide();
                }

                if (typeof config.callback != "undefined" && config.callback != null) {
                    for (var index = 0; index < config.items[itemsindex].list.length; index++) {
                        if ($(this).text() == $(".rmenu_item").eq(index).text()) {
                            config.callback(clickel, index, tdata, tid);
                            /*点击的元素，点击的菜单项，元素的属性数据，元素的id*/
                        }
                    }
                }
                if ($("#right_menu_dialog").size() >= 0) {
                    $("#right_menu_dialog").remove();
                }
            });
        }
    })
})(jQuery);