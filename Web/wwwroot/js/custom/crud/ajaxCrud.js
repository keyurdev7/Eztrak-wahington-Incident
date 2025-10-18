$.getScript("/js/custom/crud/serialize-file.js", function () {
});
$.getScript("/js/custom/crud/validation-summary.js", function () {
});

function loadModalPanel(contentUrl, modalPanelId, modalPanelBody) {
    $("#tab-content-loader").addClass("loader-floating");
    $("#" + modalPanelId).find(".modal-title").html('');
    $("#" + modalPanelId).modal("hide");
    $.ajax({
        type: "GET",
        url: contentUrl,
        success: function (htmlContent) {
            $("#tab-content-loader").removeClass("loader-floating");
            $("#" + modalPanelBody).html(htmlContent);
            $("#" + modalPanelId).find(".modal-title").html($("#modal-title").val());
            $("#" + modalPanelId).modal("show");
        },
        error: function (xhr, status, error) {
            console.error("Error:", status, error);
            $("#tab-content-loader").removeClass("loader-floating");
        }
    });
}

function approveRecord(element, modalPanelId = "crudModalPanel") {
    $("#status-id").val(1);
    updateRecord(element, modalPanelId)
}
function rejectRecord(element, modalPanelId = "crudModalPanel") {
    $("#status-id").val(2);
    updateRecord(element, modalPanelId)
}

function approveDetail(element) {
    sendApproveAjax(1);
}

function rejectDetail(element) {
    sendApproveAjax(2);
}

function sendApproveAjax(status) {
    var controller = $("#controller-name").val();
    var id = $("#log-id").val();
    var url = "/" + controller + "/ApproveStatus";
    var data = { status: status, id: id };

    $.ajax({
        type: "Get",
        url: url,
        data: data,
        success: function (result) {
            $("#crudModalPanel").modal("toggle");
            ReInitalizeData();
        }
    });
}


function updateRecord(element, modalPanelId = "crudModalPanel") {
    var form = element.closest("form")
    var updateUrl = form.action;
    removeCurrencyMasking();
    var formData = $(form).serializeFiles();
    ProcessFormData(formData);
    $(form).removeData("validator").removeData("unobtrusiveValidation");
    $.validator.unobtrusive.parse($(form));
    if ($(form).valid()) {
        clearValidationSummary(form);
        $.ajax({
            type: "POST",
            url: updateUrl,
            data: formData,
            processData: false,
            contentType: false,
            beforeSend: function () {
                disableControls(form);
            },
            success: function (result) {
                enableControls(form);
                if (!result.Success) {
                    createValidationSummary(form, result.Errors);
                }
                else {
                    onUpdateRecordSuccess(result.Id, modalPanelId, form);
                }

            },
            complete: function () {
                addCurrencyMasking();
                enableControls(form);
            },
        });
    }
    addCurrencyMasking();

}
function disableControls(form) {
    DisableProperty(form, ".form-btn", true);
    DisableProperty(form, "#approve-btn", true);
    DisableProperty(form, "#reject-btn", true);
    DisableProperty(form, "#submit-btn", true, `<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Submitting...`);
    DisableProperty(form, ".cancel-btn", true);
    let modalPanel = $(form).closest("#crudModalPanel");

    $(modalPanel).off('hide.bs.modal');
    $(modalPanel).on('hide.bs.modal', function () {
        return false;
    });
    $(modalPanel).find("fieldset").block({
        centerY: false,
        centerX: false,
        css: {
            margin: 'auto',
            border: 'none',
            padding: '15px',
            backgroundColor: 'transparent',
            '-webkit-border-radius': '10px',
            '-moz-border-radius': '10px',
            color: 'transparent'
        }

    });
    $(modalPanel).find(".blockOverlay").css({ "background-color": "#fff", "opacity": "0.5" })
}
function enableControls(form) {
    if ($(form).find(".form-btn").is(":disabled")) {
        DisableProperty(form, ".form-btn", false);
        DisableProperty(form, ".cancel-btn", false);
        DisableProperty(form, "#submit-btn", false, "Submit");
        let modalPanel = $(form).closest("#crudModalPanel");
        $(modalPanel).off('hide.bs.modal');
        $(modalPanel).on('hide.bs.modal', function () {
            return true;
        });
        $(modalPanel).find("fieldset").unblock();
    }
}

function DisableProperty(form, target, status, html = "") {
    let btn = $(form).find(target);
    if (html != "")
        $(btn).html(`Submit`);
    $(btn).prop('disabled', status);
}

function removeCurrencyMasking() {
    $(".input-currency").each(function (index, element) {
        $(element).inputmask("remove");
        $(element).attr("data-val", "true");
    });
}
function addCurrencyMasking() {
    maskCurrency(".input-currency");
}

function onUpdateRecordSuccess(recordId, modalPanelId, form) {
    /*if ($("#crud-list-table").length > 0) {*/
    ReInitalizeData();
    /*}*/
    $("#" + modalPanelId).modal("toggle");
}
function ReInitalizeData() {
    if (typeof ReInitializeDataTables === 'function') {
        ReInitializeDataTables();
    } else if (typeof InitializeDataCards === 'function') {
        InitializeDataCards();
    }
}
function DeleteDataItem(deleteObj, dtObj) {

    let confirmBtnText = deleteObj.confirmBtnText || "Yes, delete it!";
    let cancelBtnText = deleteObj.cancelBtnText || "No, cancel!";
    let deleteUrl = deleteObj.deleteUrl || "";

    swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: confirmBtnText,
        cancelButtonText: cancelBtnText,
        confirmButtonClass: 'btn btn-success me-2',
        cancelButtonClass: 'btn btn-danger',
        buttonsStyling: false
    }).then(function (result) {
        if (result.value) {
            DeleteItem(deleteUrl).then(function (ajaxResult) {
                if (ajaxResult.Success) {
                    onDeleteSuccess(ajaxResult.Id);
                    if (ajaxResult.ReloadDatatable) {
                        if (dtObj != undefined) {
                            dtObj.searchDataTable();
                        }
                    }
                    else {
                        if (deleteReturnUrl === "" || deleteReturnUrl === null || deleteReturnUrl === undefined) {
                            location.reload();
                        }
                        else {
                            window.location.href = deleteReturnUrl;
                        }
                    }

                }
                else {
                    Swal.fire("Couldn't delete. Try again later.")
                }
            });

        }
        else if (result.dismiss === swal.DismissReason.cancel) {
        }
    });


}
function DeleteItem(url) {
    return $.ajax({
        url: url,
        type: 'POST',
        success: function (res) {
        }
    });
}
function loadUpdateAndDetailModalPanel(contentUrl) {
    loadModalPanel(contentUrl, "crudModalPanel", "crudModalPanelBody")
}
function sendAjaxRequest(url, element) {
    showDatatableLoader(element);
    $.ajax({
        type: "Post",
        url: url,
        success: function (res) {
            hideDatatableLoader(element);
            toastr.options.positionClass = "toast-top-right";
            if (res.IsSuccessful) {
                toastr.success(res.Message);
            }
            else {
                toastr.error(res.Message);

            }
        },
        error: function (xhr, status, error) {
            // Handle errors
            console.error("Error:", status, error);
            hideDatatableLoader(element);
        }
    });
}
function onDeleteSuccess(recordId) {

}
function ProcessFormData(formData) {
}