function invokeService(_act, _data, callback, _type, _dataType) {
    if (_type == undefined || _type == "") {
        _type = "post";
    }
    if (_dataType == undefined || _dataType == "") {
        _dataType = "json";
    }
    $.ajax({
        type: _type,
        dataType: _dataType,
        url: 'ActionService.ashx?act=' + _act,
        data: _data,
        error: function (e) {
            if (e.readyState == 4 && e.status == 200 && e.statusText == 'OK') {
                alert("程序异常" + e.status);
            }
        },
        success: function (r) {
            if (callback != null) {
                if (r.errorCode != "") {
                    alert("异常操作：" + r.errorCode + ":" + r.errorMsg);
                } else {
                    callback(r.result);
                }
            }
        }
    });
}