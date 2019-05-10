//绑定下拉框，data数据集[只支持字典模式],vle选中值
$.fn.bindSlt = function (data) {
    var $element = $(this);
    $.each(data, function (i) {
        $element.append($("<option></option>").val(i).html(data[i]));
    });
}

//表单验证
$.fn.formValid = function () {
    return $(this).valid({
        errorPlacement: function (error, element) {
            element.parents('.formValue').addClass('has-error');
            element.parents('.has-error').find('i.error').remove();
            element.parents('.has-error').append('<i class="form-control-feedback fa fa-exclamation-circle error" data-placement="left" data-toggle="tooltip" title="' + error + '"></i>');
            $("[data-toggle='tooltip']").tooltip();
            if (element.parents('.input-group').hasClass('input-group')) {
                element.parents('.has-error').find('i.error').css('right', '33px')
            }
        },
        success: function (element) {
            element.parents('.has-error').find('i.error').remove();
            element.parent().removeClass('has-error');
        }
    });
}

//收集表单数据生成对象
$.fn.dataSerialize = function () {
    var element = $(this);
    var postdata = {};
    element.find('input,select,textarea').each(function (r) {
        var $this = $(this);
        var id = $this.attr('id');
        var dataID = id.substr(3);
        var type = $this.attr('type');
        switch (type) {
            case "checkbox":
                postdata[dataID] = $this.is(":checked");
                break;
            default:
                var value = $this.val() == "" ? "&nbsp;" : $this.val();
                if (!$.request("keyValue")) {
                    value = value.replace(/&nbsp;/g, '');
                }
                postdata[dataID] = value;
                break;
        }
    });
    return postdata;
};