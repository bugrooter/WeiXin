var currentPage;
var listAction;
var msgTimer;

window.onerror = function(){
	return true;
}
$(document).ready(function(){
	win_size();
})
$(window).resize(function(){
	win_size();
})
function win_size(){
	var win_h = $(window).height(); 
	$("#sidebar").height(win_h-50);
	$("#content").height(win_h-50);
	$(".left-nav").height(win_h-129);
}
function div_center(obj){
	var windowWidth  = $(window).width();
	var windowHeight = $(window).height();
	var popupHeight  = $(obj).height();
	var popupWidth   = $(obj).width();
	$(obj).css({
		"top": (windowHeight-popupHeight)/2+$(document).scrollTop(),
		"left": (windowWidth-popupWidth)/2
	});
}


function msg_show(options){
  if(document.getElementById("msg_show")){
    msg_close();
  }
  var defaults = {
    text : "",
    time : 1000,
    type : "success",
    isLock : true,
    isClose : false,
    callback : function(){},
    callback1 : function(){}
  };
  var opts = $.extend(defaults,options);
	$(document.body).append('<div class="msg-show" id="msg_show"><div class="msg-text" id="msgText"></div></div>');
  if(opts.isLock) $(document.body).append('<div id="msgLock" class="msg-lock"></div>');
  var icon = "";
  switch(opts.type){
    case 'success':
      icon = '<i class="icon-ok-sign success"></i> ';
    break;
    case 'error':
      icon = '<i class="icon-remove-sign error"></i> ';
    break;
    case 'warning':
      icon = '<i class="icon-warning-sign warning"></i> ';
    break;
    case 'loading':
      icon = '<i class="icon-spinner spin loading"></i> ';
    break;
    case 'confirm':
      icon = '<i class="icon-exclamation-sign confirm"></i> ';
      $('#msg_show').append('<div class="msg-btns ok"><a class="msg-btn" href="javascript:;" rel="true">确定</a><a class="msg-btn cancel" href="javascript:;" rel="false">取消</a></div>');
    break;
  }
	$('#msgText').html(icon+opts.text);
    div_center('#msg_show');
	if(opts.time!=0){
		msgTimer = setTimeout(msg_close,opts.time);
	}
  if(opts.isClose){
    $('#msg_show').append('<a class="msg-close" href="javascript:;" onclick="msg_close()">×</a>');
  }
  $(".msg-btn").bind("click",function(){
    if($(this).attr("rel")=="true"){
      opts.callback();
    }
    else{
      opts.callback1();
    }
    msg_close();
  });
}


function msg_ok(){
  msg_close();
  msg_show({text:'载入中...',time:0,type:'loading'});
}
function msg_close(){
	clearTimeout(msgTimer);
	$("#msgLock").remove();
	$('#msg_show').remove();
}

function getCookie(objName){//获取指定名称的cookie的值
	var arrStr = document.cookie.split("; ");
	for(var i = 0;i < arrStr.length;i ++){
		var temp = arrStr[i].split("=");
		if(temp[0] == objName) return unescape(temp[1]);
	}
}

function dialog_div(o_w,o_h,loadUrl,text,divId,ishid){
	if(divId==null){
		divId='dialog';
	}
    if(ishid==true){
		ishid='dialog-bd-hid';
	}
    else {
        ishid='dialog-bd';
    }
	if(loadUrl.indexOf("?")>-1){
		loadUrl+="&math="+Math.random();
	}else{
		loadUrl+="?math="+Math.random();
	}
	$(document.body).append('<div id="'+divId+'" class="dialog-box"><div id="moveHandle" style="cursor:move;" class="dialog-hd"><span>'+text+'</span><a class="icon-remove" href="javascript:void(0)" onclick="dialog_close(\''+divId+'\')"></a> </div><div class=\"'+ishid+'\" id="dialog-body"></div></div><div id="divLock" class="divLock"></div>');
	divId='#'+divId;
	var _move=false;//移动标记
	var _x,_y;//鼠标离控件左上角的相对位置
	var moveDiv=""+divId+" #moveHandle";
	$(moveDiv).click(function(){

	}).mousedown(function(e){
		_move=true;
		$(moveDiv).css("cursor","move");
		_x=e.pageX-parseInt($(divId).css("left"));
		_y=e.pageY-parseInt($(divId).css("top"));
		});
		$(document).mousemove(function(e){
			if(_move){
				var x=e.pageX-_x;//移动时根据鼠标位置计算控件左上角的绝对位置
				var y=e.pageY-_y;
                if(x<=0) x=0;
		        if(y<=0) y=0;
		        if($(divId).width()+16+x>=$(window).width()){x=$(window).width()-$(divId).width()-16}
		        if($(divId).height()+16+y>=$(window).height()){y=$(window).height()-$(divId).height()-16}
				$(divId).css({top:y,left:x});//控件新位置
			}
		}).mouseup(function(){
		_move=false;
	});
	
	$(divId).css("width",o_w);
	$(divId).css("height",o_h);
	$(".dialog-bd").css("height",o_h-50);
	$.ajax({
		type: "get",
		url: loadUrl,
		dataType:"text",
		beforeSend:function(){
			msg_loading('数据加载中,请稍候!',"");
		},
		success: function(html){
			var obj = jQuery.parseJSON(html);
            msg_close(); 
			$("#divLock").show();
			$(".dialog-box").fadeIn();
//			$("#dialog-body").html(html);  //edit by xurun 2014-6-9
			//document.getElementById('dialog-body').innerHTML=obj.info;
            //上一句异步返回js不执行，替换jquery函数解决
            $("#dialog-body").html(obj.info);
			$('.dialog-box input:text:first').focus();//焦点丢失获取焦点
		}
	});
	div_center(divId);
}
function dialog_close(obj){
	if(obj==null) obj = "dialog"; 
	$("#"+obj+"").remove();
	$("#divLock").remove(); 
}
/* 顶部导航 */
$(function(){
	$(".nav li").click(function(){
		$(".dropdown").attr("id","");
		$(this).attr("id","open");
	});
	$(".notification-nav li").click(function(){
		$(".dropdown").attr("id","");
		$(this).attr("id","open");
	});
});
$(document).bind("click",function(event){
  var e = event || window.event;  
  var elem = e.srcElement||e.target;  
  while(elem){
    if(elem.id == "open"){    
      return;
    }
    elem = elem.parentNode;       
  }
  $(".dropdown").attr("id","");
});


!function ($) {

  "use strict"; // jshint ;_;

 /* 定义标签类
  * ==================== */

  var Tab = function (element) {
    this.element = $(element)
  }

  Tab.prototype = {

    constructor: Tab

  , show: function () {
      var $this = this.element
        , $ul = $this.closest('ul:not(.dropdown-menu)')
        , selector = $this.attr('data-target')
        , previous
        , $target
        , e

      if (!selector) {
        selector = $this.attr('href')
        selector = selector && selector.replace(/.*(?=#[^\s]*$)/, '') //strip for ie7
      }

      if ( $this.parent('li').hasClass('active') ) return

      previous = $ul.find('.active:last a')[0]

      e = $.Event('show', {
        relatedTarget: previous
      })

      $this.trigger(e)

      if (e.isDefaultPrevented()) return

      $target = $(selector)

      this.activate($this.parent('li'), $ul)
      this.activate($target, $target.parent(), function () {
        $this.trigger({
          type: 'shown'
        , relatedTarget: previous
        })
      })
    }

  , activate: function ( element, container, callback) {
      var $active = container.find('> .active')
        , transition = callback
            && $.support.transition
            && $active.hasClass('fade')

      function next() {
        $active
          .removeClass('active')
          .find('> .dropdown-menu > .active')
          .removeClass('active')

        element.addClass('active')

        if (transition) {
          element[0].offsetWidth // reflow for transition
          element.addClass('in')
        } else {
          element.removeClass('fade')
        }

        if ( element.parent('.dropdown-menu') ) {
          element.closest('li.dropdown').addClass('active')
        }

        callback && callback()
      }

      transition ?
        $active.one($.support.transition.end, next) :
        next()

      $active.removeClass('in')
    }
  }


 /* TAB PLUGIN DEFINITION
  * ===================== */

  var old = $.fn.tab

  $.fn.tab = function ( option ) {
    return this.each(function () {
      var $this = $(this)
        , data = $this.data('tab')
      if (!data) $this.data('tab', (data = new Tab(this)))
      if (typeof option == 'string') data[option]()
    })
  }

  $.fn.tab.Constructor = Tab


 /* TAB NO CONFLICT
  * =============== */

  $.fn.tab.noConflict = function () {
    $.fn.tab = old
    return this
  }


 /* TAB DATA-API
  * ============ */

  $(document).on('click.tab.data-api', '[data-toggle="tab"], [data-toggle="pill"]', function (e) {
    e.preventDefault()
    $(this).tab('show')
  })

}(window.jQuery);

/* 左侧选项卡 */
$(function(){
	$('#myTab a').click(function (e) {
    e.preventDefault();
    if ($(this).attr("href").replace("#","")=="") {
        return;
    }
    $("#myTab>.active").removeClass("active");
    $(this).parent().addClass("active");
    $(".tab-pane").hide();
    $("#"+$(this).attr("href").replace("#","")).show();
	});
});

/* 搜索 - 下拉 */
function searchDown(obj){
  $(obj).toggle();
}
 

//保存数据，只要知道url即可，兼容增加和修改两种情况
function saveDialogForm(actionUrl){
		$('#flag').val("1");
		$.ajax({
			type:'post', 
			url:actionUrl,
			data:$('#dialogFrm').serialize(),
			dataType:'text', 
			beforeSend:function(){msg_loading('数据保存中，请稍候...');},
			success:function(msg){  //操作成功后的处理
				var obj = jQuery.parseJSON(msg);
				if(obj.success==false){  //业务逻辑操作失败
					$('#flag').val("0");
					msg_error("失败，原因是"+obj.info+"!");
				}else{	//业务逻辑操作成功
					dialog_close(); 
					msg_success("数据保存成功!");
					loadData(1,'false');
				}
			},
			error:function(){ //系统错误后的处理  
					$('#flag').val("0");
					msg_show("操作失败，未知原因!",0,'error');
			}
		}); 
}


	<!--取得checkbox所有选择项-->
	 function checkboxValue(checkboxName) {
		var checkVal='';
		$("input[name='"+checkboxName+"']:checkbox").each(function(){  
			if($(this).prop('checked')){
				checkVal+=","+$(this).val();
			}
		});
		if(checkVal!=""){checkVal = checkVal.substring(1);}
		return checkVal;
	}


	<!--根据url删除所有数据-->
	function deleteByUrl(url){
			$.ajax({
					type:'post', 
					url:url,
					dataType:'text',
					beforeSend:function(){msg_loading('数据删除中...');},
					success:function(msg){  //操作成功后的处理
						msg_close();
						var obj = jQuery.parseJSON(msg);
						if(obj.success==false){  //业务逻辑操作失败  
							msg_error('删除失败，原因是:'+obj.info);
						}else{	//业务逻辑操作成功 
							loadData(1,'false');		
							msg_success('数据删除成功'); 
						}
					},
					error:function(){ //系统错误后的处理 
							msg_close();
							msg_error('删除失败，未知原因');
					}
				});
		}
	 

	<!--ajax-加载内容到mainDiv->
	function loadMainDiv(url,needLoading){
		if(needLoading==null){		
			$.ajax({
					type:'post',
					url:url+"&math="+Math.random(),
					beforeSend:function(){msg_loading('数据载入中，请稍候...');},
					success:function(data){ 
						document.getElementById("mainDiv").innerHTML=data;
						msg_close();
					}
			});
		}else{
			$.ajax({
					type:'post',
					url:url+"&math="+Math.random(),
					//beforeSend:function(){msg_loading('数据载入中，请稍候...');},
					success:function(data){ 
						document.getElementById("mainDiv").innerHTML=data;
						//msg_close();
					}
			});
		}
	}

	//============================
function selectRow(obj){
  var objCheckbox = obj.find("input[type=checkbox]");
  if(objCheckbox.is(':checked')){
    objCheckbox.prop("checked",false);
    obj.removeClass("selected");
  }else{
    objCheckbox.prop("checked",true);
    obj.addClass("selected");
  }
}


//加载当前页面
function loadDataCurrentPage(){
	loadData(currentPage);
}



/**错误框*/
function msg_error(tip,t){
    if(typeof t=="undefined"||t==null)
        t=0;
	msg_show({text:tip,type:'error',time:t, isClose : true });
}


/**警告框*/
function msg_alter(tip){
	msg_show({text:tip,type:'warning',time:0, isClose : true });
}

//loading
function msg_loading(tip){
	msg_show({text:tip,type:'loading',time:0});
}

//成功
function msg_success(tip){
	msg_show({text:tip,type:'success'});
}

/**选择对话框*/
function msg_confirm(tip,callback,callback1){
  msg_show({text:tip,type:'confirm',time:0,callback:callback,callback1:callback1});
}