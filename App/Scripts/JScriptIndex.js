function returnParent(value) {//获取子窗体返回值
    var parent = window.dialogArguments; //获取父页面
    //parent.location.reload(); //刷新父页面
    if (parent != null && parent != "undefined") {
        window.returnValue = value; //返回值
        window.close(); //关闭子页面
    }
    return;
}
function loadpage() {

    if (window.top.location.pathname == "/Home") {

    } else if (window.dialogArguments == null || window.dialogArguments == "undefined") {
        this.window.location.href = "/Account";
    }
}
function dateConvert(value) {
    var reg = new RegExp('/', 'g');
    var d = eval('new ' + value.replace(reg, ''));
    return new Date(d).format('yyyy-MM-dd')
}
function formatDatebox(value) {
    if (value == null || value == '') {
        return '';
    }
    return value.split('T')[0];
    //if (value == null || value == '') {
    //    return '';
    //}
    //var dt;
    //if (value instanceof Date) {
    //    dt = value;
    //} else {
    //    dt = new Date(value);
    //}

    //return dt.format("yyyy-MM-dd"); //扩展的Date的format方法(上述插件实现)
}
function TimeFormatter(value, row) {
    if (value == undefined) {
        return "";
    }
    /*json格式时间转js时间格式*/
    var arrDate = value.split('T');
    var newDate = arrDate[0];
    var newTime = arrDate[1].split('.')[0];

    if (new Date(newDate, 'yyyy-MM-dd').getFullYear() < 1900) {
        return "";
    }

    var t = newDate + " " + newTime;

    return t;
}
//function formatDateTimebox(value) {
//    if (value == null || value == '') {
//        return '';
//    }
//    var dt;
//    if (value instanceof Date) {
//        dt = value;
//    } else {
//        dt = new Date(value);
//    }

//    return dt.format("yyyy-MM-dd hh:mm:ss"); //扩展的Date的format方法(上述插件实现)
//}

function isInt(t) {
    t.value = t.value.replace(/[^0-9]/g, '')
}
$(function () {
    //时间格式化
    Date.prototype.format = function (format) {
        /*
        * eg:format="yyyy-MM-dd hh:mm:ss";
        */
        if (!format) {
            format = "yyyy-MM-dd hh:mm:ss";
        }

        var o = {
            "M+": this.getMonth() + 1, // month
            "d+": this.getDate(), // day
            "h+": this.getHours(), // hour
            "m+": this.getMinutes(), // minute
            "s+": this.getSeconds(), // second
            "q+": Math.floor((this.getMonth() + 3) / 3), // quarter
            "S": this.getMilliseconds()
            // millisecond
        };

        if (/(y+)/.test(format)) {
            format = format.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
        }

        for (var k in o) {
            if (new RegExp("(" + k + ")").test(format)) {
                format = format.replace(RegExp.$1, RegExp.$1.length == 1 ? o[k] : ("00" + o[k]).substr(("" + o[k]).length));
            }
        }
        return format;
    };

    $.extend($.fn.datagrid.methods, {
        addToolbarItem: function (jq, items) {
            return jq.each(function () {
                var dpanel = $(this).datagrid('getPanel');
                var toolbar = dpanel.children("div.datagrid-toolbar");
                if (!toolbar.length) {
                    toolbar = $("<div class=\"datagrid-toolbar\"><table cellspacing=\"0\" cellpadding=\"0\"><tr></tr></table></div>").prependTo(dpanel);
                    $(this).datagrid('resize');
                }
                var tr = toolbar.find("tr");
                for (var i = 0; i < items.length; i++) {
                    var btn = items[i];
                    if (btn == "-") {
                        $("<td><div class=\"datagrid-btn-separator\"></div></td>").appendTo(tr);
                    } else {
                        var td = $("<td></td>").appendTo(tr);
                        var b = $("<a href=\"javascript:void(0)\"></a>").appendTo(td);
                        b[0].onclick = eval(btn.handler || function () { });
                        b.linkbutton($.extend({}, btn, {
                            plain: true
                        }));
                    }
                }
            });
        },
        removeToolbarItem: function (jq, param) {
            return jq.each(function () {
                var dpanel = $(this).datagrid('getPanel');
                var toolbar = dpanel.children("div.datagrid-toolbar");
                var cbtn = null;
                if (typeof param == "number") {
                    cbtn = toolbar.find("td").eq(param).find('span.l-btn-text');
                } else if (typeof param == "string") {
                    cbtn = toolbar.find("span.l-btn-text:contains('" + param + "')");
                }
                if (cbtn && cbtn.length > 0) {
                    cbtn.closest('td').remove();
                    cbtn = null;
                }
            });
        }
    });

})

//查询身份证输入框
$(function () {
    $(".CertificateNumber").focus(function () {
        $(this).animate({ height: '100px' });
    });
    $(".CertificateNumber").blur(function () {
        $(this).animate({ height: '20px' });
    });
});

//easyui日期之显示年月
//$(function () {
//    $('#YearMonth1').datebox({
//        onShowPanel: function () {//显示日趋选择对象后再触发弹出月份层的事件，初始化时没有生成月份层
//            span.trigger('click'); //触发click事件弹出月份层
//            if (!tds) setTimeout(function () {//延时触发获取月份对象，因为上面的事件触发和对象生成有时间间隔
//                tds = p.find('div.calendar-menu-month-inner td');
//                tds.click(function (e) {
//                    e.stopPropagation(); //禁止冒泡执行easyui给月份绑定的事件
//                    var year = /\d{4}/.exec(span.html())[0]//得到年份
//                    , month = parseInt($(this).attr('abbr'), 10) + 1; //月份
//                    $('#YearMonth1').datebox('hidePanel')//隐藏日期对象
//                    .datebox('setValue', year + month); //设置日期的值
//                });
//            }, 0)
//        },
//        parser: function (s) {//配置parser，返回选择的日期
//            if (!s) return new Date();
//            var arr = s.split('-');
//            return new Date(parseInt(arr[0], 10), parseInt(arr[1], 10) - 1, 1);
//        },
//        formatter: function (d) {
//            return d.getFullYear() +
//                (d.getMonth() < 10 ? ('0' + d.getMonth()) : d.getMonth());;
//        }//配置formatter，只返回年月
//    });
//    var p = $('#YearMonth1').datebox('panel'), //日期选择对象
//        tds = false, //日期选择对象中月份
//        span = p.find('span.calendar-text'); //显示月份层的触发控件
//});