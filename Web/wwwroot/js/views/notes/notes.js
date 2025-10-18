var notesUrl = "";
var closeNotesModalOnSubmit = true;

$(function () {
    $(document).off('click', "#submit-notes-btn")
    $(document).on('click', "#submit-notes-btn", function (e) {
        debugger;
        e.preventDefault();
        var notes = $("#notes-text").val();
        var formData = $("#notes-form").serializeFiles();
        if (notes != "" && notes != undefined && notes != null) {
            SendAjax(formData);
        }
        else {
            $(".notes-description").html("The Notes field is required.")
        }
    });

});
function SendAjax(model) {
    $.ajax({
        url: notesUrl,
        type: "post",
        data: model,
        contentType: false,
        processData: false,
        success: function (response) {
            if (response) {
                $("#notes-form")[0].reset();
                if (closeNotesModalOnSubmit) {
                    $("#crudModalPanel").modal("hide");
                }
                else {
                    getNotesList();
                }
            }
            else {
                $(".notes-summary").html("Some error occured. Please try again later.")
            }

        },
        error: function (jqXHR, textStatus, errorThrown) {
        }
    });
}
function getNotesList(model) {
}
