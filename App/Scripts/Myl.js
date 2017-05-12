var myAjax = function (sendData_, Data__, success) {
   
    var num____ = 0;
    if (typeof (arguments[3]) != 'undefined' && arguments[3] != null) {
        num____ = arguments[3];
    }
    var nowDate___ = new Date();
    nowDate___ = Date.parse(nowDate___);
    if (Data__ == "")
        Data__ = "?ajax=1&t=" + nowDate___;
    else {
        Data__ += "&ajax=1&t=" + nowDate___;
    }

    $.ajax({
        type: "get",
        dataType: "json",
        url: sendData_,
        data: Data__,
        async: false,
        success: function (json) {
           
            if (json.Data.code == "0000") {
                success(json.Data.item);
            } else {
                alert("错误：" + json.Data.message);
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (num____ <= 3) {
                num____++;
                myAjax(sendData_, Data__, success, num____);
            } else {
                alert("服务器忙，请稍后重试。");
            }
        }
    });
};
//ajax初始化

