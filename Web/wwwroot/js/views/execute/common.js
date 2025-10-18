var current_tab, next_tab, previous_tab;
var step_no = 1;
$(".previous-step").click(function (e) {
    e.preventDefault();
    current_tab = $(".step-card:visible");
    previous_tab = $(".step-card:visible").prev();
    step_no = $(this).parents(".staging-steps").find("fieldset").index(previous_tab);
    step_no++;
    $(this).parents(".staging-steps").find('.step-no').html(step_no);
    previous_tab.show();
    current_tab.hide();
});
