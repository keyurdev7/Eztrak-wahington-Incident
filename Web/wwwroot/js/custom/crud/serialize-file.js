$(function () {

    //USAGE: $("#form").serializefiles();
    $.fn.serializeFiles = function () {
        let form = [];
        var formData = new FormData();
        var obj = $(this);
        /* ADD FILE TO PARAM AJAX */
        //var formData = new FormData();
        $.each($(obj).find("input[type='file']"), function (i, tag) {
            $.each($(tag)[0].files, function (i, file) {
                formData.append(tag.name, file);
                form.push({ name: tag.name, file: file });
            });
        });
        var params = $(obj).serializeArray();
        $.each(params, function (i, val) {
            formData.append(val.name, val.value);
            form.push({ name: val.name, value: val.value });
        });
        console.log(form);
        return formData;
    };
});