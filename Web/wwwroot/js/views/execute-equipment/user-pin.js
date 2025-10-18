function fnCheckUserPin() {
    var spnRecipientBadge = $('#spnRecipientBadge');
    spnRecipientBadge.empty();
    var userPinCode = $.trim($('#userPinCode').val());
    if (userPinCode.length > 0) {
        $.ajax({
            url: "/Execute/ValidateUserPin",
            type: "GET",
            data: { 'pinCode': userPinCode },
            success: function (response) {
                if (response) {
                    //$('#recipbadge').val(userId);
                    $(this).parents(".staging-steps").find('.step-no').html(step_no)
                    current_tab = $(".step-card:visible");
                    next_tab = $(".step-card:visible").next();
                    step_no = $(this).parents(".staging-steps").find("fieldset").index(next_tab);
                    step_no++;
                    $('.step-no').html("2");
                    $('#LotNo').val('');
                    next_tab.show();
                    current_tab.hide();
                }
                else {
                    SwalErrorAlert("Invalid Pin Number!");
                }
            },
            error: function (r, s, t) {
                SwalErrorAlert('Please try again.');
            }
        });
    }
    else {
        spnRecipientBadge.html('User Pin is required');
    }
}
