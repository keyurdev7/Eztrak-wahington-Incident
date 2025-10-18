
function createValidationSummary(form, errors) {
    var validationSummaryContainer = $(form).find(".validation-summary-errors");
    var html = "<ul>";
    for (var i = 0; i < errors.length; i++) {
        html += `<li>${errors[i]}</li>`;
    }
    html += "</ul>";
    $(validationSummaryContainer).html(html);
}
function clearValidationSummary(form) {
    var validationSummaryContainer = $(form).find(".validation-summary-errors");
    $(validationSummaryContainer).empty();
}
