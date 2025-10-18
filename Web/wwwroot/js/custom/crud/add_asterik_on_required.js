$(function () {
    addAsterikToRequiredFields();
});
function addAsterikToRequiredFields() {
    $('input,select,textarea').each(function () {
        var req = $(this).attr('data-val-required');
        var isHidden = $(this).attr('type') === "hidden" && $(this).attr('class') !== "show-asterik";
        if (undefined != req && req != "" && !isHidden) {
            var label = $(this).siblings(".form-label, .control-label");
            var text = label.text();
            if (text.length > 0) {
                console.log($(this).attr('data-val-ignore'));
                if ($(this).attr('data-val-ignore') == undefined || $(this).attr('data-val-ignore') == null) {
                    label.find('span[style="color:red"]').remove();
                    label.append('<span style="color:red"> *</span>');
                }
               
            }
        }
    });
}
