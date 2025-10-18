function showLoader(selector) {
    if (!$("#loader").length > 0) {
        $(selector).append("<button id='loader' style='display:none' type='button' class='btn bg-custom-dark btn-float rounded-round'><i class='icon-spinner4 spinner'></i></button>");
    }
    $(selector).block({
        message: $("#loader"),
        centerY: false,
        centerX: false,
        css: {
            margin: 'auto',
            border: 'none',
            padding: '15px',
            backgroundColor: 'transparent',
            '-webkit-border-radius': '10px',
            '-moz-border-radius': '10px',
            color: '#fff'
        }
    });

}
function hideLoader(selector) {
    $(selector).unblock();
}