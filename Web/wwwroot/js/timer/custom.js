function convertToPST(date) {
    return new Date(date.toLocaleString('en-US', { timeZone: 'America/Los_Angeles' }));
}
function initializeCountDownTimer(selector, targetTime, pstNow) {
    var timeDifference = new Date(targetTime).getTime() - pstNow.getTime();
    $(selector).timeTo({
        seconds: (timeDifference / 1000),
        displayDays: false,
        displayCaptions: false,
        fontSize: 24,
        captionSize: 14,
        callback: function () {
            // Your callback code here
            $(selector).html("Closed");
            $(selector).replaceWith($(selector).clone().removeAttr('style').removeClass().addClass("timer-td"));
        }
    });
}