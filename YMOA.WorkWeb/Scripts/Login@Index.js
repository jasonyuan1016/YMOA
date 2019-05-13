(function ($) {
    $.login = {
        formMessage: function (msg) {
            $('.login_tips').find('.tips_msg').remove();
            $('.login_tips').append('<div class="tips_msg"><i class="fa fa-question-circle"></i>' + msg + '</div>');
        },
        loginClick: function () {
            var $username = $("#txt_account");
            var $password = $("#txt_password");
            var $code = $("#txt_code");
            if ($username.val() == "") {
                $username.focus();
                $.login.formMessage(PageResx.inputusername);
                return false;
            } else if ($password.val() == "") {
                $password.focus();
                $.login.formMessage(PageResx.inputupwd);
                return false;
            } else if ($code.val() == "") {
                $code.focus();
                $.login.formMessage(PageResx.inputcode);
                return false;
            } else {
                $("#login_button").attr('disabled', 'disabled').find('span').html("loading...");
                $.ajax({
                    url: "/Login/CheckLogin",
                    data: { username: $.trim($username.val()), password: $.md5($.trim($password.val())), code: $.trim($code.val()) },
                    type: "post",
                    dataType: "json",
                    success: function (data) {
                        if (data.state == "success") {
                            $("#login_button").find('span').html(PageResx.loginsuccessmsg);
                            window.setTimeout(function () {
                                window.location.href = "/Home/Index";
                            }, 500);
                        } else {
                            $("#login_button").removeAttr('disabled').find('span').html(PageResx.loginfaildmsg);
                            $("#switchCode").trigger("click");
                            $code.val('');
                            $.login.formMessage(data.message);
                        }
                    }
                });
            }
        },
        init: function () {
            $('.wrapper').height($(window).height());
            $(".container").css("margin-top", ($(window).height() - $(".container").height()) / 2 - 50);
            $(window).resize(function (e) {
                $('.wrapper').height($(window).height());
                $(".container").css("margin-top", ($(window).height() - $(".container").height()) / 2 - 50);
            });
            $("#switchCode").click(function () {
                $("#imgcode").attr("src", "/Login/GetAuthCode?time=" + Math.random());
            });
            var login_error = top.$.cookie('nfine_login_error');
            if (login_error != null) {
                switch (login_error) {
                    case "overdue":
                        $.login.formMessage(PageResx.login_error_overdue);
                        break;
                    case "OnLine":
                        $.login.formMessage(PageResx.login_error_online);
                        break;
                    case "-1":
                        $.login.formMessage(PageResx.login_error_unknow);
                        break;
                }
                top.$.cookie('nfine_login_error', '', { path: "/", expires: -1 });
            }
            $("#login_button").click(function () {
                $.login.loginClick();
            });
            document.onkeydown = function (e) {
                if (!e) e = window.event;
                if ((e.keyCode || e.which) == 13) {
                    document.getElementById("login_button").focus();
                    document.getElementById("login_button").click();
                }
            }
            $("#sltLanguage").change(function () {
                $.ajax({
                    url: "/Login/SetCulture",
                    data: { culture: $(this).val() },
                    success: function (result) {
                        window.location.reload(true);
                    }
                });
            });
            $("#sltLanguage").val($("#hidCulture").val());
        }
    };
    $(function () {
        $.login.init();
    });
})(jQuery);